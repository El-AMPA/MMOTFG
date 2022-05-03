using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
	class FileCommunicator: Communicator
	{
		public string InputPath = assetsPath + "Input.txt";
		private string OutputPath = assetsPath + "Output.txt";

		StreamWriter outputFile;

		public FileCommunicator() { }

		public void Init()
		{
			outputFile = new StreamWriter(OutputPath);
		}

		public override async Task SendImage(string chatId, string imageName, string imageCaption = "")
		{
			await outputFile.WriteLineAsync(imageName);
			if(imageCaption != "") await outputFile.WriteLineAsync(imageCaption);
		}

		override public async Task SendImageCollection(string chatId, string[] imagesNames)
		{
			foreach (string imageName in imagesNames)
			{
				await outputFile.WriteLineAsync(imageName);
			}
		}

		override public async Task SendAudio(string chatId, string audioName, string audioCaption)
		{
			await outputFile.WriteLineAsync(audioName);
			if (audioCaption != "") await outputFile.WriteLineAsync(audioCaption);
		}

		override public async Task SendText(string chatId, string text)
		{
			await outputFile.WriteLineAsync(text);
		}

		override public async Task SendButtons(string chatId, string text, List<string> buttonNames, int rows = 2, int columns = 2)
		{
            foreach (string button in buttonNames)
                await outputFile.WriteLineAsync(button);
        }

		override public async Task RemoveReplyMarkup(string chatId)
		{
			await outputFile.WriteLineAsync("Reply Markup removed");
		}
	}
}
