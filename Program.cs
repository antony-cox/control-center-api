using DesktopAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddEventLog(eventLogSettings =>
{
    eventLogSettings.SourceName = "ControlCenterAPI";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAnyOrigin", builder => {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddSingleton<IPowershellService, PowershellService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ControlCenter API", Version = "v1"});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();