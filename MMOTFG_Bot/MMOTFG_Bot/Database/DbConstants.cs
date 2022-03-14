using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{

	//IMPORTANT!!!
	//IF YOU CHANGE ANY OF THIS, THE APP WONT BE ABLE TO READ THE DATA STORED IN THE DATABASE PREVIOUSLY

	static class DbConstants
	{
		readonly public static string COLLEC_DEBUG = "PlayersPrueba";

		readonly public static string COLLEC_PLAYERS = "Players";
		readonly public static string COLLEC_OBJECTS = "Objects";


		readonly public static string PLAYER_FIELD_NAME = "name";
		readonly public static string PLAYER_FIELD_TELEGRAM_ID = "telegramId";
		readonly public static string PLAYER_FIELD_INVENTORY = "inventory";
		readonly public static string PLAYER_FIELD_EQUIPABLE_ITEMS = "equipable_items";
		readonly public static string PLAYER_FIELD_ACTUAL_NODE = "actual_node";
	}
}
