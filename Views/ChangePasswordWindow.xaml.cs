using System;
using System.Windows;
using Task.Services;

namespace Task.Views
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly int _userId;

        public ChangePasswordWindow(DatabaseService databaseService, int userId)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _userId = userId;
            
            txtCurrentPassword.Focus();
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = txtCurrentPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            
            if (string.IsNullOrEmpty(currentPassword) || 
                string.IsNullOrEmpty(newPassword) || 
                string.IsNullOrEmpty(confirmPassword))
            {
                txtError.Text = "Все поля обязательны для заполнения.";
                return;
            }
            
            if (newPassword != confirmPassword)
            {
                txtError.Text = "Новый пароль и подтверждение не совпадают.";
                return;
            }
            
            if (_databaseService.UpdatePassword(_userId, currentPassword, newPassword))
            {
                MessageBox.Show("Пароль успешно изменен.", "Успех", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
                txtError.Text = "Неверный текущий пароль.";
        }
    }
} 