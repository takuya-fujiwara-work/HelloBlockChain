using HelloBlockChain.Models;
using HelloBlockChain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;

namespace HelloBlockChain.Controllers
{
    public class ClientsController : ApiController
    {
        [HttpPost]
        public void Post([FromBody]Client wallet)
        {
            // Contentsからtransactionを生成
            decimal coin;
            if(!decimal.TryParse(wallet.Coin, out coin)) { return; }
            if (!Constants.KnownPublicKeys.ContainsKey(wallet.Receiver)) { return; }

            var tx = Transaction.CreateTransaction(BlocksController.blockChain, TransactionsController.transactionPool, Constants.KnownPublicKeys[Constants.ClientName], Constants.KnownPublicKeys[wallet.Receiver], coin, 0.25m, Constants.PrivateKey);

            if(tx.Signature.Length == 0) { return; }

            // post api/transactions/  知っているノードすべてに対して
            foreach (var uri in Constants.NodeList)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uri);
                    var response = client.PostAsJsonAsync("api/transactions", tx).Result;
                }
            }

        }
    }
}
