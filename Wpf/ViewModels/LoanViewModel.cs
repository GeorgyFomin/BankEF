
using BankEF.Commands;
using BankEF.Dialogs;
using Domain.Model;
using Persistence.Context;
using System;
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
        #endregion
        #region Команды управления событиями
        private RelayCommand loanSelectedCommand;
        private RelayCommand loanRemoveCommand;
        private RelayCommand loanCellEditEndCommand;
        private RelayCommand clientSelectedCommand;
        private RelayCommand oKClientSelectionCommand;
        private RelayCommand loanCellChangedCommand;
        private RelayCommand loanRowEditEndCommand;
        private bool blockAccountEditEndingHandler;
        private bool cellEdited;
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
        public ICommand LoanSelectedCommand => loanSelectedCommand ??= new RelayCommand((e) => Loan = (e as DataGrid).SelectedItem is Loan loan ? loan : null);
        public ICommand LoanRemoveCommand => loanRemoveCommand ??= new RelayCommand(RemoveLoan);
        public ICommand LoanCellEditEndCommand => loanCellEditEndCommand ??= new RelayCommand((e) => cellEdited = true);
        public ICommand LoanCellChangedCommand => loanCellChangedCommand ??= new RelayCommand(LoanCellChanged);
        public ICommand LoanRowEditEndCommand => loanRowEditEndCommand ??= new RelayCommand(LoanRowEditEnd);
        #region Команды выбора клиента вновь созданного кредита
        public ICommand ClientSelectedCommand => clientSelectedCommand ??= new RelayCommand((e) => ClientDoSelected = true);
        public ICommand OKClientSelectionCommand => oKClientSelectionCommand ??= new RelayCommand(SelectNewLoanClient);
        #endregion
        #endregion
        #endregion
        private void RemoveLoan(object e)
        {
            if (loan == null || MessageBox.Show($"Удалить кредит {loan}?", $"Удаление кредита {loan}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            Context.Loans.Remove(loan);
            Context.SaveChanges();
        }
        private void LoanCellChanged(object e)
        {
            if (!cellEdited)
                return;
            cellEdited = false;
            (e as DataGrid).CommitEdit();
            Context.SaveChanges();
            MainViewModel.Log($"Кредит {loan} отредактирован.");
            //MessageBox.Show("Cell Changed");
        }
        private void LoanRowEditEnd(object e)
        {
            if (blockAccountEditEndingHandler) return;
            DataGrid grid = e as DataGrid;
            //MessageBox.Show("Row Edited");
            cellEdited = false;
            if (loan.Client == default)
            {
                bool flag;
                do
                {
                    if (flag = (bool)new ClientsDialog { DataContext = this }.ShowDialog() && client != null)
                        client.Loans.Add(loan);
                } while (!flag);
                MainViewModel.Log($"Клиенту {client} открыт кредит {loan.Number}.");
            }
            else
                MainViewModel.Log($"Поля кредита №{loan.Number} отредактированы.");
            blockAccountEditEndingHandler = true;
            grid.CommitEdit();
            Context.SaveChanges();
            blockAccountEditEndingHandler = false;
        }
        private void SelectNewLoanClient(object e)
        {
            ClientsDialog dialog = e as ClientsDialog;
            Client = dialog.clientListBox.SelectedItem is Client client ? client : null;
            dialog.DialogResult = true;
        }
    }
}