namespace DesafioNubank.Tests;

public class Tests
{
    private const int  QTT_ACCOUNTS = 10;
    private const int QTT_TRANSACTIONS = 10;
    
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GenerateOnlyAccount()
    {
        var accounts = MocksFakes.GenerateAccountFake(QTT_ACCOUNTS, 0);
        
        Assert.True(accounts.Count()== 10, $"Generate {QTT_ACCOUNTS} transactions");
        Assert.True(accounts.All(v=> !v.History.Any()), "All accounts dont have a transaction");
    }

    [Test]
    public void GenerateTransactions()
    {
        var transactions = MocksFakes.GenerateTransaction(QTT_TRANSACTIONS);
        Assert.True(transactions.Count() == 10, $"Generate {QTT_TRANSACTIONS} transactions");
    }

    [Test]
    public void CalcLimitCorrect()
    {
        var account = GenerateAccountForTest();

        int limitAccount = 1000;
        
        Assert.That(account.AvailableLimit, Is.EqualTo(limitAccount),
            $"Check if Available limit no history is ${limitAccount}");
        
        account.AddTransaction(GenerateTransaction(500, DateTime.Now.AddMinutes(-5)));
        account.AddTransaction(GenerateTransaction(250, DateTime.Now.AddMinutes(-5)));
        
        Assert.That(account.AvailableLimit, Is.EqualTo(limitAccount-500-250),
            $"Check before add transaction limit is ${limitAccount-500-250}");
        
        Assert.That(account.TotalExpensesAllHistory(), Is.EqualTo(250+500),
            $"Check if total history is 750");
    }

    [Test]
    public void TestAddTransactionToAccount()
    {
        var account = GenerateAccountForTest();

        Assert.Throws<ArgumentOutOfRangeException>(() => 
                account.AddTransaction(GenerateTransaction(0, DateTime.Now)),
            "Is not possible add transaction 0 or minus");
        Assert.Throws<ArgumentOutOfRangeException>(() => 
                account.AddTransaction(GenerateTransaction(-10, DateTime.Now)),
            "Is not possible add transaction 0 or minus");
        
        Assert.DoesNotThrow(()=> account.AddTransaction(GenerateTransaction(50, DateTime.Now)));
    }
    
    [Test]
    public void TestGetTransactionbyMerchant()
    {
        var account = GenerateAccountForTest();
        const string merchant = "GasStation";

        account.AddTransaction(GenerateTransaction(50, DateTime.Now, merchant));
        account.AddTransaction(GenerateTransaction(100, DateTime.Now, merchant));
        account.AddTransaction(GenerateTransaction(100, DateTime.Now));
        account.AddTransaction(GenerateTransaction(100, DateTime.Now));

        var transactions = account.GetTransactionByMerchant(merchant);

        Assert.That(transactions.Count(), Is.EqualTo(2),
            $"Check if have 2 transactions with merchant {merchant}");

        var totalTransactionsTest = transactions.Sum(tt => tt.Amount);
        
        Assert.That(account.GetTotalTransactionByMerchant(merchant), Is.EqualTo(totalTransactionsTest),
            $"Check if Total of merchant {merchant} is {150}");
    }
    
    [Test]
    public void TestTotalTransactionCurrentMonth()
    {
        var account = GenerateAccountForTest();
        const string merchant = "GasStation";

        var baseDateTransaction = new DateTime(1989, 10, 11);

        account.AddTransaction(GenerateTransaction(50, baseDateTransaction.AddMonths(-1), merchant));
        account.AddTransaction(GenerateTransaction(80, baseDateTransaction.AddMonths(-1), merchant));
        account.AddTransaction(GenerateTransaction(50, baseDateTransaction.AddMonths(1), merchant));
        account.AddTransaction(GenerateTransaction(50, DateTime.Today, merchant));

        var totalTransactionMonth = account.AvailableLimitCurrentMonth();
        
        Assert.That(totalTransactionMonth, Is.EqualTo(950), "Check if Available limit is 950");
    }

    [Test]
    public void TestUpdateLimit()
    {
        var account = GenerateAccountForTest(500);
        
        Assert.That(account.Limit, Is.EqualTo(500), 
            "Check if new limit is 500");
        
        account.UpdateLimit(100);
        
        Assert.That(account.Limit, Is.EqualTo(100), 
            "Check if new limit is 50");
    }
    private Account GenerateAccountForTest(int limit = 1000) => new Account(active: true, limit: limit);

    private Transaction GenerateTransaction(int valueTransaction, DateTime transactionDate, string? merchant=null) =>
        new Transaction()
        {
            Amount = valueTransaction,
            Merchant = string.IsNullOrEmpty(merchant) ? $"Teste-{valueTransaction}" : merchant,
            TransactionDate = transactionDate

        };
}