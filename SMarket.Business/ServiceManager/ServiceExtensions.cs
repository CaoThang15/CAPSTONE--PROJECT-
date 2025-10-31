using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SMarket.Business.Hubs;
using SMarket.Business.Mappers;
using SMarket.Business.Services;
using SMarket.Business.Services.Interfaces;
using SMarket.Business.Services.Workers;
using SMarket.Business.Workers;
using SMarket.DataAccess.Common;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Repositories;
using SMarket.DataAccess.Repositories.Interfaces;
using System.Text;

namespace SMarket.Business.ServiceManager
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly("SMarket.DataAccess");
                        npgsqlOptions.UseVector();
                    })
            );

            return services;
        }

        public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authorization = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        {
                            return Task.CompletedTask;
                        }

                        if (context.Request.Cookies.ContainsKey("access_token"))
                        {
                            context.Token = context.Request.Cookies["access_token"];
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IServiceCollection ConfigureBusinessServices(this IServiceCollection services)
        {
            // Replace AutoMapper with custom mapper
            services.AddScoped<ICustomMapper, CustomMapper>();

            services.AddHttpContextAccessor();
            services.AddHttpClient<IAIService, AIService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>();
            services.AddSingleton<IOtpService, InMemoryOtpService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddSingleton<INotificationHub, NotificationHub>();
            services.AddScoped<ISystemNotificationService, SystemNotificationService>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<OtpWorker>();
            services.AddHostedService<TokenCleanupWorker>();
            services.AddHostedService<SystemNotificationWorker>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IVectorRepository, VectorRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ISystemNotificationRepository, SystemNotificationRepository>();

            // Generic repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }

        public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "https://s-market-fpt.netlify.app/")
                            .AllowCredentials()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(_ => true); // Allow SSE connections
                });
            });

            return services;
        }
    }
}