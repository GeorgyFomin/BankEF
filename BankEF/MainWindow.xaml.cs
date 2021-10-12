using Domain.Model;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BankEF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataContext context = new DataContext();
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
        public MainWindow()
        {
            InitializeComponent();
            ClearTables();
        }
    }
}
