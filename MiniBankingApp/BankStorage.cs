using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace MiniBankingApp
{
    public static class BankStorage
    {
        private static readonly string FileName =
            Path.Combine(AppContext.BaseDirectory, "accounts.json");

        public static List<Account> LoadAccounts()
        {
            if (!File.Exists(FileName))
            {
                return new List<Account>();
            }

            try
            {
                string json = File.ReadAllText(FileName);

                return JsonSerializer.Deserialize<List<Account>>(json)
                       ?? new List<Account>();
            }
            catch (JsonException)
            {
                Console.WriteLine(
                    "Error: accounts.json contains invalid JSON."
                );

                throw;
            }
        }

        public static void SaveAccounts(List<Account> accounts)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(accounts, options);

            File.WriteAllText(FileName, json);
        }
    }

}
