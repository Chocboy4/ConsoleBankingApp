
using Banking_App.Banners;
using MiniBankingApp;
using System;
using System.Collections.Generic;
using System.Reflection;


Console.OutputEncoding = System.Text.Encoding.UTF8;
List<Account> accounts;

try
{
    accounts = BankStorage.LoadAccounts();
}
catch (Exception ex) when (
    ex is System.Text.Json.JsonException ||
    ex is System.IO.IOException ||
    ex is UnauthorizedAccessException)
{
    Console.WriteLine($"Unable to load accounts: {ex.Message}");
    return;
}

var defaultPins = new Dictionary<string, string>
{
    ["francis"] = "1234",
    ["doyin"] = "5678",
    ["brian"] = "9012",
    ["nahimout"] = "3456"
};

bool updated = false;
foreach (var acc in accounts)
{
    if (string.IsNullOrEmpty(acc.Pin))
    {
        acc.Pin = defaultPins.GetValueOrDefault(acc.Name.ToLower(), "0000");
        updated = true;
    }
}

if (updated)
{
    BankStorage.SaveAccounts(accounts);
}



if (accounts.Count == 0)
{
    accounts = new List<Account>
    {
        new Account { Name = "francis", Pin = "1234", Balance = 100000m },
        new Account { Name = "doyin", Pin = "5678", Balance = 5000000m },
        new Account { Name = "brian", Pin = "9012", Balance = 250000m },
        new Account { Name = "nahimout", Pin = "3456", Balance = 7500000m }
    };

    BankStorage.SaveAccounts(accounts);
}

bool applicationRunning = true;

