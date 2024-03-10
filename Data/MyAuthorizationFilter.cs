using Hangfire.Dashboard;
using System.Globalization;

namespace BlazorApp1.Data
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // 在此处执行授权检查逻辑，例如检查用户是否拥有访问权限
            // 示例：检查当前用户是否已经认证并且具有特定的角色或声明

            var httpContext = context.GetHttpContext();

            // 例如，检查当前用户是否已经认证并且具有特定的角色
            //if (!httpContext.User.Identity.IsAuthenticated)
            //{
            //    return false;
            //}

            // 示例：检查当前用户是否具有特定的角色
            //if (!httpContext.User.IsInRole("HangfireAdmin"))
            //{
            //    return false;
            //}

            // 允许具有特定角色的用户访问 Hangfire Dashboard
            return true;
        }
    }




}
