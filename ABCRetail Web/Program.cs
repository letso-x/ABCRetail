using ABCRetail.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<TableStorageService>();
builder.Services.AddScoped<BlobStorageService>();
builder.Services.AddScoped<QueueStorageService>();
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddHttpClient<AzureFunctionsClient>(
    (serviceProvider, httpClient) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var baseUrl = configuration["AzureFunctions:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "AzureFunctions:BaseUrl is not configured.");
        }

        httpClient.BaseAddress = new Uri(
            baseUrl.TrimEnd('/') + "/");
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
