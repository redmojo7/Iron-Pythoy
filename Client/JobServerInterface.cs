using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [ServiceContract]
    internal interface JobServerInterface
    {
        [OperationContract]
        void DownloadJob(out string script, out int jobId);
        [OperationContract]
        void uploadSolution(int jobId, int clientId, dynamic dynamicResult);
    }
}
