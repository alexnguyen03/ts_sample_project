using AutoMapper;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Nest;

using Serilog;

using ServerProject.Configurations;
using ServerProject.Models;
using ServerProject.Services;

using StackExchange.Redis;

using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITerritoryService, TerritoryService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IShipperService, ShipperService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPosService, PosService>();
builder.Services.AddScoped<IMessageProducer, RabbitMQProducer>();
builder.Services.AddScoped<RabbitMQManager>();
// Add services to the container.
builder.Services.AddDbContext<MsdemoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(profile: new MapperConfig());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.Configure<TrungSonPharmaDatabaseSettings>(
    builder.Configuration.GetSection("TestElasticSearch"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
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
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", opt => opt
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("X-Pagination"));
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
    };
});
var rabbitMQManager = new RabbitMQManager();
// Tạo kênh
rabbitMQManager.CreateChannel(Channels.ORDER.ToString(), "direct", Channels.ORDER.ToString(), "ORDER_ROUTING");
rabbitMQManager.CreateChannel(Channels.PRODUCT.ToString(), "direct", Channels.PRODUCT.ToString(), "PRODUCT_ROUTING");
var node = new Uri("http://localhost:9200");
var settings = new ConnectionSettings(node);
settings.DefaultIndex("products");
var client = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(client);
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    RoleInitializer.InitializeAsync(roleManager).GetAwaiter().GetResult();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();
