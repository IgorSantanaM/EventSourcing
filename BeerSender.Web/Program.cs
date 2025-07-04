using BeerSender.Domain;
using BeerSender.EventStore;
using BeerSender.Web.EventPublishing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.RegisterDomain();
builder.Services.AddSignalR();
builder.Services.RegisterEventStore();

builder.Services.AddTransient<INotificationService, NotificationService>();

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