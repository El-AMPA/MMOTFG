using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{

	//IMPORTANT!!!
	//IF YOU CHANGE ANY OF THIS, THE APP WONT BE ABLE TO READ THE DATA STORED IN THE DATABASE PREVIOUSLY

	static class DbConstants
	{
		readonly public static string COLLEC_PLAYERS = "Playersv2";
		readonly public static string COLLEC_PARTIES = "Parties";
		readonly public static string COLLEC_OBJECTS = "Objects";

		readonly public static string PLAYER_FIELD_NAME = "name";
		readonly public static string PLAYER_FIELD_TELEGRAM_ID = "telegramId";
		readonly public static string PLAYER_FIELD_INVENTORY = "inventory";
		readonly public static string PLAYER_FIELD_EQUIPABLE_ITEMS = "equipable_items";
		readonly public static string PLAYER_FIELD_ACTUAL_NODE = "actual_node";
		readonly public static string PLAYER_ISINPARTY_FLAG = "is_in_party";
		readonly public static string PLAYER_PARTY_CODE = "player_party_code";
		readonly public static string PLAYER_FIELD_LEVEL = "lvl";
		readonly public static string PLAYER_FIELD_EXPERIENCE = "exp";
		readonly public static string PLAYER_FIELD_ATTACKS = "attacks";
		readonly public static string PLAYER_FIELD_LEARNING_ATTACK = "learning_attack";
		readonly public static string PLAYER_FIELD_FLAGS = "flags";
		readonly public static string PLAYER_FIELD_BATTLE_INFO = "player_battle_info";

		readonly public static string BATTLE_ACTIVE = "is_in_battle";
		readonly public static string BATTLE_ENEMY_LIST = "enemies";
		readonly public static string BATTLE_UP_NEXT = "up_next";
		readonly public static string BATTLE_PAUSED = "battle_paused";

		readonly public static string PARTY_FIELD_CODE = "party_code";
		readonly public static string PARTY_FIELD_LEADER = "party_leaderId";
		readonly public static string PARTY_FIELD_MEMBERS = "party_members";
        readonly public static string PARTY_FIELD_WIPEOUT = "wipeout";
		readonly public static string PARTY_FIELD_MEMBER_INFO = "party_member_info";

		readonly public static string BATTLER_INFO_FIELD_CUR_STATS = "cur_stats";
		readonly public static string BATTLER_INFO_FIELD_OG_STATS = "og_stats";
		readonly public static string BATTLER_INFO_FIELD_MAX_STATS = "max_stats";
		readonly public static string BATTLER_FIELD_NAME = "name";
		readonly public static string BATTLER_FIELD_ITEM_DROP = "item_drop";
		readonly public static string BATTLER_FIELD_ITEM_DROP_AMOUNT = "item_drop_amount";
		readonly public static string BATTLER_FIELD_TURN_OVER = "turn_over";
		readonly public static string BATTLER_FIELD_ON_HIT_FLAGS = "on_hit_flags";
		readonly public static string BATTLER_FIELD_ON_TURN_END_FLAGS = "on_turn_end_flags";
		readonly public static string BATTLER_FIELD_ON_KILL_FLAGS = "on_kill_flags";
	}
}
