using TF2SA.StatsETLService;
using Microsoft.AspNetCore.HttpOverrides;
using TF2SA.Query;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddStatsETLService(builder.Configuration);
builder.Services.AddQueries(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// configure for reverse proxy
app.UseForwardedHeaders(
	new ForwardedHeadersOptions
	{
		ForwardedHeaders =
			ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
	}
);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
