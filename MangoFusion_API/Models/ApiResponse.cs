using System.Net;
using System.Runtime.InteropServices.ObjectiveC;

namespace MangoFusion_API.Models
{
    public class ApiResponse
    {
        /// <summary>
        /// Http Status Code will hold here
        /// </summary>
        public HttpStatusCode statusCode { get; set; }
        /// <summary>
        /// Is success check and default value is true
        /// </summary>
        public bool isSuccess { get; set; } = true;
        /// <summary>
        /// error Message detail in list and default value is empty array
        /// </summary>
        public List<string> errorMessage { get; set; } = [];
        /// <summary>
        /// Result in object fromat to share under json object with null check
        /// </summary>
        public object? result { get; set; }
    }
}
