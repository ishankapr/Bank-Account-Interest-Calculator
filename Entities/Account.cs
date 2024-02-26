using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Account_Interest.Entities
{
    public class Account : BaseClass
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public decimal MonthlyInterest { get; set; }
    }
}
