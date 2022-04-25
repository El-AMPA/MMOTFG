using MMOTFG_Bot.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace MMOTFG_Bot
{
    class LevelUpRoadmap
    {
        //base experience needed to level up
        [DefaultValue(1)]
        public float levelUpExperience;

        //1 for linear, 2 for cuadratic, 3 for cubic...
        [DefaultValue(1)]
        public float levelUpExponent;

        [DefaultValue(1)]
        public int minLevel;
        [DefaultValue(100)]
        public int maxLevel;

        //stats at min level
        public float[] firstStats;
        //stats at max level
        public float[] finalStats;

        public struct LevelUpEvent
        {
            public int level;
            public List<Event> events;
        }

        public List<LevelUpEvent> levelUpEvents;

        //experience needed for each level
        public List<int> neededExperience; 

        public void CalculateLevels()
        {
            neededExperience = new List<int>();
            for(int i = minLevel; i < maxLevel; i++)
            {
                int exp = (int)(levelUpExperience * Math.Pow(i, levelUpExponent));
                neededExperience.Add(exp);
            }
        }

        public float getStatDifference(int s)
        {
            return (finalStats[s] - firstStats[s]) / (maxLevel - minLevel);
        }
    }
}
