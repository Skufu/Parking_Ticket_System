using System;
using System.Collections.Generic;
using System.IO;
using ParkingTicketSystem.Utils;

namespace ParkingTicketSystem.Models
{
    public class Account
    {
        public void systemMenu()
        {
            Validation validator = new Validation();
            int choice = validator.IntValidation("\n1 - Login\n" +
                "2 - Signup\n" +
                "3 - Update Account\n" +
                "4 - Deactivate/Activate Account\n" +
                "5 - Exit\n" +
                "\nEnter your choice: ", 1, 5);

            switch (choice)
            {
                case 1:
                    loginAccount();                     // Login user after creating a account
                    break;
                case 2:
                    createAccount();                    // Create an account
                    break;
                case 3:
                    updateAccount();                    // Update user account (password, position[M/S])
                    break;
                case 4:
                    statusAccount();                    // Activate or deactivate the account
                    break;
                case 5:
                    break;                              // Terminate the program
            }
        }

        public void loginAccount()
        {
            Validation validator = new Validation();
            string username = validator.stringValidation("Enter Username: ", 1, 20);
            string password = validator.stringValidation("Enter Password: ", 1, 15);
            bool found = false;

            if (File.Exists("Account.Txt"))
            {
                string[] accounts = File.ReadAllLines("Account.Txt");
                foreach (string account in accounts)
                {
                    string[] accountInfo = account.Split('|');
                    if (accountInfo[1].Trim() == username && accountInfo[2].Trim() == password)
                    {
                        if (accountInfo[5].Trim().ToUpper() == "ACTIVE")
                        {
                            found = true;
                            Console.WriteLine("Login successful!");
                            //post-login logic here
                        }
                        else
                        {
                            Console.WriteLine("Account is not active. Please contact administrator.");
                        }
                        break;
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Invalid username or password.");
                }
            }
            else
            {
                Console.WriteLine("No accounts exist. Please create an account first.");
            }
            systemMenu();
        }

        public void createAccount()
        {
            Validation validator = new Validation();
            string username = validator.stringValidation("Enter Username: ", 1, 20);
            string password = validator.stringValidation("Enter Password: ", 1, 15);
            string empName = validator.stringValidation("Enter Employee Name: ", 1, 30).ToUpper();
            string dCode = validator.deptValidation("Department Code [M-Manager/S-Staff]: ");
            string accNumber = GenerateAccountNumber();
            string statuesAccount = "ACTIVE";

            StreamWriter accountWriter = new StreamWriter("Account.Txt", true);
            accountWriter.WriteLine($"{accNumber} | {username} | {password} | {empName} | {dCode} | {statuesAccount}");
            accountWriter.Close();
            Console.WriteLine("Account successfully created.");
            systemMenu();
        }

        private string GenerateAccountNumber()
        {
            int accountNumber = 1;

            if (File.Exists("Account.Txt"))
            {
                int lineCount = File.ReadAllLines("Account.Txt").Length;
                accountNumber = lineCount + 1;
            }

            return accountNumber.ToString("D5");
        }

        public void updateAccount()
        {
            Validation validator = new Validation();

            int choice = validator.IntValidation("Update Account Menu:\n" +
                "1 - Update Password\n" +
                "2 - Update Department Code\n" +
                "3 - Exit\n" +
                "Enter your choice: ", 1, 3);

            switch (choice)
            {
                case 1:
                    UpdatePassword();
                    break;
                case 2:
                    UpdateDepartmentCode();
                    break;
                case 3:
                    systemMenu();
                    break;
            }
        }

        private void UpdatePassword()
        {
            Validation validator = new Validation();
            string accountNum = validator.stringValidation("Enter Account Number: ", 1, 5);
            bool lValue = false;

            List<string> accounts = new List<string>(File.ReadAllLines("Account.Txt"));

            for (int i = 0; i < accounts.Count; i++)
            {
                string[] accountInfo = accounts[i].Split('|');
                if (accountInfo[0].Trim() == accountNum)
                {
                    lValue = true;
                    string oldPassword = validator.stringValidation("Enter old password: ", 1, 15);
                    if (accountInfo[2].Trim() == oldPassword)
                    {
                        string newPassword;
                        bool mValue;
                        do
                        {
                            newPassword = validator.stringValidation("Enter new password: ", 1, 15);
                            mValue = true;

                            foreach (string account in accounts)
                            {
                                if (account.Split('|')[2].Trim() == newPassword)
                                {
                                    Console.WriteLine("Password is taken, please try again.");
                                    mValue = false;
                                    break;
                                }
                            }
                        } while (!mValue);

                        accountInfo[2] = " " + newPassword + " ";
                        accounts[i] = string.Join("|", accountInfo);
                        Console.WriteLine("Password updated successfully.");
                        updateAccount();
                    }
                    else
                    {
                        Console.WriteLine("Incorrect old password.");
                        updateAccount();
                    }
                    break;
                }
            }
            if (!lValue)
            {
                Console.WriteLine("Account doesn't exist!");
                updateAccount();
            }
            File.WriteAllLines("Account.Txt", accounts);
        }

        private void UpdateDepartmentCode()
        {
            Validation validator = new Validation();
            string accountNum = validator.stringValidation("Enter Account Number: ", 1, 5);
            List<string> accounts = new List<string>(File.ReadAllLines("Account.Txt"));
            bool lValue = false;

            for (int i = 0; i < accounts.Count; i++)
            {
                string[] accountInfo = accounts[i].Split('|');
                if (accountInfo[0].Trim() == accountNum)
                {
                    lValue = true;

                    string currentDept = accountInfo[4].Trim();
                    string newDept = currentDept == "M" ? "S" : "M";
                    accountInfo[4] = " " + newDept + " ";
                    accounts[i] = string.Join("|", accountInfo);
                    Console.WriteLine($"Department code updated successfully to {newDept}.");
                    updateAccount();
                }
            }
            if (!lValue)
            {
                Console.WriteLine("Account doesn't exist!");
                updateAccount();
            }
            File.WriteAllLines("Account.Txt", accounts);
        }

        public void statusAccount()
        {
            Validation validator = new Validation();
            string accountNum = validator.stringValidation("Enter Account Number: ", 1, 5);

            List<string> accounts = new List<string>(File.ReadAllLines("Account.Txt"));
            bool lValue = false;

            for (int i = 0; i < accounts.Count; i++)
            {
                string[] accountInfo = accounts[i].Split('|');

                if (accountInfo[0].Trim() == accountNum)
                {
                    lValue = true;
                    string currentStatus = accountInfo[5].Trim().ToUpper();

                    if (currentStatus == "ACTIVE")
                    {
                        string response = validator.stringValidation("Account is ACTIVE. Do you want to DEACTIVATE it? [YES/NO]: ", 2, 3).ToUpper();
                        if (response == "YES")
                        {
                            accountInfo[5] = " DEACTIVE ";
                            accounts[i] = string.Join("|", accountInfo);
                            Console.WriteLine("Account deactivated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No changes made to account status.");
                        }
                    }
                    else if (currentStatus == "DEACTIVE")
                    {
                        string response = validator.stringValidation("Account is DEACTIVATED. Do you want to ACTIVATE it? [YES/NO]: ", 2, 3).ToUpper();
                        if (response == "YES")
                        {
                            accountInfo[5] = " ACTIVE ";
                            accounts[i] = string.Join("|", accountInfo);
                            Console.WriteLine("Account activated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No changes made to account status.");
                        }
                    }
                    else if (currentStatus == "NOT ACTIVE")
                    {
                        string response = validator.stringValidation("Account is NOT ACTIVE. Do you want to ACTIVATE it? [YES/NO]: ", 2, 3).ToUpper();
                        if (response == "YES")
                        {
                            accountInfo[5] = " ACTIVE ";
                            accounts[i] = string.Join("|", accountInfo);
                            Console.WriteLine("Account activated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No changes made to account status.");
                        }
                    }
                    break;
                }
            }

            if (!lValue)
            {
                Console.WriteLine("Account doesn't exist!");
            }

            File.WriteAllLines("Account.Txt", accounts);
            systemMenu();
        }

        public void listAccount()
        {
            StreamReader accountReader = new StreamReader("Account.Txt");
            string accountRecord = accountReader.ReadLine();

            Console.WriteLine("List of Accounts");
            while (accountRecord != null)
            {
                string[] accountInformation = accountRecord.Split('|');
                Console.WriteLine(accountInformation[0] + " - " + accountInformation[1] + " - " + accountInformation[2]);
                accountRecord = accountReader.ReadLine();
            }
            accountReader.Close();
            Console.ReadKey();
        }
    }
} 