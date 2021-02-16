using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string srvCertCN = "wcfService";

            /// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
            /// .NET WindowsIdentity class provides information about Windows user running the given process
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

            /// Define subjectName for certificate used for signing which is not as expected by the service
            string wrongCertCN = "wrong_sign";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/WCFService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (WCFClient proxy = new WCFClient(binding, address))
            {

                #region testing
                // Communication test
                proxy.TestCommunication();
                Console.WriteLine("Communication established and tested. Press <enter> to continue ...");
                Console.ReadLine();

                // Digital signing test
                string message = "Message";

                /// Create a signature based on the "signCertCN"

                X509Certificate2 certificateSignTest = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);

                byte[] signatureTest = DigitalSignature.Create(message, HashAlgorithm.SHA1, certificateSignTest);

                proxy.SendMessage(message, signatureTest);
                Console.WriteLine($"SendMessage() using {signCertCN} certificate finished. Press <enter> to continue ...");
                Console.ReadLine();


                // For the same message, create a signature based on the "wrongCertCN"

                X509Certificate2 certificateSignWrong = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, wrongCertCN);

                byte[] signatureWrong = DigitalSignature.Create(message, HashAlgorithm.SHA1, certificateSignWrong);

                proxy.SendMessage(message, signatureWrong);
                Console.WriteLine($"SendMessage() using {wrongCertCN} certificate finished. Press <enter> to continue ...");
                Console.ReadLine();
                #endregion testing


                for (; ;)
                {
                    Console.WriteLine("Press 1 - manual database input \nPress 2 - automatic database input \nPress 0 - Exit");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "0":
                            break;

                        case "1":
                            // Manual input
                            #region setting up device
                            string name = "";
                            string group = "";
                            string measurementUnit = "";

                            int heatDevCounter = 0;
                            int humidityDevCounter = 0;
                            int pressureDevCounter = 0;
                            int windDevCounter = 0;
                            if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name) == "heatCtrlDevice")
                            {
                                name = "thermometer";
                                group = "heatCtrl";
                                measurementUnit = "C";
                                if (heatDevCounter != 0)
                                    name += heatDevCounter.ToString();

                                Console.WriteLine("Enter measured value in [C]: ");

                                heatDevCounter++;
                            }
                            else if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name) == "humidityCtrlDevice")
                            {
                                name = "hygrometer";
                                group = "humidityCtrl";
                                measurementUnit = "%";
                                if (humidityDevCounter != 0)
                                    name += humidityDevCounter.ToString();

                                Console.WriteLine("Enter measured value in [%]: ");

                                humidityDevCounter++;
                            }
                            else if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name) == "pressureCtrlDevice")
                            {
                                name = "barometer";
                                group = "pressureCtrl";
                                measurementUnit = "hPa";
                                if (pressureDevCounter != 0)
                                    name += pressureDevCounter.ToString();

                                Console.WriteLine("Enter measured value in [hPa]: ");

                                pressureDevCounter++;
                            }
                            else
                            {
                                name = "anemometer";
                                group = "windCtrl";
                                measurementUnit = "m/s";
                                if (windDevCounter != 0)
                                    name += windDevCounter.ToString();

                                Console.WriteLine("Enter measured value in [m/s]: ");

                                windDevCounter++;
                            }

                            #endregion setting up device

                            string measuredValue = Console.ReadLine();
                            DateTime timestamp = DateTime.Now;

                            string objectToSend = name + ";" + timestamp + ";" + group + ";" + measurementUnit + ";" + measuredValue;

                            /// Create a signature based on the "signCertCN"
                            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);

                            byte[] signature = DigitalSignature.Create(objectToSend, HashAlgorithm.SHA1, certificateSign);

                            proxy.SendMessage(objectToSend, signature);
                            Console.WriteLine($"SendMessage() using {signCertCN} certificate finished. Press <enter> to continue ...");
                            Console.ReadLine();
                            continue;

                        case "2":
                            //TODO script call

                            break;

                        default:
                            Console.WriteLine("You did not press 1 or 2");
                            continue;
                    }
                    break;
                }
                
            }
        }
    }
}
