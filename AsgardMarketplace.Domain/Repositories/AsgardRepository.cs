using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsgardMarketplace.Domain.DataModels;
using AsgardMarketplace.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AsgardMarketplace.Domain.Repositories
{
    public class AsgardRepository
    {
        AsgardContext context;
        public AsgardRepository(AsgardContext context)
        {
            this.context = context;
        }

        #region Items
        // Gets item by itemId
        public async Task<Item> GetItem(int id)
        {
            var item = await context.Items.FindAsync(
                id //ItemId
                );

            if (item is null)
            {
                item = new Item() { State = "NOT_FOUND" };
            }
            return item;
        }

        //Insert items
        public async Task<Item> AddItem(
            int id,         // Id for item
            string name,    // Item name
            DateTime? date, // Date when item was created
            int q,          // Quantity of item
            string userId,  // Id of the seller
            float p)        // Item price
        {

            Item item = new Item(id, name, date, q, userId, p);
            var addedItem = context.Items.AddAsync(item);
            await context.SaveChangesAsync();

            return addedItem.Result.Entity;
        }

        //Removes items
        public async void deleteItem(Item item)
        {
            context.Items.Remove(item);
            await context.SaveChangesAsync();
        }

        #endregion
        #region Orders

        //Gets all orders
        public async Task<List<Order>> GetAllOrders(int offset, int limit, int userId)
        {
            List<Order> result;
            result = await context.Orders
                .Where(x => x.Buyer == userId.ToString())
                .Skip(offset).Take(limit).ToListAsync();
            return result;
        }

        //Gets order
        public async Task<Order> GetOrder(Guid orderId)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            return order;
        }

        //Gets order
        public async Task<bool> MarkOrderAsPaid(Guid orderId, string paymentId, string paymentUrl)
        {
            Order order = await GetOrder(orderId);
            order.PaymentId = paymentId;
            order.PaymentUrl = paymentUrl;
            await context.SaveChangesAsync();
            return true;
        }

        // Create Order
        public async Task<bool> CreateOrder(Order order) 
        {
            try {
                context.Orders.Add(order);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Deletes orders that are not completed and older than 2 hours
        public async Task<int> RemoveOrders()
        {
            List<Order> ord;
            ord = await context.Orders.ToListAsync();

            int cnt = 0;
            //Searching for orders that are not completed, but older than 2 hours
            for (int i = 0; i < ord.Count(); i++)
            {
                if (ord[i].CreatedDate.AddMinutes(120) > DateTime.Now && ord[i].State != "COMPLETED")
                {
                    context.Orders.Remove(ord[i]);
                    cnt += 1;
                }
            }

            if (cnt > 0)
            {
                await context.SaveChangesAsync();
            }

            return cnt;

        }

        //Marks order as completed if it was not completed before
        internal async Task<(bool, Order)> MarkCompleted(Guid guid)
        {
            var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == guid);

            if (order.State != "COMPLETED")
            {
                order.State = "COMPLETED";
                await context.SaveChangesAsync();
                return (true, order);
            }
            return (false, order);
        }

        #endregion
    }
}
