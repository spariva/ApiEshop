using ApiEshop.Data;
using ApiEshop.Mappings;
using ApiEshop.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiEshop.Helpers;
using Microsoft.Extensions.Azure;
using Azure.Security.KeyVault.Secrets;


var builder = WebApplication.CreateBuilder(args);

// Add services.
//string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});
SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();

KeyVaultSecret secretConnectionString = await secretClient.GetSecretAsync("SqlAzure");
string connectionString = secretConnectionString.Value;
builder.Services.AddDbContext<EshopContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<RepositoryStores>();
builder.Services.AddTransient<RepositoryUsers>();
builder.Services.AddTransient<RepositoryPayments>();
HelperActionServicesOAuth helperOAuth = new HelperActionServicesOAuth(builder.Configuration);
builder.Services.AddSingleton<HelperActionServicesOAuth>(helperOAuth);
builder.Services.AddAuthentication(
    helperOAuth.GetAuthenticationSchema()).AddJwtBearer(helperOAuth.GetJwtBearerOptions());


// Automapper: It seems i wont need this to config automapper, I will delete in case everything works once I finish =)
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
//builder.Services.AddAutoMapper();
builder.Services.AddAutoMapper(typeof(MapperProfile));


builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {

}
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Api e-Shop");
        options.RoutePrefix = "";
    });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
