using System;
using System.Windows;
using Task.Services;

namespace Task.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private int _currentUserId;
        private string _currentUserRole;

        public LoginWindow()
        {
            InitializeComponent();
            _databaseService.BlockInactiveUsers();
            txtLogin.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Логин и пароль обязательны для заполнения.";
                return;
            }

            if (_databaseService.IsUserBlocked(login))
            {
                txtError.Text = "Вы заблокированы. Обратитесь к администратору.";
                return;
            }

            if (_databaseService.AuthenticateUser(login, password, out _currentUserId, out _currentUserRole, out bool needPasswordChange))
            {
                MessageBox.Show("Вы успешно авторизовались.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                if (needPasswordChange)
                    ShowChangePasswordWindow();
                else
                    ShowMainWindow();
            }
            else
            {
                txtError.Text = "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные.";
            }
        }

        private void ShowChangePasswordWindow()
        {
            var changePasswordWindow = new ChangePasswordWindow(_databaseService, _currentUserId);
            if (changePasswordWindow.ShowDialog() == true)
                ShowMainWindow();
            else
                MessageBox.Show("Вы должны сменить пароль при первом входе.", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowMainWindow()
        {
            if (_currentUserRole?.Equals("admin", StringComparison.OrdinalIgnoreCase) == true)
                new AdminMainWindow(_databaseService, _currentUserId).Show();
            else
                new UserMainWindow(_currentUserId).Show();
                
            Close();
        }
    }
} 