using FinancialAccounting.Models;
using FinancialAccounting.Services;

public class AddOperationCommand : ICommand
{
    private readonly IOperationService _service;
    private readonly Operation _operation;
    private readonly BankAccountService _accountService;

    public AddOperationCommand(IOperationService service, Operation operation, BankAccountService accountService)
    {
        _service = service;
        _operation = operation;
        _accountService = accountService;
    }

    public void Execute() => _service.AddOperation(_operation, _accountService);
}
