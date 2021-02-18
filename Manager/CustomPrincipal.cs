using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manager
{
    class CustomPrincipal : IPrincipal
    {
        GenericIdentity identity;
        public CustomPrincipal(GenericIdentity identity)
        {
            this.identity = identity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string permission)
        {
            //foreach (ClaimSet claimSet in identity.AuthenticationType.)
            //{
            //    X509CertificateClaimSet certificateClaimSet = claimSet as X509CertificateClaimSet;
            //    if (certificateClaimSet != null)
            //    {
            //        X509Certificate2 certificate = certificateClaimSet.X509Certificate;
            //        if (certificate.SubjectName.Name.Contains(permission))
            //            return true;
            //    }
            //}


            if(identity.Name.Contains($"OU={permission}"))
            {
                return true;
            }



            return false;
        }
    }
}
