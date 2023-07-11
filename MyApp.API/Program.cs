using MyApp.Util;
using System.Data.SqlClient;
using System.Data;
using MyApp.DAL;
using Microsoft.Extensions.Configuration;
using MyApp.Models;
using MyApp.API.Filters;
using Serilog;
using Serilog.Filters;
using MyApp.Services;
using Microsoft.OpenApi.Models;
using MyApp.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, configuration) =>
    configuration
    //.ReadFrom.Configuration(context.Configuration)
    .MinimumLevel.Information()
    .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"))
    .Enrich.FromLogContext()
    .WriteTo.File(path: "Logs/ErrorLog_.log", rollingInterval: RollingInterval.Day)
);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilterAttribute>();
});

builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApp.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
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
            new string[] {}
        }
    });
    //c.IncludeXmlComments(xmlPath);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Replace the ConnectionString DB path in AppSetting.json
string currentDirectory = Directory.GetCurrentDirectory();
string relativePath = Path.Combine("Database", "MyAppDatabase.mdf");
string absolutePath = Path.Combine(currentDirectory, relativePath);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!.Replace("{PathProvidedIn_Program.cs}", absolutePath);
builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(connectionString));

#region ReadConfig from AppSettings
    builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JwtConfig"));
#endregion

#region Register Repositories
builder.Services.AddScoped<IGenericRepository<ProductModel>,GenericRepository<ProductModel>>();
    builder.Services.AddScoped<IGenericRepository<CustOrderModel>, GenericRepository<CustOrderModel>>();
    builder.Services.AddScoped<IGenericRepository<CustOrderDetailModel>, GenericRepository<CustOrderDetailModel>>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IAdhocRepository, AdhocRepository>();
#endregion

#region Register Services
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IMiscellaneousService, MiscellaneousService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>(); //app.UseAuthorization();
app.MapControllers();

app.Run();


