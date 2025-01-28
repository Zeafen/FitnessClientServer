using SomwApp.domain.models;
using SomwApp.presentation;
using SomwApp.presentation.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SomwApp.ui.Frames
{
    /// <summary>
    /// Interaction logic for CustomersPage.xaml
    /// </summary>
    public partial class CustomersPage : Page
    {
        public CustomersPageViewModel viewModel { get; set; }
        public CustomersPage()
        {
            viewModel = new CustomersPageViewModel();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (viewModel.CustomerDeleteCommand.CanExecute(viewModel.SelectedCustomer))
                        viewModel.CustomerDeleteCommand.Execute(viewModel.SelectedCustomer);
                    else MessageBox.Show("Ошибка удаления записи: такой записи нет", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления записи", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnClearFilterClick(object sender, RoutedEventArgs e)
        {
            viewModel.StatusFilter = null;
        }

        private void CustomersDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as DataGrid)?.SelectedItem as CustomerModel;
            if (item != null)
            {
                viewModel.SelectedCustomer = item.Copy();
            }
        }
    }
}
