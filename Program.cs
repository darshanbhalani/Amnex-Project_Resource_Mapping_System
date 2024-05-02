using Amnex_Project_Resource_Mapping_System.Models;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            var configuration = builder.Configuration;
            var dbConfiguration = configuration.GetSection("DBConfiguration").Get<DBConfiguration>();
            var connectionString = $"Host={dbConfiguration!.Host};Port={dbConfiguration.Port};Username={dbConfiguration.Username};Password={dbConfiguration.Password};Database={dbConfiguration.Database}";


            var smtpConfiguration = configuration.GetSection("SMTPConfiguration").Get<SMTPConfiguration>();
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
                Console.WriteLine("Database connection error...");
                Console.WriteLine(ex.Message);
            }
            var app = builder.Build();

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

                var path = context.Request.Path;
                var session = context.Session;
                var userId = session.GetString("userId");

                if (path.StartsWithSegments("/Account/Login") || !string.IsNullOrEmpty(userId) || path.StartsWithSegments("/Account/ForgotPassword"))
                {
                    try
                    {
                        await next();
                        if (context.Response.StatusCode == 404)
                        {
                            context.Response.Redirect("/Home/Error");
                        }
                        return;
                    }
                    catch(NpgsqlException npgsqlEx)
                    {
                        string _connectionString = $"Host={dbConfiguration!.Host};Port={dbConfiguration.Port};Username={dbConfiguration.Username};Password={dbConfiguration.Password};Database={dbConfiguration.Database}";
                        using (var _connection = new NpgsqlConnection(_connectionString))
                        {
                            _connection.Open();
                            using (var cmd = new NpgsqlCommand($"SELECT recordError('{DateTime.Now}','{npgsqlEx.Message}','{npgsqlEx.StackTrace}','{context.Request.RouteValues["controller"]}','{context.Request.RouteValues["action"]}');", _connection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string _connectionString = $"Host={dbConfiguration!.Host};Port={dbConfiguration.Port};Username={dbConfiguration.Username};Password={dbConfiguration.Password};Database={dbConfiguration.Database}";
                        using (var _connection = new NpgsqlConnection(_connectionString))
                        {
                            _connection.Open();
                            using (var cmd = new NpgsqlCommand($"SELECT recordError('{DateTime.Now}','{ex.Message}','{ex.StackTrace}','{context.Request.RouteValues["controller"]}','{context.Request.RouteValues["action"]}');", _connection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

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
