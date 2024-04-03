using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using  Microsoft.EntityFrameworkCore.Sqlite;
using TodoAPP.Configuration;
using TodoAPP.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddAuthentication(options=>{
        options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme= JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt=>{
        var key=Encoding.ASCII.GetBytes(builder.Configuration["JwtConfiguration:Secret"]);
        jwt.SaveToken=true;
        jwt.TokenValidationParameters=new TokenValidationParameters{
            ValidateIssuerSigningKey=true,
            IssuerSigningKey= new SymmetricSecurityKey(key),
            ValidateIssuer=false,
            ValidateAudience=false,
            ValidateLifetime=true,
            RequireExpirationTime=true,
        };
    });
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApiDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
