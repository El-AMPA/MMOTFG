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
		static private string assetsPath = "./Assets/";

		private static ITelegramBotClient botClient;

		public static void Init(ITelegramBotClient client)
        {
			botClient = client;
		}

		/// <summary>
		/// Send a single image to a user. ImageCaption supports HTML formatting.
		/// </summary>
		public static async Task SendImage(long chatId, string imageName, string imageCaption = "")
		{
			using (var stream = System.IO.File.OpenRead(assetsPath + imageName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
				//ImageCaption supports emojis! 👏👏
				botClient.SendPhotoAsync(chatId, inputOnlineFile, imageCaption, ParseMode.Html).Wait();
				stream.Close();
			}
		}

		/// <summary>
		/// Send a collection of images to a user.
		/// Currently doesn't support captions on individual images because they're not shown as text on chat as
		/// normal images do. You have to open the individual images of the collection to see the text. Not worth
		/// the effort.
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

			botClient.SendMediaGroupAsync(chatId, media).Wait();

			foreach (var stream in streams) stream.Close();
		}

		/// <summary>
		/// Send an audio to a user. ImageCaption supports HTML formatting.
		/// </summary>
		static public async Task SendAudio(long chatId, string audioName, string audioCaption)
		{
			using (var stream = System.IO.File.OpenRead(assetsPath + audioName))
			{
				InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);

				botClient.SendAudioAsync(chatId, inputOnlineFile, audioCaption, ParseMode.Html).Wait();
				stream.Close();
			}
		}

		static public async Task SendText(long chatId, string text)
        {
			botClient.SendTextMessageAsync(chatId, text, ParseMode.Html).Wait();
        }

		static public async Task SendButtons(long chatId, int buttonNum, string[] buttonNames)
        {
			var keyboard = new KeyboardButton[buttonNum/2][];
			for(int i = 0; i< buttonNum; i+=2)
            {
				keyboard[i/2] = new KeyboardButton[] { 
					new KeyboardButton(buttonNames[i]),
					new KeyboardButton(buttonNames[i+1])
				};
            }
			var rkm = new ReplyKeyboardMarkup(keyboard);
			botClient.SendTextMessageAsync(chatId, "Battle starts!", replyMarkup: rkm).Wait();
		}

		static public async Task RemoveReplyMarkup(long chatId)
        {
			botClient.SendTextMessageAsync(chatId, "Battle ends!", replyMarkup: new ReplyKeyboardRemove()).Wait();
        }
	}
}
