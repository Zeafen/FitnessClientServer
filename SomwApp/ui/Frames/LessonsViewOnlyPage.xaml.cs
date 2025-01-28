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
    /// Interaction logic for LessonsViewOnlyPage.xaml
    /// </summary>
    public partial class LessonsViewOnlyPage : Page
    {
        public LessonsPageViewModel viewModel { get; set; }
        public LessonsViewOnlyPage()
        {
            viewModel = new LessonsPageViewModel();
            InitializeComponent();
        }
    }
}
