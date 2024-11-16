using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Luxury_Shop.Models
{
    public class PaymentWebhookModel
    {
        public bool Status { get; set; }
        public List<TransactionDataModel> Data { get; set; }
    }

    public class TransactionDataModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string TransactionID { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Bank { get; set; }
    }
}