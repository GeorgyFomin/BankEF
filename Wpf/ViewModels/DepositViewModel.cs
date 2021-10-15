using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using Domain.Model;
using BankEF.Commands;
using BankEF.Dialogs;
using System.Collections.ObjectModel;
using Persistence.Context;

namespace BankEF.ViewModels
{
    public class DepositViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит ссылку на текущий депозит.
        /// </summary>
        private Deposit depo;
        private DataContext dataContext;
        /// <summary>
        /// Хранит ссылку на текущий источник данных таблицы депозитов.
        /// </summary>
        private object dataSource;
        /// <summary>
        /// Хранит флаг, определяющий состояние выборки депозита, из которого будут переведены средства.
        /// </summary>
        private bool sourceTransferDepoSelected;
        /// <summary>
        /// Хранит флаг, определяющий доступность совершения транзакции.
        /// </summary>
        private bool transferEnabled;
        /// <summary>
        /// Хранит флаг, подтверждающий транзакцию указанной суммы.
        /// </summary>
        private bool transferSumOKEnabled;
        /// <summary>
        /// Хранит ссылку на депозит, на который переводятся средства.
        /// </summary>
        private Account targetTransferDepo;
        /// <summary>
        /// Хранит величину перевода.
        /// </summary>
        private decimal transferAmount;
        /// <summary>
        /// Хранит флаг активации списка депозитов, на который могут быть переведены средства.
        /// </summary>
        private bool targetTransferListEnabled;
        private RelayCommand selectionChangedCommand;
        private RelayCommand removeDepoCommand;
        private RelayCommand transferCommand;
        private RelayCommand targetTransferDepoSelectionChangedCommand;
        private RelayCommand oKTransferCommand;
        private RelayCommand transfAmountChangedCommand;
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий депозит.
        /// </summary>
        public Deposit Depo { get => depo; set { depo = value; RaisePropertyChanged(nameof(Depo)); } }
        /// <summary>
        /// Возвращает список всех депозитов банка.
        /// </summary>
        public ObservableCollection<Deposit> Deposits
        {
            get
            {
                ObservableCollection<Deposit> deposits = new();
                foreach (Department dep in dataContext.Departments)
                    foreach (Client client in dep.Clients)
                        foreach (Deposit deposit in client.Deposits)
                        {
                            // Блокируем появление в списке депозитов, на которые могут быть переведены средства, депозит Depo, с которого средства списываются.
                            if (!targetTransferListEnabled || deposit.Client != Depo?.Client)
                                deposits.Add(deposit);
                        }

                return deposits;
            }
        }
        public DataContext DataContext { get => dataContext; set { dataContext = value; RaisePropertyChanged(nameof(DataContext)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get => dataSource; set { dataSource = value; RaisePropertyChanged(nameof(DataSource)); } }
        #region Transfer properties
        public bool SourceTransferDepoSelected { get => sourceTransferDepoSelected; set { sourceTransferDepoSelected = value; RaisePropertyChanged(nameof(SourceTransferDepoSelected)); } }
        public Account TargetTransferDepo { get => targetTransferDepo; set { targetTransferDepo = value; RaisePropertyChanged(nameof(TargetTransferDepo)); } }
        public bool TransferEnabled { get => transferEnabled; set { transferEnabled = value; RaisePropertyChanged(nameof(TransferEnabled)); } }
        public bool TransferSumOKEnabled { get => transferSumOKEnabled; set { transferSumOKEnabled = value; RaisePropertyChanged(nameof(TransferSumOKEnabled)); } }
        public decimal TransferAmount { get => transferAmount; set { transferAmount = value; RaisePropertyChanged(nameof(TransferAmount)); } }
        #endregion
        public ICommand SelectionChangedCommand => selectionChangedCommand ??=
            new RelayCommand((e) => SourceTransferDepoSelected = (Depo = (e as DataGrid).SelectedItem is Deposit depo ? depo : null) != null);
        public ICommand RemoveDepoCommand => removeDepoCommand ??= new RelayCommand(RemoveDepo);
        #region Команды перечисления средств с одного депозита на другой.
        public ICommand TransferCommand => transferCommand ??= new RelayCommand(Transfer);
        public ICommand TransfAmountChangedCommand => transfAmountChangedCommand ??= new RelayCommand(TransfAmountChanged);
        public ICommand OKTransferCommand => oKTransferCommand ??= new RelayCommand((e) => (e as TransferDialog).DialogResult = true);
        public ICommand TargetTransferDepoSelectionChangedCommand => targetTransferDepoSelectionChangedCommand ??= new RelayCommand(SelectTargetTransferDepo);
        #endregion
        #endregion
        public DepositViewModel() { sourceTransferDepoSelected = transferEnabled = false; }
        private void RemoveDepo(object e)
        {
            if (depo == null || MessageBox.Show($"Удалить депозит {depo}?", $"Удаление депозита {depo}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            dataContext.Deposits.Remove(depo);
            dataContext.SaveChanges();
        }
        private void Transfer(object e)
        {
            void TryToTransfer()
            {
                void DoTransfer()
                {
                    if (MessageBox.Show($"Вы действительно хотите перевести со счета №{depo.Number} на счет №{targetTransferDepo.Number} сумму {TransferAmount}?",
                        "Перевод между счетами", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        return;
                    Depo.Size -= TransferAmount;
                    TargetTransferDepo.Size += TransferAmount;
                    MainViewModel.Log($"Со счета {depo} будет переведено {TransferAmount} на счет {targetTransferDepo}");
                    string comment = $"Со счета {depo} переведено {TransferAmount} на счет {targetTransferDepo}";
                    MainViewModel.Log(comment);
                    MessageBox.Show(comment);
                }
                TransferAmount = 0;
                TransferDialog dialog = new() { DataContext = this };
                RaisePropertyChanged(nameof(Deposits));
                if ((bool)dialog.ShowDialog() && targetTransferDepo != null)
                    DoTransfer();
            }
            if (depo == null)
                return;
            // Блокируем появление в списке депозитов, на которые могут быть переведены средства, депозит, с которого средства списываются.
            targetTransferListEnabled = true;
            TryToTransfer();
            // Снимаем блокировку со списка доступных депозитов.
            targetTransferListEnabled = false;
            RaisePropertyChanged(nameof(Deposits));
            dataContext.SaveChanges();
            (e as DataGrid).Items.Refresh();
        }
        private void SelectTargetTransferDepo(object e)
        {
            if ((TargetTransferDepo = (e as ListBox).SelectedItem is Account account ? account : null) == null)
                return;
            // Активируем процесс перевода.
            TransferEnabled = true;
            // Инициализируем флаг подтверждения перевода запрошенной суммы.
            TransferSumOKEnabled = false;
        }
        private void TransfAmountChanged(object e)
        {
            if (!decimal.TryParse((e as TextBox).Text, out decimal amount) || amount <= 0)
                return;
            TransferAmount = amount;
            TransferSumOKEnabled = depo.Size - amount >= Account.MinSize;
            if (!TransferSumOKEnabled)
            {
                MessageBox.Show(
                    $"Количество средств {depo.Size} на счету {depo.Number} не допускает сумму {transferAmount} списания.\n" +
                    $"Максимальная сумма списания {depo.Size - Account.MinSize}");
                return;
            }
        }
    }
}