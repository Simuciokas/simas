using System;
using System.Threading.Tasks;
using AsgardMarketplace.Domain.Models;
using AsgardMarketplace.Domain.Repositories;

namespace AsgardMarketplace.Domain.Services
{
    public class OrderService
    {
        AsgardRepository repo;
        public OrderService(AsgardRepository repo)
        {
            this.repo = repo;
        }

        public async Task removeOrders()
        {
            await repo.RemoveOrders();
        }

        public async Task<string> CreateOrder(string buyerId, string sellerId, Item item, int quantity) 
        {
            Order order = new Order(){
                Id = Guid.NewGuid(),
                ItemId = item.Id,
                Quantity = quantity,
                CreatedDate = DateTime.Now,
                State = "Created",
                PaymentId = "",
                PaymentUrl = "",
                Buyer = buyerId,
                Seller = sellerId
            };

            var result = await repo.CreateOrder(order);
            if (!result)
                return "Error saving Order to DB";
            return "Order created succcesfully with ID: " + order.Id;
            
        }

        public async Task<bool> MarkOrderAsPaid(Guid guid, string paymentId, string paymentUrl) 
        {
            var result = await repo.MarkOrderAsPaid(guid, paymentId, paymentUrl);
            return result;
        }
        
        public async Task<bool> MarkOrderAsDelivered(string userId, Guid guid)
        {

            var result = await repo.MarkCompleted(guid);

            if (result.Item1 == true) //If marking successful
            {
                var NotificationService = new NotificationService();

                NotificationService.SendNotification(
                    userId,             // Seller
                    result.Item2.Buyer,  // Buyer
                    guid.ToString(),     // OrderId
                    null,                // PaymentId
                    "Orderred delivered" // Message
                    );
            }

            return result.Item1;
        }
    }
}
