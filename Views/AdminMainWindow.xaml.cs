using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Task.Services;

namespace Task.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        private readonly DatabaseService _databaseService;

        public AdminMainWindow(DatabaseService databaseService, int currentUserId)
        {
            InitializeComponent();
            _databaseService = databaseService;
            cmbRole.SelectedIndex = 0;
            LoadUsers();
        }

        private void LoadUsers() => 
            dgUsers.ItemsSource = _databaseService.GetAllUsers().DefaultView;

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            string login = txtNewLogin.Text.Trim();
            string password = txtNewPassword.Password;
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
            
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                txtStatus.Text = "Все поля обязательны для заполнения.";
                return;
            }
            
            if (_databaseService.CreateUser(login, password, role))
            {
                MessageBox.Show("Пользователь успешно создан.", "Успех", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                txtNewLogin.Clear();
                txtNewPassword.Clear();
                cmbRole.SelectedIndex = 0;
                LoadUsers();
                txtStatus.Text = string.Empty;
            }
            else
                txtStatus.Text = "Пользователь с таким логином уже существует.";
        }

        private void BtnUnblock_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                txtStatus.Text = "Выберите пользователя для разблокировки.";
                return;
            }
            
            int userId = Convert.ToInt32(((DataRowView)dgUsers.SelectedItem)["Id"]);
            
            if (_databaseService.UnblockUser(userId))
            {
                MessageBox.Show("Пользователь разблокирован.", "Успех", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
                txtStatus.Text = string.Empty;
            }
            else
                txtStatus.Text = "Не удалось разблокировать пользователя.";
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
            txtStatus.Text = string.Empty;
        }
    }
}