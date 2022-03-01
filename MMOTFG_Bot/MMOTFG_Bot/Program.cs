﻿using MMOTFG_Bot.Commands;
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
		static Battle battle = null;

		//TO-DO: Esto es un poco bastante feo.
		static ICommand[] commandList = { new cUseItem(), new cAddItem(), new cThrowItem(), new cShowInventory(), new cEquipItem(),
			new cNavigate(), new cDirections(), new cInspectRoom()};

		static async Task Main(string[] args)
		{
			//Set Working Directory
			//Console.WriteLine("Old:" + Directory.GetCurrentDirectory());
			Directory.SetCurrentDirectory("./../../..");
			//Console.WriteLine("New:" + Directory.GetCurrentDirectory());

			string token = "";
            try
            {
				token = System.IO.File.ReadAllText("token.txt");
			}
			catch(FileNotFoundException e)
            {
				Console.WriteLine("ERROR: Token.txt couldn't be found in root directory.");
				Environment.Exit(-1);
            }
			
			var botClient = new TelegramBotClient(token);
			var me = await botClient.GetMeAsync();

			//Module initializers
			TelegramCommunicator.Init(botClient);
			InventorySystem.Init();
			Map.BuildMap("assets/map.json");
			foreach (ICommand c in commandList) c.Init();

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

			if(message.Type == MessageType.Text) //Si le mandas una imagen explota ahora mismo
			{
				List<string> subStrings = message.Text.ToLower().Split(' ').ToList();
				string command = subStrings[0];
				string[] args = new string[subStrings.Count - 1];
				subStrings.CopyTo(1, args, 0, args.Length);

                //TO-DO: Integrarlo con los comandos.
                //Descomentar esto y comentar lo de abajo si se quiere probar el sistema de combate.

                //switch (subStrings[0])
                //{
                //    case "/fight":
                //        Player player = new Player();
                //        Enemy enemy = new Enemy();
                //        battle = new Battle(player, enemy);
                //        battle.setPlayerOptions(chatId);
                //        break;
                //    default:
                //        if (battle != null)
                //        {
                //            battle.playerAttack(chatId, subStrings[0]);
                //        }
                //        break;
                //}

                foreach (ICommand c in commandList)
				{
					if (c.ContainsKeyWord(command, chatId, args)) break;
				}

				//switch (command)
				//{
				//	case ("/add"):
				//		Console.WriteLine("Adding item");
				//		Potion potion = new Potion();
				//		inventorySystem.AddItem(chatId, potion, 3);
				//	break;
				//	case ("/show"):
				//		if (subStrings[1] == "inventory") inventorySystem.ShowInventory(chatId);
				//	break;
				//	case "/fight":
				//		Player player = new Player();
				//		Enemy enemy = new Enemy();
				//		Battle battle = new Battle(player, enemy);

				//	break;
				//}
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