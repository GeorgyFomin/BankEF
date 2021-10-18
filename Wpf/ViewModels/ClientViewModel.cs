using BankEF.Commands;
using Domain.Model;
using Persistence.Context;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace BankEF.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        private RelayCommand removeClientCommand;
        public DataContext Context { get; set; }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get; set; }
        public ICommand RemoveClientCommand => removeClientCommand ??= new RelayCommand(RemoveClient);
        private void RemoveClient(object e)
        {
            object item = (e as DataGrid).SelectedItem;
            Client client= item == null ? null : (Client)item;
            if (client == null || MessageBox.Show($"Удалить клиента {client}?", $"Удаление клиента {client}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            Context.Clients.Remove(client);
            Context.SaveChanges();
        }
    }
}