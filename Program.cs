using Bank_Account_Interest.DataLayer;
using Bank_Account_Interest.Entities;
using Bank_Account_Interest.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Bank_Account_Interest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Registering initial interest rates

            var rule1 = new InterestRule() { CreatedOn = DateTime.Parse("2023-01-01"), Rate = 1.95m, RuleNumber = "RULE01" };
            var rule2 = new InterestRule() { CreatedOn = DateTime.Parse("2023-05-20"), Rate = 1.90m, RuleNumber = "RULE02" };
            SharedData.SetInterestRule(rule1);
            SharedData.SetInterestRule(rule2);

            Console.Write("Welcome to AwesomeGIC Bank!\n\n What would you like to do? \n\n ");
            MainMenuAndOperations.MainMenu();
        }


    }
}
