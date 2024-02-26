using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Account_Interest.Entities
{
    public class AccountTransaction : BaseClass
    {
        public string AccountNumber { get; set; }
        public char TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

    }
}
