using iText.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;


namespace Greensign
{
    class Signature
    {
        public static X509Certificate[] GetChain(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Cert)
        {
            X509Certificate cert = DotNetUtilities.FromX509Certificate(x509Cert);

            X509CertificateParser parser = new X509CertificateParser();
            X509Certificate[] chain = new X509Certificate[]
            {
                parser.ReadCertificate(x509Cert.RawData)
            };

            return chain;
        }
        public static IExternalSignature GetFromX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Cert)
        {
            IExternalSignature externalSignature = new X509Certificate2Signature(x509Cert, "SHA-1");

            return externalSignature;
        }
    }
}
