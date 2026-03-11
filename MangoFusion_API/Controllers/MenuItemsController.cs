using MangoFusion_API.Data;
using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MangoFusion_API.Controllers
{
    /// <summary>
    /// Adding Route and Api controller attributes to enable the api calling
    /// </summary>
    [Route("api/MenuItems")]
    [ApiController]
    [AllowAnonymous]
    public class MenuItemsController : Controller
    {
        // adding main class instance to use all over the application
        private readonly ApplicationDbContext _dbContext;
        private readonly ApiResponse _response;
        private readonly IWebHostEnvironment _webEv;


        /// <summary>
        /// Adding constructor
        /// </summary>
        /// <param name="dbContext"></param>
        public MenuItemsController(ApplicationDbContext dbContext, IWebHostEnvironment webEV)
        {
            _dbContext = dbContext;
            _response = new ApiResponse();
            // to get the wwwroot folder
            _webEv = webEV;
        }
        /// <summary>
        /// adding a Get all to give All menu Items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMenuItems()
        {
            List<MenuItem> menuItems = _dbContext.MenuItems.ToList();
            List<OrderDetails> orderDetailsWithRatings = _dbContext.OrderDetails.Where(u=>u.Rating!=null).ToList();

            foreach (var menuItem in menuItems)
            {
                var ratings = orderDetailsWithRatings.Where(u => u.MenuItemId == menuItem.id).Select(u => u.Rating.Value);
                menuItem.Rating = ratings.Any() ? ratings.Average() : 0;
            }

            _response.result = menuItems;
            _response.statusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        /// <summary>
        /// Created an api to get menu items by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id=int}", Name = "GetMenuItem")]
        public IActionResult GetMenuItem(int id)
        {
            if (id <= 0)
            {
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.isSuccess = false;
                return BadRequest(_response);
            }
            // Added null check for this menu items
            MenuItem? menuItems = _dbContext.MenuItems.FirstOrDefault(u => u.id == id);
            List<OrderDetails> orderDetails = _dbContext.OrderDetails.Where(u=>u.Rating!= null && u.MenuItem.id == menuItems.id).ToList();
            var ratings = orderDetails.Select(u => u.Rating.Value);
            double avgRating = ratings.Any() ? ratings.Average() : 0;
            menuItems.Rating = avgRating;
            _response.result = menuItems;
            _response.statusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        /// <summary>
        /// Post Method - Add new menu item
        /// </summary>
        /// <param name="menuItemCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
                    {
                        _response.statusCode = HttpStatusCode.BadRequest;
                        _response.isSuccess = false;
                        _response.errorMessage = ["File is Required To Proceed"];
                        return BadRequest(_response);
                    }
                    var imagePath = Path.Combine(_webEv.WebRootPath, "Images");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }
                    var filePath = Path.Combine(imagePath, menuItemCreateDTO.File.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // here it will load the new file and upload it to root directory
                        await menuItemCreateDTO.File.CopyToAsync(stream);
                    }
                    // upload path
                    MenuItem menuItem = new()
                    {
                        name = menuItemCreateDTO.name,
                        description = menuItemCreateDTO.description,
                        category = menuItemCreateDTO.category,
                        specialTag = menuItemCreateDTO.specialTag,
                        price = menuItemCreateDTO.price,
                        image = "Images/" + menuItemCreateDTO.File.FileName
                    };
                    // save changes
                    _dbContext.MenuItems.Add(menuItem);
                    _dbContext.SaveChanges();
                    _response.result = menuItemCreateDTO;
                    _response.statusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetMenuItem", new { id = menuItem.id }, _response);
                }
                else
                {
                    _response.isSuccess = false;

                }
            }
            catch (Exception ex)
            {

                _response.isSuccess = false;
                _response.errorMessage = [ex.ToString()];
            }

            return BadRequest(_response);
        }

        /// <summary>
        /// Update the menu item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menuItemUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemUpdateDTO.id == null || menuItemUpdateDTO.id != id)
                    {
                        _response.statusCode = HttpStatusCode.BadRequest;
                        _response.isSuccess = false;
                        return BadRequest(_response);
                    }

                    // calling old data from DB
                    MenuItem? menuItemsFromDB = await _dbContext.MenuItems.FirstOrDefaultAsync(u => u.id == id);

                    if (menuItemsFromDB == null)
                    {
                        _response.statusCode = HttpStatusCode.NotFound;
                        _response.isSuccess = false;
                        return NotFound(_response);
                    }

                    // map the latest details to menu from DB
                    menuItemsFromDB.name = menuItemUpdateDTO.name;
                    menuItemsFromDB.category = menuItemUpdateDTO.category;
                    menuItemsFromDB.description = menuItemUpdateDTO.description;
                    menuItemsFromDB.specialTag = menuItemUpdateDTO.specialTag;
                    menuItemsFromDB.price = menuItemUpdateDTO.price;

                    // for image we need to do required condtional checks
                    if (menuItemUpdateDTO.File != null && menuItemUpdateDTO.File.Length > 0)
                    {
                        var imagePath = Path.Combine(_webEv.WebRootPath, "Images");
                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }
                        // checking for new value path as a normal process
                        var filePath = Path.Combine(imagePath, menuItemUpdateDTO.File.FileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        // to check file path based on Data base
                        var file_OldPath = Path.Combine(_webEv.WebRootPath, menuItemsFromDB.image);
                        if (System.IO.File.Exists(file_OldPath))
                        {
                            System.IO.File.Delete(file_OldPath);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await menuItemUpdateDTO.File.CopyToAsync(stream);
                        }
                        menuItemsFromDB.image = "Images/" + menuItemUpdateDTO.File.FileName;

                    }
                    // upload path
                    _dbContext.MenuItems.Update(menuItemsFromDB);
                    // save changes
                    _dbContext.SaveChanges();
                    _response.result = menuItemUpdateDTO;
                    _response.statusCode = HttpStatusCode.NoContent;
                    return Ok(_response);
                }
                else
                {
                    _response.isSuccess = false;

                }
            }
            catch (Exception ex)
            {

                _response.isSuccess = false;
                _response.errorMessage = [ex.ToString()];
            }

            return BadRequest(_response);
        }

        /// <summary>
        /// Delete from DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menuItemUpdateDTO"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0)
                    {
                        _response.statusCode = HttpStatusCode.BadRequest;
                        _response.isSuccess = false;
                        return BadRequest(_response);
                    }

                    // calling old data from DB
                    MenuItem? menuItemsFromDB = await _dbContext.MenuItems.FirstOrDefaultAsync(u => u.id == id);

                    if (menuItemsFromDB == null)
                    {
                        _response.statusCode = HttpStatusCode.NotFound;
                        _response.isSuccess = false;
                        return NotFound(_response);
                    }

                    // Delete image path form root folder
                    // to check file path based on Data base
                    var file_OldPath = Path.Combine(_webEv.WebRootPath, menuItemsFromDB.image);
                    if (System.IO.File.Exists(file_OldPath))
                    {
                        System.IO.File.Delete(file_OldPath);
                    }

                    // upload path
                    _dbContext.MenuItems.Remove(menuItemsFromDB);
                    // save changes
                    _dbContext.SaveChanges();
                    _response.statusCode = HttpStatusCode.NoContent;
                    return Ok(_response);
                }
                else
                {
                    _response.isSuccess = false;

                }
            }
            catch (Exception ex)
            {

                _response.isSuccess = false;
                _response.errorMessage = [ex.ToString()];
            }

            return BadRequest(_response);
        }
    }
}
