using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
// Add services to the container.
builder.Services.AddDbContext<ClassFileSharingContext>(options =>
{
    options.UseSqlServer(config.GetConnectionString("APDBConnectionStr"));
});
//builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
builder.Services.AddIdentity<ApplicationUser , IdentityRole>(options =>
{     // configure identity options
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 5;
})
    .AddDefaultUI()
    //.AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ClassFileSharingContext>()
    .AddDefaultTokenProviders();

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
    var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
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
    var adminUser = new ApplicationUser
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

    //create default teacher user
    var teacherUser = new ApplicationUser
    {
        UserName = config.GetValue<string>("DefaultUser:teacherUsername"),
        Email = config.GetValue<string>("DefaultUser:teacherEmail"),
        FullName = config.GetValue<string>("DefaultUser:teacherFullName"),
    };
    string teacherUserPWD = config.GetValue<string>("DefaultUser:teacherPassword");
    var _userTe = await UserManager.FindByEmailAsync(config.GetValue<string>("DefaultUser:teacherEmail"));

    if (_userTe == null)
    {
        var createPowerUser = await UserManager.CreateAsync(teacherUser, teacherUserPWD);
        if (createPowerUser.Succeeded)
        {
            //here we tie the new user to the role
            await UserManager.AddToRoleAsync(teacherUser, "Teacher");
        }
    }

    var teacherUser1 = new ApplicationUser
    {
        UserName = config.GetValue<string>("DefaultUser:teacherUsername1"),
        Email = config.GetValue<string>("DefaultUser:teacherEmail1"),
        FullName = config.GetValue<string>("DefaultUser:teacherFullName1"),
    };
    string teacherUserPWD1 = config.GetValue<string>("DefaultUser:teacherPassword1");
    var _userTe1 = await UserManager.FindByEmailAsync(config.GetValue<string>("DefaultUser:teacherEmail1"));

    if (_userTe1 == null)
    {
        var createPowerUser = await UserManager.CreateAsync(teacherUser1, teacherUserPWD1);
        if (createPowerUser.Succeeded)
        {
            //here we tie the new user to the role
            await UserManager.AddToRoleAsync(teacherUser1, "Teacher");
        }
    }
}