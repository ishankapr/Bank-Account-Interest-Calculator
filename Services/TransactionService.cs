using Bank_Account_Interest.DataLayer;
using Bank_Account_Interest.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Account_Interest.Services
{
    public interface ITransactionService
    {
        bool InputTransaction(string transaction);
        bool DefineInterestRules(string rule);
        bool PrintStatement(string statement);
        decimal TransactionOrRuleChange(string account, int month, DateTime startDayOfMonth, DateTime endDayOfMonth);
        decimal CalculateInterest(decimal balance, decimal rate, int numberOfDates);
    }

    public class TransactionService : ITransactionService
    {

        /*
         * 
         * 
         Insert transactions and all the validations related to that included here
         * 
         *
         */


        public bool InputTransaction(string transaction)
        {
            // insert transaction
            try
            {
                var newTransaction = new AccountTransaction();
                var account = new Account();
                string[] transactionValues = transaction.Split('|');

                if (DateTime.TryParseExact(transactionValues[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    //Parsed Successfully
                    var type = Convert.ToChar(transactionValues[2].ToUpper());
                    var count = SharedData.GetTransactins().Where(x => x.CreatedOn.Date == dateValue.Date).Count();

                    var amount = decimal.Parse(transactionValues[3]);
                    var accountNumber = transactionValues[1];

                    if (amount <= 0 || amount.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length > 2)
                    {
                        Console.Write("Values greater than 0 with 2 decimal points will allow.. \n ");
                        return false;
                    }

                    account.AccountNumber = accountNumber;
                    account.CreatedOn = dateValue;

                    newTransaction.CreatedOn = dateValue;
                    newTransaction.TransactionDate = dateValue;

                    var runningNumber = $"{dateValue:yyyyMMdd}-{count + 1}";

                    //first transaction cannot be withdrawal 
                    if (SharedData.GetAccounts().Where(x => x.AccountNumber == accountNumber).Count() == 0 && type == 'W')
                    {
                        Console.Write("First transaction cannot be withdrawal.. \n ");
                        return false;
                    }

                    // Other characters invalid
                    if (type != 'D' && type != 'W')
                    {
                        Console.Write("Please Enter Valid Transaction Type.. \n ");
                        return false;
                    }

                    //if it's withdrawal, if balance will be less than 0
                    if (type == 'W' && SharedData.GetAccounts().Where(x => x.AccountNumber == accountNumber).FirstOrDefault().Balance - amount <= 0)
                    {
                        Console.Write("Balance should be greater than zero.. \n ");
                        return false;
                    }



                    if (SharedData.GetAccounts().Where(x => x.AccountNumber == accountNumber).Count() == 0)
                    {
                        //adding account if not available
                        account.Balance = amount;
                        SharedData.SetAccount(account);
                        Console.Write($"Account {transactionValues[1]} added.. \n ");
                        newTransaction.Balance = amount;
                    }
                    else
                    {
                        var balance = SharedData.GetAccounts().Where(x => x.AccountNumber == accountNumber).First().Balance;
                        SharedData.GetAccounts().Where(x => x.AccountNumber == accountNumber).First().Balance = type == 'D' ? balance + amount : balance - amount;
                        newTransaction.Balance = SharedData.GetAccounts().Where(x => x.AccountNumber == accountNumber).First().Balance;
                    }



                    newTransaction.AccountNumber = transactionValues[1];
                    newTransaction.TransactionType = type;
                    newTransaction.TransactionId = runningNumber;
                    newTransaction.Amount = amount;

                    SharedData.SetTransactin(newTransaction);
                    Console.Write("Transaction added.. \n\n ");

                    Console.Write($"Account {accountNumber} \n ");
                    Console.Write("Date \t\t | Txn Id \t | Type \t | Amount \t | Balance \n ");

                    foreach (AccountTransaction tr in SharedData.GetTransactins().Where(x => x.AccountNumber == accountNumber))
                    {
                        Console.Write($"{tr.CreatedOn:yyyyMMdd} \t | {tr.TransactionId} \t | {tr.TransactionType} \t\t | {tr.Amount} \t | {tr.Balance} \n\n ");
                    }
                    Console.Write("Is there anything else you'd like to do? \n ");
                    return true;
                }
                else
                {
                    Console.Write("Invalid Date Input. Starting again.. \n ");
                    return false;
                }

            }
            catch (Exception)
            {
                Console.Write("Invalid Transaction Input. Starting again.. \n ");
                return false;
            }

        }


        /*
         * 
         *
         Insert Interest Definitions Here
         * 
         *
         */


        public bool DefineInterestRules(string rule)
        {
            try
            {
                string[] ruleValues = rule.Split('|');
                DateTime dateValue;
                InterestRule interestRule = new InterestRule();

                if (DateTime.TryParseExact(ruleValues[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                {
                    var rate = decimal.Parse(ruleValues[2]);

                    if (rate <= 0 || rate >= 100)
                    {
                        Console.Write("Please add valid rate.. \n ");
                        return false;
                    }

                    var existingRule = SharedData.GetInterestRules().Where(x => x.CreatedOn == dateValue).FirstOrDefault();
                    if (existingRule != null)
                    {
                        SharedData.RemoveInterestRule(dateValue);
                    }

                    interestRule.CreatedOn = dateValue;
                    interestRule.Rate = rate;
                    interestRule.RuleNumber = ruleValues[1];
                    SharedData.SetInterestRule(interestRule);


                    Console.Write("Date \t\t | Rule Id \t | Rate \n ");

                    foreach (InterestRule r in SharedData.GetInterestRules())
                    {
                        Console.Write($"{r.CreatedOn:yyyyMMdd} \t | {r.RuleNumber} \t | {r.Rate} \n\n ");
                    }

                    Console.Write("Is there anything else you'd like to do? \n ");
                    return true;
                }
                else
                {
                    Console.Write("Invalid Date Input. Starting again.. \n ");
                    return false;
                }
            }
            catch (Exception)
            {
                Console.Write("Invalid Rule Input. Starting again.. \n ");
                return false;
            }

        }



        /*
         * 
         * 
         Print Statements Here
         * 
         * 
         */


        public bool PrintStatement(string statement)
        {
            try
            {
                string[] statementValues = statement.Split('|');

                var account = statementValues[0];
                var month = int.Parse(statementValues[1]);

                // consider 2023 only. 
                var startDayOfMonth = new DateTime(2023, month, 1);
                var endDayOfMonth = new DateTime(2023, month, DateTime.DaysInMonth(2023, month));

                var interest = TransactionOrRuleChange(account, month, startDayOfMonth, endDayOfMonth.AddDays(1));
                var transactions = SharedData.GetTransactins().Where(x => x.AccountNumber == account && x.TransactionDate.Month == month).ToList();
                SharedData.SetAccountInterest(account,interest);


                Console.Write($"Account: {account} \n ");
                Console.Write("Date \t\t | Txn Id \t | Type \t | Amount \t | Balance \n ");

                foreach (AccountTransaction tr in transactions)
                {
                    Console.Write($"{tr.CreatedOn:yyyyMMdd} \t | {tr.TransactionId} \t | {tr.TransactionType} \t\t | {tr.Amount} \t | {tr.Balance} \n\n ");
                }

                Console.Write($"{endDayOfMonth:yyyyMMdd} \t |  \t\t | I \t\t | {interest} \t | { SharedData.GetAccounts().Where(x => x.AccountNumber == account).FirstOrDefault().Balance + interest} \n\n ");

                Console.Write("Is there anything else you'd like to do? \n ");

                return true;
            }
            catch (Exception)
            {
                Console.Write("Invalid Statement Input. Starting again.. \n ");
                return false;
            }

        }
        /*
         * 
         * 
         Support to calculate interest function in between transactions and rule changes
         * 
         * 
         */

        public decimal TransactionOrRuleChange(string account, int month, DateTime startDayOfMonth, DateTime endDayOfMonth)
        {
            var interestAmount = 0.0m;

            try
            {
                List<ChangeType> changes = new List<ChangeType>();

                var transactionChanges = SharedData.GetTransactins().Where(x => x.AccountNumber == account && x.TransactionDate.Month == month).GroupBy(x => x.TransactionDate).Select(x => new ChangeType { Type = "TR", Date = x.Key });
                var ruleChanges = SharedData.GetInterestRules().Where(x => x.CreatedOn.Month == month).Select(x => new ChangeType { Type = "RU", Date = x.CreatedOn });

                changes.AddRange(transactionChanges);
                changes.AddRange(ruleChanges);

                // Added two dummy transactions to changes list to easily split the date ranges
                if (changes.Where(x => x.Date == startDayOfMonth).Count() == 0) { changes.Add(new ChangeType() { Date = startDayOfMonth, Type = "TR" }); };
                if (changes.Where(x => x.Date == endDayOfMonth).Count() == 0) { changes.Add(new ChangeType() { Date = endDayOfMonth, Type = "TR" }); };

                changes = changes.OrderBy(x => x.Date).ToList();

                for (int i = 1; i < changes.Count(); i++)
                {
                    var interest = SharedData.GetInterestRules().Where(x => x.CreatedOn < changes[i].Date).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    var balance = SharedData.GetTransactins().Where(x => x.CreatedOn < changes[i].Date && x.AccountNumber == account).OrderByDescending(x => x.TransactionId).Select(x => x.Balance).FirstOrDefault();
                    var noOfDates = changes[i].Date.Subtract(changes[i - 1].Date).Days;
                    interestAmount += CalculateInterest(balance, interest.Rate, noOfDates);
                }

                interestAmount /= 365;
            }

            catch (Exception)
            {
                Console.Write("Invalid Input. Starting again.. \n ");
                return -1;
            }

            return Math.Round(interestAmount, 2);
        }



        /*
         * 
         * 
         Calculate interest by balance, decimal and numberOfDates
         * 
         * 
         */

        public decimal CalculateInterest(decimal balance, decimal rate, int numberOfDates)
        {
            return balance * rate * numberOfDates / 100;
        }

    }
}
