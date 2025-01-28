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
    /// Interaction logic for LessonsPage.xaml
    /// </summary>
    public partial class LessonsPage : Page
    {
        public LessonsPageViewModel viewModel {  get; set; }
        public LessonsPage()
        {
            viewModel = new LessonsPageViewModel();
            InitializeComponent();
        }

        /// <summary>
        /// Обработа события нажатия на кнопку удаления занятия
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
                    if(viewModel.LessonDeleteCommand.CanExecute(viewModel.SelectedLesson))
                        viewModel.LessonDeleteCommand.Execute(viewModel.SelectedLesson);
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
        private void LessonsDGr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dgr && dgr.SelectedItem is LessonsModel model)
            {
                viewModel.SelectedLesson = model.Copy();
            }
        }
    }
}
