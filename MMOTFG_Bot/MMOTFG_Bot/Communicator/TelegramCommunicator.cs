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
	class TelegramCommunicator : Communicator
    {
		private static ITelegramBotClient botClient;

		public TelegramCommunicator() { }

		public void Init(ITelegramBotClient client)
        {
			botClient = client;
		}

		/// <summary>
		/// Send a single image to a user. ImageCaption supports HTML formatting.
		/// </summary>
		public override async Task SendImage(string chatId, string imageName, string imageCaption = "")
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
		/// </summary>
		override public async Task SendImageCollection(string chatId, string[] imagesNames)
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
		/// </summary>
		override public async Task SendAudio(string chatId, string audioName, string audioCaption)
		{
			using (var stream = System.IO.File.OpenRead(assetsPath + audioName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);

				await botClient.SendAudioAsync(chatId, inputOnlineFile, audioCaption, ParseMode.Html);
				stream.Close();
			}
		}

		override public async Task SendText(string chatId, string text)
        {
			await botClient.SendTextMessageAsync(chatId, text, parseMode: ParseMode.Html);
        }

		override public async Task SendButtons(string chatId, string text, List<string> buttonNames, int rows = 2, int columns = 2)
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

		override public async Task RemoveReplyMarkup(string chatId)
        {
			await botClient.SendTextMessageAsync(chatId, "Battle ends!", replyMarkup: new ReplyKeyboardRemove());
        }
	}
}
