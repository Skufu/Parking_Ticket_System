using System;
using System.Collections.Generic;
using System.IO;
using ParkingTicketSystem.Utils;
using sampleLang;
using ParkingTicketSystem;

namespace ParkingTicketSystem.Models
{
    public class Account
    {
        private readonly ParkingManager _parkingManager;

        public Account()
        {
            _parkingManager = new ParkingManager();
        }

        public void systemMenu()
        {
            Validation validator = new Validation();
            int choice = validator.IntValidation("\n1 - Login\n" +
                "2 - Signup\n" +
                "3 - Exit\n" +
                "\nEnter your choice: ", 1, 3);

            switch (choice)
            {
                case 1:
                    loginAccount();                     // Login user after creating a account
                    break;
                case 2:
                    createAccount();                    // Create an account
                    break;
                case 3:
                    ExitProgram();
                    break;                   // Terminate the program
            }
        }

        static void ExitProgram()
        {
            Console.WriteLine("\nThank you for using Parking Management System!");
            Environment.Exit(0);
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
                        if (accountInfo[5].Trim().ToUpper() == "ACTIVE" && accountInfo[4].Trim().ToUpper() == "M")
                        {
                            found = true;
                            Console.WriteLine("Login successful!");
                            ManagerMenu();
                        }
                        else if (accountInfo[5].Trim().ToUpper() == "ACTIVE" && accountInfo[4].Trim().ToUpper() == "S")
                        {
                            found = true;
                            Console.WriteLine("Login successful!");
                            staffMenu();
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

        public void staffMenu()
        {
            _parkingManager.ShowParkingMenu();
        }

        public void ManagerMenu()
        {
            Validation validator = new Validation();
            int choice = validator.IntValidation("\n1 - Account Management\n" +
                "2 - Parking Management\n" +
                "3 - Exit to main menu\n" +
                "\nEnter your choice: ", 1, 3);

            switch (choice)
            {
                case 1:
                    accountManagementMenu();
                    break;
                case 2:
                    _parkingManager.ShowParkingMenu();
                    break;
                case 3:
                    systemMenu();
                    break;
            }
        }

        private void accountManagementMenu()
        {
            Validation validator = new Validation();
            while (true)
            {
                int choice = validator.IntValidation("\nAccount Management Menu\n" +
                    "1 - List All Accounts\n" +
                    "2 - Change Account password\n" +
                    "3 - Update Account Status\n" +
                    "4 - Back to Manager Menu\n" +
                    "\nEnter your choice: ", 1, 4);

                switch (choice)
                {
                    case 1:
                        listAccount();
                        break;
                    case 2:
                        updateAccount();
                        break;
                    case 3:
                        statusAccount();
                        break;
                    case 4:
                        ManagerMenu();
                        break;
                }
            }
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