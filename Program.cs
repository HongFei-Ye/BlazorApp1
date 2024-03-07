using BlazorApp1.Components;
using BlazorApp1.Components.Account;
using BlazorApp1.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();


// 官方模板IDS自动生成的
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();


//var connectionString_EF = builder.Configuration.GetConnectionString("DefaultConnection_EF") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var connectionString_SL = builder.Configuration.GetConnectionString("DefaultConnection_SL") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseSqlServer(connectionString))
    options.UseSqlite(connectionString_SL));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;

    // 默认的账户锁定设置
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60); // 账户锁定时长为60分钟
    options.Lockout.MaxFailedAccessAttempts = 5; // 允许失败登录的最大次数为5次
    options.Lockout.AllowedForNewUsers = false; // 允许新用户被锁定

    // 密码设置
    options.Password.RequireDigit = true; // 密码需要包含数字
    options.Password.RequireLowercase = false; // 密码不需要包含小写字母
    options.Password.RequireNonAlphanumeric = false; // 密码不需要包含特殊字符
    options.Password.RequireUppercase = false; // 密码不需要包含大写字母
    options.Password.RequiredLength = 6; // 密码最小长度为6个字符
    options.Password.RequiredUniqueChars = 1; // 密码需要包含的唯一字符数为1个

    // 确认验证方式
    options.SignIn.RequireConfirmedEmail = false; // 需要邮箱验证
    options.SignIn.RequireConfirmedPhoneNumber = false; // 不需要手机号验证
    options.SignIn.RequireConfirmedAccount = false; // 不需要账户确认

    // 用户设置
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // 允许的用户名字符
    options.User.RequireUniqueEmail = false; // 不需要邮箱地址唯一

})
    .AddEntityFrameworkStores<ApplicationDbContext>() // 添加实体框架存储支持
    .AddSignInManager<SignInManager<ApplicationUser>>() // 添加登录管理器支持
    .AddDefaultTokenProviders() // 添加默认的令牌提供程序
    ;

//builder.Services.AddIdentityServer(options =>
//{
//    options.LicenseKey = builder.Configuration["IdentityServerLicenseKey"];
//    //options.LicenseKey = "Zpl7M3F6pvzJPG7IJr35gbJu+CglNFphrtbSpIByxaM=";
//    options.Events.RaiseErrorEvents = true;
//    options.Events.RaiseInformationEvents = true;
//    options.Events.RaiseFailureEvents = true;
//    options.Events.RaiseSuccessEvents = true;
//})
//    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
//    {
//        options.IdentityResources["openid"].UserClaims.Add("roleVIP");

//        // Client localhost
//        var url2 = "localhost";
//        var spaClient2 = ClientBuilder
//            .SPA("BlazorOIDC.Localhost")
//            .WithRedirectUri($"https://{url2}:5001/authentication/login-callback")
//            .WithLogoutRedirectUri($"https://{url2}:5001/authentication/logout-callback")
//            .Build();
//        spaClient2.AllowOfflineAccess = true;

//        spaClient2.AllowedCorsOrigins = new[]
//        {
//            $"https://{url2}:5001"
//        };
//        options.Clients.Add(spaClient2);

//        var spaClientBlazor5002 = ClientBuilder
//        .SPA("Blazor5002")
//        .WithScopes("api")
//        .Build();

//        spaClientBlazor5002.AllowedCorsOrigins = new[]
//        {
//            $"http://0.0.0.0",
//            $"http://0.0.0.0:5001",
//            $"http://0.0.0.0:5002",
//            $"http://localhost",
//            $"http://localhost:5001",
//            $"http://localhost:5002",
//            $"https://localhost",
//            $"https://localhost:5001",
//            $"https://localhost:5002"
//        };

//        foreach (var item in spaClientBlazor5002.AllowedCorsOrigins)
//        {
//            spaClientBlazor5002.RedirectUris.Add($"{item}/authentication/login-callback");
//            spaClientBlazor5002.PostLogoutRedirectUris.Add($"{item}/authentication/logout-callback");
//        }

//        spaClientBlazor5002.AllowOfflineAccess = true;

//        options.Clients.Add(spaClientBlazor5002);

//    });


// 想要添加一个JWT来给自己的API接口使用，API接口用来对外开发一些数据
// 现在遇到的问题就是一旦增加JWT，登录就失效
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddCookie().AddJwtBearer();

//builder.Services.Configure<AuthenticationOptions>(options => options.DefaultScheme = "ApplicationDefinedAuthentication");


//builder.Services.AddAuthentication()
//    .AddIdentityServerJwt()
//    .AddPolicyScheme("ApplicationDefinedAuthentication", null, CustomerPolicyScheme);

//static void CustomerPolicyScheme(PolicySchemeOptions options)
//{
//    options.ForwardDefaultSelector = (context) =>
//    {
//        //特定方案授权
//        Console.WriteLine(context.Request.Path);
//        string? authorization = context.Request.Headers[HeaderNames.Authorization];
//        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
//        {
//            var token = authorization.Substring("Bearer ".Length).Trim();
//            //使用 JWT 持有者身份验证
//            return IdentityServerJwtConstants.IdentityServerJwtBearerScheme;
//        }
//        else
//        {
//            //针对登录使用基于 cookie 的身份验证
//            return IdentityConstants.ApplicationScheme;

//        }
//    };
//}


builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// 添加 MVC 控制器服务 => API控制器
builder.Services.AddControllers();

//添加 Swagger 服务配置
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API 开发测试", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    //启用 Swagger 中间件
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//app.UseIdentityServer();


// 配置 API 路由
app.MapControllers();


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
