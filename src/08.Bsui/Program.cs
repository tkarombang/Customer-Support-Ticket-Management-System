using TicketManagement.Client.Interfaces;
using TicketManagement.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ITicketApiClient, TicketApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WebApiBaseUrl"]!);
});


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddDistributedMemoryCache(); // dibutuhkan Session

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
