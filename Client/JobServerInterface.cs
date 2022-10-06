using System.ServiceModel;

namespace Desktop.Common
{
    [ServiceContract]
    public interface JobServerInterface
    {
        [OperationContract]
        void DownloadJob(out string script, out byte[] hash, out int jobId);
        [OperationContract]
        void UploadSolution(int jobId, int clientId, dynamic dynamicResult);
        [OperationContract]
        void FetchJobInfo(out int numCompletedJobs);
    }
}