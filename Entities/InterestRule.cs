using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Account_Interest.Entities
{
    public class InterestRule : BaseClass
    {
        public string RuleNumber { get; set; }
        public decimal Rate { get; set; }
    }
}
