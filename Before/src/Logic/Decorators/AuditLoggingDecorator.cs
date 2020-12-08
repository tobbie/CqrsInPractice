using static System.Console;
using CSharpFunctionalExtensions;
using Logic.AppServices;
using Newtonsoft.Json;

namespace Logic.Decorators
{
    public class AuditLoggingDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        public AuditLoggingDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler;

        }

        public Result Handle(TCommand command)
        {
            string commandJson = JsonConvert.SerializeObject(command);
            WriteLine($"Command of type {command.GetType().Name}: {commandJson}");
            return _handler.Handle(command);
        }
    }
}
