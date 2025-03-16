
using Core.Interfaces;
using Core.Model;
using Core.Servises;
using Core.Settings;
using Infrastructure;
using Infrastructure.Authentication;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace ProjectAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ProdcutionConnection"),
                    options => options.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            builder.Services.AddAuthorization(auth => auth.AddPolicy("AdminRole", p => p.RequireRole("Admin")));
            builder.Services.AddAuthorization(auth => auth.AddPolicy("UserRole", p => p.RequireRole("User")));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("jwt"));

            builder.Services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            builder.Services.AddScoped<PasswordHasher<AppUser>>();

            builder.Services.AddIdentity<AppUser, IdentityRole>().
                AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddTransient<IAuthentication, Authentication>();
            builder.Services.AddTransient<Service>();




            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Key"]))

                };

            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,

                });

                options.AddSecurityRequirement(securityRequirement: new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()

                    }

                });

            });

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
