using BlazorApp1.Components;
using BlazorApp1.Components.Account;
using BlazorApp1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();


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

// 想要添加一个JWT来给自己的API接口使用，API接口用来对外开发一些数据
// 现在遇到的问题就是一旦增加JWT，登录就失效
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddCookie().AddJwtBearer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Issuer1",
            ValidAudience = "audience1",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Zpl7M3F6pvzJPG7IJr35gbJu+CglNFphrtbSpIByxaM="))
        };

    });


// 官方模板IDS自动生成的
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme; // 值：Identity.Application
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

//builder.Services.Configure<AuthenticationOptions>(options => options.DefaultScheme = "ApplicationDefinedAuthentication");

//builder.Services.AddAuthentication()
////    .AddIdentityServerJwt()
//    .AddPolicyScheme("ApplicationDefinedAuthentication", null, CustomerPolicyScheme);


//static void CustomerPolicyScheme(PolicySchemeOptions options)
//{
//    options.ForwardDefaultSelector = (context) =>
//    {
//        //特定方案授权
//        System.Console.WriteLine("Path目录：" + context.Request.Path);
//        string? authorization = context.Request.Headers[HeaderNames.Authorization];
//        System.Console.WriteLine("Header头部：" + authorization);
//        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
//        {
//            var token = authorization.Substring("Bearer ".Length).Trim();
//            //使用 JWT 持有者身份验证
//            return "JwtBearerScheme";
//        }
//        else
//        {
//            //针对登录使用基于 cookie 的身份验证
//            return IdentityConstants.ApplicationScheme;

//        }

//        //if (context.Request.Path.StartsWithSegments(new PathString("/Identity"), StringComparison.OrdinalIgnoreCase) ||
//        //    context.Request.Path.StartsWithSegments(new PathString("/_blazor"), StringComparison.OrdinalIgnoreCase))
//        //{
//        //    //针对登录使用基于 cookie 的身份验证
//        //    return IdentityConstants.ApplicationScheme;
//        //}
//        //else
//        //{
//        //    //使用 JWT 持有者身份验证
//        //    return "JwtBearerScheme";
//        //}
//    };
//}



builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// 添加 MVC 控制器服务 => API控制器
builder.Services.AddControllers();


// 添加 Swagger 服务配置
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API 开发测试", Version = "v1" });

    JwtService jwtService = new("Issuer1", "audience1", "Zpl7M3F6pvzJPG7IJr35gbJu+CglNFphrtbSpIByxaM=");

    var Token2 = jwtService.GenerateToken("123456", "2104563259");

    // 添加JWT授权按钮
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWTToken：" + Token2,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    // 使用 JWT 授权
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

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


// 使用身份验证中间件
app.UseAuthentication();

// 添加路由
app.UseRouting();

// 使用授权中间件
app.UseAuthorization();


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
