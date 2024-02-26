using Bank_Account_Interest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Account_Interest.DataLayer
{
    public static class SharedData
    {
        private static List<Account> _accounts = new List<Account>();
        private static List<AccountTransaction> _transactions = new List<AccountTransaction>();
        private static List<InterestRule> _interestRules = new List<InterestRule>();

        /*
         * 
         Accounts
         *
         */

        // Get Accounts
        public static List<Account> GetAccounts()
        {
            return _accounts;
        }

        // Add Account
        public static void SetAccount(Account account)
        {
            _accounts.Add(account);
        }

        // Add Account
        public static void SetAccountInterest(string acc,decimal interest)
        {
            var account = _accounts.Where(x => x.AccountNumber == acc).FirstOrDefault();
            account.MonthlyInterest = interest;
        }

        /*
         * 
         Transactins
         *
         */

        // Get Transactins
        public static List<AccountTransaction> GetTransactins()
        {
            return _transactions;
        }

        // Add Transactin
        public static void SetTransactin(AccountTransaction transaction)
        {
            _transactions.Add(transaction);
        }


        /*
         * 
         Interest Rules
         *
         */

        // Get Interest Rules
        public static List<InterestRule> GetInterestRules()
        {
            return _interestRules;
        }

        // Add Interest Rules
        public static void SetInterestRule(InterestRule rule)
        {
            _interestRules.Add(rule);
        }

        // Remove Interest Rules
        public static void RemoveInterestRule(DateTime date)
        {
            var existingRule = _interestRules.Where(x => x.CreatedOn == date).FirstOrDefault();
            _interestRules.Remove(existingRule);
        }

        public static void ResetData()
        {
            _interestRules.Clear();
            _accounts.Clear();
            _transactions.Clear();
        }

    }
}
