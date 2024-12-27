using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using NetCoreSpace.Models;
using NetCoreSpace.Services;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using NetCoreSpace.Utilities;
using NetCoreSpace.DTO;
using System.Security.Claims;
using static NetCoreSpace.DTO.ResponseDTO;
using Microsoft.EntityFrameworkCore;
namespace NetCoreSpace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "G_User")]
    public class usersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UsersService _usersService;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public usersController(ApplicationDbContext context
            ,UsersService usersService
            ,IWebHostEnvironment hostingEnvironment) 
        {
            _usersService = usersService;
            _context = context;
            _hostingEnvironment = hostingEnvironment;

        }
        ///// <summary> 
        /////     創造使用者        
        ///// </summary>
        //[HttpPost("CreateUser")]
        //public IActionResult Create([Required] string _account, [Required] string _password, [Required] string _email)
        //{
        //    //string newUserId = _context.InsertUserAccount(_account, _password, _email);
        //    return Ok("");
        //    //return CreatedAtAction(nameof(GetUserById), new { id = newUserId }, item);
        //}

        /// <summary> 
        /// 檢查用户基本訊息
        /// </summary>
        /// <returns></returns> 
        [Authorize]
        [HttpGet("get-user-info")]
        public async Task<ResponseDTO> getUserInfo()
        {
            string? userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Find the target user
            var user = await _context.Users
                .Where(u => u.UserId.ToString() == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                // If user does not exist, return an error response
                return ErrorResponse("Error", new { message = "User does not exist." });
            }


            return SuccessResponse(new { UserInfo = user });
        }




        ///// <summary> 
        ///// 登入使用者帳號
        ///// </summary>
        ///// <param name="model">用戶的登入信息</param>
        ///// <returns>回應 DTO</returns> 
        ///// <remarks>注意事項：此 API 用於登入，請確保傳入有效的帳號與密碼。</remarks> 
        [HttpPost("login")]
        public async Task<ResponseDTO> userLogin([FromBody] LoginDTO model)
        {
            return await _usersService.Login(model);
        }

        /// <summary>
        /// 上傳圖片到伺服器
        /// </summary>
        /// <remarks>限制最大上傳大小為1MB</remarks>
        /// <param name="file">上傳的圖片文件</param>
        /// <returns>
        /// 200 - OK，上傳成功，返回圖片路徑
        /// 400 - Bad Request，無效的請求或檔案問題
        /// </returns>
        [HttpPost("upload-image-file-to-server")]
        public async Task<ResponseDTO> uploadImageFileToServer(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ErrorResponse("請提供有效的檔案進行上傳。");
            }

            if (file.Length > 1 * 1024 * 1024) // 檢查檔案大小是否超過 1MB
            {
                return ResponseDTO.ErrorResponse("上傳失敗，檔案大小超過限制（1MB）。");
            }

            try
            {
                var uploadResults = await FileHelper.UploadImagesAsync(new List<IFormFile> { file }, _hostingEnvironment.WebRootPath);

                if (uploadResults.Count > 0)
                {
                    return ResponseDTO.SuccessResponse(uploadResults);
                }
                else
                {
                    return ResponseDTO.ErrorResponse("上傳失敗，請檢查檔案格式或伺服器配置。");
                }
            }
            catch (Exception ex)
            {
                return ResponseDTO.ErrorResponse($"伺服器內部錯誤：{ex.Message}");
            }
        }



        [HttpGet("get-assign-ip-info")]
        public async Task<IActionResult> getAssignIPInfoAsync([Required]string ipAddress)
        {
            try
            {
                var httpClientService = new http_client_service();
                string response;
                response = await httpClientService.GetNordVPNDataAsync(ipAddress);      
                if (response != null)
                {
                    //string combinedData = $"User IP Address: {ipAddress}\n响应数据：\n{response}";
                    return Content(response);
                }
                else
                {
                    return Content("未能获取响应数据。");
                }
            }
            catch (Exception ex)
            {
                return Content($"发生异常：{ex.Message}");
            }

        }

        [HttpGet("get-client-ip")]
        public async Task<IActionResult> getClientIPAsync()
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP Address not available";
            //return Content($"User IP Address: {ipAddress}");
            //string ipAddress = "36.224.158.30";
            try
            {

                var httpClientService = new http_client_service();
                string response;

                if (ipAddress == "::1" || ipAddress == "127.0.0.1")
                {
                    var mylocalip = await httpClientService.GetLocalPublicIpAddressAsync();
                    response = await httpClientService.GetNordVPNDataAsync(mylocalip);
                }

                else
                {
                    response = await httpClientService.GetNordVPNDataAsync(ipAddress);

                }
                if (response != null)
                {
                    //string combinedData = $"User IP Address: {ipAddress}\n响应数据：\n{response}";
                    return Content(response);
                }
                else
                {
                    return Content("未能获取响应数据。");
                }
            }
            catch (Exception ex)
            {
                return Content($"发生异常：{ex.Message}");
            }

        }

        /// <summary> 
        /// 取得tag群組表
        /// </summary>
        /// <response code="200">OK</response> 
        /// <response code="400">Not found</response> d
        /// <returns></returns> 
        /// <remarks>注意事項</remarks> 
        /// 
        [HttpGet("get-tag-group-details")]
        public async Task<ResponseDTO> getTagGroupDetails()
        {
            var resultList = await _context.VTagGroupDetails.ToListAsync();
            return SuccessResponse(resultList);
        }

        /// <summary> 
        /// Get Api call record table
        /// </summary>
        /// <response code="200">OK</response> 
        /// <response code="400">Not found</response> d
        /// <returns></returns> 
        /// <remarks>注意事項</remarks> 
        /// 
        [HttpGet("get-request-logs")]
        public async Task<ResponseDTO> getRequestLogs()
        {
            var screens = await _context.VTagGroupDetails.ToListAsync();
            return SuccessResponse(screens);
        }


    }
}
