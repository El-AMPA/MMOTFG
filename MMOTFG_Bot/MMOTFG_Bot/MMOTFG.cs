using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MMOTFG_Bot.Communicator;
using MMOTFG_Bot.Navigation;
using MMOTFG_Bot.Loader;
using MMOTFG_Bot.Battle;
using MMOTFG_Bot.Inventory;
using MMOTFG_Bot.Persistency;

namespace MMOTFG_Bot
{
	class MMOTFG
	{
		private static bool processedNewEvents = false;
		private static long launchTime;
		static cHelp helpCommand = new cHelp();
		static cCreateCharacter createCommand = new cCreateCharacter();

		public static List<ICommand> commandList = new List<ICommand> { new cDeleteCharacter(), new cCreateCharacter(), new cUseItem(), new cAddItem(), new cThrowItem(),
            new cShowInventory(), new cEquipItem(), new cUnequipItem(), new cInfo(), new cStatus(), new cFight(),
			new cNavigate(), new cDirections(), new cInspectRoom(), new cAttack(), helpCommand,
			new cCreateParty(), new cJoinParty(), new cExitParty(), new cShowParty(), new cGiveItem()};

		static async Task Main(string[] args)
		{
			/*Si estamos depurando, tenemos que cambiar la ruta relativa en PC
			* para que funcione igual que en el contenedor de Docker*/
			if (Environment.GetEnvironmentVariable("PLATFORM_PC") != null)
            {
				Console.WriteLine("Estamos en visual");
				Directory.SetCurrentDirectory("./../../..");
			} else Console.WriteLine("Estamos en modo release o en Docker");
			

			string token = "";
			try
			{
				token = System.IO.File.ReadAllText("Assets/private/token.txt");
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("No se ha encontrado el archivo token.txt en la carpeta assets.");
				Environment.Exit(-1);
			}

			var botClient = new TelegramBotClient(token);
			var me = await botClient.GetMeAsync();
			launchTime = DateTime.UtcNow.Ticks;

			//Module initializers
			TelegramCommunicator.Init(botClient);
			JSONSystem.Init("Assets/GameFiles/map.json", "Assets/GameFiles/enemies.json", "Assets/GameFiles/player.json", 
				"Assets/GameFiles/attacks.json", "Assets/GameFiles/items.json", "Assets/GameFiles/directionSynonyms.json");
			Map.Init();
			InventorySystem.Init();
			BattleSystem.Init();
			DatabaseManager.Init();
			foreach (ICommand c in commandList)
			{
				c.SetKeywords();
				c.SetDescription();
			}
			createCommand.SetKeywords();
			helpCommand.setCommandList(new List<ICommand>(commandList));

			Console.WriteLine("Hello World! I am user " + me.Id + " and my name is " + me.FirstName);

			using var cts = new CancellationTokenSource();

			botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), null, cts.Token);

			Console.WriteLine($"Start listening for @{me.Username}");
			Thread.Sleep(Timeout.Infinite);

			cts.Cancel();
		}

		static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var handler = update.Type switch
			{
				// UpdateType.Unknown:
				// UpdateType.ChannelPost:
				// UpdateType.EditedChannelPost:
				// UpdateType.ShippingQuery:
				// UpdateType.PreCheckoutQuery:
				// UpdateType.Poll:
				UpdateType.MyChatMember => onChatMemeberUpdateReceived(update),
				UpdateType.Message => BotOnMessageReceived(update.Message),
				UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage),
				//UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
				//UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
				//UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
				//_ => UnknownUpdateHandlerAsync(botClient, update)
			};

			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				await HandleErrorAsync(botClient, exception, cancellationToken);
			}
		}

		static async Task BotOnMessageReceived(Message message)
		{
			var chatId = message.Chat.Id.ToString();
			var senderName = message.From.FirstName;

			if (!processedNewEvents) //Don't process messages with a date prior to the program's launch date
			{
				if (message.Date.Ticks < launchTime) return;
				else processedNewEvents = true;
			}

			Console.WriteLine("Received message: " + message.Text + " from " + senderName);

			if (message.Type == MessageType.Text) //Si le mandas una imagen explota ahora mismo
			{
				List<string> subStrings = ProcessMessage(message.Text);
				string command = subStrings[0];
				string[] args = new string[subStrings.Count - 1];
				subStrings.CopyTo(1, args, 0, args.Length);

				bool recognizedCommand = false;

				if (!await canUseCommand(chatId, command))
				{
					await TelegramCommunicator.SendText(chatId, "You need a character to play, use /create to create a new character");
					return;
				}
				foreach (ICommand c in commandList)
				{
					if (c.ContainsKeyWord(command))
					{
						recognizedCommand = true;
						if (await c.TryExecute(command, chatId, args)) break;

						else await TelegramCommunicator.SendText(chatId, "Incorrect use of that command.\nUse /help_" + command + " for further information.");
					}
				}
				if (!recognizedCommand) await TelegramCommunicator.SendText(chatId, "Unrecognized command.\n Try /help if you don't know what to use");
			}
		}

		/// <summary>
		/// Processes the message recieved from the user by filtering out certain chars and splitting it into words
		/// </summary>
		private static List<string> ProcessMessage(string message)
		{
			List<string> processedMsg = message.ToLower().Split(' ').ToList();

			//If we want to process a hyperlink type command (/equip_sulfuras_hand_of_ragnaros), we only need to split it by the first '_'
			if (processedMsg[0].Contains('_') && processedMsg[0][0] != '_')
			{
				List<string> aux = processedMsg[0].Split('_', 2).ToList();
				processedMsg.RemoveAt(0);
				processedMsg.Insert(0, aux[1]);
				processedMsg.Insert(0, aux[0]);
			}
			if (processedMsg[0][0] == '/') processedMsg[0] = processedMsg[0].Substring(1);
			return processedMsg;
		}

		static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var errorMessage = exception switch
			{
				ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};
			Console.WriteLine(errorMessage);
			return Task.CompletedTask;
		}

		static async Task<bool> canUseCommand(string chatId, string command)
		{

			bool characterExists = await DatabaseManager.IsDocumentInCollection(chatId, DbConstants.COLLEC_PLAYERS);

			bool isIntroductoryCommand = createCommand.ContainsKeyWord(command) ||
											helpCommand.ContainsKeyWord(command);

			return characterExists || isIntroductoryCommand;

		}

		static async Task onChatMemeberUpdateReceived(Update update)
		{
			//If the update is from a new /start
			if (update.MyChatMember.NewChatMember.Status == ChatMemberStatus.Member) { 

				string response = @"Welcome to MMOTFG, please create a character with /create to start playing.
Remember that you can get help with /help .";

				string chatId = update.MyChatMember.From.Id.ToString();

				await TelegramCommunicator.SendText(chatId, response);
			}
		}
	}
}