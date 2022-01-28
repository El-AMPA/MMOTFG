using System;
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

		static void Main(string[] args)
		{
			IInteraccionable sal = new Saludador();

			string[] palabras = { "a", "b" };

			ICommand cSaludo = new cMandaSaludo(palabras, sal, "hola");

			cSaludo.miraPalabras("a");

			//Map mapa = new Map();
			//Inventory.Init();

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

			//char c;
			//         while (true)
			//         {
			//	c = (char)Console.Read();
			//             switch (c)
			//             {
			//		case ('w'):
			//			mapa.navigate(Direction.North);
			//			break;
			//		case ('s'):
			//			mapa.navigate(Direction.South);
			//			break;
			//		case ('d'):
			//			mapa.navigate(Direction.East);
			//			break;
			//		case ('a'):
			//			mapa.navigate(Direction.West);
			//			break;
			//		case ('l'):
			//			mapa.lookAround();
			//			break;
			//		case ('u'):
			//                     if (!Inventory.useItem(ItemID.PenDrive))
			//                     {
			//				Console.WriteLine("Ibas a usar un objeto, pero llevas el bolsillo más vacío que la sección de calificaciones del campus virtual");
			//                     }
			//			break;
			//	}
			//         }
		}

 //       static async Task Main(string[] args)
 //       {
	//		var botClient = new TelegramBotClient("1985137093:AAFk7-_Zyc2lSijP5diw2ghWPvmGVHKbB4E");
	//		var me = await botClient.GetMeAsync();
	//		Console.WriteLine("Hello World! I am user " + me.Id + " and my name is " + me.FirstName);

 //           using var cts = new CancellationTokenSource();

 //           botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

 //           Console.WriteLine($"Start listening for @{me.Username}");
 //           Console.ReadLine();

 //           cts.Cancel();
 //       }

	//	static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
	//	{
	//		var handler = update.Type switch
	//		{
	//			// UpdateType.Unknown:
	//			// UpdateType.ChannelPost:
	//			// UpdateType.EditedChannelPost:
	//			// UpdateType.ShippingQuery:
	//			// UpdateType.PreCheckoutQuery:
	//			// UpdateType.Poll:
	//			UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
	//			//UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
	//			//UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
	//			//UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
	//			//UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
	//			//_ => UnknownUpdateHandlerAsync(botClient, update)
	//		};

	//		try
	//		{
	//			await handler;
	//		}
	//		catch (Exception exception)
	//		{
	//			await HandleErrorAsync(botClient, exception, cancellationToken);
	//		}
	//	}

	//	static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery query)
	//	{
	//		InlineQueryResultBase[] results = {
	//				//// displayed result
	//				//new InlineQueryResultArticle(
	//				//	id: "0",
	//				//	title: "dumbify your message!",
	//				//	inputMessageContent: new InputTextMessageContent(
	//				//		DumbifyText(query.Query)
	//				//	)
	//				//)
	//			};

	//		await botClient.AnswerInlineQueryAsync(
	//			inlineQueryId: query.Id,
	//			results: results,
	//			isPersonal: true,
	//			cacheTime: 0);
	//	}

	//	static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
	//	{
	//		var chatId = message.Chat.Id;
	//		var senderName = message.From.FirstName;

	//		Console.WriteLine("Received message: " + message.Text + " from " + senderName);

	//		//await botClient.SendTextMessageAsync(chatId: chatId, text: DumbifyText(message.Text));
	//	}

	//	static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
	//	{
	//		var errorMessage = exception switch
	//		{
	//			ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
	//			_ => exception.ToString()
	//		};
	//		Console.WriteLine(errorMessage);
	//		return Task.CompletedTask;
	//	}

	}
}