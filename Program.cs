using System;
using System.Collections.Generic;

class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public List<Account> Accounts { get; set; } = new List<Account>();

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

class Account
{
    public int AccountNumber { get; set; }
    public string HolderName { get; set; }
    public string AccountType { get; set; } // "savings" or "checking"
    public decimal Balance { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    private static int accountUnqiue = 1000;
    private const decimal InterestRate = 0.02m; //  2% monthly interest
    private DateTime? LastInterestAddedDate = null;

    public Account(string holderName, string accountType, decimal initialDeposit)
    {
        HolderName = holderName;
        AccountType = accountType;
        Balance = initialDeposit;
        AccountNumber = accountUnqiue++;
    }

    public void CalculateAndAddInterest()
    {
        if (AccountType.ToLower() == "savings")
        {
            if (LastInterestAddedDate == null || LastInterestAddedDate.Value.Month != DateTime.Now.Month)
            {
                decimal interest = Balance * InterestRate;
                Balance += interest;

                var transaction = new Transaction("interest", interest);
                Transactions.Add(transaction);

                LastInterestAddedDate = DateTime.Now;
                Console.WriteLine($"Interest of {interest:C} added to account {AccountNumber}.");
            }
            else
            {
                Console.WriteLine("Interest has already been added for this month.");
            }
        }
    }
}


class Transaction
{
    public int TransactionId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } // "deposit" , "withdrawal"
    public decimal Amount { get; set; }

    private static int tidUnique = 1;

    public Transaction(string type, decimal amount)
    {
        TransactionId = tidUnique++;
        Date = DateTime.Now;
        Type = type;
        Amount = amount;
    }
}
class Program
{
    static List<User> users = new List<User>();
    static User loggedInUser = null;

    static void Main()
    {
        bool running = true;

        while (running)
        {
           // Console.Clear();
            Console.WriteLine("!--- Banking Application ---!");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();
            Console.Clear();
            switch (choice)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    if (LoginUser())
                    {
                        ShowAccountMenu(); 
                    }
                    break;
                case "3":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void RegisterUser()
    {
        Console.Write("Enter username: ");
        var username = Console.ReadLine();
        Console.Write("Enter password: ");
        var password = Console.ReadLine();

        var user = new User(username, password);
        users.Add(user);
        Console.WriteLine("User registered successfully.");
    }

    static bool LoginUser()
    {
        Console.Write("Enter username: ");
        var username = Console.ReadLine();
        Console.Write("Enter password: ");
        var password = Console.ReadLine();

        loggedInUser = users.Find(user => user.Username == username && user.Password == password);

        if (loggedInUser != null)
        {
            Console.WriteLine("Login successful.");
            return true; 
        }
        else
        {
            Console.WriteLine("Invalid credentials.");
            return false; // login failed
        }
    }

    static void ShowAccountMenu()
    {
        while (loggedInUser != null)
        {
            //Console.Clear();
            Console.WriteLine("=== Account Menu ===");
            Console.WriteLine("1. Open Account");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Check Balance");
            Console.WriteLine("5. View Statement");
            Console.WriteLine("6. Calculate Monthly Interest");
            Console.WriteLine("7. Logout");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();
            Console.Clear();
            switch (choice)
            {
                case "1":
                    OpenAccount();
                    break;
                case "2":
                    ProcessTransaction("deposit");
                    break;
                case "3":
                    ProcessTransaction("withdrawal");
                    break;
                case "4":
                    CheckBalance();
                    break;
                case "5":
                    ViewStatement();
                    break;
                case "6":
                    CalculateInterestForAllSavingsAccounts();
                    break;
                case "7":
                    loggedInUser = null; 
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
    static void CalculateInterestForAllSavingsAccounts()
    {
        foreach (var account in loggedInUser.Accounts)
        {
            account.CalculateAndAddInterest();
        }
    }
    static void OpenAccount()
    {
        Console.Write("Enter account holder name: ");
        var holderName = Console.ReadLine();
        Console.Write("Enter account type (savings/checking): ");
        var accountType = Console.ReadLine();
        Console.Write("Enter initial deposit amount: ");
        var initialDeposit = Convert.ToDecimal(Console.ReadLine());

        var account = new Account(holderName, accountType, initialDeposit);
        loggedInUser.Accounts.Add(account);
        Console.WriteLine($"Account created successfully.");
        Console.WriteLine($"*****Account Number: {account.AccountNumber}*****");
    }

    static void ProcessTransaction(string type)
    {
        Console.Write("Enter account number: ");
        var accountNumber = Convert.ToInt32(Console.ReadLine());
        var account = loggedInUser.Accounts.Find(acc => acc.AccountNumber == accountNumber);

        if (account != null)
        {
            Console.Write($"Enter amount to {type}: ");
            var amount = Convert.ToDecimal(Console.ReadLine());

            if (type == "withdrawal" && amount > account.Balance)
            {
                Console.WriteLine("Insufficient funds.");
                return;
            }

            var transaction = new Transaction(type, amount);
            account.Transactions.Add(transaction);

            account.Balance += (type == "deposit") ? amount : -amount;
            Console.WriteLine($"{type} successful.");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    static void CheckBalance()
    {
        Console.Write("Enter account number: ");
        var accountNumber = Convert.ToInt32(Console.ReadLine());
        var account = loggedInUser.Accounts.Find(acc => acc.AccountNumber == accountNumber);

        if (account != null)
        {
            Console.WriteLine($"Current balance: {account.Balance:C}");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    static void ViewStatement()
    {
        Console.Write("Enter account number: ");
        var accountNumber = Convert.ToInt32(Console.ReadLine());
        var account = loggedInUser.Accounts.Find(acc => acc.AccountNumber == accountNumber);

        if (account != null)
        {
            Console.WriteLine();
            Console.WriteLine($"!---- Statement for Account {accountNumber} ({account.HolderName}) ----!");
            Console.WriteLine($"Account Type: {account.AccountType}");
            Console.WriteLine($"Current Balance: {account.Balance:C}\n");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"{"Date",-20} {"Transaction",-15} {"Amount",10}");
            Console.WriteLine("--------------------------------------------------");
            foreach (var trans in account.Transactions)
            {
                Console.WriteLine($"{trans.Date,-20} {trans.Type,-15} {trans.Amount,10:C}");
            }
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("End of statement\n");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

}
