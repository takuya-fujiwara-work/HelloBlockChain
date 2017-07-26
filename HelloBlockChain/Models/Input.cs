using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloBlockChain.Models
{
    public class Input
    {
        public string TxHash { get; set; }

        public string PublicKey { get; set; }

        public decimal Amount { get; set; }
    }
}