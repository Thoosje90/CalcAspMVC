var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");
// Add Httpcontext Processor to use sessions in controllers
builder.Services.AddHttpContextAccessor();
// Add Memory Cache for Services
builder.Services.AddDistributedMemoryCache();

// Add/Configure Session Services
builder.Services.AddSession(options =>
{
    // Set Session timout and enable cookies
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//builder.Services.AddSingleton(CalcAspMVC.Models.MemoryModel);
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

// Use session
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Index}/{id?}");

app.Run();
