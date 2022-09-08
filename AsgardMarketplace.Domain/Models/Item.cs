using System;
using System.ComponentModel.DataAnnotations;

namespace AsgardMarketplace.Domain.Models
{
    public class Item
    {
        public Item()
        {
        }

        public Item(int id, string name, DateTime? date, int q, string userId, float p)
        {
            Id = id;
            Name = name;
            if (date is null)
            {
                CreatedDate = DateTime.Now;
            }
            else
            {
                CreatedDate = date.Value;
            }
            Quantity = q;
            Seller = userId;
            price = p;
            State = "CREATED";
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Quantity { get; set; }
        public string Seller { get; set; }
        public float price { get; set; }
        /* available values: 
            CREATED
            NOT_FOUND
        */
        public string State { get; set; }
    }
}
