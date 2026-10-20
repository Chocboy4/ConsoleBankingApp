using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBankingApp
{
    public class Account
    {
        public string Name { get; set; } = "";
        public decimal Balance { get; set; }
        public List<string> Transactions { get; set; } = new();
    }
}
