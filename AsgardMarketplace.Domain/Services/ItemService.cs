using System;
using System.Threading.Tasks;
using AsgardMarketplace.Domain.Models;
using AsgardMarketplace.Domain.Repositories;

namespace AsgardMarketplace.Domain.Services
{
    public class ItemService
    {
        AsgardRepository repo;
        public ItemService(AsgardRepository ar)
        {
            repo = ar;
        }

        public Item CreateItem(Item item)
        {

            /*Quantity validation*/
            if (item.Quantity <= 0 || item.Quantity > 10)
            {
                if (item.Quantity <= 0) //We do not want to allow items created with negative quantity
                {
                    throw new ArgumentException("Quantity must be positive");
                }

                if (item.Quantity > 10)
                {
                    throw new ArgumentException("Invalid quantity");
                }
            }


            /*Price validation*/

            if (item.price <= 0) //We do not want to allow items created with negative quantity
            {
                throw new ArgumentException("Price must be positive");
            }

            var task = Task.Run(async () => await repo.AddItem(
                item.Id,
                item.Name,
                item.CreatedDate,
                item.Quantity,
                item.Seller,
                item.price));
            task.Wait();
            var result = task.Result;

            return result;
        }
    }
}
