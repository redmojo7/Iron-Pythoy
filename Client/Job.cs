using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    internal class Job
    {
        internal int Id { get; set; }

        internal string Script { get; set; }
        internal List<ClientInfo> ClientInfos { get; set; }

        public Job(string script, List<ClientInfo> clientInfos)
        {
            this.Id = new Random().Next(10000, 99999);
            this.Script = script;
            this.ClientInfos = clientInfos;
        }
    }
}
