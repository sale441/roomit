using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using roomit2.Data;
using roomit2.Models;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("SqlCustomersConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<RoomitContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDataConnection"));
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(
    options =>
    {
        //options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        
    }).AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();




// ***** 1- diese Code hier um die Daten aus dem Sechtion in appsetting in den Eigenschaften des Klasse zu passen bzw. zu füllen, die EigenschaftenName sollten die gleiche Namen im Section in der appsettings sein, damit es funktioniert.
// ***** 2- und So dann kann ein aus dem KLasse einen Instanz machen und auf die Eigenschaften lesend zugreifen
// ****  3- oder über @inject in der HTML Code die Werte passen indem man eingibt ===> @inject IOptions<'Name der Klasse'> 'und hier eine ObjektVariable' und nicht vergessen => ObjektVariable.Value.Eigenschaftname 
// ****  4- wenn man aber auf die Klassen zugreift in einem Service oder in einenm Controller oder PageModel macht man über dependency injection im Konstruktor in dem man IOptionsMonitor<'Name der Klasse'> objectName

// builder.Services.Configure<'Name der Klasse, die Ihre Eigenschaften zu laden'>(builder.Configuration.GetSection('Name des Sections in appsettings'));


//builder.Services.AddTransient<and IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
//builder.Services.AddTransient<IUserEmailStore<ApplicationUser>, UserEmailStore<ApplicationUser>>();

//builder.Services.AddAuthorization();

//builder.Services.AddMemoryCache();

builder.Services.AddRazorPages()
                    .AddSessionStateTempDataProvider();

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.AddDistributedMemoryCache();       // for das Session Notwendig

builder.Services.AddSession(options =>             // for das Session Notwendig
{
    options.IdleTimeout = TimeSpan.FromDays(7);            
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Shop/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // fuer das Session Notwendig die Reihenfolge ist Wichtig


app.MapControllerRoute(
    name: "ablauf",
    pattern: "ablauf/",
    defaults: new { controller = "Shop", action = "Ablauf" });

app.MapControllerRoute(
    name: "vorteile",
    pattern: "vorteile/",
    defaults: new { controller = "Shop", action = "vorteile" }); 

app.MapControllerRoute(
    name: "faq",
    pattern: "faq/",
    defaults: new { controller = "Shop", action = "faq" });

app.MapControllerRoute(
    name: "warenkorb",
    pattern: "warenkorb/",
    defaults: new { controller = "Shop", action = "warenkorb" });

app.MapControllerRoute(
    name: "shop",
    pattern: "shop/",
    defaults: new { controller = "Shop", action = "Shop" });


app.MapControllerRoute(
    name: "Kontakt",
    pattern: "Kontakt/",
    defaults: new { controller = "Shop", action = "Kontakt" });
app.MapControllerRoute(
    name: "Impressum",
    pattern: "Impressum/",
    defaults: new { controller = "Shop", action = "Impressum" });

app.MapControllerRoute(
    name: "Datenschutz",
    pattern: "Datenschutz/",
    defaults: new { controller = "Shop", action = "Datenschutz" });

app.MapControllerRoute(
    name: "AGB",
    pattern: "AGB/",
    defaults: new { controller = "Shop", action = "AGB" });



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Shop}/{action=Index}/{id?}");



app.MapRazorPages();

app.Run();
