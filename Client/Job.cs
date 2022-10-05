using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [DataContract]
    internal class Job
    {
        [DataMember]
        internal int id { get; set; }
        [DataMember]
        internal string script { get; set; }
        [DataMember]
        internal List<ClientInfo> clientInfos { get; set; }

        public Job(string script, List<ClientInfo> clientInfos)
        {
            this.id = new Random().Next(10000, 99999);
            this.script = script;
            this.clientInfos = clientInfos;
        }
    }
}
