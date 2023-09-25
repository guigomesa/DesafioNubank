namespace DesafioNubank.Models;


public class Transaction
{
    public string? Merchant { get; set; }
    public int Amount { get; set; } 
    public TimeOnly Time => TimeOnly.FromDateTime(TransactionDate);
    public DateOnly Date => DateOnly.FromDateTime(TransactionDate);
    public DateTime TransactionDate { get; set; } = DateTime.Now;
}

public class Account
{

    public Account(){ }

public Account(bool active, int limit)
    {
        Active = active;
        Limit = limit;
    }

    public bool Active { get; set; }
    public int AvailableLimit => Limit - TotalExpensesAllHistory();
    public int Limit { get; private set; }

    private List<Transaction> _history { get; set; } = new List<Transaction>();
    public IEnumerable<Transaction> History => _history.ToList();

    public void UpdateLimit(int newLimit)
    {
        //apply any product rules
        Limit = newLimit;
    }

    public int TotalTransactions(DateOnly initialDate, DateOnly endDate)
    {
       return History
           .Where(h
               => h.Date >= initialDate 
                  && h.Date <= endDate)
            .Sum(h => h.Amount);
    }

    public int TotalExpensesAllHistory()
        => TotalTransactions(DateOnly.MinValue, DateOnly.MaxValue);
    
    public int AvailableLimitCurrentMonth()
    {
        var now = DateTime.Today;

        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        return Limit - TotalTransactions(
            DateOnly.FromDateTime(firstDayOfMonth), 
            DateOnly.FromDateTime(lastDayOfMonth));
    }

    public IEnumerable<Transaction> GetTransactionByMerchant(string merchant)
        => History.Where(h => h.Merchant == merchant);

    public int GetTotalTransactionByMerchant(string merchant)
        => GetTransactionByMerchant(merchant).Sum(t => t.Amount);

    public void AddTransaction(Transaction transaction)
    {
        //apply any product rules
        if (transaction.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(transaction), "Value of ammount can be 0 or minus");
        }
        _history.Add(transaction);
    }

    public void AddTransaction(List<Transaction> transactions) =>
        transactions.ForEach(AddTransaction);
}