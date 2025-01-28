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
    /// Interaction logic for AppointmentsPage.xaml
    /// </summary>
    public partial class AppointmentsPage : Page
    {
        public AppointmentsPageViewModel viewModel { get; set; }
        public AppointmentsPage()
        {
            viewModel = new AppointmentsPageViewModel();
            InitializeComponent();
        }

        /// <summary>
        /// Обработа события нажатия на кнопку удаления отметки о посещении
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
                    if(viewModel.AppointmentDeleteCommand.CanExecute(viewModel.SelectedAppointment))
                        viewModel.AppointmentDeleteCommand.Execute(viewModel.SelectedAppointment);
                    else MessageBox.Show("Ошибка удаления записи: такой записи нет", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления записи", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработка события изменения выбранного элемента в DataGrid
        /// </summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="e">Параметры события</param>
        private void AppointmentsDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is DataGrid dgr && dgr.SelectedItem is AppointmentsForClassesModel model)
            {
                viewModel.SelectedAppointment = model.Copy();
            }
        }
    }
}
