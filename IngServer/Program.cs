using IngServer;
using IngServer.DataBase;
using IngServer.Objects.User;
using IngServer.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Minio;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

const string allowPolicy = "_ing";

services.AddControllers();
services.AddDbContext<ApplicationContext>();

services.Configure<MinioConfig>(builder.Configuration.GetSection("Minio"));
services.AddMinio(configureClient =>
{
    configureClient.WithEndpoint("194.67.105.245:9000");
    configureClient.WithCredentials("8FX1wXOWmvwIoExsIsUs", "gLL9GtEu6zHVjMeKrtRi8UDpRmjUFMurn7jD94xp");
});

services.AddScoped<CategoryRepository>();
services.AddScoped<ProductRepository>();
services.AddScoped<CharacteristicRepository>();
services.AddScoped<OrderRepository>();
services.AddScoped<ProductMovementRepository>();
services.AddScoped<WishListRepository>();
services.AddScoped<UserRepository>();

services.AddScoped<BreadCrumbManager>();
services.AddScoped<Encrypter>();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie("Cookies");
services.AddMemoryCache();
services.AddCors(options =>
{
    options.AddPolicy(name: allowPolicy,
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
            policy.SetPreflightMaxAge(TimeSpan.MaxValue);
        });
});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(allowPolicy);

app.MapControllers();

app.Run();