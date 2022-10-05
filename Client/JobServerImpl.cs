using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class JobServerImpl : JobServerInterface
    {
        public void UploadSolution(int id, int clientId, dynamic dynamicResult)
        {
            if (MyJob.jobs != null && MyJob.jobs.Count > 0)
            {
                foreach (Job tempJob in MyJob.jobs)
                {
                    if (tempJob.Id == id)
                    {
                        foreach (ClientInfo info in tempJob.ClientInfos)
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
                    foreach (ClientInfo info in tempJob.ClientInfos)
                    {
                        if (!info.Downloaded) {
                            // return the first job which has not be downloaded by this client
                            jobId = tempJob.Id;
                            script = tempJob.Script;
                            info.Downloaded = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}
