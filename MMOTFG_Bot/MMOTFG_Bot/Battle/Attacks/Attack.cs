using JsonSubTypes;
using MMOTFG_Bot.Events;
using MMOTFG_Bot.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot
{
    class Attack
    {
        public string name;
        public float power;
        public int mpCost;
        protected Battler user;
        protected Battler target;

        public bool affectsSelf;

        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(ATK)]
        public StatName statToScale = ATK;

        public int fixedDamage;

        public List<Event> onAttack;

        public Attack(string name_, float power_, int mpCost_)
        {
            name = name_;
            power = power_;
            mpCost = mpCost_;
        }

        public void SetUser(Battler user_)
        {
            user = user_;
        }

        public void SetTarget(Battler target_)
        {
            target = target_;
        }

        public int GetDamage()
        {
            //fixed damage has priority
            if (fixedDamage != 0) return fixedDamage;

            //scaling based on chosen stat
            return (int)(user.GetStat(statToScale) * power);
        }

        public string GetInformation()
        {
            string target = (affectsSelf) ? "User" : "Foe";
            //scaling on attack is assumed
            string scale = (statToScale == ATK) ? "" : $"\nScales from: {statToScale}";
            //power only displayed if not 0
            string powers = (power == 0) ? "" : $"Power: {power}{scale}\n";
            //if fixed damage exists, only display that
            string damage = (fixedDamage == 0) ? $"{powers}" : $"Fixed Damage: {fixedDamage}\n";
            string info = $"Name: {name}\n{damage}MP Cost: {mpCost}\nTarget: {target}\n";

            if (onAttack != null && onAttack.Count > 0)
            {
                info += "Effect:\n";
                foreach (Event e in onAttack)
                {
                    string i = e.GetInformation();
                    if (i != "") info += i + "\n";
                }
            }

            return info;
        }

        public async Task OnAttack(string chatId) {
            if (onAttack != null)
            {
                await ProgressKeeper.LoadSerializable(chatId);

                foreach (Event e in onAttack)
                {
                    e.SetUser(user);
                    if (target != null) e.SetTarget(target);
                    await e.ExecuteEvent(chatId);
                }

                await ProgressKeeper.SaveSerializable(chatId);
            }
        }
    }
}
