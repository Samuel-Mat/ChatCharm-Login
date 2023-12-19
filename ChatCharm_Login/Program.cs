global using ChatCharm_Login.Service;
using Microsoft.EntityFrameworkCore;
using ChatCharm_Login.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using TypedMongoDbDriverWrapper;
using ChatCharm_Login.Repositories;
using MongoDbDriverWrapper.Demo.Db.DbContext;


//Diese API wurde mit dieser Vorlage erstellt: https://github.com/michelschlatter/TypedMongoDbDriverWrapper.Demo

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Created with the help of: https://www.youtube.com/watch?v=6sMPvucWNRE

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("*")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Key").Value!))

    };
});
var config = builder.Configuration;
var dbContext = await DbContextFactory.CreateAsync(
    "ChatCharm-LoginDB",
    config.GetConnectionString("Default") ?? throw new Exception($"Add 'MongoDb' to ConnectionStrings in application.{builder.Environment.EnvironmentName}.json"));

builder.Services.AddSingleton(_ => dbContext);
builder.Services.AddSingleton<UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
