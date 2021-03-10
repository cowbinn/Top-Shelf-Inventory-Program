using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace login.classes
{
    public class Orders
    {
        public string OrderID { get; set; }

        public string ClientID { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; }

        public string ShippingMethod { get; set; }

        public string TrackingNumber { get; set; }

        public string DeliveryAddress { get; set; }
    }
}
