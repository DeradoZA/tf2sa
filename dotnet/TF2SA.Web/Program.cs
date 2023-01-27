using TF2SA.StatsETLService;
using Microsoft.AspNetCore.HttpOverrides;
using TF2SA.Query;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
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

//app.UseHttpsRedirection();
app.UsePathBase("/api");

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors(
	options =>
		options
			.WithOrigins("http://localhost:4200", "https://localhost:4200")
			.AllowAnyMethod()
			.AllowAnyHeader()
);

app.UseAuthorization();

app.MapControllers();

app.Run();
