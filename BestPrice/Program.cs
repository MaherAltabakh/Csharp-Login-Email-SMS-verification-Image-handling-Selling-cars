using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//1:
//add the following lines for session
builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//end

////Maher Added for session
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
        options.AccessDeniedPath = "/Forbidden/";
        options.LoginPath = "/Login/Login/";
        options.LogoutPath = "/Login/LogoutAction/";
        options.SlidingExpiration = true;
    });
////end Maher

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//2:
//add the following lines for session
app.UseSession();
app.UseAuthentication();
//end

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
//pattern: "{controller=Login}/{action=Login}/{id?}");
pattern: "{controller=Login}/{action=Login}/{id?}");


app.Run();
