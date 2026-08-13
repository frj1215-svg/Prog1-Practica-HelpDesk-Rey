using MiSolucion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// inyectamos el repositorio y el servicio de tickets
var rutaArchivo = Path.Combine(AppContext.BaseDirectory, "tickets.json");
builder.Services.AddScoped(sp => new TicketRepository(rutaArchivo));
builder.Services.AddScoped<TicketService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

