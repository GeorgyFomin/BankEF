
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
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get; set; }
        public DataContext Context { get; set; }
        public ICommand RemoveLoanCommand => removeLoanCommand ??= new RelayCommand(RemoveLoan);
        private void RemoveLoan(object e)
        {
            object item = (e as DataGrid).SelectedItem;
            Loan loan = item == null ? null : (Loan)item;
            if (loan == null || MessageBox.Show($"Удалить кредит {loan}?", $"Удаление кредита {loan}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            Context.Loans.Remove(loan);
            Context.SaveChanges();
        }
    }
}