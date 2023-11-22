using SfmcCustomActivities.Helpers;
using SfmcCustomActivities.Services;
using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;

namespace SfmcCustomActivities
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var conf = builder.Configuration;

            // Add services to the container.
            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c => {
                c.DocumentFilter<DisableSwaggerFilter>();
            });
            conf.GetSection("Settings:SMS").Bind(SmsSettings.Instance);

            //Singleton for Azure service bus
            var busConnect = conf.GetSection("Settings:ServiceBus").GetValue<string>("ConnectionString");
            var queue = conf.GetSection("Settings:ServiceBus").GetValue<string>("SendQueue");
            if (!string.IsNullOrEmpty(busConnect) && !string.IsNullOrEmpty(queue))
            {
                builder.Services.AddAzureClients(xx =>
                {
                    xx.AddServiceBusClient(busConnect);
                    xx.AddClient<ServiceBusSender, ServiceBusSenderOptions>((_, _, provider) =>
                        provider.GetService<ServiceBusClient>()
                            .CreateSender(queue)
                    )
                    .WithName("SendQueue");
                });
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
               name: "default",
               pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }

    }
}