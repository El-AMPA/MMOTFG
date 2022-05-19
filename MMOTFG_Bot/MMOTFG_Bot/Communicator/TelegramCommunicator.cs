using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace MMOTFG_Bot
{
	class TelegramCommunicator
    {
		static private string imagesPath = "./Assets/Images/";
		static private string audiosPath = "./Assets/Audios/";

		private static ITelegramBotClient botClient;

		public static void Init(ITelegramBotClient client)
        {
			botClient = client;
		}

		/// <summary>
		/// Send a single image to a user. ImageCaption supports HTML formatting.
		/// </summary>
		public static async Task SendImage(string chatId, string imageName, bool broadcast = false, string imageCaption = "")
		{
			using (var stream = System.IO.File.OpenRead(imagesPath + imageName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
				//ImageCaption supports emojis! 👏👏
				if (broadcast && await PartySystem.IsInParty(chatId))
				{
					List<Task> tasks = new List<Task>();
					List<string> chatIds = await PartySystem.GetPartyMembers(await PartySystem.GetPartyCode(chatId), true);
					foreach (string id in chatIds) tasks.Add(botClient.SendPhotoAsync(id, inputOnlineFile, imageCaption, ParseMode.Html));
					await Task.WhenAll(tasks);
				}
				else await botClient.SendPhotoAsync(chatId, inputOnlineFile, imageCaption, ParseMode.Html);
				stream.Close();
			}
		}

		/// <summary>
		/// Send a collection of images to a user.
		/// Currently doesn't support captions on individual images because they're not shown as text on chat as
		/// normal images do. You have to open the individual images of the collection to see the text. Not worth
		/// the effort.
		/// </summary>
		static public async Task SendImageCollection(string chatId, string[] imagesNames, bool broadcast = false)
		{
			List<string> chatIds = new List<string>();
			if(broadcast && await PartySystem.IsInParty(chatId)) chatIds = await PartySystem.GetPartyMembers(await PartySystem.GetPartyCode(chatId), true);
			else chatIds.Add(chatId);

			List<Task> tasks = new List<Task>();

			foreach(string id in chatIds)
            {
				List<FileStream> streams = new List<FileStream>();
				List<InputMediaPhoto> media = new List<InputMediaPhoto>();
				foreach (string imageName in imagesNames)
				{
					FileStream stream = System.IO.File.OpenRead(imagesPath + imageName);
					streams.Add(stream);
					media.Add(new InputMediaPhoto(new InputMedia(stream, imageName)));
				}
				tasks.Add(botClient.SendMediaGroupAsync(id, media));
				//foreach (var stream in streams) stream.Close();
			}
			await Task.WhenAll(tasks);
		}

		/// <summary>
		/// Send an audio to a user. ImageCaption supports HTML formatting.
		/// </summary>
		static public async Task SendAudio(string chatId, string audioName, string audioCaption, bool broadcast = false)
		{
			using (var stream = System.IO.File.OpenRead(audiosPath + audioName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);

				if (broadcast && await PartySystem.IsInParty(chatId))
				{
					List<Task> tasks = new List<Task>();
					List<string> chatIds = await PartySystem.GetPartyMembers(await PartySystem.GetPartyCode(chatId), true);
					foreach (string id in chatIds) tasks.Add(botClient.SendAudioAsync(id, inputOnlineFile, audioCaption, ParseMode.Html));
					await Task.WhenAll(tasks);
				}
				else await botClient.SendAudioAsync(chatId, inputOnlineFile, audioCaption, ParseMode.Html);
				stream.Close();
			}
		}

		static public async Task SendText(string chatId, string text, bool broadcast = false, string excludedId = null, ParseMode parseMode = ParseMode.Html)
        {
			if(broadcast && await PartySystem.IsInParty(chatId))
            {
				List<Task> tasks = new List<Task>();
				List<string> chatIds = await PartySystem.GetPartyMembers(await PartySystem.GetPartyCode(chatId), true);
				foreach (string id in chatIds) if (excludedId != id) tasks.Add(botClient.SendTextMessageAsync(id, text));
				await Task.WhenAll(tasks);
            }
			else await botClient.SendTextMessageAsync(chatId, text, parseMode);
        }

		static public async Task SendButtons(string chatId, string text, List<string> buttonNames, int rows = 2, int columns = 2)
        {
			for (int i = buttonNames.Count; i < rows * columns; i++) buttonNames.Add("");
			var keyboard = new List<List<KeyboardButton>>();
			for(int i = 0; i< rows; i++)
            {
				keyboard.Add(new List<KeyboardButton>()); ;
				for (int j = 0; j < columns; j++)
				{
					keyboard[i].Add(new KeyboardButton(buttonNames[i * columns + j]));
				}
			}
			var rkm = new ReplyKeyboardMarkup(keyboard);
			await botClient.SendTextMessageAsync(chatId, text, replyMarkup: rkm);
		}

		static public async Task RemoveReplyMarkup(string chatId, string message)
        {
			await botClient.SendTextMessageAsync(chatId, message, replyMarkup: new ReplyKeyboardRemove());
        }
	}
}
