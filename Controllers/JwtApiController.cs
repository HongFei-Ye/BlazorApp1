using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers
{

    [Route("/Api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class JwtApiController : ControllerBase
    {

        [HttpGet("JwtTest1", Name = "GetJwtTest1")]
        public IActionResult Get1()
        {
            // 处理 GET 请求的逻辑
            return Ok("JwtTest1 API GET 请求成功");
        }

        [HttpGet("Test2", Name = "GetJwtTest2")]
        public IActionResult Get2()
        {
            // 处理 GET 请求的逻辑
            return Ok("JwtTest2 API GET 请求成功");
        }



        [HttpPost("JwtTest1", Name = "PostJwtTest1")]
        public IActionResult Post1([FromBody] MyModel model)
        {
            // 处理 POST 请求的逻辑
            // 你可以从请求体中获取传递的数据（例如使用 [FromBody] 特性）
            return Ok("JwtTest1 API POST 请求成功");
        }

        [HttpPost("JwtTest2", Name = "PostJwtTest2")]
        public IActionResult Post2([FromBody] MyModel model)
        {
            // 处理 POST 请求的逻辑
            // 你可以从请求体中获取传递的数据（例如使用 [FromBody] 特性）
            return Ok("JwtTest2 API POST 请求成功");
        }

        [HttpGet("CheckAuthenticationType")]
        public IActionResult CheckAuthenticationType()
        {
            var authenticationType = HttpContext.User.Identity.AuthenticationType;

            return Ok($"API is using {authenticationType} authentication.");
        }

    }



    //BadRequest: 请求无效，常用于表示请求参数不正确或格式错误。
    //Unauthorized: 未授权，常用于表示需要用户进行身份验证或者权限不足。
    //Forbidden: 禁止访问，常用于表示虽然已经进行了身份验证，但是没有权限访问资源。
    //NotFound: 请求的资源未找到，常用于表示请求的资源不存在。
    //InternalServerError: 服务器内部错误，常用于表示处理请求时发生了意外错误。
    //NoContent: 请求成功处理，但不需要返回任何内容，常用于表示删除操作或者一些不需要返回内容的请求。
    //Created: 资源创建成功，通常与 CreatedAtRoute 方法一起使用，用于返回新创建资源的位置和信息。
    //Accepted: 请求已被接受，但处理尚未完成，常用于异步操作。
    //Redirect: 重定向到另一个 URL 地址。
    //File: 返回文件内容。


}
