using System.ServiceModel;

namespace Client.Common
{
    [ServiceContract]
    public interface JobServerInterface
    {
        [OperationContract]
        void DownloadJob(out string script, out int jobId);
        [OperationContract]
        void UploadSolution(int jobId, int clientId, dynamic dynamicResult);
        [OperationContract]
        void FetchJobInfo(out int numCompletedJobs);
    }
}