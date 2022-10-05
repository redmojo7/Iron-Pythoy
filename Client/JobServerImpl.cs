using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class JobServerImpl : JobServerInterface
    {
        public void uploadSolution(int id, int clientId, dynamic dynamicResult)
        {
            if (MyJob.jobs != null && MyJob.jobs.Count > 0)
            {
                foreach (Job tempJob in MyJob.jobs)
                {
                    if (tempJob.id == id)
                    {
                        foreach (ClientInfo info in tempJob.clientInfos)
                        {
                            if (info.Id == clientId) 
                            {
                                info.Answered = true;
                            }
                        }
                    }
                }
            }
        }

        public void DownloadJob(out string script, out int jobId)
        {
            script = null;
             jobId = -1;
            if (MyJob.jobs != null && MyJob.jobs.Count > 0) 
            {
                foreach (Job tempJob in MyJob.jobs) 
                {
                    foreach (ClientInfo info in tempJob.clientInfos)
                    {
                        if (!info.Downloaded) {
                            // return the first job which has not be downloaded by this client
                            jobId = tempJob.id;
                            script = tempJob.script;
                            info.Downloaded = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}
