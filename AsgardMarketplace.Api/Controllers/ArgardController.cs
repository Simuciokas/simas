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
        [HttpGet("items/{Id}")]
        public async Task<ActionResult<Item>> Get(string Id)
        {
            int itemId;
            try {
                itemId = int.Parse(Id.Trim());
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            Item item = await repo.GetItem(itemId);

            if (item.State == "NOT_FOUND")
            {
                return NotFound("Item for id: " + Id + " not found");
            }

            return Ok(item);
        }

        //Purchase item
        [HttpPost("items/purchase/")]
        public async Task<ActionResult<Item>> Post([FromQuery]string buyerId, [FromQuery]string sellerId, [FromQuery]string itemIdString, [FromQuery]string quantityString)
        {
            int itemId;
            try {
                itemId = int.Parse(itemIdString.Trim());
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            int quantity;
            try {
                quantity = int.Parse(quantityString.Trim());
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            Item item = await repo.GetItem(itemId);

            if (item.State == "NOT_FOUND")
            {
                return NotFound("Item for id: " + itemIdString + " not found");
            }
            if (item.Quantity < quantity)
                return Ok("Not enough quantity");
            OrderService orderService = new OrderService(repo);
            var marked = await orderService.CreateOrder(buyerId, sellerId, item, quantity);
            return Ok(marked);
        }

        //Create an item
        [HttpPost("items/createItem")]
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
        [HttpGet("items/delete/{Id}")]
        public async Task<ActionResult<Item>> DeleteItem(string Id)
        {
            int itemId;
            try {
                itemId = int.Parse(Id.Trim());
            }
            catch (Exception ex) {
                return Ok("Invalid Id");
            }
            Item item = await repo.GetItem(itemId);
            if (item.State != "NOT_FOUND") {
                repo.deleteItem(item);
                return Ok("Item with id: " + itemId + " was successfully deleted");
            }

            return NotFound("Item with id: " + itemId + " was not found");
        }

        #endregion
        #region Orders
        //get?id=00&offset=aa
        //Gets all orders for user
        [HttpGet("orders/")]
        public async Task<ActionResult<Order>> GetOrders([FromQuery]string Id, [FromQuery]int offset, [FromQuery]int limit)
        {
            int userId;
            try {
                userId = int.Parse(Id.Trim());
            }
            catch (Exception ex) {
                return Ok("Invalid user Id");
            }
            List<Order> userOrders = await repo.GetAllOrders(offset, limit, userId);

            if (userOrders.Count() == 0)
            {
                return NotFound();
            }

            return Ok(userOrders);
        }

        //Marks order as delivered
        // Id - Seller user ID
        [HttpPost("markAsDelivered/")]
        public async Task<ActionResult<Order>> MarkOrderAsDelivered([FromQuery]string Id, [FromQuery]string orderId)
        {
            int userId;
            try {
                userId = int.Parse(Id.Trim());
            }
            catch (Exception ex) {
                return Ok("Invalid user Id");
            }
            OrderService orderService = new OrderService(repo);
            
            var marked = await orderService.MarkOrderAsDelivered(userId.ToString(), Guid.Parse(orderId));

            // If marking the order did not failed we return Ok
            if (marked != false)
                return BadRequest();
            else
                return Ok();

        }

        [HttpPost("markAsPaid/")]
        public async Task<ActionResult<Order>> markAsPaid([FromQuery]string Id, [FromQuery]string paymentId, [FromQuery]string paymentUrl)
        {
            OrderService orderService = new OrderService(repo);
            var result = await orderService.MarkOrderAsPaid(Guid.Parse(Id.Trim()), paymentId, paymentUrl);
            if (!result)
                return Ok("An error occured while marking order: " + Id + " as Paid");
            return Ok("Order with id: " + Id + " marked as paid");
        }

        #endregion
    }
}
