using SomwApp.domain;
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
using System.Windows.Shapes;

namespace SomwApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public AuthPageViewModel viewModel { get; set; }
        public LoginWindow()
        {
            viewModel = new AuthPageViewModel();
            InitializeComponent();
        }


        /// <summary>
        /// Обрабатывает событие нажатия на кнопку подтверждения авторизации. В случае успешной авторизации открывает главное окно, передавая в зависимости от ролиперечень доступных экранов.
        /// </summary>
        /// <param name="sender">Оптарвитель события</param>
        /// <param name="e">Параметры события надатия на кнопку</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = viewModel.ConfirmAuth(viewModel.AuthRequest);
                if (result == null || string.IsNullOrEmpty(result.Token) || string.IsNullOrEmpty(result.Role))
                    MessageBox.Show("Ошибка авторизации. Проверьте правильность введенных данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    Application.Current.Resources.Add(ResourceKeys.TOKEN_RESOURCE_KEY, result.Token);
                    List<ScreensEnum>? screens = null;
                    switch (result.Role)
                    {
                        case "Trainer":
                            screens = new List<ScreensEnum>()
                            {
                                ScreensEnum.Lessons,
                                ScreensEnum.Appointments,
                                ScreensEnum.ReviewFiles
                            };
                            break;
                        case "Admin":
                            screens = new List<ScreensEnum>()
                            {
                                ScreensEnum.Lessons,
                                ScreensEnum.Appointments,
                                ScreensEnum.Coaches,
                                ScreensEnum.Accounts,
                                ScreensEnum.Customers,
                                ScreensEnum.Payments,
                                ScreensEnum.ReviewFiles,
                            };
                            break;
                        case "Leader":
                            screens = new List<ScreensEnum>()
                            {
                                ScreensEnum.LessonsViewOnly,
                                ScreensEnum.ReviewFilesViewOnly
                            };
                            break;
                    }
                    if(screens == null)
                        MessageBox.Show("Ошибка авторизации. Нет данных о роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        MainWindow window = new MainWindow(screens);
                        window.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка авторизации. Проверьте интернет осединение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
