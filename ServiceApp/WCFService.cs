using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class WCFService : IWCFContract
    {
        static List<Device> HeatControlDevices = new List<Device>();
        static List<Device> HumidityControlDevices = new List<Device>();
        static List<Device> PressureControlDevices = new List<Device>();
        static List<Device> WindControlDevices = new List<Device>();


        public void TestStore()
        {
            DateTime dt1 = new DateTime(2020, 02, 09, 22, 00, 00);
            DateTime dt2 = new DateTime(2020, 02, 10, 22, 00, 00);
            DateTime dt3 = new DateTime(2020, 02, 11, 22, 00, 00);

            Device thermometer1 = new Device("thermometer1", dt1, "heat", "C", 100);
            Device thermometer2 = new Device("thermometer2", dt2, "heat", "C", 110);
            Device thermometer3 = new Device("thermometer3", dt3, "heat", "C", 120);


            SQLiteDataAccess.SaveHeatControlDevice(thermometer1);
            SQLiteDataAccess.SaveHeatControlDevice(thermometer2);
            SQLiteDataAccess.SaveHeatControlDevice(thermometer3);
        }

        public void TestLoad()
        {
            List<Device> loaded = SQLiteDataAccess.LoadHeatControlDevices();
            foreach (var item in loaded)
            {
                Console.WriteLine($"{item.Name} {item.Timestamp} {item.Group} {item.MeasurementUnit} {item.MeasuredValue}");
            }
        }

        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }

        public void SendMessage(string message, byte[] sign)
        {
            string clienName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string[] partsOfName = clienName.Split(',');

            string clientNameSign = partsOfName[0] + "_sign";
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);

            /// Verify signature using SHA1 hash algorithm
            if (DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("Sign is valid");
            }
            else
            {
                Console.WriteLine("Sign is invalid");
            }

            if(message != "Message")
            {
                string[] parts = message.Split(';');
                Device device = new Device(parts[0], Convert.ToDateTime(parts[1]), parts[2], parts[3], double.Parse(parts[4]));
                
                if(parts[0].Contains("thermometer"))
                {
                    SQLiteDataAccess.SaveHeatControlDevice(device);
                }
                if (parts[0].Contains("hygrometer"))
                {
                    SQLiteDataAccess.SaveHumidityControlDevice(device);
                }
                if (parts[0].Contains("barometer"))
                {
                    SQLiteDataAccess.SavePressureControlDevice(device);
                }
                if (parts[0].Contains("anemometer"))
                {
                    SQLiteDataAccess.SaveWindControlDevice(device);
                }


                Console.WriteLine($"{parts[0]} {parts[1]} {parts[2]} {parts[3]} {parts[4]} successfully added to database.");
            }
        }
    }
}
