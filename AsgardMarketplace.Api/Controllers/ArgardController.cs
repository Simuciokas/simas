using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsgardMarketplace.Domain.Models;
using AsgardMarketplace.Domain.Repositories;
using AsgardMarketplace.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AsgardMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class AsgardController : ControllerBase
    {
        AsgardRepository repo;

        public AsgardController(AsgardRepository repo)
        {
            this.repo = repo;
        }

        #region Items

        //Gets Item by id
        [HttpGet("items/{id}")]
        public async Task<ActionResult<Item>> Get(string id)
        {
            id = id.ToLower().Trim();
            Item item = await repo.GetItem(id);

            if (item.State != "NOT_FOUND")
            {
                return NotFound("Item for id:" + id + "not found");
            }
            else
            {
                return Ok(item);
            }
        }

        //Create an item
        [HttpPost("items/create-item")]
        public ActionResult<Item> Post(Item item)
        {
            var itemService = new ItemService(repo);
            Item newItem;
            try //trying to create an item
            {
                newItem = itemService.CreateItem(item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(newItem);
        }

        //Deletes item
        [HttpGet("items/delete/{itemId}")]
        public async Task<ActionResult<Item>> DeleteItem(string itemId)
        {
            Item item = await repo.GetItem(itemId.ToString());
            repo.deleteItem(item);

            return NoContent();
        }

        #endregion
        #region Orders

        //Gets all orders for user
        [HttpGet("orders/{userId}")]
        public async Task<ActionResult<Order>> GetOrders(string userId, int ofs, int lmt)
        {

            List<Order> orders = await repo.GetAllOrders(
                ofs, //Offset
                lmt //Limit
                );

            List<Order> userOrders = new List<Order>();
            foreach (Order order in orders)
            {
                if (order.Buyer == userId) //We only want to return orders for this user
                {
                    userOrders.Add(order);
                }
            }

            if (userOrders.Count() == 0)
            {
                return NotFound();
            }

            return Ok(userOrders);
        }

        //Marks order as delivered
        [HttpPost("markAsDelivered/{orderId}")]
        public async Task<ActionResult<Order>> MarkOrderAsDelivered(string user_id_1 /*Seller userId*/, string orderId)
        {
            OrderService orderService = new OrderService(repo);

            var marked = await orderService.MarkOrderAsDelivered(user_id_1, Guid.Parse(orderId));

            // If marking the order did not failed we return Ok
            if (marked != false)
                return BadRequest();
            else
                return Ok();

        }

        #endregion
    }
}
