using BankEF.Commands;
using Domain.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace BankEF.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        /// <summary>
        /// Хранит ссылку на текущий источник данных таблицы клиентов.
        /// </summary>
        private object dataSource;
        private RelayCommand removeClientCommand;
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get => dataSource; set { dataSource = value; RaisePropertyChanged(nameof(DataSource)); } }
        public ICommand RemoveClientCommand => removeClientCommand ??= new RelayCommand(RemoveClient);
        private void RemoveClient(object e)
        {
            object item = (e as DataGrid).SelectedItem;
            Client client = item == null ? null : (Client)item;
            if (client == null || MessageBox.Show($"Удалить клиента {client}?", $"Удаление клиента {client}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            MainViewModel.context.Clients.Remove(client);
            MainViewModel.context.SaveChanges();
        }
    }
}