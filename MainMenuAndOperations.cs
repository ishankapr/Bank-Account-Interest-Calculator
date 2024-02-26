using Bank_Account_Interest.Services;
using System;

namespace Bank_Account_Interest
{
    public static class MainMenuAndOperations
    {
        /*
         * 
         * 
         Any action invalid in main functions will be redirect here
         * 
         *
         */

        public static void MainMenu()
        {
            Console.Write("[I]nput transactions \n [D]efine interest rules \n [P]rint statement \n [Q]uit \n\n [ > ] ");
            var readLine = Console.ReadLine();

            if(readLine == null)
            {
                return;
            }

            try
            {
                var charValue = char.Parse(readLine.ToUpper());
                Operations(charValue);
            }
            catch (Exception)
            {
                Console.Write("Please enter one character.. \n\n ");
                MainMenu();
            }

        }


        /*
         * 
         * 
         All type of operations defined here and relavent method will be execute accordingly
         * 
         *
         */


        public static void Operations(char opt)
        {
            var transactionService = new TransactionService();

            Console.Write($"\n Your input: \n\n [ > ] {opt} \n");
            switch (opt)
            {
                case 'I':
                    Console.Write("Please enter transaction details in <Date>|<Account>|<Type>|<Amount> format \r\n(or enter blank to go back to main menu):");
                    var transaction = Console.ReadLine();
                    if (transaction == " ") { MainMenu(); } 
                    else {
                        var result = transactionService.InputTransaction(transaction);
                        if (!result){ MainMenuAndOperations.Operations('I'); } else { MainMenu(); }
                    };
                    break;
                case 'D':
                    Console.Write("Please enter interest rules details in <Date>|<RuleId>|<Rate in %> format \r\n(or enter blank to go back to main menu):");
                    var rule = Console.ReadLine();
                    if (rule == " ") { MainMenu(); } 
                    else { 
                        var result = transactionService.DefineInterestRules(rule);
                        if (!result) { MainMenuAndOperations.Operations('D'); } else { MainMenu(); }
                    };
                    break;
                case 'P':
                    Console.Write("Please enter account and month to generate the statement <Account>|<Month>\r\n(or enter blank to go back to main menu):");
                    var statement = Console.ReadLine();
                    if (statement == " ") { MainMenu(); } 
                    else {
                        var result = transactionService.PrintStatement(statement);
                        if (!result) { MainMenuAndOperations.Operations('P'); } else { MainMenu(); }
                    };
                    break;
                case 'Q':
                    Console.Write("Thank you for banking with AwesomeGIC Bank.\nHave a nice day!");
                    Console.ReadLine();
                    break;
                default:
                    {
                        Console.Write("Invalid character. Moving to Main Menu.. \n ");
                        MainMenu();
                        break;
                    }
            }
        }


    }
}
