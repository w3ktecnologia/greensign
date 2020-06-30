using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Greensign
{
    class CertStore
    {
        public static X509Certificate2 LoadCertificate()
        {
            var x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection collection = x509Store.Certificates;
            X509Certificate2Collection fcollection = collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            
            X509Certificate2Collection sel = X509Certificate2UI.SelectFromCollection(fcollection, null, null, X509SelectionFlag.SingleSelection);

            if (sel.Count > 0)
            {
                return sel[0];
            }

            return null;
        }
    }
}
