using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
//NECESITAMOS LA CADENA DE CONEXION DE APP INSIGHT
string cnnInsight =
    builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString");
builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.SetMinimumLevel(LogLevel.Trace);
    //DEBEMOS INDICAR EL PROVEEDOR DE LOGGER
    options.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
    //HABILITAMOS LOGGER DENTRO DE AZURE INSIGHT
    options.AddApplicationInsights(cnnInsight);
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
