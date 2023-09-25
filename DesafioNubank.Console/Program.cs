// See https://aka.ms/new-console-template for more information

using DesafioNubank.Mocks;

Console.WriteLine("Hello, World!");

var accs = MocksFakes.GenerateAccountFake(10).ToList();

accs.ForEach(acc=> Console.WriteLine($@"Limit: {acc.Limit}
Active: {acc.Active}
Current Limit: {acc.AvailableLimit}
Total Transactions: {acc.TotalExpensesAllHistory()}
========================="));

