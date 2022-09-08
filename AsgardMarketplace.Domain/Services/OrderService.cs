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

        public async Task<bool> MarkOrderAsDelivered(string userId1, Guid guid)
        {

            var result = await repo.MarkCompleted(guid);

            if (result.Item1 == true) //If marking successful
            {
                var NotificationService = new NotificationService();

                NotificationService.SendNotification(
                    userId1,             // Seller
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
