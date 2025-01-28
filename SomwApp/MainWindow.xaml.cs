using SomwApp.presentation;
using SomwApp.ui.Frames;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SomwApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ScreensEnum> _availableScreens { get; private set; }
        public MainWindow(List<ScreensEnum> availableScreens)
        {
            _availableScreens = availableScreens;
            InitializeComponent();
            this.Pages.Content = new AccountsPage();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ListView listView && listView.SelectedItem is ScreensEnum selectedScreen)
            {
                switch (selectedScreen)
                {
                    case ScreensEnum.Appointments:
                        Pages.Content = new AppointmentsPage();
                        break;
                    case ScreensEnum.Lessons:
                        Pages.Content = new LessonsPage();
                        break;
                    case ScreensEnum.LessonsViewOnly:
                        Pages.Content = new LessonsViewOnlyPage();
                        break;
                    case ScreensEnum.Accounts:
                        Pages.Content = new AccountsPage();
                        break;
                    case ScreensEnum.Customers:
                        Pages.Content = new CustomersPage();
                        break;
                    case ScreensEnum.Subscriptions:
                        
                        break;
                    case ScreensEnum.Payments:
                        Pages.Content = new PaymentsPage();
                        break;
                    case ScreensEnum.Coaches:
                        Pages.Content = new CoachPage();
                        break;
                    case ScreensEnum.Branches:

                        break;
                    case ScreensEnum.ReviewFiles:
                        Pages.Content = new ReviewFilesPage();
                        break;
                    case ScreensEnum.ReviewFilesViewOnly:
                        Pages.Content = new ReviewFilesViewOnlyPage();
                        break;
                }
            }
        }
    }
}