using Domain.Model;
using FontAwesome.Sharp;
using Persistence.Context;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using BankEF.Commands;

namespace BankEF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public static void Log(string report)
        {
            using TextWriter tw = File.AppendText("log.txt");
            tw.WriteLine($"{DateTime.Now}:{report}");
        }
        #region Fields
        private readonly DataContext context = new();
        private ViewModelBase viewModel;
        private RelayCommand dragCommand;
        private RelayCommand minimizeCommand;
        private RelayCommand maximizeCommand;
        private RelayCommand closeCommand;
        private RelayCommand listCommand;
        private RelayCommand clientsCommand;
        private RelayCommand depositsCommand;
        private RelayCommand loansCommand;
        private RelayCommand resetBankCommand;
        #endregion
        #region Properties
        public ViewModelBase ViewModel { get => viewModel; set { viewModel = value; RaisePropertyChanged(nameof(ViewModel)); } }
        public ICommand DragCommand => dragCommand ??= new RelayCommand((e) => (e as MainWindow).DragMove());
        public ICommand MinimizeCommand => minimizeCommand ??= new RelayCommand((e) => (e as MainWindow).WindowState = WindowState.Minimized);
        public ICommand MaximizeCommand => maximizeCommand ??= new RelayCommand((e) =>
        {
            MainWindow window = e as MainWindow;
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            window.MaxIconBlock.Icon = window.WindowState == WindowState.Maximized ? IconChar.WindowRestore : IconChar.WindowMaximize;
        });
        public ICommand CloseCommand => closeCommand ??= new RelayCommand((e) => (e as MainWindow).Close());
        public ICommand ListCommand => listCommand ??= new RelayCommand((e) => ViewModel = new ListViewModel());
        public ICommand ClientsCommand => clientsCommand ??= new RelayCommand((e) => ViewModel = new ClientViewModel()
        {
            Context = context,
            DataSource = context.Clients.Local.ToBindingList()
        });
        public ICommand DepositsCommand => depositsCommand ??= new RelayCommand((e) => ViewModel = new DepositViewModel()
        {
            Context = context,
            DataSource = context.Deposits.Local.ToBindingList()
        });
        public ICommand LoansCommand => loansCommand ??= new RelayCommand((e) => ViewModel = new LoanViewModel()
        {
            Context = context,
            DataSource = context.Loans.Local.ToBindingList()
        });
        public ICommand ResetBankCommand => resetBankCommand ??= new RelayCommand((e) => ResetBank());
        #endregion
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
        private void ResetBank()
        {
            ClearTables();
            FillTables();
            string bankName = RandomBank.GetRandomString(4, RandomBank.random);
            ViewModel = new BankNameViewModel() { BankName = bankName };
            Log($"Создан банк {bankName}.");
        }
        public MainViewModel()
        {
            ResetBank();
        }
    }
}