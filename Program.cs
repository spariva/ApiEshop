using ApiEshop.Data;
using ApiEshop.Mappings;
using ApiEshop.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services.
string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddDbContext<EshopContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<RepositoryStores>();
builder.Services.AddTransient<RepositoryUsers>();
builder.Services.AddTransient<RepositoryPayments>();

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MapperProfile));
//Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
//builder.Services.AddAutoMapper();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

app.UseAuthorization();

app.MapControllers();

app.Run();
