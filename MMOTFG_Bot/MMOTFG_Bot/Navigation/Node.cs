﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Events;

namespace MMOTFG_Bot.Navigation
{
    /// <summary>
    /// Vertex of the map graph.
    /// </summary>
    class Node
    {
        /// <summary>
        /// Edge of the map graph.
        /// </summary>
        internal class NodeConnection
        {
            public string ConnectingNode
            {
                get;
                set;
            }

            public Node Node;
        }

        internal class TriggerCondition
        {
            public string Name
            {
                get;
                set;
            }

            public bool Condition
            {
                get;
                set;
            }
        }

        internal struct EventCollection
        {
            public Event[] events
            {
                get;
                set;
            }

            public TriggerCondition triggerCondition
            {
                get;
                set;
            }
            
        }

        //TO-DO: Revisar si realmente merece la pena dejarlo como diccionario o pensar en otra estructura.
        public Dictionary<string, NodeConnection> NodeConnections
        {
            get;
            set;
        }

        public List<EventCollection> OnArriveEvent
        {
            get;
            set;
        }

        public List<EventCollection> OnExitEvent
        {
            get;
            set;
        }

        public List<EventCollection> OnInspectEvent
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        } = "";


        /// <summary>
        /// Triggers the OnExit events when leaving the node
        /// </summary>
        public async Task OnExit(long chatId)
        {
            //Load flag marker
            //foreach (Event e in OnExitEvent) e.Execute(chatId);
            await ProgressKeeper.LoadSerializable(chatId);

            if (OnExitEvent != null)
            {
                foreach (EventCollection eColl in OnExitEvent)
                {
                    bool condition = true;

                    if (eColl.triggerCondition != null)
                    {
                        string condName = eColl.triggerCondition.Name;
                        if (condName == "Visited") condName = Name + "Visited";
                        condition =
                            ProgressKeeper.IsFlagActive(chatId, condName) == eColl.triggerCondition.Condition;
                    }

                    if (condition)
                    {
                        foreach (Event ev in eColl.events)
                        {
                            await ev.Execute(chatId);
                        }
                    }
                }
            }

            ProgressKeeper.SetFlagAs(chatId, Name + "Visited", true);
            await ProgressKeeper.SaveSerializable(chatId);
        }

        /// <summary>
        /// Triggers the OnArrive events when entering the node (TO-DO : estos dos métodos son iguales, no repetir código)
        /// </summary>
        public async Task OnArrive(long chatId)
        {
            if (OnArriveEvent != null)
            {
                await ProgressKeeper.LoadSerializable(chatId);

                foreach (EventCollection eColl in OnArriveEvent)
                {
                    bool condition = true;

                    if (eColl.triggerCondition != null)
                    {
                        string condName = eColl.triggerCondition.Name;
                        if (condName == "Visited") condName = Name + "Visited";
                        condition =
                            ProgressKeeper.IsFlagActive(chatId, condName) == eColl.triggerCondition.Condition;
                    }

                    if (condition)
                    {
                        foreach (Event ev in eColl.events)
                        {
                            await ev.Execute(chatId);
                        }
                    }
                }

                await ProgressKeeper.SaveSerializable(chatId);
            }
        }

        /// <summary>
        /// Triggers the OnInspect events when inspecting the node
        /// </summary>
        public async Task OnInspect(long chatId)
        {
            bool triggeredEvent = false;
            if (OnInspectEvent != null)
            {
                await ProgressKeeper.LoadSerializable(chatId);

                foreach (EventCollection eColl in OnInspectEvent)
                {
                    bool condition = true;

                    if (eColl.triggerCondition != null)
                    {
                        string condName = eColl.triggerCondition.Name;
                        if (condName == "Visited") condName = Name + "Visited";
                        condition =
                            ProgressKeeper.IsFlagActive(chatId, condName) == eColl.triggerCondition.Condition;
                    }

                    if (condition)
                    {
                        triggeredEvent = true;
                        foreach (Event ev in eColl.events)
                        {
                            await ev.Execute(chatId);
                        }
                    }
                }

                await ProgressKeeper.SaveSerializable(chatId);
            }
            if(!triggeredEvent) await TelegramCommunicator.SendText(chatId, "There is nothing of interest in here.");
        }

        /// <summary>
        /// Builds a new connection to a node
        /// </summary>
        public void BuildConnection(string direction, Node node)
        {
            //When deserializing the map, the Nodes are not instantiated. Each node knows that they have x connections in
            //y directions but they don't know what node they point to. They just know their name.

            if (NodeConnections.TryGetValue(direction, out NodeConnection connection)) connection.Node = node;
        }

        /// <summary>
        /// Returns whether or not this node is connected to another node in a specified direction.
        /// </summary>
        public bool GetConnectingNode(string direction, out Node connectingNode)
        {
            connectingNode = null;
            if (NodeConnections.TryGetValue(direction, out NodeConnection connection))
            {
                connectingNode = connection.Node;
                return true;
            }
            else return false;
        }
    }
}
