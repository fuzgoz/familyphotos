using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FamilyPhotos.Entities;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace FamilyPhotos
{
    public class Program
    {

        /*#region Initialize Roles and Admin
        private static async Task InitializeRolesAndAdmin(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Create Admin role if it doesn't exist
            var roleExist = await roleManager.RoleExistsAsync("Admin");
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Check if an admin user exists, and if not, create one
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
                var result = await userManager.CreateAsync(adminUser, "admin");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Naplózd a hibákat, ha a regisztráció nem sikerült
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
        }

        #endregion*/


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configuring DbContext with PostgreSQL
            builder.Services.AddDbContext<FamilyPhotoContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity services
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<FamilyPhotoContext>()
                .AddDefaultTokenProviders();

            // Configure application cookie settings for login, logout, etc.
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // Initialize roles and admin user (this will run once at app startup)
            builder.Services.AddTransient<IApplicationBuilder, ApplicationBuilder>(); // To run Role initialization

            var app = builder.Build();

            // Seed roles and admin user on app startup
            //InitializeRolesAndAdmin(app).Wait();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Add Authentication middleware
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

    }
}
