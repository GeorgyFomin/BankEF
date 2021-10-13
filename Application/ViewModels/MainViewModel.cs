using BankEF;
using Domain.Model;
using Persistence;
using System;
using System.IO;

namespace Applic.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public static void Log(string report)
        {
            using (TextWriter tw = File.AppendText("log.txt"))
                tw.WriteLine(DateTime.Now.ToString() + ":" + report);
        }

        private readonly DataContext context = new();

        private void ClearTables()
        {
            foreach (Deposit deposit in context.Deposits)
            {
                context.Deposits.Remove(deposit);
            }
            foreach (Loan loan in context.Loans)
            {
                context.Loans.Remove(loan);
            }
            foreach (Client client in context.Clients)
            {
                context.Clients.Remove(client);
            }
            foreach (Department department in context.Departments)
            {
                context.Departments.Remove(department);
            }
            context.SaveChanges();
        }

        private void FillTables()
        {
            context.Departments.AddRange(RandomBank.Deps);
            foreach (Department dep in context.Departments)
            {
                context.Clients.AddRange(dep.Clients);
                foreach (Client client in context.Clients)
                {
                    context.Deposits.AddRange(client.Deposits);
                    context.Loans.AddRange(client.Loans);
                }
            }
            context.SaveChanges();

        }
        public MainViewModel()
        {
            ClearTables();
            FillTables();
        }
    }
}