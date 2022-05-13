using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MMOTFG_Bot.Battle
{
    class aScaled : Attack
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToScale;

        public float fixedDamage;

        public aScaled(string name_, float power_, float mpCost_) : base(name_, power_, mpCost_) { }

        public override float GetDamage()
        {
            //ataque de daño fijo
            if (fixedDamage != 0) return fixedDamage;

            //ataque que escala con el otro stat
            return user.GetStat(statToScale) * power;
        }
    }
}
