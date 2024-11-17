using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Luxury_Shop.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class TransactionResponse
    {
        public bool Status { get; set; }
        public string Messages { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

}