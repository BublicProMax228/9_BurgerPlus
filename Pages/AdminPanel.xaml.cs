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

namespace _9_BurgerPlus.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Page
    {
        private Users _selectedUser;
        public AdminPanel()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            using (var db = new DBBurgerPlusEntities())
            {
                DgUsers.ItemsSource = db.Users
                    .Include("Roles")
                    .ToList();
            }
        }
        private void ClearForm()
        {
            TxtLogin.Text = "";
            TxtPassword.Password = "";
            CmbRole.SelectedIndex = 1; 
            _selectedUser = null;
            BtnAdd.IsEnabled = true;
            BtnUpdate.IsEnabled = true;
        }

        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = DgUsers.SelectedItem as Users;

            if (_selectedUser != null)
            {
                TxtLogin.Text = _selectedUser.Login;
                TxtPassword.Password = _selectedUser.Password;

                foreach (ComboBoxItem item in CmbRole.Items)
                {
                    if (item.Tag.ToString() == _selectedUser.Role.ToString())
                    {
                        CmbRole.SelectedItem = item;
                        break;
                    }
                }

                BtnAdd.IsEnabled = false;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;
            int roleId = int.Parse(((ComboBoxItem)CmbRole.SelectedItem).Tag.ToString());

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните логин и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new DBBurgerPlusEntities())
            {
                if (db.Users.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newUser = new Users
                {
                    Login = login,
                    Password = password,
                    Role = roleId,
                    Block = false,
                    LogTry = 0
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }

            MessageBox.Show("Пользователь добавлен", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            LoadUsers();
            ClearForm();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;
            int roleId = int.Parse(((ComboBoxItem)CmbRole.SelectedItem).Tag.ToString());

            using (var db = new DBBurgerPlusEntities())
            {
                var user = db.Users.Find(_selectedUser.Id);

                if (user != null)
                {
                    if (user.Login != login && db.Users.Any(u => u.Login == login))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    user.Login = login;
                    user.Password = password;
                    user.Role = roleId;

                    db.SaveChanges();
                }
            }

            MessageBox.Show("Данные пользователя обновлены", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            LoadUsers();
            ClearForm();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnUnblock_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            int userId = (int)btn.Tag;

            using (var db = new DBBurgerPlusEntities())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    user.Block = false;
                    user.LogTry = 0;
                    db.SaveChanges();
                }
            }

            LoadUsers();

            MessageBox.Show("Пользователь разблокирован", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Navigation());
        }
    }
}
