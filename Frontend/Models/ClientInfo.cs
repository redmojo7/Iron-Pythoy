namespace Frontend.Models
{
    public class ClientInfo
    {
        public ClientInfo(int id, string host, int port, int numCompletedJobs, bool status)
        {
            this.Id = id;
            this.Host = host;
            this.Port = port;
            this.NumCompletedJobs = numCompletedJobs;
            this.Status = status;
        }

        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int NumCompletedJobs { get; set; }
        public bool Status { get; set; }
    }
}