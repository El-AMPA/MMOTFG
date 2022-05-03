using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
	class Communicator
	{
		protected static string assetsPath = "./assets/";

		public virtual void Init()
        {
		}

		/// <summary>
		/// Send a single image to a user. ImageCaption supports HTML formatting.
		/// </summary>
		public virtual async Task SendImage(string chatId, string imageName, string imageCaption = "")
		{
		}

		/// <summary>
		/// Send a collection of images to a user.
		/// Currently doesn't support captions on individual images because they're not shown as text on chat as
		/// normal images do. You have to open the individual images of the collection to see the text. Not worth
		/// the effort.
		/// </summary>
		public virtual async Task SendImageCollection(string chatId, string[] imagesNames)
		{
		}

		/// <summary>
		/// Send an audio to a user. ImageCaption supports HTML formatting.
		/// </summary>
		public virtual async Task SendAudio(string chatId, string audioName, string audioCaption)
		{
		}

		public virtual async Task SendText(string chatId, string text)
        {
        }

		public virtual async Task SendButtons(string chatId, string text, List<string> buttonNames, int rows = 2, int columns = 2)
        {
		}

		public virtual async Task RemoveReplyMarkup(string chatId)
        {
        }
	}
}
