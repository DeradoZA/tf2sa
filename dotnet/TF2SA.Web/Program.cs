using TF2SA.Data;
using Microsoft.AspNetCore.HttpOverrides;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Repositories.MariaDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddScoped<IPlayersRepository<Player, ulong>, PlayersRepository>();

var app = builder.Build();

// configure for reverse proxy
app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    }
);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
