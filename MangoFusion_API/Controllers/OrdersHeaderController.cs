using MangoFusion_API.Data;
using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using MangoFusion_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MangoFusion_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/OrdersHeader")]

    public class OrdersHeaderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApiResponse _response;
        public OrdersHeaderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _response = new ApiResponse();
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetOrders(string userId="")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders = _dbContext.OrderHeaders.Include(u => u.OrderDetails).ThenInclude(u => u.MenuItem).OrderByDescending(u => u.OrderHeaderId);

                if (!string.IsNullOrEmpty(userId))
                {
                    orderHeaders = orderHeaders.Where(u => u.ApplicationUserId == userId);
                }
                _response.result = orderHeaders;
                _response.statusCode = HttpStatusCode.OK;
                _response.isSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.errorMessage.Add("Something went Wrong");
                return BadRequest(_response);
            }
            
        }

        [HttpGet("{orderId:int}")]
        public ActionResult<ApiResponse> GetOrders(int orderId)
        {
            try
            {
                if (orderId == 0)
                {
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.errorMessage.Add("Invalid order id");
                    return BadRequest(_response);
                }

                OrderHeader? orderHeaders = _dbContext.OrderHeaders.Include(u => u.OrderDetails).ThenInclude(u => u.MenuItem).FirstOrDefault(u => u.OrderHeaderId == orderId);
                if (orderHeaders == null)
                {
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.errorMessage.Add("Order not found");
                    return NotFound(_response);
                }
                _response.result = orderHeaders;
                _response.statusCode = HttpStatusCode.OK;
                _response.isSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.errorMessage.Add("Something went Wrong");
                return BadRequest(_response);
            }
            
        }

        [HttpPost]
        public ActionResult<ApiResponse> CreateOrder([FromBody]OrderHeaderCreateDTO orderHeaderDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OrderHeader orderHeader = new()
                    {
                        PickupName = orderHeaderDTO.PickupName,
                        PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber,
                        PickupEmail = orderHeaderDTO.PickupEmail,
                        OrderDate = DateTime.Now,
                        OrderTotal = orderHeaderDTO.OrderTotal,
                        Status = StaticDetailForRoles.status_confirmed,
                        TotalItem = orderHeaderDTO.TotalItem,
                        ApplicationUserId = orderHeaderDTO.ApplicationUserId
                    };
                    _dbContext.OrderHeaders.Add(orderHeader);
                    _dbContext.SaveChanges();
                    // getting Order Details.
                    foreach(var orderDetailsDTO in orderHeaderDTO.OrderDetailsDTO)
                    {
                        OrderDetails orderDetails = new()
                        {
                            OrderHeaderId = orderHeader.OrderHeaderId,
                            MenuItemId = orderDetailsDTO.MenuItemId,
                            Quantity = orderDetailsDTO.Quantity,
                            ItemName = orderDetailsDTO.ItemName,
                            Price = orderDetailsDTO.Price
                        };
                        // adding this to db context
                        _dbContext.OrderDetails.Add(orderDetails);
                    }
                    // saving all changes to database of order details at once
                    _dbContext.SaveChanges();
                    _response.result = orderHeader;
                    orderHeader.OrderDetails = [];
                    _response.statusCode = HttpStatusCode.Created;
                    _response.isSuccess = true;
                    return CreatedAtAction(nameof(GetOrders), new { orderId = orderHeader.OrderHeaderId }, _response);

                }
                else
                {
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.errorMessage = ModelState.Values.SelectMany(u => u.Errors).Select(u => u.ErrorMessage).ToList();
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {

                _response.isSuccess = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.errorMessage.Add("Something went Wrong");
                return BadRequest(_response);
            }
        }
        [HttpPut("{orderId:int}")]
        public ActionResult<ApiResponse> UpdateOrder(int orderId,[FromBody] OrderHeaderUpdateDTO orderHeaderDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if(orderId !=orderHeaderDTO.OrderHeaderId)
                    {
                        _response.isSuccess = false;
                        _response.statusCode = HttpStatusCode.BadRequest;
                        _response.errorMessage.Add("Order not found");
                        return BadRequest(_response);
                    }

                    // getting order header Details based on orderID with null check
                    OrderHeader? orderHeaderFromDB = _dbContext.OrderHeaders.FirstOrDefault(u=>u.OrderHeaderId == orderId);

                    if (orderHeaderFromDB == null)
                    {
                        _response.isSuccess = false;
                        _response.statusCode = HttpStatusCode.NotFound;
                        _response.errorMessage.Add("Order not found");
                        return NotFound(_response);
                    }
                    // update order header from payload Order Header DTO
                    if(!string.IsNullOrEmpty(orderHeaderDTO.PickupName))
                    {
                        orderHeaderFromDB.PickupName = orderHeaderDTO.PickupName;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderDTO.PickupEmail))
                    {
                        orderHeaderFromDB.PickupEmail = orderHeaderDTO.PickupEmail;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderDTO.PickupPhoneNumber))
                    {
                        orderHeaderFromDB.PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber;
                    }
                    // update status from DTO request payload
                    if(orderHeaderFromDB.Status.Equals(StaticDetailForRoles.status_confirmed, StringComparison.InvariantCultureIgnoreCase)
                        && orderHeaderDTO.Status.Equals(StaticDetailForRoles.status_readyForPickUp, StringComparison.InvariantCultureIgnoreCase))
                    {
                        orderHeaderFromDB.Status = StaticDetailForRoles.status_readyForPickUp;
                    }
                    if (orderHeaderFromDB.Status.Equals(StaticDetailForRoles.status_readyForPickUp, StringComparison.InvariantCultureIgnoreCase)
                       && orderHeaderDTO.Status.Equals(StaticDetailForRoles.status_completed, StringComparison.InvariantCultureIgnoreCase))
                    {
                        orderHeaderFromDB.Status = StaticDetailForRoles.status_completed;
                    }
                    if (orderHeaderDTO.Status.Equals(StaticDetailForRoles.status_cancelled, StringComparison.InvariantCultureIgnoreCase))
                    {
                        orderHeaderFromDB.Status = StaticDetailForRoles.status_cancelled;
                    }
                    // saving all changes to database of order details at once
                    _dbContext.SaveChanges();
                    _response.statusCode = HttpStatusCode.NoContent;
                    _response.isSuccess = true;
                    return Ok(_response);

                }
                else
                {
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.errorMessage = ModelState.Values.SelectMany(u => u.Errors).Select(u => u.ErrorMessage).ToList();
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {

                _response.isSuccess = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.errorMessage.Add("Something went Wrong");
                return BadRequest(_response);
            }
        }
    }
}
