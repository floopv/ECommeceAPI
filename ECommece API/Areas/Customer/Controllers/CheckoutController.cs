using ECommece_API.DTOs.Response;
using ECommece_API.Models;
using ECommerceAPI.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using static ECommece_API.Utilities.Enums;

namespace ECommece_API.Areas.Customer.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class CheckoutController : ControllerBase
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IEmailSender _emailSender;

        public CheckoutController(IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository, IRepository<Cart> cartRepository, IRepository<Product> productRepository, IEmailSender emailSender)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _emailSender = emailSender;
        }
        [HttpPost("Success")]
        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _orderRepository.GetOneAsync(o => o.Id == orderId, includes: [o => o.ApplicationUser]);
            if (order is null)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "order Not found",
                });
            }
            // send mail 
            var user = order.ApplicationUser;
            await _emailSender.SendEmailAsync(user.Email, " Paymanent", $"<h1> your payment is in Progress</a>  </h1>");

            //  change  status to inprogress 
            order.OrderStatus = OrderStatus.InProgress;
            var service = new SessionService();
            var session = service.Get(order.SessionId);
            order.TransactionId = session.PaymentIntentId;
            await _orderRepository.CommitAsync();
            // add order items from Cart and delete it from Cart 
            var carts = await _cartRepository.GetAllAsync(c => c.ApplicationUserId == user.Id);
            foreach (var item in carts)
            {
                OrderItem orderItem = new OrderItem()
                {
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                    count = item.Count,
                    Price = item.Price,
                };
                await _orderItemRepository.AddAsync(orderItem);
                var product = await _productRepository.GetOneAsync(p => p.Id == item.ProductId);

                // decrease the quentity of the products 
                product.Quantity -= item.Count;
                await _productRepository.CommitAsync();
                _cartRepository.Delete(item);
            }
            await _orderItemRepository.CommitAsync();


            return Ok(new
            {
                msg = "paid Successfully "
            });
        }
        [HttpPost("Cancel")]
        public IActionResult Cancel()
        {
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Canceled successfully"
            });
        }
    }
}
