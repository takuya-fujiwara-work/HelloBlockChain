using HelloBlockChain.Models;
using HelloBlockChain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HelloBlockChain.Controllers
{
    public class UnspentController : ApiController
    {
        [HttpGet]
        public IEnumerable<Transaction> Get(string name)
        {
            if (!Constants.KnownPublicKeys.ContainsKey(name)) { return null; }
            var unspent = Transaction.CollectUTXO(BlocksController.blockChain, Constants.KnownPublicKeys[name]);
            return unspent.Values.ToList();
        }
    }
}
