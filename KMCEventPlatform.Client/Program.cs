using KMCEventPlatform.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Enable session storage.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

string apiBaseUrl =
    builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException(
        "The API base URL is missing."
    );

builder.Services.AddHttpClient<
    IEventApiService,
    EventApiService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    });

builder.Services.AddHttpClient<
    IAuthApiService,
    AuthApiService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    });

builder.Services.AddHttpClient<
    IRegistrationApiService,
    RegistrationApiService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    });

builder.Services.AddHttpClient<
    IOrganizerEventApiService,
    OrganizerEventApiService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Session must be enabled before controllers run.
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Events}/{action=Index}/{id?}"
);

app.Run();