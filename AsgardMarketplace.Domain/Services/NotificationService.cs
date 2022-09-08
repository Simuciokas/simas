using System;
using System.Threading.Tasks;
using AsgardMarketplace.Domain.Models;
using AsgardMarketplace.Domain.Repositories;

namespace AsgardMarketplace.Domain.Services
{
    public class NotificationService
    {
        public NotificationService()
        {
        }


        public void SendNotification(
            string user1, //Seller
            string user2, //Buyer
            string orderId,
            string paymentId,
            string message)
        {
            if (message is null || message.Length == 0 || message == "")
                throw new ArgumentNullException("Message must have a text");

            if (orderId is not null)
            {
                message = "User: " + user2 + message;
                Console.WriteLine(message);
            }
            else
            {
                message = "User: " + user1 + message;
                Console.WriteLine(message);
            }
        }
    }
}
