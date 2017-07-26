using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloBlockChain.Util
{
    public class Constants
    {
        const string publicKey1 = "<RSAKeyValue><Modulus>2c2gzQ2Vxny1nAHnn5F5EchtTDl9QwOULhk2ExmwtdY7aAl86nMgin2q47Q+pQwS964PubzhKeFn+xk1hxQunx2oK+JvMaV+mBJp/R7DXzyo8EZLSu7GDDEp+15ffVDUsTVWEVx3P5BrLS29ji9mHiUxcncx6Pz6gGIxQGtKHFU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        const string publicKey2 = "<RSAKeyValue><Modulus>5TpEWiupKrBSF+ge+HkKFRWl3gx90I1krEJdNrLqtIaTrzZqoeTydtRhcm1swqPxbenZj6qK3WPj/m6un1XrRzYLPFaEGJ4eVbpho4hv0C/kMfipMbDJtvyAAGYHYXG4StvbwOd4K1yhZOhAuNLlySTNxGT6sWYwx9y3XLKlHOE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        const string publicKey3 = "<RSAKeyValue><Modulus>3Q51CvvgThNocx/0Nn2br/sqMbXVk9U3+l+D5POjzqRxrBMOagVEeHzsgp5CrU9BRAHQ054qpi6RbD3w+lOrLpwK+OSnjpkouslpsvo4+7hGfQajly4esQM/b2R0/8iGduRPYFHStHojbzxxLVWq75Y0ee3VLDmklXZD4tjxlCk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        const string publicKey4 = "<RSAKeyValue><Modulus>x+aXpKQDa8/wnXMrfqdRCM5mAGNfYs6m5T8f3qwEN3faGoZsOJ3izmTnwIrsXmYxsCT9FJCL9eLeQfxkPshBs/ce/iUuOHnGMAe1HzTL1Js18ffHLK/4f1fM4rnfbtYJg8Prgcnk/JCcBSy7x9lTPy69GvKf8p450pMvoygiaNE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        const string publicKey5 = "<RSAKeyValue><Modulus>txLYHc+0NKbBFhCvTyt1sMhntYuxEq56Cwf10DsP40p7kPqjv/nsjjUgjSfIyxxOSSanvtGxSTJuB3k6fGSVCmuoaUuzj3mcG+O7BJWD7G4XRg5VW943DkyVhmjW49h/C+AxW26arDFqhJFdt+20D/SvefVM3nucTVh/fHtnSxk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        const string publicKey6 = "<RSAKeyValue><Modulus>mV7ZR2bYRRwvjPrECrsEkJmC0iR4nyVNZCP9mgYla7CxyEjT6NdU5hoHoaJcLZCFR7pqwI5uUNJ7VKWKB5WQzeZ5fjFjB8Uw+ywUcelhS9OQS/4WXug8giKLK6LR5F8XPFCHa10j9orcfvufNLLcMgqQdYD8aws81tK7iq4TxtE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static Dictionary<string, string> KnownPublicKeys { get; }

        public static int Difficulty { get; }

        public static List<string> NodeList { get; }

        public static Boolean IsMiner { get; }

        public static string PrivateKey { get; }

        public static string GenesisPrivateKey { get; }

        public static string ClientName { get; }

        static Constants()
        {
            Difficulty = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Difficulty"]);
            NodeList = System.Configuration.ConfigurationManager.AppSettings["WELKNOWN_NODE_LIST"].Split(',').ToList<string>();
            IsMiner = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsMiner"]);
            PrivateKey = System.Configuration.ConfigurationManager.AppSettings["PrivateKey"];
            GenesisPrivateKey = System.Configuration.ConfigurationManager.AppSettings["GenesisPrivateKey"];
            ClientName = System.Configuration.ConfigurationManager.AppSettings["ClientName"];

            KnownPublicKeys = new Dictionary<string, string>();
            KnownPublicKeys.Add("Joe", publicKey1);
            KnownPublicKeys.Add("Alice", publicKey2);
            KnownPublicKeys.Add("Bob", publicKey3);
            KnownPublicKeys.Add("Jing", publicKey4);
            KnownPublicKeys.Add("Suzu", publicKey5);
            KnownPublicKeys.Add("White Rabbit", publicKey6);
        }
    }
}