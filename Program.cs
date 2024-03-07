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


// �ٷ�ģ��IDS�Զ����ɵ�
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

    // Ĭ�ϵ��˻���������
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60); // �˻�����ʱ��Ϊ60����
    options.Lockout.MaxFailedAccessAttempts = 5; // ����ʧ�ܵ�¼��������Ϊ5��
    options.Lockout.AllowedForNewUsers = false; // �������û�������

    // ��������
    options.Password.RequireDigit = true; // ������Ҫ��������
    options.Password.RequireLowercase = false; // ���벻��Ҫ����Сд��ĸ
    options.Password.RequireNonAlphanumeric = false; // ���벻��Ҫ���������ַ�
    options.Password.RequireUppercase = false; // ���벻��Ҫ������д��ĸ
    options.Password.RequiredLength = 6; // ������С����Ϊ6���ַ�
    options.Password.RequiredUniqueChars = 1; // ������Ҫ������Ψһ�ַ���Ϊ1��

    // ȷ����֤��ʽ
    options.SignIn.RequireConfirmedEmail = false; // ��Ҫ������֤
    options.SignIn.RequireConfirmedPhoneNumber = false; // ����Ҫ�ֻ�����֤
    options.SignIn.RequireConfirmedAccount = false; // ����Ҫ�˻�ȷ��

    // �û�����
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // ������û����ַ�
    options.User.RequireUniqueEmail = false; // ����Ҫ�����ַΨһ

})
    .AddEntityFrameworkStores<ApplicationDbContext>() // ���ʵ���ܴ洢֧��
    .AddSignInManager<SignInManager<ApplicationUser>>() // ��ӵ�¼������֧��
    .AddDefaultTokenProviders() // ���Ĭ�ϵ������ṩ����
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


// ��Ҫ���һ��JWT�����Լ���API�ӿ�ʹ�ã�API�ӿ��������⿪��һЩ����
// �����������������һ������JWT����¼��ʧЧ
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddCookie().AddJwtBearer();

//builder.Services.Configure<AuthenticationOptions>(options => options.DefaultScheme = "ApplicationDefinedAuthentication");


//builder.Services.AddAuthentication()
//    .AddIdentityServerJwt()
//    .AddPolicyScheme("ApplicationDefinedAuthentication", null, CustomerPolicyScheme);

//static void CustomerPolicyScheme(PolicySchemeOptions options)
//{
//    options.ForwardDefaultSelector = (context) =>
//    {
//        //�ض�������Ȩ
//        Console.WriteLine(context.Request.Path);
//        string? authorization = context.Request.Headers[HeaderNames.Authorization];
//        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
//        {
//            var token = authorization.Substring("Bearer ".Length).Trim();
//            //ʹ�� JWT �����������֤
//            return IdentityServerJwtConstants.IdentityServerJwtBearerScheme;
//        }
//        else
//        {
//            //��Ե�¼ʹ�û��� cookie �������֤
//            return IdentityConstants.ApplicationScheme;

//        }
//    };
//}


builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// ��� MVC ���������� => API������
builder.Services.AddControllers();

//��� Swagger ��������
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API ��������", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    //���� Swagger �м��
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


// ���� API ·��
app.MapControllers();


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
