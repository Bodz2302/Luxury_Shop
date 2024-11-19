using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Luxury_Shop.Models
{
    public class InvoiceStatusResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}