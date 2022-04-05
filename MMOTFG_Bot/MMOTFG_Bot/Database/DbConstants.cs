using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{

	//IMPORTANT!!!
	//IF YOU CHANGE ANY OF THIS, THE APP WONT BE ABLE TO READ THE DATA STORED IN THE DATABASE PREVIOUSLY

	static class DbConstants
	{
		readonly public static string COLLEC_DEBUG = "Players";

		readonly public static string COLLEC_PLAYERS = "Players";
		readonly public static string COLLEC_OBJECTS = "Objects";

		readonly public static string PLAYER_FIELD_NAME = "name";
		readonly public static string PLAYER_FIELD_TELEGRAM_ID = "telegramId";
		readonly public static string PLAYER_FIELD_INVENTORY = "inventory";
		readonly public static string PLAYER_FIELD_EQUIPABLE_ITEMS = "equipable_items";
		readonly public static string PLAYER_FIELD_ACTUAL_NODE = "actual_node";
		readonly public static string PLAYER_FIELD_BATTLE_ACTIVE = "is_in_battle";
		readonly public static string PLAYER_FIELD_BATTLE_INFO = "player_battle_info";
		readonly public static string PLAYER_FIELD_BATTLER_LIST = "battler";		
		readonly public static string BATTLER_INFO_FIELD_CUR_STATS = "cur_stats";
		readonly public static string BATTLER_INFO_FIELD_OG_STATS = "og_stats";
		readonly public static string BATTLER_INFO_FIELD_MAX_STATS = "max_stats";
		readonly public static string BATTLER_FIELD_NAME = "name";
		readonly public static string BATTLER_FIELD_MONEY_DROP = "money_drop";
		readonly public static string BATTLER_FIELD_ITEM_DROP = "item_drop";
		readonly public static string BATTLER_FIELD_ITEM_DROP_AMOUNT = "item_drop_amount";
		readonly public static string BATTLER_FIELD_IS_ALLY = "is_ally";
		readonly public static string BATTLER_FIELD_IS_PLAYER = "is_player";
		readonly public static string BATTLER_FIELD_TURN_OVER = "turn_over";
		readonly public static string BATTLER_FIELD_LEVEL = "lvl";
		readonly public static string BATTLER_FIELD_EXPERIENCE = "exp";
		readonly public static string BATTLER_FIELD_ATTACKS = "attacks";
	}
}
