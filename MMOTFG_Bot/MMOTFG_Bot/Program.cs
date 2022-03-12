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
		static List<ICommand> commandList = new List<ICommand>{new cCreateCharacter(), new cUseItem(), new cAddItem(), new cThrowItem(),
            new cShowInventory(), new cEquipItem(), new cUnequipItem(), new cInfo(), new cStatus(), new cFight(),
			new cNavigate(), new cDirections(), new cInspectRoom(), new cShowGear()};

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





			DatabaseManager.Init();

			//Dictionary<string, Dictionary<string,string>> prueba = new Dictionary<string, Dictionary<string, string>>
			//{+		[1]	{projects/mmotfg-database/databases/(default)/documents/Estructura/players/ObjectList/escudo}	object {Google.Cloud.Firestore.DocumentReference}

			//	{ "y se puede", new Dictionary<string, string>{ {"agregar sin pisar","nice" } } },
			//};

			//await DatabaseManager.addAsync<string, Dictionary<string, string>>(prueba);




			//Dictionary<string, object> wr = new Dictionary<string, object> {

			//	{"uno",1 },
			//	{ "uve", "v"},
			//	{
			//	"mapa", new Dictionary<string, int>{
			//			{"vaya", 2 },

			//	}},
			//	{"que", "chulo" } 
			//};

			//await DatabaseManager.addDocumentToCollection(wr, "mmmh", "Estructura2");

			//Dictionary<string, object> wr2 = new Dictionary<string, object> {

			//	{"hhg",1000 },
			//};

			//await DatabaseManager.modifyDocumentFromCollection(wr2, "mmmh", "Estructura2");

			//Dictionary<string, object>[] ret = await DatabaseManager.getDocumentByFieldValue("nombre", "alba", "prueba");

			//Dictionary<string, object> returned = await DatabaseManager.getDocument("prueba1", "Estructura");
			//Dictionary<string, object> mapaRaw = (Dictionary<string, object>)returned["mapa"];
			//Dictionary<string, string> mapa = new Dictionary<string, string>();

			//foreach (KeyValuePair<string, object> pair in mapaRaw)
			//{
			//	mapa[(string)pair.Key] = (string)pair.Value;
			//}


			//Console.WriteLine("{0}: {1}", 1,1 );

			//foreach (KeyValuePair<string, object> pair in returned)
			//{
			//	Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
			//}


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
			Map.Init("assets/map.json");
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
				List<string> subStrings = message.Text.ToLower().Split(' ').ToList();
				string command = subStrings[0];
				string[] args = new string[subStrings.Count - 1];
				subStrings.CopyTo(1, args, 0, args.Length);

				foreach (ICommand c in commandList)
                {
                    if (c.ContainsKeyWord(command, chatId, args)) break;
                }
            }
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