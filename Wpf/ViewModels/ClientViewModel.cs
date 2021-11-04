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
        private const int DefaultDepartmentNumber = 0;
        private RelayCommand clientRemoveCommand;
        private RelayCommand departmentSelectedCommand;
        private RelayCommand defaultDepartmentCommand;
        private RelayCommand clientCellEditEndCommand;
        private RelayCommand clientSelectionCommand;
        private RelayCommand clientCellChangedCommand;
        private RelayCommand clientRowEditEndCommand;
        private bool blockAccountEditEndingHandler;
        private bool cellEdited;
        private Client client;
        private Department department;
        #endregion
        #region Properties
        public DataContext Context { get; set; }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get; set; }
        public Client Client { get => client; set { client = value; RaisePropertyChanged(nameof(Client)); } }
        public Department Department { get => department; set { department = value; RaisePropertyChanged(nameof(Department)); } }
        public ObservableCollection<Department> Departments { get => Context.Departments.Local.ToObservableCollection(); }
        public ICommand ClientSelectionCommand => clientSelectionCommand ??= new RelayCommand((e) => Client = (e as DataGrid).SelectedItem is Client client ? client : null);
        public ICommand ClientRemoveCommand => clientRemoveCommand ??= new RelayCommand(RemoveClient);
        public ICommand ClientCellEditEndCommand => clientCellEditEndCommand ??= new RelayCommand((e) => cellEdited = true);
        public ICommand DepartmentSelectedCommand => departmentSelectedCommand ??= new RelayCommand(DepartmentSelection);
        public ICommand DefaultDepartmentCommand
            => defaultDepartmentCommand ??= new RelayCommand((e) => ((ListBox)e).SelectedItem = ((ListBox)e).Items[DefaultDepartmentNumber]);
        public ICommand ClientCellChangedCommand => clientCellChangedCommand ??= new RelayCommand(ClientCellChanged);
        public ICommand ClientRowEditEndCommand => clientRowEditEndCommand ??= new RelayCommand(ClientRowEditEnd);
        #endregion
        private void DepartmentSelection(object e)
        {
            DepsDialog dialog = e as DepsDialog;
            Department = dialog.depListBox.SelectedItem as Department;
            dialog.DialogResult = true;
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
        private void ClientCellChanged(object e)
        {
            if (!cellEdited)
                return;
            cellEdited = false;
            (e as DataGrid).CommitEdit();
            Context.SaveChanges();
            MainViewModel.Log($"Клиент {client} отредактирован.");
            //MessageBox.Show("Cell Changed");
        }
        private void ClientRowEditEnd(object e)
        {
            if (blockAccountEditEndingHandler) return;
            cellEdited = false;
            if (client.Department == null)
            {
                // Выбор отдела, к которому относится клиент.
                // Выбираем по умолчанию.
                Department = Context.Departments.Local.ToBindingList()[DefaultDepartmentNumber];
                // Показываем список отделов в диалоговом рeжиме.
                // Выбираем из списка.
                _ = new DepsDialog { DataContext = this }.ShowDialog();
                // Добавляем в отдел клиента.
                Department.Clients.Add(client);
                MainViewModel.Log($"В отдел {department} добавлен клиент {client}.");
            }
            else
                MainViewModel.Log($"Отредактирован клиент {client}");
            //MessageBox.Show("Row Edited");
            blockAccountEditEndingHandler = true;
            (e as DataGrid).CommitEdit();
            Context.SaveChanges();
            blockAccountEditEndingHandler = false;
        }
    }
}