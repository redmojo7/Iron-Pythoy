using Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Web;

namespace Backend
{
    public class ScheduledTask
    {
        //private static clientdbEntities db = new clientdbEntities();

        private static Timer checkForTime;

        private static long logNumber = 0;

        static long intervalMS = 60 * 1000;

        internal static void Schedule_Timer()
        {

            checkForTime = new Timer(intervalMS);
            Console.WriteLine("### Timer Started ###");
            checkForTime.Elapsed += new ElapsedEventHandler(CheckForTime_Elapsed);
            checkForTime.Start();
        }

        static void CheckForTime_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            System.Console.WriteLine(string.Format("\n[task-{0}][{1}:] {2}", logNumber, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff tt"), "### Timer Stopped ###"));
            checkForTime.Stop();
            Console.WriteLine("### Scheduled Task Started ###");
            Console.WriteLine("Performing scheduled task: check all client if still alive...");
            //
            List<Backend.Models.Client> clients = null;//db.Clients.ToList();
            foreach (Backend.Models.Client client in clients)
            {
                string URL = "net.tcp://" + client.Host + ":" + client.Port + "/JobServer";
                try
                {

                    // for client side
                    ChannelFactory<JobServerInterface> foobFactory;
                    NetTcpBinding netTcpBinding = new NetTcpBinding();
                    //Set the URL and create the connection!
                    Console.WriteLine($"dowmloadJob from {URL}");
                    foobFactory = new ChannelFactory<JobServerInterface>(netTcpBinding, URL);
                    JobServerInterface foob = foobFactory.CreateChannel();
                    int numCompletedJobs = 0;
                    foob.FetchJobInfo(out numCompletedJobs);
                    foobFactory.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception connect to {URL} failed : {e.Message}");
                    // set it status as dead in db

                }
            }


            System.Console.WriteLine(string.Format("[task-{0}][{1}:] {2}", logNumber, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff tt"), "### Task Finished ###\n"));
            logNumber++;
            Schedule_Timer();
        }
    }
}