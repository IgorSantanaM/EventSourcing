using BeerSender.Domain;
using BeerSender.Domain.Boxes;
using BeerSender.Web.EventPublishing;
using JasperFx.Events.Daemon;
using Marten;
using System.Reflection; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterDomain();
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen();

builder.Services.AddMarten(opt =>
{
    opt.Connection
    (builder.Configuration.GetConnectionString("Marten")!);

    opt.ApplyDomainConfig();
    opt.AddProjections();
})
    .AddSubscriptionWithServices<EventHubSubscription>(ServiceLifetime.Singleton, opt =>
    {
        opt.FilterIncomingEventsOnStreamType(typeof(Box));
        opt.Options.BatchSize = 10;
    })
    .AddAsyncDaemon(DaemonMode.Solo );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "BeerSender API v1");
    });
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
