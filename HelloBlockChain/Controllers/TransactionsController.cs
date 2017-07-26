using HelloBlockChain.Models;
using HelloBlockChain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;

namespace HelloBlockChain.Controllers
{
    public class TransactionsController : ApiController
    {
        public static List<Transaction> transactionPool = new List<Transaction>();

        [HttpPost]
        public void Post([FromBody]Transaction tx)
        {
            if (!tx.Verify()){ return; }

            if (!tx.VerifyWithTxPool(transactionPool)) { return; }

            transactionPool.Add(tx);

            if (transactionPool.Count % 3 == 0 && Constants.IsMiner)
            {
                // create generation transaction
                transactionPool.Insert(0, Transaction.CreateTransaction(transactionPool.Sum(a => a.Inputs.Sum(b => b.Amount) - a.Outputs.Sum(c => c.Amount)) + 50m, Constants.PrivateKey, Constants.KnownPublicKeys["Jing"]));

                // create block
                var block = Block.CreateBlock(transactionPool, BlocksController.blockChain, Constants.Difficulty);

                // post api/transactions/  知っているノードすべてに対して
                foreach (var uri in Constants.NodeList)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(uri);
                        var response = client.PostAsJsonAsync("api/blocks", block).Result;
                    }
                }
            }
        }

        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            return transactionPool;
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            if (id < 0 || transactionPool.Count < id)
            {
                return NotFound();
            }
            return Ok(transactionPool[id]);
        }
    }
}
