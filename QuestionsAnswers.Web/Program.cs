using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Enable logging to monitor routing and requests
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();  // Output logs to the console
    logging.SetMinimumLevel(LogLevel.Debug);  // Set log level to Debug
});


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Added controllers for handling login and other actions
builder.Services.AddSession(); // Enable session management

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001"); // REST API base address: QuestionsAnswers.API
});

// Configure Authentication and Session
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";  // Path to redirect to login if not authenticated
        options.LogoutPath = "/Logout"; // Path to redirect after logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session expiration time
        options.AccessDeniedPath = "/Login"; // Redirect to login if access is denied
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use Authentication and Authorization middleware
app.UseAuthentication();  // Ensure authentication middleware is applied before authorization
app.UseAuthorization();   // Authorization middleware should be after authentication

app.UseRouting();
app.UseSession();  // Enable session middleware

// Map Razor pages and controllers
app.MapRazorPages();
app.MapControllers();

app.Run();
