using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MMOTFG_Bot.Battle
{
    class aScaled : Attack
    {
        public aScaled(string name_, float power_, int mpCost_) : base(name_, power_, mpCost_) { }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToScale;

        public int fixedDamage;

        public override int GetDamage()
        {
            //ataque de daño fijo
            if (fixedDamage != 0) return fixedDamage;

            //ataque que escala con el otro stat
            return (int)(user.GetStat(statToScale) * power);
        }
    }
}
