using BankEF.Commands;
using Domain.Model;
using Persistence.Context;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace BankEF.ViewModels
{
    //public class MyBehavior
    //{
    //    public static ICommand GetCommand(DataGridRow row)
    //    {
    //        return (ICommand)row.GetValue(CommandProperty);
    //    }

    //    public static void SetCommand(DataGridRow row, ICommand value)
    //    {
    //        row.SetValue(CommandProperty, value);
    //    }

    //    public static readonly DependencyProperty CommandProperty =
    //        DependencyProperty.RegisterAttached(
    //        "Command",
    //        typeof(ICommand),
    //        typeof(MyBehavior),
    //        new UIPropertyMetadata(null, OnCommandChanged));

    //    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        DataGridRow row = d as DataGridRow;
    //        row.MouseRightButtonUp += Row_MouseRightButtonUp;
    //    }

    //    private static void Row_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    //    {
    //        DataGridRow row = sender as DataGridRow;
    //        ICommand command = GetCommand(row);
    //        if (command != null)
    //            command.Execute(row.DataContext);
    //    }
    //}
    public class ClientViewModel : ViewModelBase
    {
        /// <summary>
        /// Хранит ссылку на текущий источник данных таблицы клиентов.
        /// </summary>
        private object dataSource;
        /// <summary>
        /// Хранит ссылку на текущего клиента.
        /// </summary>
        private Client client;
        private DataContext dataContext;
        private RelayCommand selectionChangedCommand;
        private RelayCommand removeClientCommand;
        public DataContext DataContext { get => dataContext; set { dataContext = value; RaisePropertyChanged(nameof(DataContext)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object DataSource { get => dataSource; set { dataSource = value; RaisePropertyChanged(nameof(DataSource)); } }
        /// <summary>
        /// Устанавливает и возвращает текущего клиента.
        /// </summary>
        public Client Client { get => client; set { client = value; RaisePropertyChanged(nameof(Client)); } }
        public ICommand SelectionChangedCommand => selectionChangedCommand ??= new RelayCommand((e) => Client = (e as DataGrid).SelectedItem is Client client ? client : null);
        public ICommand RemoveClientCommand => removeClientCommand ??= new RelayCommand(RemoveClient);
        private void RemoveClient(object e)
        {
            if (client == null || MessageBox.Show($"Удалить клиента {client}?", $"Удаление клиента {client}", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            dataContext.Clients.Remove(client);
            dataContext.SaveChanges();
        }
    }
}