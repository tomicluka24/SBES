using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp
{
    public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract, IDisposable
    {
        IWCFContract factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
            /// Chaintrust
            Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            factory = this.CreateChannel();
        }

        public void TestCommunication()
        {
            try
            {
                factory.TestCommunication();
            }
            catch (FaultException e)
            {
                Console.WriteLine("[TestCommunication] ERROR : {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunication] ERROR : {0}", e.Message);
            }
        }

        public void SendMessage(string message, byte[] sign)
        {
            try
            {
                factory.SendMessage(message, sign);
            }
            catch (FaultException e)
            {
                Console.WriteLine("[SendMessage] ERROR : {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR : {0}", e.Message);
            }
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }

        public string ReadValuesFromFile(string name)
        {
            string contents = File.ReadAllText($@"C:\Users\Luka\Desktop\SBES\Projekat\Values\{name}.txt");
            return contents;
        }
    }
}
