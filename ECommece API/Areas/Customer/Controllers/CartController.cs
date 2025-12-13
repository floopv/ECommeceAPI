using ECommece_API.DTOs.Response;
using ECommece_API.Models;
using ECommerceAPI.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using static ECommece_API.Utilities.Enums;

namespace ECommece_API.Areas.Customer.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class CartController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Order> _orderRepository;
        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Promotion> promotionRepository, IRepository<Product> productRepository, IRepository<Order> orderRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _promotionRepository = promotionRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? code = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "User not found."
                });
            }
            var cartItems = await _cartRepository.GetAllAsync(c => c.ApplicationUserId == user.Id, [p => p.Product]);
            string returnMsg = string.Empty;
            if (code != null)
            {
                var promotion = await _promotionRepository.GetOneAsync(pr => pr.Code == code);
                if (promotion != null)
                {
                    if (promotion.IsValid && promotion.ExpiryDate > DateTime.UtcNow && promotion.MaxUsage > 0)
                    {
                        var applicableCartItem = cartItems.FirstOrDefault(c => c.ProductId == promotion.ProductId);
                        if (applicableCartItem != null)
                        {
                            applicableCartItem.Price -= applicableCartItem.Price * (promotion.Discount / 100);
                            promotion.MaxUsage--;
                            if (promotion.MaxUsage == 0)
                            {
                                promotion.IsValid = false;
                            }
                            _promotionRepository.Update(promotion);
                            await _promotionRepository.CommitAsync();
                            //returnMsg = "Promotion code applied successfully.";
                        }
                        else
                        {
                            returnMsg = "This promotion code is not applicable to any items in your cart.";
                        }
                    }
                    else
                    {
                        promotion.IsValid = false;
                        returnMsg = "This promotion code is no longer valid.";
                    }
                }
                else
                {
                    returnMsg = "Invalid promotion code.";
                }
            }
            var totalAmount = cartItems.Sum(c => c.Price * c.Count);
            if (string.IsNullOrEmpty(returnMsg))
            {
                return Ok(cartItems);
            }
            return BadRequest(new ReturnModelResponse
            {
                ReturnCode = 400,
                ReturnMessage = returnMsg
            });
            
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int ProductId, int Count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "User not found"
                });
            }
            var product = await _productRepository.GetOneAsync(p => p.Id == ProductId);
            if (product == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "product not found"
                });
            }
            if (Count <= 0 || Count > product.Quantity)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400 ,
                    ReturnMessage = "Invalid Number of product"
                });
            }
            var existingCartItem = await _cartRepository.GetOneAsync(c => c.ProductId == ProductId && c.ApplicationUserId == user.Id);
            if (existingCartItem != null)
            {
                existingCartItem.Count += Count;
                _cartRepository.Update(existingCartItem);
            }
            else
            {
                var cart = new Cart()
                {
                    ProductId = product.Id,
                    ApplicationUserId = user.Id,
                    Count = Count,
                    Price = product.Price - (product.Price * product.Discount / 100)
                };
                await _cartRepository.AddAsync(cart);
            }
            await _cartRepository.CommitAsync();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200 ,
                ReturnMessage = "Added To Cart"
            });
        }
        [HttpPut("Increment/{productId}")]
        public async Task<IActionResult> IncrementProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "user not found"
                });
            }
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            var cartItem = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartItem != null)
            {
                if (cartItem.Count >= product.Quantity)
                {
                    return BadRequest(new ReturnModelResponse
                    {
                        ReturnCode = 400 ,
                        ReturnMessage = "count cant exceed product quantity"
                    });
                }
                cartItem.Count += 1;
                _cartRepository.Update(cartItem);
                await _cartRepository.CommitAsync();
            }
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200 ,
                ReturnMessage = "item incremented"
            });
        }
        [HttpPut("Decrement/{productId}")]
        public async Task<IActionResult> DecrementProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "user not found"
                });
            }
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            var cartItem = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartItem != null)
            {
                if (cartItem.Count <= 1)
                {
                    return BadRequest(new ReturnModelResponse
                    {
                        ReturnCode = 400 ,
                        ReturnMessage = "cant be less than 1"
                    });
                }
                cartItem.Count--;
                _cartRepository.Update(cartItem);
                await _cartRepository.CommitAsync();
            }
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "item decremented"
            });
        }
        [HttpPost("Delete/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "user not found"
                });
            }
            var cartItem = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartItem != null)
            {
                _cartRepository.Delete(cartItem);
                await _cartRepository.CommitAsync();
            }
            return NoContent();
        }
        [HttpGet("Payment")]
        public async Task<IActionResult> Payment()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "user not found"
                });
            }
            var cartItems = await _cartRepository.GetAllAsync(c => c.ApplicationUserId == user.Id, [p => p.Product]);
            if (cartItems is null || cartItems.Count() == 0)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "carts empty ",
                });
            }
            Order order = new Order()
            {
                ApplicationUserId = user.Id,
                TotalPrice = cartItems.Sum(c => (c.Price * c.Count)),
                OrderStatus = OrderStatus.Pending,
            };
            await _orderRepository.AddAsync(order);
            await _orderRepository.CommitAsync();
            var orderId = order.Id;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/success?orderId={orderId}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/cancel?orderId={orderId}",
            };
            foreach (var item in cartItems)
            {
                var sessionLineItems = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                        },
                        UnitAmount = (long)item.Price * 100,
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItems);
            }
            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            await _orderRepository.CommitAsync();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = $"check payment from here : {session.Url}"
            });
        }
    }
}