while (applicationRunning)
{
    const int MaxAttempts = 3;
    int attempts = 0;
    Account? current = null;

    while (current == null)
    {
        Console.Write("Enter the name on your account: ");
        string userName = (Console.ReadLine() ?? "").Trim();

        current = accounts.Find(
            a => a.Name.Equals(userName, StringComparison.OrdinalIgnoreCase)
        );

        if (current != null)
        {
            bool pinCorrect = false;

            for (int pinAttempt = 1; pinAttempt <= MaxAttempts; pinAttempt++)
            {
                Console.Write("Enter your PIN: ");
                string enteredPin = (Console.ReadLine() ?? "").Trim();

                if (enteredPin == current.Pin)
                {
                    pinCorrect = true;
                    break;
                }

                int attemptsLeft = MaxAttempts - pinAttempt;
                if (attemptsLeft > 0)
                {
                    Console.WriteLine($"Incorrect PIN. {attemptsLeft} attempt(s) left.");
                }
            }

            if (pinCorrect)
            {
                break;
            }

            Console.WriteLine("Too many incorrect PIN attempts. Thank you for using TatumBank. Goodbye!");
            return;
        }

        Console.WriteLine("\nAccount not found, Please check the name and try again.");
        attempts++;
        

        if (attempts >= MaxAttempts)
        {
            Console.WriteLine(
                "Too many incorrect attempts. Thank you for using TatumBank. Goodbye!"
            );
            return;
        }


        Console.WriteLine($"Attempts remaining: {MaxAttempts - attempts}");
        Console.WriteLine("1. Try again");
        Console.WriteLine("2. Create a new account");
        Console.Write("Choose an option: ");

        string option = (Console.ReadLine() ?? "").Trim();

        if (option == "2")
        {
            Console.Write("Enter a name for your new account: ");
            string newName = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(newName))
            {
                Console.WriteLine("Name cannot be empty.\n");
                continue;
            }

            if (accounts.Exists(
                a => a.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("That account name already exists.\n");
                continue;
            }

            Console.Write("Choose a 4-digit PIN: ");
            string newPin = (Console.ReadLine() ?? "").Trim();

            if (newPin.Length != 4 || !newPin.All(char.IsDigit))
            {
                Console.WriteLine("PIN must be exactly 4 digits.\n");
                continue;
            }

            current = new Account
            {
                Name = newName,
                Pin = newPin,
                Balance = 0m
            };

            accounts.Add(current);
            BankStorage.SaveAccounts(accounts);

            Console.WriteLine($"Account created for {current.Name}.\n");
        }
        else if (option != "1")
        {
            Console.WriteLine("Invalid option. Try again.\n");
        }
        else
        {
            Console.WriteLine();
        }
    }



    string[] validBranches = { "Lagos", "Abuja" };
    string? branch = null;

    while (branch == null)
    {
        Console.Write("Enter your branch city (Lagos or Abuja): ");
        string branchInput = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(branchInput))
        {
            Console.WriteLine("Branch cannot be empty. Please try again.\n");
            continue;
        }

        branch = Array.Find(
            validBranches,
            b => b.Equals(branchInput, StringComparison.OrdinalIgnoreCase)
        );

        if (branch == null)
        {
            Console.WriteLine("Invalid branch. Please enter Lagos or Abuja.\n");
        }
    }

    bool running = true;

    while (running)
    {


        Banner.Display($"Welcome to TatumBank {branch} branch");

        Console.WriteLine(
            $"\nWelcome to TatumBank {branch} branch, {current.Name}"
        );

        Console.WriteLine("\n1. Check balance");
        Console.WriteLine("2. Transfer");
        Console.WriteLine("3. Airtime");
        Console.WriteLine("4. Transactions");
        Console.WriteLine("5. Logout");
        Console.WriteLine("6. Exit");
        Console.Write("\nChoose an option: ");

        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                Console.WriteLine(
                    $"\nYour balance is: ₦{current.Balance:N2}"
                );
                Pause();
                break;

            case "2":
                Console.Write("Enter the recipient's account name: ");

                string transferTo =
                    (Console.ReadLine() ?? "").Trim();

                Account? recipient = accounts.Find(
                    a => a.Name.Equals(
                        transferTo,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

                if (recipient == null)
                {
                    Console.WriteLine("Recipient account not found.");
                    Pause();
                    break;
                }

                if (recipient == current)
                {
                    Console.WriteLine(
                        "You cannot transfer money to yourself."
                    );
                    Pause();
                    break;
                }

                Console.Write("Enter amount to transfer: ");

                if (!decimal.TryParse(
                    Console.ReadLine(),
                    out decimal amount
                ))
                {
                    Console.WriteLine("Please enter a valid amount.");
                    Pause();
                    break;
                }

                if (amount <= 0)
                {
                    Console.WriteLine(
                        "Amount must be greater than zero."
                    );
                    Pause();
                    break;
                }

                if (amount > current.Balance)
                {
                    Console.WriteLine("Insufficient funds.");
                    Pause();
                    break;
                }

                //Console.WriteLine("Enter Your pin: ");
                //string enteredPin = (Console.ReadLine() ?? "").Trim();

                //if (!enteredPin.Equals(current.Pin))
                //{
                //    Console.WriteLine("Incorrect pin.");
                //    Pause();
                //    break;
                //}

                const int MaxPinAttempts = 3;
                bool pinOk = false;

                for (int attempt = 1; attempt <= MaxPinAttempts; attempt++)
                {
                    Console.Write("Enter your PIN: ");
                    string enteredPin = (Console.ReadLine() ?? "").Trim();

                    if (enteredPin == current.Pin)
                    {
                        pinOk = true;
                        break; // leaves the for loop only
                    }

                    int left = MaxPinAttempts - attempt;
                    if (left > 0)
                        Console.WriteLine($"Incorrect PIN. {left} attempt(s) left.");
                }

                if (!pinOk)
                {
                    Console.WriteLine("Too many incorrect attempts. You have been logged out.");

                    running = false;

                    Pause();
                    break; 
                }



                current.Balance -= amount;
                recipient.Balance += amount;


                current.Transactions.Add(
                    $"Transferred ₦{amount:N2} to {recipient.Name}"
                );

                recipient.Transactions.Add(
                    $"Received ₦{amount:N2} from {current.Name}"
                );


                try
                {
                    BankStorage.SaveAccounts(accounts);

                    Console.WriteLine(
                        $"\nTransfer successful! You sent ₦{amount:N2} " +
                        $"to {recipient.Name}."
                    );

                    Console.WriteLine(
                        $"Your new balance is {current.Balance:N2}."
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Could not save the transfer: {ex.Message}"
                    );

                    Console.WriteLine(
                        "Restarting is not recommended until the data is checked."
                    );
                }

                Pause();
                break;

            case "3":
                Console.Write("Choose network (MTN, Airtel, Glo, 9mobile): ");

                string network =
                    (Console.ReadLine() ?? "").Trim();

                string[] validNetworks =
                    { "MTN", "Airtel", "Glo", "9mobile", "Etisalat" };

                bool validNetwork = Array.Exists(
                    validNetworks,
                    n => n.Equals(
                        network,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

                if (!validNetwork)
                {
                    Console.WriteLine("Invalid network.");
                    Pause();
                    break;
                }

                Console.Write("Enter phone number: ");

                string phoneNumber =
                    (Console.ReadLine() ?? "").Trim();

                if (phoneNumber.Length != 11 ||
                    !phoneNumber.StartsWith("0") ||
                    !phoneNumber.All(char.IsDigit))
                {
                    Console.WriteLine(
                        "Enter a valid 11-digit Nigerian phone number."
                    );
                    Pause();
                    break;
                }

                Console.Write("Enter airtime amount: ");

                if (!decimal.TryParse(
                    Console.ReadLine(),
                    out decimal airtimeAmount
                ))
                {
                    Console.WriteLine("Please enter a valid amount.");
                    Pause();
                    break;
                }

                if (airtimeAmount <= 50)
                {
                    Console.WriteLine(
                        "Amount must be greater than Fourty nine Naira."
                    );
                    Pause();
                    break;
                }
                const int PinAttempts = 3;
                bool isPinCorrect = false;

                for (int attempt = 1; attempt <= PinAttempts; attempt++)
                {
                    Console.Write("Enter your PIN: ");
                    string enteredPin = (Console.ReadLine() ?? "").Trim();

                    if (enteredPin == current.Pin)
                    {
                        isPinCorrect = true;
                        break; // leaves the for loop only
                    }

                    int left = PinAttempts - attempt;
                    if (left > 0)
                        Console.WriteLine($"Incorrect PIN. {left} attempt(s) left.");
                }

                if (!isPinCorrect)
                {
                    Console.WriteLine("Too many incorrect attempts. You have been logged out.");

                    running = false;

                    Pause();
                    break;
                }

                if (airtimeAmount > current.Balance)
                {
                    Console.WriteLine("Insufficient funds.");
                    Pause();
                    break;
                }


                current.Balance -= airtimeAmount;

                current.Transactions.Add(
                    $"Purchased airtime for {phoneNumber} " +
                    $"on {network}, amount: ₦{airtimeAmount:N2}"
                );

                try
                {
                    BankStorage.SaveAccounts(accounts);

                    Console.WriteLine("\nAirtime purchase successful!");

                    Console.WriteLine($"Phone number: {phoneNumber}");

                    Console.WriteLine($"Amount: ₦{airtimeAmount:N2}");

                    Console.WriteLine($"New balance: ₦{current.Balance:N2}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not save the airtime purchase: {ex.Message}");
                }

                Pause();
                break;

            case "4":
                Console.WriteLine($"\n{current.Name}'s Transaction History");

                if (current.Transactions.Count == 0)
                {
                    Console.WriteLine("No transactions found.");
                }
                else
                {
                    for (
                        int i = 0;
                        i < current.Transactions.Count;
                        i++
                    )
                    {
                        Console.WriteLine(
                            $"{i + 1}. {current.Transactions[i]}"
                        );
                    }
                }

                Pause();
                break;

            case "5":
                
                Console.WriteLine(
                    $"\nGoodbye, {current.Name}! You have been logged out."
                );
                running = false;
                break;

            case "6":
                Console.WriteLine("Thank you for using TatumBank!");
                applicationRunning = false;
                running = false;
                break;

            default:
                Console.WriteLine(
                    "Invalid option. Please choose 1 to 6."
                );
                Pause();
                break;
        }
    }
}

void Pause()
{
    Console.WriteLine("\nPress Enter to continue...");
    Console.ReadLine();
}
