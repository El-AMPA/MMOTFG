using MMOTFG_Bot.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MMOTFG_Bot.Battle
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

        public List<LevelUpEvent> levelUpEvents;

        //experience needed for each level
        public List<int> neededExperience;

        public struct LevelUpEvent
        {
            public int level;
            public Event ev;
        }

        public void CalculateLevels()
        {
            neededExperience = new List<int>();
            for(int i = minLevel; i < maxLevel; i++)
            {
                int exp = (int)(levelUpExperience * Math.Pow(i, levelUpExponent));
                neededExperience.Add(exp);
            }
        }

        public float GetStatDifference(int s)
        {
            return (finalStats[s] - firstStats[s]) / (maxLevel - minLevel);
        }
    }
}
