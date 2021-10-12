using Domain.Model;
using Persistence;
using System.Windows;

namespace BankEF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DataContext context = new();
        void ClearTables()
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
        }
        void FillTables()
        {
            context.Departments.AddRange(ClassLibrary.RandomBank.Deps);
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
        public MainWindow()
        {
            InitializeComponent();
            ClearTables();
            FillTables();
        }
    }
}
