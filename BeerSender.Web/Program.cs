using BeerSender.Domain;
using BeerSender.EventStore;
using BeerSender.Web.EventPublishing;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.RegisterDomain();
builder.Services.AddSignalR();
builder.Services.RegisterEventStore();

builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddMarten(opt =>
{
    opt.Connection
    (builder.Configuration.GetConnectionString("Marten")!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "BeerSender API"));
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();
app.MapHub<EventHub>("event-hub");

app.Run();