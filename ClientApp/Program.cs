using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
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

                #region setting up device
                string name = "";
                string group = "";
                string measurementUnit = "";


                if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name).Contains("heatCtrlDevice"))
                {
                    if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name).Equals("heatCtrlDevice"))
                    {
                        name = "thermometer";
                    }
                    else
                    {
                        name = "thermometer2";
                    }
                    group = "heatCtrl";
                    measurementUnit = "C";
                }
                else if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name).Contains("humidityCtrlDevice"))
                {
                    if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name).Equals("humidityCtrlDevice"))
                    {
                        name = "hygrometer";
                    }
                    else
                    {
                        name = "hygrometer2";
                    }
                    group = "humidityCtrl";
                    measurementUnit = "%";
                }
                else if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name).Contains("pressureCtrlDevice"))
                {
                    if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name).Equals("pressureCtrlDevice"))
                    {
                        name = "barometer";
                    }
                    else
                    {
                        name = "barometer2";
                    }
                    group = "pressureCtrl";
                    measurementUnit = "hPa";
                }
                else
                {
                    if (Formatter.ParseName(WindowsIdentity.GetCurrent().Name).Equals("windCtrlDevice"))
                    {
                        name = "anemometer";
                    }
                    else
                    {
                        name = "anemometer2";
                    }
                    group = "windCtrl";
                    measurementUnit = "m/s";
                }

                #endregion setting up device

                for (; ;)
                {
                    Console.WriteLine("Press 1 - manual database input \nPress 2 - automatic database input \nPress 0 - Exit");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "0":
                            break;

                            // Manual input
                        case "1":

                            if (name.Contains("thermometer"))
                            {
                                Console.WriteLine("Enter measured value in [C]: ");
                            }
                            else if (name.Contains("hygrometer"))
                            {
                                Console.WriteLine("Enter measured value in [%]: ");
                            }
                            else if (name.Contains("barometer"))
                            {
                                Console.WriteLine("Enter measured value in [hPa]: ");
                            }
                            else
                            {
                                Console.WriteLine("Enter measured value in [m/s]: ");
                            }
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

                        // Auto input
                        case "2":
                            DateTime timestampAuto = DateTime.Now;
                            string values = proxy.ReadValuesFromFile(name);
                            string[] splitedValues = values.Split(';');

                            string objectToSendAuto = "";

                            /// Create a signature based on the "signCertCN"
                            X509Certificate2 certificateSignAuto = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);

                            for (int i = 0; i < splitedValues.Length; i++)
                            {
                                objectToSendAuto = name + ";" + timestampAuto + ";" + group + ";" + measurementUnit + ";" + splitedValues[i];
                                byte[] signatureAuto = DigitalSignature.Create(objectToSendAuto, HashAlgorithm.SHA1, certificateSignAuto);

                                proxy.SendMessage(objectToSendAuto, signatureAuto);
                                Console.WriteLine($"SendMessage() using {signCertCN} certificate finished. Reading other values ...");
                                Thread.Sleep(2000);
                            }

                            continue;

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
