using SomwApp.domain.models;
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
    /// Interaction logic for AccountsPage.xaml
    /// </summary>
    public partial class AccountsPage : Page
    {
        public AccountsPageViewModel viewModel { get; set; }
        public AccountsPage()
        {
            viewModel = new AccountsPageViewModel();
            InitializeComponent();
        }

        /// <summary>
        /// Обработка события изменения выбранного элемента
        /// </summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="e">Праметры события</param>
        private void OnUserAccountsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is DataGrid DGr && DGr.SelectedItem is UserAccountModel model)
            {
                viewModel.SelectedAccount = model.Copy();
            }
        }

        /// <summary>
        /// Обработка события надатия на кнопку удаления записи
        /// </summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="e">Параметры события</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (viewModel.AccountDeleteCommand.CanExecute(viewModel.SelectedAccount))
                        viewModel.AccountDeleteCommand.Execute(viewModel.SelectedAccount);
                    else MessageBox.Show("Ошибка удаления записи: такой записи нет", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления записи", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
