using Amnex_Project_Resource_Mapping_System.Models;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                // Set a short timeout for easy testing
                options.IdleTimeout = TimeSpan.FromMinutes(20); // Adjust as needed
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();

            var configuration = builder.Configuration;
            var dbConfiguration = configuration.GetSection("DBConfiguration").Get<DBConfiguration>();
            var connectionString = $"Host={dbConfiguration.Host};Port={dbConfiguration.Port};Username={dbConfiguration.Username};Password={dbConfiguration.Password};Database={dbConfiguration.Database}";

            try
            {
                using (var tempConnection = new NpgsqlConnection(connectionString))
                {
                    tempConnection.Open();
                    if (tempConnection.State == System.Data.ConnectionState.Open)
                    {
                        tempConnection.Close();
                        Console.WriteLine("Database connection successful!");
                        builder.Services.AddScoped<NpgsqlConnection>(_ => new NpgsqlConnection(connectionString));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
            }
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404)
                {
                    context.Response.Redirect("/Home/Error");
                }
            });

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;
                var session = context.Session;
                var userId = session.GetString("userId");

                if (path.StartsWithSegments("/Account/Login") || !string.IsNullOrEmpty(userId))
                {
                    await next();
                    return;
                }

                context.Response.Redirect("/Account/Login");
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Dashboard}/{id?}");


            app.Run();
        }
    }
}
