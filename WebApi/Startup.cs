using Application.Service;
using Domain.Model;
using Domain.Model.PagSeguro;
using Domain.Model.Request;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using System.IO;
using System.Linq;
using System.Text;

namespace MS_Order
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddControllers().AddFluentValidation();
            services.AddControllers();
            services.AddCors();
            services.AddLogging();
            services.AddSignalR();

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("MSOrderSettings").GetSection("PrivateSecretKey").Value);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Add framework services.

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "V1";
                    document.Info.Title = "PAM - Microservice Order";
                    document.Info.Description = "API's Documentation of Microservice Order of PAM Plataform";
                };

                config.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                });

                config.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            string logFilePath = Configuration.GetSection("LogSettings").GetSection("LogFilePath").Value;
            string logFileName = Configuration.GetSection("LogSettings").GetSection("LogFileName").Value;

            string connectionString = Configuration.GetSection("MSOrderSettings").GetSection("ConnectionString").Value;
            string privateSecretKey = Configuration.GetSection("MSOrderSettings").GetSection("PrivateSecretKey").Value;
            string tokenValidationMinutes = Configuration.GetSection("MSOrderSettings").GetSection("TokenValidationMinutes").Value;

            PagSeguroAccess pagSeguroAccess = new PagSeguroAccess()
            {
                Account_Id = Configuration.GetSection("MSOrderSettings").GetSection("PagSeguro:Account_Id").Value,
                Method_Split = Configuration.GetSection("MSOrderSettings").GetSection("PagSeguro:Method_Split").Value,
                Token = Configuration.GetSection("MSOrderSettings").GetSection("PagSeguro:Token").Value,
                Url = Configuration.GetSection("MSOrderSettings").GetSection("PagSeguro:Url").Value,
            };

            EmailSettings emailSettings = new EmailSettings()
            {
                PrimaryDomain = Configuration.GetSection("EmailSettings:PrimaryDomain").Value,
                PrimaryPort = Configuration.GetSection("EmailSettings:PrimaryPort").Value,
                UsernameEmail = Configuration.GetSection("EmailSettings:UsernameEmail").Value,
                UsernamePassword = Configuration.GetSection("EmailSettings:UsernamePassword").Value,
                FromEmail = Configuration.GetSection("EmailSettings:FromEmail").Value,
                ToEmail = Configuration.GetSection("EmailSettings:ToEmail").Value,
                CcEmail = Configuration.GetSection("EmailSettings:CcEmail").Value,
                EnableSsl = Configuration.GetSection("EmailSettings:EnableSsl").Value,
                UseDefaultCredentials = Configuration.GetSection("EmailSettings:UseDefaultCredentials").Value
            };


            services.AddSingleton((ILogger)new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.File(Path.Combine(logFilePath, logFileName), rollingInterval: RollingInterval.Day)
              .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
              .CreateLogger());

            services.AddScoped<IOrderRepository, OrderRepository>(
                provider => new OrderRepository(connectionString, provider.GetService<ILogger>()));

            services.AddScoped<IOrderService, OrderService>(
                provider => new OrderService(provider.GetService<IOrderRepository>(),
                provider.GetService<ILogger>(), privateSecretKey, tokenValidationMinutes, provider.GetService<IHubContext<OrderStatusHub>>(),pagSeguroAccess));

            services.AddScoped<IShippingCompanyRepository, ShippingCompanyRepository>(
                provider => new ShippingCompanyRepository(connectionString, provider.GetService<ILogger>()));

            services.AddScoped<IShippingCompanyService, ShippingCompanyService>(
                provider => new ShippingCompanyService(provider.GetService<IShippingCompanyRepository>(),
                provider.GetService<ILogger>(), privateSecretKey, tokenValidationMinutes));

            services.AddTransient<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();
            services.AddTransient<IValidator<UpdateOrderRequest>, UpdateOrderRequestValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            // add the Swagger generator and the Swagger UI middlewares   
            app.UseSwaggerUi3();

            app.UseReDoc(options =>
            {
                options.RoutePrefix = "docs";
                options.DocumentTitle = "Microservice Workflow - PAM";
            });

           
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseCors(builder => builder
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true)
               .AllowCredentials());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<OrderStatusHub>("/order-status-hub");
            });
            app.UseMvc();


        }
    }
}