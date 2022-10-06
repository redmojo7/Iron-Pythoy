using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    internal class ClientInfo
    {

        public ClientInfo(string Host, int Port)
        {
            this.Host = Host;
            this.Port = Port;
            this.Status = true;
        }

        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Downloaded { get; set; }
        public bool Answered { get; set; }
        public bool Status { get; set; }
    }
}
