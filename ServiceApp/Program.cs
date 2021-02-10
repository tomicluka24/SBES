using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/WCFService";

            ServiceHost host = new ServiceHost(typeof(WCFService));
            host.AddServiceEndpoint(typeof(IWCFContract), binding, address);


            WCFService service = new WCFService();

            Console.WriteLine("Testing reading from DB...");
            service.TestLoad();

            Console.WriteLine("\n\nTesting writing to DB...");
            service.TestStore();
            service.TestLoad();


            host.Open();
            Console.WriteLine("WCFService is opened. Press <enter> to finish...");

           
            Console.ReadLine();

            


            host.Close();
        }
    }
}
