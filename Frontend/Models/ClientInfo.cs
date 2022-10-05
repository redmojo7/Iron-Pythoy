namespace Frontend.Models
{
    public class ClientInfo
    {
        public ClientInfo(int id, string host, int port, int numCompletedJobs)
        {
            Id = id;
            Host = host;
            Port = port;
            NumCompletedJobs = numCompletedJobs;
        }

        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int NumCompletedJobs { get; set; }
    }
}