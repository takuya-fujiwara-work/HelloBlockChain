using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelloBlockChain.Models
{
    public class Client
    {
        [Required]
        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Coin { get; set; }
    }
}