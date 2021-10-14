using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using Domain.Model;
using BankEF.Commands;

namespace BankEF.ViewModels
{
    public class DepositViewModel : ViewModelBase
    {
        /// <summary>
        /// Хранит ссылку на текущий источник данных таблицы депозитов.
        /// </summary>
        private object dataSource;
        private RelayCommand removeDepoCommand;
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get => dataSource; set { dataSource = value; RaisePropertyChanged(nameof(DataSource)); } }
        public ICommand RemoveDepoCommand => removeDepoCommand ??= new RelayCommand(RemoveDepo);
        private void RemoveDepo(object e)
        {
            object item = (e as DataGrid).SelectedItem;
            Deposit depo = item == null ? null : (Deposit)item;
            if (depo == null || MessageBox.Show($"Удалить депозит {depo}?", $"Удаление депозита {depo}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            MainViewModel.context.Deposits.Remove(depo);
            MainViewModel.context.SaveChanges();
        }
    }
}