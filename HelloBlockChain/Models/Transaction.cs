using HelloBlockChain.Controllers;
using HelloBlockChain.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HelloBlockChain.Models
{
    public class Transaction
    {
        [Required]
        public string Signature { get; set; }

        [Required]
        public List<Input> Inputs { get; set; }

        [Required]
        public List<Output> Outputs { get; set; }

        [Required]
        public string Hash { get; set; }

        public Transaction()
        {
            Inputs = new List<Input>();
            Outputs = new List<Output>();
        }

        public void CalcHash()
        {
            using (var sha256 = new SHA256Managed())
            {
                Hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.Unicode.GetBytes(this.GetContent())));
            }
        }

        public void Sign(string privateKey)
        {
            Signature = DigitalSignature.Sign(Hash, privateKey);
        }

        public override string ToString()
        {
            var sw = new StringBuilder();
            sw.Append(" Input: ");
            foreach (Input i in Inputs) { sw.Append(" Hash:" + i.TxHash + " Amount:" + i.Amount); }
            sw.Append(" Output: ");
            foreach (Output o in Outputs) { sw.Append(" PublicKey:" + o.PublicKey + " Amount:" + o.Amount); }
            sw.Append(" Signature:" + Signature);

            return sw.ToString();
        }

        public string GetContent()
        {
            var sw = new StringBuilder();
            foreach (Input i in Inputs) { sw.Append(" Hash:" + i.TxHash + " Amount:" + i.Amount); }
            foreach (Output o in Outputs) { sw.Append(" PublicKey:" + o.PublicKey + " Amount:" + o.Amount); }

            return sw.ToString();
        }

        public static Transaction CreateTransaction(decimal amount, string senderPrivateKey, string receiverPublicKey)
        {
            var o = new Output();
            o.PublicKey = receiverPublicKey;
            o.Amount = amount;
            var tx = new Transaction();
            tx.Outputs.Add(o);
            tx.CalcHash();
            tx.Sign(senderPrivateKey);
            return tx;
        }

        public static Transaction CreateTransaction(List<Block> blockChain, List<Transaction> txPool, string senderPublicKey, string receiverPublicKey, decimal amount, decimal fee, string senderPrivateKey)
        {
            var utxo = CollectUTXO(blockChain, senderPublicKey);

            txPool.ForEach(a => a.Inputs.Where(c => utxo.ContainsKey(c.TxHash) && c.PublicKey.Equals(senderPublicKey))
                .Select(d => d.TxHash)
                .ToList().ForEach(e => 
                utxo.Remove(e)));


            var tmpSum = 0m;
            var tx = new Transaction();
            foreach (var pair in utxo)
            {
                var tmpAmount = pair.Value.Outputs.Where(a => a.PublicKey.Equals(senderPublicKey)).Select(b => b.Amount).Single();
                tx.Inputs.Add(new Input() { TxHash = pair.Value.Hash, PublicKey = senderPublicKey, Amount = tmpAmount });
                tmpSum += tmpAmount;

                if (amount + fee <= tmpSum) break;
            }

            if(tmpSum < amount + fee) { return tx; }

            tx.Outputs.Add(new Output() { PublicKey = receiverPublicKey, Amount = amount });
            tx.Outputs.Add(new Output() { PublicKey = senderPublicKey, Amount = tmpSum - amount - fee });

            tx.CalcHash();
            tx.Sign(senderPrivateKey);
            return tx;
        }

        public static Dictionary<string, Transaction> CollectUTXO(List<Block> blockChain, string senderPublicKey)
        {
            var utxo = new Dictionary<string,Transaction>();

            blockChain.ForEach(a => a.Txs.ForEach(b => utxo.Add(b.Hash, b)));
            blockChain.ForEach(a => a.Txs.ForEach(b => b.Inputs.Where(c => utxo.ContainsKey(c.TxHash) && c.PublicKey.Equals(senderPublicKey))
                .Select(d => d.TxHash)
                .ToList().ForEach(e => utxo.Remove(e))));

            return utxo;
        }

        public bool Verify()
        {
            if(!DigitalSignature.Verify(Hash, Inputs.First().PublicKey, Signature)){ return false; }

            if(Outputs.Sum(a => a.Amount) > Inputs.Sum(b => b.Amount)) { return false; }

            var isVerified = true;
            Inputs.ForEach(a => 
            {
                var utxo = Transaction.CollectUTXO(BlocksController.blockChain, a.PublicKey);
                if(utxo.Values.Where(b => b.Outputs.Any(c => c.PublicKey.Equals(a.PublicKey) && b.Hash.Equals(a.TxHash))).Count() != 1) { isVerified = false; }
            });
            if (!isVerified) { return false; }

            return true;
        }

        public bool VerifyWithTxPool(List<Transaction> txPool)
        {
            var isVerified = true;
            Inputs.ForEach(a => TransactionsController.transactionPool.ForEach(b => b.Inputs.ForEach(c =>
            {
                if (c.TxHash.Equals(a.TxHash) && c.PublicKey.Equals(a.PublicKey)) { isVerified = false; }
            })));
            return isVerified;
        }
    }
}