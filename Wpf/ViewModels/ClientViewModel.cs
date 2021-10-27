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
        private Client client;
        private Department dep;
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
            client = selItem as Client;
        }
        private void RemoveClient(object e)
        {
            if (client == null || MessageBox.Show($"Удалить клиента {client}?", $"Удаление клиента {client}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            // Удаляем все счета клиента.
            foreach (Deposit deposit in client.Deposits)
                Context.Deposits.Remove(deposit);
            foreach (Loan loan in client.Loans)
                Context.Loans.Remove(loan);
            // Удаляем самого клиента.
            Context.Clients.Remove(client);
            // Удаляем из списка клиентов того отдела, к которому клиент принадлежит.
            Context.Departments.First((g) => g == client.Department).Clients.Remove(client);
            Context.SaveChanges();
            MainViewModel.Log($"Удален клиент {client}.");
        }
        private bool endEditFlag;
        private void CellEditEnding(object e)
        {
            if (client == null)
                return;
            endEditFlag = true;
            if (client.Department == null)
            {
                // Выбор отдела, к которому относится клиент.
                // Выбираем по умолчанию.
                Dep = Context.Departments.Local.ToBindingList()[DepClientAddToDefault];
                // Показываем список отделов в диалоговом рeжиме.
                // Выбираем из списка.
                _ = new DepsDialog { DataContext = this }.ShowDialog();
                // Добавляем в отдел клиента.
                Dep.Clients.Add(client);
                MainViewModel.Log($"В отдел {dep} добавлен клиент {client}.");
            }
            else
                MainViewModel.Log($"Отредактирован клиент {client}");
        }
    }
}