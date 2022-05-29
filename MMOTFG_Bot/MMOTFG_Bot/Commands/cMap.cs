using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{/// <summary>
 /// Comando añadido durante el desarrollo de Anacleta la plaqueta.
 /// Muestra una imagen del mapa humano, con la posición actual del jugador remarcada. 
 /// </summary>
	class cMap : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Muestra un mapa del cuerpo humano";
		}

		public override void SetKeywords()
		{
			key_words = new string[] {
				"map",
				"mapa"
			};
		}

		internal override async Task Execute(string command, string chatId, string[] args = null)
		{
			Node n = await Map.getCurrentnode(chatId);
			string imgName = n.Name + "Mapa.PNG";
			await TelegramCommunicator.SendImage(chatId, imgName);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 0) return true;
			return false;
		}
	}
}
