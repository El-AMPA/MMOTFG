using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class aStatChanging : Attack
    {
        public aStatChanging(string name_, float power_, int mpCost_) : base(name_, power_, mpCost_) { }

        public StatChange[] statChanges;

        public override string OnAttack() 
        {
            Battler tgt = affectsSelf ? user : target;

            string message = "";

            foreach (StatChange sc in statChanges)
            {
                message += tgt.ApplyStatChange(sc);
            }

            return message;
        }
    }
}
