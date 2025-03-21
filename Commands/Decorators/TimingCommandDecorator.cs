using System.Diagnostics;

namespace FinancialAccounting.Commands.Decorators
{
    public class TimingCommandDecorator : ICommand
    {
        private readonly ICommand _decoratedCommand;
        public TimeSpan ExecutionTime { get; private set; }

        public TimingCommandDecorator(ICommand command)
        {
            _decoratedCommand = command;
        }

        public void Execute()
        {
            var stopwatch = Stopwatch.StartNew();
            _decoratedCommand.Execute();
            stopwatch.Stop();
            ExecutionTime = stopwatch.Elapsed;
        }
    }
}
