using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security;

namespace Manager
{
	public class CertManager
	{
		/// <summary>
		/// Get a certificate with the specified subject name from the predefined certificate storage
		/// Only valid certificates should be considered
		/// </summary>
		/// <param name="storeName"></param>
		/// <param name="storeLocation"></param>
		/// <param name="subjectName"></param>
		/// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
		public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

			/// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
			string subjectGroup = "";
			

			// Certificate with OU = group of device (ex. CN=heatCtrlDevice, OU=heatCtrl)
			if (subjectName != "wcfService" && !subjectName.Contains("_sign"))
				 subjectGroup = subjectName.Substring(0, subjectName.Length - 6);


			foreach (X509Certificate2 c in certCollection)
			{
				if(subjectName == "wcfService")
                {
					if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
					{
						return c;
					}
				}
				else
                {
					if(subjectName.Contains("_sign"))
                    {
						if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
						{
							return c;
						}
					}
					else 
					{
						if((c.SubjectName.Name.Contains(string.Format("CN={0}, OU={1}", subjectName, subjectGroup))))
						return c;
					}
				}				
				



				if(subjectName != "wcfService" || !subjectName.Contains("_sign"))
                {
					//// Device
					//if (!subjectName.Contains("_sign"))
					//{
						
					//}
					//// Signature
					//else
					//{ 
					//	if (c.SubjectName.Name.Contains(string.Format("CN={0}, OU={1}", subjectName, subjectName += "_sign")))
					//	{
					//		return c;
					//	}
					//}
				}
				//else if(subjectName == "wcfService")
    //            {
				//	if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
				//	{
				//		return c;
				//	}
				//}
				else
                {
					
				}
					

                //Console.WriteLine($"{c.SubjectName.Name}");
			}

			return null;
		}
	}
}