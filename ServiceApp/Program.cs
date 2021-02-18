using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:9999/WCFService";

            ServiceHost host = new ServiceHost(typeof(WCFService));
			host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

	

			// Podesavamo da se koristi CustomAuthorizationManager umesto ugradjenog
			//host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();
			// Podesavamo custom polisu, odnosno nas objekat principala
			host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

			// Podesavamo listu polisa
			List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
			policies.Add(new CustomAuthorizationPolicy());
			host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();


			ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
			if (debug == null)
			{
				host.Description.Behaviors.Add(
					 new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
			}
			else
			{
				// make sure setting is turned ON
				if (!debug.IncludeExceptionDetailInFaults)
				{
					debug.IncludeExceptionDetailInFaults = true;
				}
			}




            /// Chaintrust
			host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
			host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
			host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

			try
			{
				host.Open();
				Console.WriteLine("WCFService is started.\nPress <enter> to stop ...");
				Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("[ERROR] {0}", e.Message);
				Console.WriteLine("[StackTrace] {0}", e.StackTrace);
			}
			finally
			{
				host.Close();
			}
		}
    }
}
