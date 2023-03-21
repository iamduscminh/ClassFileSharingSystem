using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ClassFileSharingContextConnection") ?? throw new InvalidOperationException("Connection string 'ClassFileSharingContextConnection' not found.");
var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

// Add services to the container.
builder.Services.AddDbContext<ClassFileSharingContext>(options =>
{

    options.UseSqlServer(config.GetConnectionString("APDBConnectionStr"));
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{     // configure identity options
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 5;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ClassFileSharingContext>();
//.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    CreateRoles(scope.ServiceProvider).Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();




async Task CreateRoles(IServiceProvider serviceProvider)
{

    //initializing custom roles 
    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string[] roleNames = { "Admin", "Teacher", "Student" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            //create the roles and seed them to the database: Question 1
            await RoleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    //create default admin user
    var adminUser = new IdentityUser
    {
        UserName = config.GetValue<string>("DefaultUser:adminUsername"),
        Email = config.GetValue<string>("DefaultUser:adminEmail"),
    };
    string userAdminPWD = config.GetValue<string>("DefaultUser:adminPassword");
    var _userAd = await UserManager.FindByEmailAsync(config.GetValue<string>("DefaultUser:adminEmail"));

    if (_userAd == null)
    {
        var createPowerUser = await UserManager.CreateAsync(adminUser, userAdminPWD);
        if (createPowerUser.Succeeded)
        {
            //here we tie the new user to the role
            await UserManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}