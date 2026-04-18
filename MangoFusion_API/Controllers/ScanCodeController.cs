using MangoFusion_API.Data;
using MangoFusion_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;


namespace MangoFusion_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ScanCodeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApiResponse _response;

        public ScanCodeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _response = new ApiResponse();
        }
        [HttpGet]
        public IActionResult GetScanCode(string urlToGenerateScanCode)
        {
            if (string.IsNullOrEmpty(urlToGenerateScanCode))
            {
                return BadRequest("URL is not given, kinldy pass it again");
            }
            using(QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(urlToGenerateScanCode, QRCodeGenerator.ECCLevel.Q);
                using(QRCode qrCode = new QRCode(qRCodeData))
                {
                    using(Bitmap qrCodeImage =qrCode.GetGraphic(20))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            qrCodeImage.Save(memoryStream,ImageFormat.Jpeg);
                            var byteArrayData = memoryStream.ToArray();
                            return File(byteArrayData, "image/jpeg");
                        }
                    }
                }
            }
        }
    }
}
