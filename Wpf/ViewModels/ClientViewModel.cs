using BankEF.Commands;
using BankEF.Dialogs;
using Domain.Model;
using Persistence.Context;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace BankEF.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит индекс отдела по умолчанию, в который добавляется клиент.
        /// </summary>
        private const int DepClientAddToDefault = 0;
        private RelayCommand removeClientCommand;
        private RelayCommand oKDepartmentCommand;
        private RelayCommand depSelDefaultCommand;
        private RelayCommand cellEditEndingCommand;
        private RelayCommand selectionChangedCommand;
        private Client selClient;
        private Department dep;
        private bool endEditFlag;
        #endregion
        #region Properties
        public DataContext Context { get; set; }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get; set; }
        public Department Dep { get => dep; set { dep = value; RaisePropertyChanged(nameof(Dep)); } }
        public ObservableCollection<Department> Deps { get => Context.Departments.Local.ToObservableCollection(); }
        public ICommand SelectionChangedCommand => selectionChangedCommand ??= new RelayCommand(SelectionChanged);
        public ICommand RemoveClientCommand => removeClientCommand ??= new RelayCommand(RemoveClient);
        public ICommand CellEditEndingCommand => cellEditEndingCommand ??= new RelayCommand(CellEditEnding);
        public ICommand OKDepartmentCommand => oKDepartmentCommand ??= new RelayCommand((e) =>
        {
            DepsDialog dialog = e as DepsDialog;
            Dep = dialog.depListBox.SelectedItem as Department;
            dialog.DialogResult = true;
        });
        public ICommand DepSelDefaultCommand
            => depSelDefaultCommand ??= new RelayCommand((e) => ((ListBox)e).SelectedItem = ((ListBox)e).Items[DepClientAddToDefault]);
        #endregion
        private void SelectionChanged(object e)
        {
            if (endEditFlag)
            {
                Context.SaveChanges();
                endEditFlag = false;
            }
            // Определяем выделенный элемент списка.
            object selItem = (e as DataGrid).SelectedItem;
            // Фильтруем ссылку.
            if (selItem == null)
                return;
            // Запоминаем выделенного клиента.
            selClient = selItem as Client;
        }
        private void RemoveClient(object e)
        {
            if (selClient == null || MessageBox.Show($"Удалить клиента {selClient}?", $"Удаление клиента {selClient}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            // Удаляем все счета клиента.
            foreach (Deposit deposit in selClient.Deposits)
                Context.Deposits.Remove(deposit);
            foreach (Loan loan in selClient.Loans)
                Context.Loans.Remove(loan);
            // Удаляем самого клиента.
            Context.Clients.Remove(selClient);
            // Удаляем из списка клиентов того отдела, к которому клиент принадлежит.
            Context.Departments.First((g) => g == selClient.Department).Clients.Remove(selClient);
            Context.SaveChanges();
            MainViewModel.Log($"Удален клиент {selClient}.");
        }
        private void CellEditEnding(object e)
        {
            if (selClient == null)
                return;
            endEditFlag = true;
            if (selClient.Department == null)
            {
                // Выбор отдела, к которому относится клиент.
                // Выбираем по умолчанию.
                Dep = Context.Departments.Local.ToBindingList()[DepClientAddToDefault];
                // Показываем список отделов в диалоговом рeжиме.
                // Выбираем из списка.
                _ = new DepsDialog { DataContext = this }.ShowDialog();
                // Добавляем в отдел клиента.
                Dep.Clients.Add(selClient);
                MainViewModel.Log($"В отдел {dep} добавлен клиент {selClient}.");
            }
            else
                MainViewModel.Log($"Отредактирован клиент {selClient}");
        }
    }
}