using System;
using System.ComponentModel.DataAnnotations;

namespace AsgardMarketplace.Domain.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        /* available values: 
            CREATED
            COMPLETED
        */
        public string State { get; set; }
        //If buyer pays for payment these properties are populated
        public string PaymentId { get; set; }
        public string PaymentUrl { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
    }
}
