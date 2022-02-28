//TO-DO: Delete once Navigation/Map.cs is completed

//using System;
//using System.Collections.Generic;
//using System.Text;
//using Telegram.Bot;
//using Telegram.Bot.Exceptions;
//using Telegram.Bot.Extensions.Polling;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;
//using Telegram.Bot.Types.InlineQueryResults;

//namespace MMOTFG_Bot
//{
//	public enum Direction { North, South, East, West, NUM_DIRECTIONS }; //quitar esto porfavor
//	//patrón command
//	public enum ItemID { PenDrive, NUM_ITEMS}; //no se puede tocar desde fuera

//	static class ItemInfo
//	{
//		private static Event[] onConsumeEvents; //listas
//		private static Event[] onInspectEvents;
//		private static string[] itemNames;

//		public static void Init()
//		{
//			onConsumeEvents = new Event[(int)ItemID.NUM_ITEMS];
//			onInspectEvents = new Event[(int)ItemID.NUM_ITEMS];
//			itemNames = new string[(int)ItemID.NUM_ITEMS];
//		}
//		public static void consume(ItemID id) { onConsumeEvents[(int)id].trigger(); }
//		public static void inspect(ItemID id) { onInspectEvents[(int)id].trigger();  }
//		public static string getItemName(ItemID id) { return itemNames[(int)id]; }

//		public static void addConsumeEvent(ItemID id, Event e) {
//			onConsumeEvents[(int)id] = e;
//		}

//		public static void addInspectEvent(ItemID id, Event e)
//		{
//			onInspectEvents[(int)id] = e;
//		}

//		public static void setItemName(ItemID id, string name)
//		{
//			itemNames[(int)id] = name;
//		}
//	}

//	class MapNode
//	{
//		public string name;
//		public string description;
//		public MapNode[] connectedNodes = new MapNode[(int)Direction.NUM_DIRECTIONS]; //listas
//		public Event onArriveEvent, onExitEvent, onLookEvent;

//		/*class MapNode {...}
//		class MapNodeCastle
//		{
//			public onArrive()
//			{
//				....
//			 }
//		}
		
//		 class MapNodeMove {
//		onArrive () {}
//		onExit() {}
//		onLook() { showDescription() }
//		}

//		class MapNodeCastle : MapNodeMove {
//		constructor() {
//		description = "..............";
//			}
//		}

//		class MapNodeMove {
//	string description;
//   constructor (descripition) {...}
//	onArrive () {}
//	onExit() {}
//	onLook() { showDescription() }
//}
//new MapNodeMove("esto es un castillo")
		 
//		 */

//		public MapNode()
//		{
//			//lol
//		}

//		public void onArrive()
//		{
//			if (onArriveEvent != null) onArriveEvent.trigger();
//		}

//		public void onExit()
//		{
//			if (onExitEvent != null) onExitEvent.trigger();
//		}

//		public void onLook()
//		{
//			if  (onLookEvent != null) onLookEvent.trigger();
//		}

//		public void connectNode(MapNode node, Direction dir)
//		{
//			connectedNodes[(int)dir] = node;
//		}

//		public bool isConnected(Direction dir)
//		{
//			return (connectedNodes[(int)dir] != null);
//		}

//		public MapNode getConnectedNode(Direction dir){
//			return connectedNodes[(int)dir];
//		}
//	}

//	class Map
//	{
//		Dictionary<string, MapNode>  nodes;
//		int nNodes = 0;
//		MapNode currentNode; //podría ser string

//		public Map()
//		{
//			nodes = new Dictionary<string, MapNode>();
//		}

//		public void addNode(string name, MapNode node)
//		{
//			nodes[name] = node;
//		}

//		public void navigate(ITelegramBotClient botClient, long chatId, string dir)
//		{
//            Direction direction = stringToDir(dir);
//			if(currentNode.isConnected(direction) && direction != Direction.NUM_DIRECTIONS)
//			{
//				currentNode.onExit();
//				currentNode = currentNode.getConnectedNode(direction);
//				currentNode.onArrive();
//			}
//            botClient.SendTextMessageAsync(chatId: chatId, text: "You moved " + dir); //ESTO ES FEO. PJ NO ABRAS LA PRESENTACION CON ESTO
//		}

//        private Direction stringToDir(string dir){
//            switch(dir) {
//                case ("north"):
//                    return Direction.North;
//                case ("south"):
//                    return Direction.North;
//                case ("east"):
//                    return Direction.East;
//                case ("west"):
//                    return Direction.West;
//                default:
//                    return Direction.NUM_DIRECTIONS; //lol
//            }
//        }

//		public void connectNode(string origin, string destination, Direction dir)
//		{
//			if (!nodes.ContainsKey(origin) || !nodes.ContainsKey(destination)) throw new Exception("No existen los nodos");
//			nodes[origin].connectNode(nodes[destination], dir);
//		}

//		public void setPosition(string node)
//		{
//			currentNode = nodes[node];
//			currentNode.onArrive();
//		}

//		public void lookAround()
//		{
//			currentNode.onLook();
//		}
//	}

//	class Event
//	{
//		private List<Action> actions;

//		public Event() {
//			actions = new List<Action>();
//		}

//		public void addAction(Action a)
//		{
//			actions.Add(a);
//		}

//		public void trigger()
//		{
//			for  (int k = 0; k < actions.Count; k++) actions[k].execute();
//		}
//	}

//	abstract class Action
//	{
//		public abstract void execute();
//	}

//	class DescriptorAction : Action
//	{
//		string text;

//		public DescriptorAction(string t) { text = t;  }

//		public override void execute()
//		{
//			Console.WriteLine(text);
//		}
//	}

//	class GiveItemAction : Action
//	{
//		ItemID item;

//		public GiveItemAction(ItemID it) { 
//			item = it; 
//		}

//		public override void execute()
//		{
//			Console.WriteLine("Has obtenido el item " + ItemInfo.getItemName(item) + ".");
//			//Inventory.addItem(item);
//		}
//	}

//	class ConsumeItemAction : Action
//	{
//		ItemID item;

//		public ConsumeItemAction(ItemID it) { 
//			item = it; 
//		}

//		public override void execute()
//		{
			
//			Console.WriteLine("Has usado el item " + ItemInfo.getItemName(item) + ".");
//			//Inventory.useItem(item);
//		}
//	}
//}
