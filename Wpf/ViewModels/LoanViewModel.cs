
using BankEF.Commands;
using BankEF.Dialogs;
using Domain.Model;
using Persistence.Context;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BankEF.ViewModels
{
    public class LoanViewModel : ViewModelBase
    {
        #region Fields
        #region Поля общего назначения
        /// <summary>
        /// Хранит ссылку на текущий кредит.
        /// </summary>
        private Loan loan;
        /// <summary>
        /// Хранит ссылку на клиента, которому открыт новый кредит.
        /// </summary>
        private Client client;
        /// <summary>
        /// Хранит флаг, определяющий состояние выборки из списка клиента открываемого депозита.
        /// </summary>
        private bool clientDoSelected;
        private bool endEditFlag;
        #endregion
        #region Команды управления событиями
        private RelayCommand selectionChangedCommand;
        private RelayCommand removeLoanCommand;
        private RelayCommand loanEditEndingCommand;
        private RelayCommand clientSelectedCommand;
        private RelayCommand oKClientSelectionCommand;
        #endregion
        #endregion
        #region Properties
        #region Свойства общего назначения
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get; set; }
        public DataContext Context { get; set; }
        /// <summary>
        /// Возвращает список всех кредитов банка.
        /// </summary>
        //public ObservableCollection<Account> Loans
        //{
        //    get
        //    {
        //        ObservableCollection<Account> loans = new ObservableCollection<Account>();
        //        foreach (Department dep in bank.Deps)
        //        {
        //            foreach (Client client in dep.Clients)
        //            {
        //                foreach (Account loan in client.Loans)
        //                {
        //                    loans.Add(loan);
        //                }
        //            }
        //        }
        //        return loans;
        //    }
        //}
        /// <summary>
        /// Возвращает список всех клиентов банка.
        /// </summary>
        public ObservableCollection<Client> Clients => Context.Clients.Local.ToObservableCollection();
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий кредит.
        /// </summary>
        public Loan Loan { get => loan; set { loan = value; RaisePropertyChanged(nameof(Loan)); } }
        /// <summary>
        /// Возвращает и устанавливает клиента, которому приписываетя вновь открываемый кредит.
        /// </summary>
        public Client Client { get => client; set { client = value; RaisePropertyChanged(nameof(Client)); } }
        /// <summary>
        /// Хранит и возвращает флаг выбора клиента, которому приписываетя вновь открываемый депозит.
        /// </summary>
        public bool ClientDoSelected { get => clientDoSelected; set { clientDoSelected = value; RaisePropertyChanged(nameof(ClientDoSelected)); } }
        #endregion
        #region Команды - обработчики событий.
        public ICommand SelectionChangedCommand => selectionChangedCommand ??= new RelayCommand((e) =>
        {
            if (endEditFlag)
            {
                Context.SaveChanges();
                endEditFlag = false;
            }
            Loan = (e as DataGrid).SelectedItem is Loan loan ? loan : null;
        });
        public ICommand RemoveLoanCommand => removeLoanCommand ??= new RelayCommand(RemoveLoan);
        public ICommand LoanEditEndingCommand => loanEditEndingCommand ??= new RelayCommand(EditLoan);
        #region Команды выбора клиента вновь созданного кредита
        public ICommand ClientSelectedCommand => clientSelectedCommand ??= new RelayCommand((e) => ClientDoSelected = true);
        public ICommand OKClientSelectionCommand => oKClientSelectionCommand ??= new RelayCommand((e) =>
        {
            ClientsDialog dialog = e as ClientsDialog;
            Client = dialog.clientListBox.SelectedItem is Client client ? client : null;
            dialog.DialogResult = true;
        });
        #endregion
        #endregion
        #endregion
        private void EditLoan(object e)
        {
            endEditFlag = true;
            void InsertLoanIntoClient()
            {
                client.Loans.Add(loan);
                MainViewModel.Log($"Клиенту {client} открыт кредит {loan}.");
            }
            if (loan.Client == default)
            {
                bool flag;
                do
                {
                    if (flag = (bool)new ClientsDialog { DataContext = this }.ShowDialog() && client != null)
                        InsertLoanIntoClient();
                } while (!flag);
            }
            else
                MainViewModel.Log($"Поля кредита {loan} отредактированы.");

        }
        private void RemoveLoan(object e)
        {
            if (loan == null || MessageBox.Show($"Удалить кредит {loan}?", $"Удаление кредита {loan}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            Context.Loans.Remove(loan);
            Context.SaveChanges();
        }
    }
}