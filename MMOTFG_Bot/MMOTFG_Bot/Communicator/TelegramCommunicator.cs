using System;
using System.Collections.Generic;
using System.IO;
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
		static private string assetsPath = "./../../../assets/"; //TO-DO: Yikes dawg

		private static ITelegramBotClient botClient;

		public static void Init(ITelegramBotClient client)
        {
			botClient = client;
		}

		/// <summary>
		/// Send a single image to a user. ImageCaption supports HTML formatting.
		/// TO-DO: Quitar el botClient de aquí, ahora está aqui porque el warreo es warreo.
		/// </summary>
		public static async Task SendImage(long chatId, string imageName, string imageCaption = "")
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
		static public async Task SendImageCollection(long chatId, string[] imagesNames)
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
		static public async Task SendAudio(long chatId, string audioName, string audioCaption)
		{
			using (var stream = System.IO.File.OpenRead(assetsPath + audioName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
				await botClient.SendAudioAsync(chatId, inputOnlineFile, audioCaption, ParseMode.Html);
				stream.Close();
			}
		}

		static public async Task SendText(long chatId, string text)
        {
			await botClient.SendTextMessageAsync(chatId, text, ParseMode.Html);
        }

		static public async Task SendButtons(long chatId, int buttonNum, string[] buttonNames)
        {
			var rkm = new ReplyKeyboardMarkup();
			var keyboard = new KeyboardButton[buttonNum][];
			for(int i = 0; i< buttonNum; i++)
            {
				keyboard[i] = new KeyboardButton[] { new KeyboardButton(buttonNames[i]) };
            }
			rkm.Keyboard = keyboard;
			await botClient.SendTextMessageAsync(chatId, "Text", ParseMode.Html, null, false, false, 0, false, rkm);
		}
	}
}
