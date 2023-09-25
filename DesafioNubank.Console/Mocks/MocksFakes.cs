using Bogus;
using DesafioNubank.Models;

namespace DesafioNubank.Mocks;


public static class MocksFakes
{
    private static Faker<Account> _accountFakeRule = new Faker<Account>("pt_BR")
        .RuleFor(c => c.Active, f => f.PickRandomParam(new bool[] { true, true, false }))
        .RuleFor(c => c.Limit, f => f.Random.Int(900, 5000));

    private static Faker<Transaction> _transactionFakeRule = new Faker<Transaction>("pt_BR")
        .RuleFor(c => c.Amount, f => f.Random.Int(1, 250))
        .RuleFor(c => c.TransactionDate, f => f.Date.Future(0, DateTime.Now));

    public static IEnumerable<Transaction> GenerateTransaction(int qtt) => _transactionFakeRule
        .Generate(qtt);

    public static IEnumerable<Account> GenerateAccountFake(int qtt, int qttTransaction = 10)
        => Enumerable.Range(0, qtt)
            .Select(_ =>
            {
                var acc = _accountFakeRule.Generate(1).First();
                if (qttTransaction > 0)
                {
                    acc.AddTransaction( GenerateTransaction(qttTransaction).ToList());
                }

                return acc;
            });
}
