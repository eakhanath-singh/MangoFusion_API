using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using MangoFusion_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MangoFusion_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        // Adding required configuration Items
        private readonly ApiResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string secretKey;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            // adding secert key along with null check
            secretKey = configuration.GetValue<string>("ApiSettings:Secret") ??"";
            _response = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    NormalizedEmail = model.Email.ToUpper()
                };

                // Using helper methods in .Net Core the below code will create a user with user name and password
                var result = await _userManager.CreateAsync(newUser,model.Password);

                if(result.Succeeded)
                {
                    // Here we will add the Role
                    // cause of this is async method we need to call Awaiter and result
                    if(!_roleManager.RoleExistsAsync(StaticDetailForRoles.Role_Admin).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetailForRoles.Role_Admin));
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetailForRoles.Role_customer));
                    }

                    if(model.Role.Equals(StaticDetailForRoles.Role_Admin, StringComparison.CurrentCultureIgnoreCase))
                    {
                        await _userManager.AddToRoleAsync(newUser,StaticDetailForRoles.Role_Admin);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, StaticDetailForRoles.Role_customer);
                    }

                    _response.statusCode = HttpStatusCode.OK;
                    _response.isSuccess = true;
                    return Ok(_response);

                }
                else
                {
                    _response.statusCode =HttpStatusCode.BadRequest;
                    _response.isSuccess = false;
                    foreach(var error in result.Errors)
                    {
                        _response.errorMessage.Add(error.Description);
                    }
                    return BadRequest(_response);
                }
            }
            else
            {
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.isSuccess = false;
                foreach(var error in ModelState.Values)
                {
                    foreach(var item in error.Errors)
                    {
                        _response.errorMessage.Add(item.ErrorMessage);
                    }
                }
                return BadRequest(_response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = await _userManager.FindByEmailAsync(model.Email);
                if(userFromDb !=null)
                {
                    bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);
                    if(!isValid)
                    {
                        _response.result = new LoginResponseDTO();
                        _response.statusCode = HttpStatusCode.BadRequest;
                        _response.isSuccess = false;
                        _response.errorMessage.Add("Invalid Credentials");
                        return BadRequest(_response);
                    }
                    // enable the JWT token here 
                    JwtSecurityTokenHandler tokenHandler = new();
                    // getting secert key from constructor and encrypting using encoding ASCII Get Bytes.
                    byte[] key = Encoding.ASCII.GetBytes(secretKey);
                    // creating a token descriptor to map a token using key here JWTsecurity token handler has a inhert class of security token handler using that 
                    // we can create a security token handler and add all properties of token descriptor 
                    // token descriptor will have subject-> claimIdentity -> claims and Expires, signing credentials which will hold the key
                    SecurityTokenDescriptor tokenDescriptor = new()
                    {
                        Subject = new ClaimsIdentity(
                            [
                                new("fullname",userFromDb.Name),
                                new("id",userFromDb.Id),
                                // using Claim Types, claim types are default class form cliams identity to get property names and map with user From DB 
                                new(ClaimTypes.Email,userFromDb.Email!.ToString()),
                                // getting role form DB using Get roleAysnc (from DB).Result.FirstorDefault ! -> to null check 
                                new (ClaimTypes.Role,_userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault()!)
                            ]),
                        // setting Expires for 7 days
                        Expires = DateTime.UtcNow.AddDays(7),
                        // getting the key map - > key must be above 256 bits of size
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                    };
                    // mapping security token
                    // Tokenhanler can create a token descriptor 
                    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                    // map this token to login Response
                    LoginResponseDTO loginResponse = new()
                    {
                        Email = userFromDb.Email,
                        Role = _userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault()!,
                        Token = tokenHandler.WriteToken(token)
                    };
                    _response.result = loginResponse;
                    _response.isSuccess = true;
                    _response.statusCode = HttpStatusCode.OK;
                    return Ok(_response);

                }
                _response.result = new LoginResponseDTO();
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.isSuccess = false;
                _response.errorMessage.Add("Invalid User");
                return BadRequest(_response);
            }
            else
            {
                _response.result = new LoginResponseDTO();
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.isSuccess = false;
                _response.errorMessage.Add("Invalid User");
                return BadRequest(_response);
            }
        }
    }
}
