using MMOTFG_Bot.Events;
using System.ComponentModel;

namespace MMOTFG_Bot
{
    class Enemy : Battler
    {
        public string imageName;
        public string imageCaption;

        public string droppedItem;
        [DefaultValue(1)]
        public int droppedItemAmount;
        [DefaultValue(1)]
        public int experienceGiven;     

        public override void OnCreate()
        {
            base.OnCreate();
            //by default, every enemy creates a flag upon death
            onKill.Add(new eSetFlag() { Name = name + "Killed", SetAs = true });
        }      
    }
}
