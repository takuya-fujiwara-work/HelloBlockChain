using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HelloBlockChain.Util
{
    public class DigitalSignature
    {

        public static string Sign(string message, string privateKey)
        {
            var sha256 = new SHA256Managed();
            var hash = sha256.ComputeHash(Encoding.Unicode.GetBytes(message));

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm("SHA256");

            return Convert.ToBase64String(rsaFormatter.CreateSignature(hash));
        }

        public static bool Verify(string message, string publicKey, string signature)
        {
            var sha256 = new SHA256Managed();
            var hash = sha256.ComputeHash(Encoding.Unicode.GetBytes(message));

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");

            return rsaDeformatter.VerifySignature(hash, Convert.FromBase64String(signature)); ;
        }

    }
}