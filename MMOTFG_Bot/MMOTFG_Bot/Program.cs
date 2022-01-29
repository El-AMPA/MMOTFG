using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;

namespace MMOTFG_Bot
{
	class Program
	{
		static Map mapa = new Map();
		static string assetsPath = "./../../../assets/"; //TO-DO: Yikes dawg

		static async Task Main(string[] args)
		{
			
			Inventory.Init();

			Event arriveEvent = new Event();
			arriveEvent.addAction(new DescriptorAction("Nunca te gustó mucho la entrada a la facultad, pero depsués de tantos años ya te has acostumbrado."));
			MapNode entranceNode = new MapNode();
			entranceNode.onArriveEvent = arriveEvent;
			mapa.addNode("entrance", entranceNode);

			arriveEvent = new Event();
			arriveEvent.addAction(new DescriptorAction("Al aula cinco ya no le queda ni un ápice del brillo que tenía en 2018. Te sientes triste solo de mirar a través de la puerta."));
			MapNode aulaNode = new MapNode();
			aulaNode.onArriveEvent = arriveEvent;
			Event exitEvent = new Event();
			exitEvent.addAction(new DescriptorAction("Al salir del aula cinco sientes al fantasma de Fede acechándote. Yikes dawg"));
			aulaNode.onExitEvent = exitEvent;
			mapa.addNode("aula5", aulaNode);
			Event lookEvent = new Event();
			lookEvent.addAction(new DescriptorAction("Encima de la mesa del profesor, observas que hay un pen-drive abandonado. Decides cogerlo"));

			ItemInfo.setItemName(ItemID.PenDrive, "pendrive misterioso");
			Event onUseEvent = new Event();
			onUseEvent.addAction(new DescriptorAction("El pendrive contiene el examen de consolas con la solución de la última práctica. ¡Menudo éxito!"));
			ItemInfo.addConsumeEvent(ItemID.PenDrive, onUseEvent);

			lookEvent.addAction(new GiveItemAction(ItemID.PenDrive));
			aulaNode.onLookEvent = lookEvent;

			mapa.connectNode("entrance", "aula5", Direction.North);
			mapa.connectNode("aula5", "entrance", Direction.South);

			mapa.setPosition("entrance");

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

			var botClient = new TelegramBotClient("1985137093:AAFk7-_Zyc2lSijP5diw2ghWPvmGVHKbB4E");
			var me = await botClient.GetMeAsync();

			Console.WriteLine("Hello World! I am user " + me.Id + " and my name is " + me.FirstName);

			using var cts = new CancellationTokenSource();

			botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);
			BotCommand command = new BotCommand();

			Console.WriteLine($"Start listening for @{me.Username}");
			Console.ReadLine();

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

		static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery query)
		{
			InlineQueryResultBase[] results = {
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

			await SendImage(botClient, chatId, "Dog1.png", "<b>I shall smite thee heathen🐶</b>");
			await SendImageCollection(botClient, chatId, new[] { "Dog1.png", "Dog2.png" });
			await SendAudio(botClient, chatId, "DoggoMusic.mp3", "Why do I hear boss music?");

			if(message.Type == MessageType.Text) //Si le mandas una imagen explota ahora mismo
            {
				string[] subStrings = message.Text.ToLower().Split(' ');

				switch (subStrings[0])
				{
					case ("/move"):
						if (subStrings.Length == 2)
						{
							mapa.navigate(botClient, chatId, subStrings[1]);
						}
						break;
				}
			}

			//await botClient.SendTextMessageAsync(chatId: chatId, text: DumbifyText(message.Text));
		}

		/// <summary>
		/// Send a single image to a user. ImageCaption supports HTML formatting.
		/// TO-DO: Quitar el botClient de aquí, ahora está aqui porque el warreo es warreo.
		/// </summary>
		static async Task SendImage(ITelegramBotClient botClient, long chatId, string imageName, string imageCaption = "")
        {
			using (var stream = System.IO.File.OpenRead(assetsPath + imageName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
				//ImageCaption supports emojis! 👏👏
				await botClient.SendPhotoAsync(chatId, inputOnlineFile, imageCaption, ParseMode.Html);
				stream.Close();
			}
		}

		/// <summary>
		/// Send a collection of images to a user.
		/// Currently doesn't support captions on individual images because they're not shown as text on chat as
		/// normal images do. You have to open the individual images of the collection to see the text. Not worth
		/// the effort.
		/// TO-DO: Quitar el botClient de aquí, ahora está aqui porque el warreo es warreo.
		/// </summary>
		static async Task SendImageCollection(ITelegramBotClient botClient, long chatId, string[] imagesNames)
		{
			List<FileStream> streams = new List<FileStream>();
			List<InputMediaPhoto> media = new List<InputMediaPhoto>();
			foreach (string imageName in imagesNames)
            {
				FileStream stream = System.IO.File.OpenRead(assetsPath + imageName);
				streams.Add(stream);
				media.Add(new InputMediaPhoto(new InputMedia(stream, imageName)));
			}

			await botClient.SendMediaGroupAsync(chatId, media);

			foreach (var stream in streams) stream.Close();
		}

		/// <summary>
		/// Send an audio to a user. ImageCaption supports HTML formatting.
		/// TO-DO: Quitar el botClient de aquí, ahora está aqui porque el warreo es warreo.
		/// </summary>
		static async Task SendAudio(ITelegramBotClient botClient, long chatId, string audioName, string audioCaption)
        {
			using (var stream = System.IO.File.OpenRead(assetsPath + audioName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
				await botClient.SendAudioAsync(chatId, inputOnlineFile, audioCaption, ParseMode.Html);
				stream.Close();
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