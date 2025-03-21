using System;
using System.Collections.Generic;
using FinancialAccounting.Models;

namespace FinancialAccounting.Services
{
    public class BankAccountService
    {
        private readonly List<BankAccount> _accounts = new();

        public BankAccount CreateAccount(string name)
        {
            var account = new BankAccount { Name = name };
            _accounts.Add(account);
            return account;
        }

        public void UpdateBalance(Guid accountId, decimal amount)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == accountId);
            if (account != null) account.Balance += amount;
        }
        public BankAccount GetAccount(Guid accountId) =>
    _accounts.FirstOrDefault(a => a.Id == accountId);

        public void CreateAccountWithId(Guid id, string name)
        {
            _accounts.Add(new BankAccount
            {
                Id = id,
                Name = name
            });
        }
        public void Clear() => _accounts.Clear();

        public IEnumerable<BankAccount> GetAllAccounts() =>
            _accounts;
    }
}