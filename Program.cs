using ApiEshop.Data;
using ApiEshop.Mappings;
using ApiEshop.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiEshop.Helpers;
using Microsoft.Extensions.Azure;
using Azure.Security.KeyVault.Secrets;
using NSwag;
using NSwag.Generation.Processors.Security;
using Microsoft.OpenApi.Models;
//using Azure.Storage.Blobs;


var builder = WebApplication.CreateBuilder(args);

// Add services.
builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});
SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();


KeyVaultSecret secretConnectionString = await secretClient.GetSecretAsync("SqlAzure");
string connectionString = secretConnectionString.Value;
builder.Services.AddDbContext<EshopContext>(x => x.UseSqlServer(connectionString));

HelperOAuth helperOAuth = new HelperOAuth(builder.Configuration, secretClient);
builder.Services.AddSingleton<HelperOAuth>(helperOAuth);
builder.Services.AddAuthentication(helperOAuth.GetAuthenticationSchema()).AddJwtBearer(helperOAuth.GetJwtBearerOptions());
builder.Services.AddSingleton<HelperTokenUser>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


builder.Services.AddTransient<RepositoryStores>();
builder.Services.AddTransient<RepositoryUsers>();
builder.Services.AddTransient<RepositoryPayments>();

builder.Services.AddAutoMapper(typeof(MapperProfile));

//Blobs:
//KeyVaultSecret secretStorageAccount = await secretClient.GetSecretAsync("Storageaccount");
//string azurekeys = secretStorageAccount.Value;
//BlobServiceClient blobServiceClient = new BlobServiceClient(azurekeys);
//builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);
//builder.Services.AddTransient<ServiceBlobs>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api Eshop";
    document.Description = "Api de azure para E-shop";

    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

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
