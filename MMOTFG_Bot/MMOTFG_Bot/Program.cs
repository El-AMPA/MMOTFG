using MMOTFG_Bot.Commands;
using MMOTFG_Bot.Navigation;
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
using Telegram.Bot.Types.InlineQueryResults;

namespace MMOTFG_Bot
{
	class Program
	{
		static List<ICommand> commandList = new List<ICommand> { new cDebug(), new cCreateCharacter(), new cUseItem(), new cAddItem(), new cThrowItem(),
            new cShowInventory(), new cEquipItem(), new cUnequipItem(), new cInfo(), new cStatus(), new cFight(),
			new cNavigate(), new cDirections(), new cInspectRoom()};

		static async Task Main(string[] args)
		{	
			/*Si estamos depurando en visual studio, tenemos que cambiar la ruta relativa en PC
			* para que funcione igual que en el contenedor de Docker*/
			if (Environment.GetEnvironmentVariable("PLATFORM_PC") != null)
			{
				Console.WriteLine("Estamos en PC");
				Directory.SetCurrentDirectory("./../../..");
			}
			else
			{
				Console.WriteLine("Estamos en Docker");
			}

			string token = "";
			try
			{
				token = System.IO.File.ReadAllText("assets/private/token.txt");
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("No se ha encontrado el archivo token.txt en la carpeta assets.");
				Environment.Exit(-1);
			}

			var botClient = new TelegramBotClient(token);
			var me = await botClient.GetMeAsync();

			//Module initializers
			BattleSystem.Init();
			TelegramCommunicator.Init(botClient);
			InventorySystem.Init();
			Map.Init("assets/map.json", "assets/directionSynonyms.json");
			DatabaseManager.Init();
			foreach (ICommand c in commandList) c.SetKeywords();

			//set attack keywords
			cAttack cAttack = new cAttack();
			cAttack.setKeywords(new Player().attackNames.ConvertAll(s => s.ToLower()).ToArray());
			commandList.Add(cAttack);

			Console.WriteLine("Hello World! I am user " + me.Id + " and my name is " + me.FirstName);

			using var cts = new CancellationTokenSource();

			//var a = new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync);
			botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), null, cts.Token);
			BotCommand command = new BotCommand();

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
				UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
				UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
				//UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
				UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
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

		static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery query)
		{
			InlineQueryResult[] results = {
					//// displayed result
					//new InlineQueryResultArticle(
					//	id: "0",
					//	title: "dumbify your message!",
					//	inputMessageContent: new InputTextMessageContent(
					//		DumbifyText(query.Query)
					//	)
					//)
				};

			await botClient.AnswerInlineQueryAsync(
				inlineQueryId: query.Id,
				results: results,
				isPersonal: true,
				cacheTime: 0);
		}

		static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
		{
			var chatId = message.Chat.Id;
			var senderName = message.From.FirstName;
			var senderID = message.From.Id;

			Console.WriteLine("Received message: " + message.Text + " from " + senderName);

			if(message.Type == MessageType.Text) //Si le mandas una imagen explota ahora mismo
			{
				List<string> subStrings = processMessage(message.Text);
				string command = subStrings[0];
				string[] args = new string[subStrings.Count - 1];
				subStrings.CopyTo(1, args, 0, args.Length);

				bool understoodCommand = false;
				foreach (ICommand c in commandList)
                {
					if (c.ContainsKeyWord(command, chatId, args)) understoodCommand = true;
                }
				if (!understoodCommand) await TelegramCommunicator.SendText(chatId, "That command doesn't exist");
            }
		}

		/// <summary>
		/// Processes the message recieved from the user by filtering out certain chars and splitting it into words
		/// </summary>
		private static List<string> processMessage(string message)
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

	}
}