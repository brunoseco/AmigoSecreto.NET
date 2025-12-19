using AmigoSecreto.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization options globally
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = false;
});

// Add services to the container.
builder.Services.AddRazorPages()
    .AddJsonOptions(options =>
    {
        // Use camelCase for JSON serialization (default, but explicit for clarity)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Register HttpClient for Comtele API
builder.Services.AddHttpClient<ComteleSmsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
