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
        public async Task<Item> GetItem(string id)
        {
            var item = await context.Items.FindAsync(
                int.Parse(id) //ItemId
                );

            if (item is null)
            {
                item = new Item() { State = "NOT_FOUNT" };
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
        public async Task<List<Order>> GetAllOrders(
            int ofs, //Offset
            int lmt) //Limit
        {
            List<Order> result;
            result = await context.Orders.Skip(ofs).Take(lmt).ToListAsync();
            return result;
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

            bool marked;

            if (order.State != "COMPLETED")
            {
                order.State = "COMPLETED";
                await context.SaveChangesAsync();
                marked = true;
            }
            else
            {
                marked = false;
            }

            return (marked, order);
        }

        #endregion
    }
}
