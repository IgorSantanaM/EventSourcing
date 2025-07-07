namespace BeerSender.Domain
{
    /// <summary>
    /// Command Router - an orchestrator that savechanges on each command handler.
    /// </summary>
    /// <param name="eventStore"></param>
    /// <param name="serviceProvider"></param>
    public class CommandRouter(IServiceProvider serviceProvider)
    {
        public void HandleCommand(object command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handler = serviceProvider.GetService(handlerType);
            var methodInfo = handlerType.GetMethod("Handle");
            methodInfo?.Invoke(handler, [command]);
        }
    }
}
