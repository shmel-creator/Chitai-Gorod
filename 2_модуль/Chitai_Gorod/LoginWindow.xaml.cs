using System;
using System.Linq;
using System.Windows;
using Chitai_Gorod.Models;

namespace Chitai_Gorod
{
    public partial class LoginWindow : Window
    {
        private DBContext db = new DBContext();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = db.Пользователиs.FirstOrDefault(u => u.Логин == login && u.Пароль == password);

                if (user != null)
                {
                    // Формируем ФИО, пропуская пустое отчество
                    string fio = user.Фамилия + " " + user.Имя;
                    if (!string.IsNullOrWhiteSpace(user.Отчество))
                    {
                        fio = fio + " " + user.Отчество;
                    }

                    MainWindow mainWindow = new MainWindow(fio, user.IdРоли);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка БД: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow("Гость", 0);
            mainWindow.Show();
            this.Close();
        }
    }
}