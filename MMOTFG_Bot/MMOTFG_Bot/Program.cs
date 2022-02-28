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
using System.Data.SqlClient;

namespace MMOTFG_Bot
{
	class Program
	{
		static Player player = new Player();

		static Map map = MapReader.BuildMap("./assets/map.json");

		static List<ICommand> commandList = new List<ICommand>{ new cUseItem(), new cAddItem(), new cThrowItem(),
            new cShowInventory(), new cEquipItem(), new cInfo(), new cStatus(), new cFight()};

		static async Task Main(string[] args)
		{
            //<------CÓDIGO DE SQL------>
            //Server = tcp:localhost,Port; Initial Catalog = databaseName; "
            //	  + "User ID =UserID;Password =Password";

            //Referencia a las connectionStrings
            //https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring?view=dotnet-plat-ext-6.0

            string connectionString =
            "Server=tcp:sqldatabase,1433;Initial Catalog=model;"
            + "User ID =sa;Password = 22MMOTFGDatabasePassword23;"
            + "Timeout = 10";

            // Provide the query string with a parameter placeholder.
            //string queryString =
            //	"SELECT ProductID, UnitPrice, ProductName from dbo.products "
            //		+ "WHERE UnitPrice > @pricePoint "
            //		+ "ORDER BY UnitPrice DESC;";
            string queryString =
                "CREATE TABLE ola (@pricepoint int);";

            // Specify the parameter value.
            int paramValue = 5;

            Console.WriteLine("Creando conexion");

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.AddWithValue("@pricePoint", paramValue);

                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    Console.WriteLine("Conectado");
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("Leyendo");
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(ex.Message);
                }
                //Console.ReadLine();
            }
            //<------FIN DEL CÓDIGO DE SQL------>

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

            //Event arriveEvent = new Event();
            //arriveEvent.addAction(new DescriptorAction("Nunca te gustó mucho la entrada a la facultad, pero depsués de tantos años ya te has acostumbrado."));
            //MapNode entranceNode = new MapNode();
            //entranceNode.onArriveEvent = arriveEvent;
            //mapa.addNode("entrance", entranceNode);

            //arriveEvent = new Event();
            //arriveEvent.addAction(new DescriptorAction("Al aula cinco ya no le queda ni un ápice del brillo que tenía en 2018. Te sientes triste solo de mirar a través de la puerta."));
            //MapNode aulaNode = new MapNode();
            //aulaNode.onArriveEvent = arriveEvent;
            //Event exitEvent = new Event();
            //exitEvent.addAction(new DescriptorAction("Al salir del aula cinco sientes al fantasma de Fede acechándote. Yikes dawg"));
            //aulaNode.onExitEvent = exitEvent;
            //mapa.addNode("aula5", aulaNode);
            //Event lookEvent = new Event();
            //lookEvent.addAction(new DescriptorAction("Encima de la mesa del profesor, observas que hay un pen-drive abandonado. Decides cogerlo"));

            //ItemInfo.setItemName(ItemID.PenDrive, "pendrive misterioso");
            //Event onUseEvent = new Event();
            //onUseEvent.addAction(new DescriptorAction("El pendrive contiene el examen de consolas con la solución de la última práctica. ¡Menudo éxito!"));
            //ItemInfo.addConsumeEvent(ItemID.PenDrive, onUseEvent);

            //lookEvent.addAction(new GiveItemAction(ItemID.PenDrive));
            //aulaNode.onLookEvent = lookEvent;

            //mapa.connectNode("entrance", "aula5", Direction.North);
            //mapa.connectNode("aula5", "entrance", Direction.South);

            //mapa.setPosition("entrance");

            // char c;
            // while (true)
            // {
            //     c = (char)Console.Read();
            //     switch (c)
            //     {
            //         case ('w'):
            //             mapa.navigate(Direction.North);
            //             break;
            //         case ('s'):
            //             mapa.navigate(Direction.South);
            //             break;
            //         case ('d'):
            //             mapa.navigate(Direction.East);
            //             break;
            //         case ('a'):
            //             mapa.navigate(Direction.West);
            //             break;
            //         case ('l'):
            //             mapa.lookAround();
            //             break;
            //         case ('u'):
            //             if (!Inventory.useItem(ItemID.PenDrive))
            //             {
            //                 Console.WriteLine("Ibas a usar un objeto, pero llevas el bolsillo más vacío que la sección de calificaciones del campus virtual");
            //             }
            //             break;
            //     }
            // }

            string token = "";
            try
            {
                token = System.IO.File.ReadAllText("assets/token.txt");
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
			map.BuildMap(); //TO-DO: Static?
			foreach (ICommand c in commandList) c.Init();

			//set attack keywords
			cAttack cAttack = new cAttack();
			cAttack.setKeywords(player.attackNames.ConvertAll(s => s.ToLower()).ToArray());
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
				//UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
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