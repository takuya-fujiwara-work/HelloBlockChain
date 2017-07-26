using HelloBlockChain.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HelloBlockChain.Models
{
	public class Block
	{
        [Required]
        public string PrevHash { get; set; }

        [Required]
        public string Nonce { get; set; }

        [Required]
        public string Hash { get; set; }

        [Required]
        public List<Transaction> Txs { get; set; }

        public string Timestamp { get; set; }

        public int Difficulty { get; set; }

        public Block()
        {
            Txs = new List<Transaction>();
        }

        public void CalcBlockHash()
        {
            CalcNonce(GetMessage(), Difficulty);
        }

        private string GetMessage()
        {
            var sb = new StringBuilder();
            foreach (var tx in Txs)
            {
                sb.Append(tx.GetContent());
            }

            sb.Append(PrevHash)
                .Append(Difficulty);

            return sb.ToString();
        }
        private void CalcNonce(string message, int difficulty)
        {
            var hash = string.Empty;
            var nonce = 0;
            string head = string.Empty;

            Enumerable.Range(1, difficulty).ToList().ForEach(a => head += "0");

            using (var sha256 = new SHA256Managed())
            {
                while (nonce < 100000)
                {
                    hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.Unicode.GetBytes(nonce.ToString() + message)));
                    if (difficulty <= hash.Length && hash.StartsWith(head))
                    {
                        break;
                    }
                    nonce++;
                }
            }

            Hash = hash;
            Nonce = nonce.ToString();
        }

        public void UpdateTimestamp()
        {
            Timestamp = DateTime.UtcNow.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Txs.ForEach(x => sb.Append(x.ToString()));

            sb.Append(" PrevHash:").Append(PrevHash)
                .Append(" Dificulty:").Append(Difficulty)
                .Append(" Nonce:").Append(Nonce)
                .Append(" Hash:").Append(Hash);

            return sb.ToString();
        }

        public static Block CreateBlock(List<Transaction> txPool, List<Block> blockChain, int difficulty)
        {
            var block = new Block();
            block.Txs = txPool;
            block.PrevHash = blockChain.Last().Hash;
            block.Difficulty = difficulty;
            block.CalcBlockHash();
            block.UpdateTimestamp();
            return block;
        }

        public bool Verify()
        {
            string head = string.Empty;
            Enumerable.Range(1, Difficulty).ToList().ForEach(a => head += "0");

            if (!Hash.StartsWith(head)) { return false; }

            using (var sha256 = new SHA256Managed())
            {
                if(!Hash.Equals(
                    Convert.ToBase64String(
                        sha256.ComputeHash(
                            Encoding.Unicode.GetBytes(Nonce.ToString() + GetMessage())))))
                {
                    return false;
                }
            }

            var tx = Txs.Skip(1).ToList();
            var isVerified = true;
            tx.ForEach(a => { if (!a.Verify()) { isVerified = false; } });
            if (!isVerified) { return false; }

            // 同一Inputがないか？
            var dic = new Dictionary<string, HashSet<string>>();
            
            tx.ForEach(a => a.Inputs.ForEach(b =>
            {
                if (!dic.ContainsKey(b.PublicKey)) { dic.Add(b.PublicKey, new HashSet<string>()); }
                if(dic[b.PublicKey].Contains(b.TxHash)) { isVerified = false; }
                else { dic[b.PublicKey].Add(b.TxHash); }
            }));
            if (!isVerified) { return false; }

            //generation transaction
            var txGen = Txs.First();
            if(!txGen.Outputs.Sum(d => d.Amount)
                    .Equals(tx.Sum(a => a.Inputs.Sum(b => b.Amount) - a.Outputs.Sum(c => c.Amount)) + 50m)){
                return false;
            }

            return true;
        }
    }
}