
using BankEF.Commands;
using Domain.Model;
using Persistence.Context;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BankEF.ViewModels
{
    public class LoanViewModel : ViewModelBase
    {
        private RelayCommand removeLoanCommand;
        private DataContext dataContext;
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get; set; }
        public DataContext DataContext { get => dataContext; set { dataContext = value; RaisePropertyChanged(nameof(DataContext)); } }
        public ICommand RemoveLoanCommand => removeLoanCommand ??= new RelayCommand(RemoveLoan);
        private void RemoveLoan(object e)
        {
            object item = (e as DataGrid).SelectedItem;
            Loan loan = item == null ? null : (Loan)item;
            if (loan == null || MessageBox.Show($"Удалить кредит {loan}?", $"Удаление кредита {loan}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            dataContext.Loans.Remove(loan);
            dataContext.SaveChanges();
        }
    }
}