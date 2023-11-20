using SfmcCustomActivities.Helpers;
using SfmcCustomActivities.Services;

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
            //builder.Services.AddApiExplorer();
            builder.Services.AddSwaggerGen(c => {
                c.DocumentFilter<DisableSwaggerFilter>();
            });
            conf.GetSection("Settings:SMS").Bind(SmsSettings.Instance);

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