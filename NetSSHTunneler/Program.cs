using Microsoft.AspNetCore.Builder;
using NetSSHTunneler.Services;
using Microsoft.Extensions.DependencyInjection;
using NetSSHTunneler.Services.Interfaces;
using NetSSHTunneler.Services.Services;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Host.ConfigureServices((hostBuilderContext, services) =>
{
    services.AddSignalR();
    services.AddSingleton<ISshConnector, SshConnector>();
    services.AddSingleton<IFileOperations, FileOperations>();
    services.AddSingleton<INetworkOperations, NetworkOperations>();

});

//builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
//{
//    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
//}));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseCors(builder =>
{
    builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins("http://localhost:5070", "https://localhost:44429", "http://localhost:4200");
});
//app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
    
app.MapFallbackToFile("index.html");

app.MapHub<ChatHub>("/events");

app.Run();
