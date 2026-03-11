using MangoFusion_API.Data;
using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using MangoFusion_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MangoFusion_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
  
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApiResponse _response;

        public OrderDetailsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _response = new ApiResponse();
        }

        [HttpPut("{orderDetailsId:int}")]
        public ActionResult<ApiResponse> UpdateOrder(int orderDetailsId, [FromBody] OrderDetailsUpdateDto orderDetailsDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (orderDetailsId != orderDetailsDTO.OrderDetailsId)
                    {
                        _response.isSuccess = false;
                        _response.statusCode = HttpStatusCode.BadRequest;
                        _response.errorMessage.Add("Order Details not found");
                        return BadRequest(_response);
                    }

                    // getting order Details based on orderID with null check
                    OrderDetails? orderDetailsFromDB = _dbContext.OrderDetails.FirstOrDefault(u => u.OrderDetailsId == orderDetailsId);

                    if (orderDetailsFromDB == null)
                    {
                        _response.isSuccess = false;
                        _response.statusCode = HttpStatusCode.NotFound;
                        _response.errorMessage.Add("Order Information not found");
                        return NotFound(_response);
                    }
                    // updating the rating
                    orderDetailsFromDB.Rating = orderDetailsDTO.Rating;
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
