using HelloBlockChain.Models;
using HelloBlockChain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HelloBlockChain.Controllers
{
    public class BlocksController : ApiController
    {
        public static readonly List<Block> blockChain = new List<Block>();

        static BlocksController()
        {
            var genesis = new Block();
            genesis.Txs.Add(Transaction.CreateTransaction(100m, Constants.GenesisPrivateKey, Constants.KnownPublicKeys["Alice"]));
            genesis.Txs.Add(Transaction.CreateTransaction(50m, Constants.GenesisPrivateKey, Constants.KnownPublicKeys["Alice"]));
            genesis.Txs.Add(Transaction.CreateTransaction(500m, Constants.GenesisPrivateKey, Constants.KnownPublicKeys["Alice"]));
            genesis.Txs.Add(Transaction.CreateTransaction(50m, Constants.GenesisPrivateKey, Constants.KnownPublicKeys["Suzu"]));
            genesis.Txs.Add(Transaction.CreateTransaction(80m, Constants.GenesisPrivateKey, Constants.KnownPublicKeys["White Rabbit"]));

            genesis.Txs.ForEach(a => DigitalSignature.Verify(a.Hash, Constants.KnownPublicKeys["Joe"], a.Signature));

            genesis.PrevHash = string.Empty;
            genesis.Difficulty = Constants.Difficulty;
            genesis.CalcBlockHash();
            genesis.UpdateTimestamp();
            blockChain.Add(genesis);

        }


        [HttpPost]
        public void Post([FromBody]Block block)
        {
            // blockの検証
            if (!block.Verify()) { return; }
            
            blockChain.Add(block);
            TransactionsController.transactionPool.Clear();
        }

        [HttpGet]
        public IEnumerable<Block> Get()
        {
            return blockChain;
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            if (id < 0 || blockChain.Count < id)
            {
                return NotFound();
            }
            return Ok(blockChain[id]);
        }
    }
}
