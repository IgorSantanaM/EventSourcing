using Marten;
using Microsoft.AspNetCore.Http;

namespace BeerSender.Domain
{
    /// <summary>
    /// Command Router - an orchestrator that savechanges on each command handler.
    /// </summary>
    /// <param name="eventStore"></param>
    /// <param name="serviceProvider"></param>
    public class CommandRouter(IServiceProvider serviceProvider, IDocumentStore store, IHttpContextAccessor httpContextAccessor)
    {
        public async Task HandleCommand(ICommand command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handler = serviceProvider.GetService(handlerType);
            var methodInfo = handlerType.GetMethod("Handle");

            await using var session = store.IdentitySession();

            // If you have a command ID from the outside(message ID, request ID) use it over here instead of a new GUID.
            var commandId = Guid.NewGuid();

            StoreCommand(session, command, commandId);

            ConfigureSession(session, commandId);

            var handle = (Task)methodInfo?.Invoke(handler, [session, command])!;
            await handle!;

            await session.SaveChangesAsync();
        }

        private void ConfigureSession(IDocumentSession session, Guid commandId)
        {
            session.CausationId = commandId.ToString();

            session.CorrelationId = commandId.ToString();

            session.SetHeader("TraceIdentifier", httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty);
        }

        private void StoreCommand(IDocumentSession session, ICommand command, Guid commandId)
        {
            LoggedCommand loggdCommand = new(
                commandId,
                httpContextAccessor.HttpContext.User.Identity?.Name,
                DateTime.UtcNow,
                command);

            session.Insert(loggdCommand);
        }
    }

    public record LoggedCommand(
        Guid CommandId,
        string? UserName,
        DateTime Timestamp,
        ICommand command);
}
