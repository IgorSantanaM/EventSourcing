using BeerSender.Domain;
using BeerSender.EventStore;
using BeerSender.QueryAPI.Database;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.RegisterDomain();
builder.Services.RegisterEventStore();
builder.Services.RegisterReadDatabase();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "BeerSender QueryAPI"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
