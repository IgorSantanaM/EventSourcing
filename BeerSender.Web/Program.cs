using BeerSender.Domain;
using BeerSender.Web.EventPublishing;
using JasperFx.Events.Daemon;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.RegisterDomain();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddMarten(opt =>
{
    opt.Connection
    (builder.Configuration.GetConnectionString("Marten")!);

    opt.ApplyDomainConfig();
    opt.AddProjections();
}).AddAsyncDaemon(DaemonMode.Solo);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
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