using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedioTest
{
    public class TrackingRequest
    {
        public string Event { get; set; }
        public string tracking { get; set; }
        public long date { get; set; }
        public string message { get; set; }
        public string city { get; set; }
        public long est_delivery_date { get; set; }
        public long first_status_date { get; set; }
        public long late { get; set; }
        public long code { get; set; }
        public long OrderID { get; set; }
        public long TransactionID { get; set; }
        public string TrackingCarrier { get; set; }
        public long MarketplaceID { get; set; }
        public string MarketplacePlatform { get; set; }
        public string ExternalLink { get; set; }
    }
}