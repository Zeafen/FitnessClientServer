using SomwApp.domain.commands;
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
    /// Interaction logic for ReviewFilesPage.xaml
    /// </summary>
    public partial class ReviewFilesPage : Page
    {
        public ReviewFilesPageViewModel viewModel { get; set; }
        public ReviewFilesPage()
        {
            viewModel = new ReviewFilesPageViewModel();
            InitializeComponent();
            this.InputBindings.Add(new InputBinding(
                new RelayCommand(
                    obj => { this.ReviewsDgr.UnselectAll(); },
                    obj => { return this.ReviewsDgr.SelectedItems.Count > 0; }),
                new KeyGesture(Key.Escape, ModifierKeys.Shift)
                ));

        }
        /// <summary>
        /// Копирование выбранного файла модель представления присмене выбранного элемента DataGrid
        /// </summary>
        /// <param name="sender">отправитель</param>
        /// <param name="e">параметры события смены выбранного значения</param>
        private void OnReviewsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
                foreach (var itemToRemove in e.RemovedItems)
                    if (itemToRemove is ReviewFile fileToRemove && viewModel.ReviewsToOperate.Contains(fileToRemove))
                        viewModel.ReviewsToOperate.Remove(fileToRemove);
            if (e.AddedItems.Count > 0)
                foreach (var itemToAdd in e.AddedItems)
                    if (itemToAdd is ReviewFile fileToAdd && !viewModel.ReviewsToOperate.Contains(fileToAdd))
                        viewModel.ReviewsToOperate.Add(fileToAdd);
        }
    }
}
