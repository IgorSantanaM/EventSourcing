using System.Reflection;

namespace BeerSender.Domain
{
    /// <summary>
    /// Command Router - an orchestrator that savechanges on each command handler.
    /// </summary>
    /// <param name="eventStore"></param>
    /// <param name="serviceProvider"></param>
    public class CommandRouter(IEventStore eventStore, IServiceProvider serviceProvider)
    {
        public void HandleCommand(object command)
        {
            Type? commandType = command.GetType();

            Type? handlerType = typeof(CommandHandler<>).MakeGenericType(commandType);

            object? handler = serviceProvider.GetService(handlerType);

            MethodInfo? methodInfo = handlerType.GetMethod("Handle");

            methodInfo?.Invoke(handler, [command]);

            eventStore.SaveChanges();
        }
    }
}
