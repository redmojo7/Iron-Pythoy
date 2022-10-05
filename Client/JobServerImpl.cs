﻿using Client.Common;
using System.Linq;


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

        public void FetchJobInfo(out int numCompletedJobs)
        {
            numCompletedJobs = 0;
            if (MyJob.jobs != null && MyJob.jobs.Count > 0)
            {
                foreach (Job tempJob in MyJob.jobs)
                {
                    int numCompleted = tempJob.ClientInfos.FindAll(x => x.Answered).Count();
                    int numTotal = tempJob.ClientInfos.Count();
                    if (numCompleted == numTotal)
                    {
                        numCompletedJobs++;
                    }
                }
            }
        }
    }
}