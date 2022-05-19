using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Party
    {
        public string code;
        public string leaderId;
        public List<long> members;

        public Party(string code, string leaderId) {
            this.code = code;
            this.leaderId = leaderId;
            members = new List<long>();
        }

        public Dictionary<string, object> getSerializable()
        {
            Dictionary<string, object> partyInfo = new Dictionary<string, object>();

            partyInfo.Add(DbConstants.PARTY_FIELD_CODE, code);
            partyInfo.Add(DbConstants.PARTY_FIELD_LEADER, leaderId);
            partyInfo.Add(DbConstants.PARTY_FIELD_MEMBERS, members);

            return partyInfo;
        }

        public void loadSerializable(Dictionary<string, object> pInfo)
        {
            code = (string)pInfo[DbConstants.PARTY_FIELD_CODE];
            leaderId = (string)pInfo[DbConstants.PARTY_FIELD_LEADER];
            members = (List<long>)pInfo[DbConstants.PARTY_FIELD_CODE];
        }
    }
}
