using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Authorization
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AuthorizationBdEntities db = new AuthorizationBdEntities();

        string phonePattern = @"^(\+?7|7|8)[\s\-\(]*(\d{3})[\s\-\)]*(\d{3})[\s\-]*(\d{2})[\s\-]*(\d{2})$";
        string emailPattern = @"^[a-zA-Z0-9.-]+@[a-zA-Z0-9-]+\.[a-zA-Z-]{2,}$";
        string passPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = NameField.Text.Trim();
                string surname = SurnameField.Text.Trim();
                string patronymic = PatronymicField.Text.Trim();
                string phone = PhoneField.Text.Trim();
                string email = emailField.Text.Trim();
                string login = RegLoginField.Text.Trim();
                string pass = RegPasswordField.Text.Trim();
                string passApply = PasswordApplyField.Text.Trim();
                string hashedPass = BCrypt.Net.BCrypt.HashPassword(pass);

                if (name == "" || surname == "" || phone == "" || email == ""
                    || login == "" || pass == "" || passApply == ""
                    || !Regex.IsMatch(phone, phonePattern)
                    || !CheckEmail(email, emailPattern)
                    || !Regex.IsMatch(pass, passPattern))
                    throw new Exception();

                if (db.Users.Any(x => x.Login == login))
                {
                    MessageBox.Show("Логин занят");
                    return;
                }
                phone = Regex.Replace(phone, @"\D", "");

                db.Users.Add(new User
                {
                    Name = name,
                    Surname = surname,
                    Patronymic = patronymic,
                    Phone = phone,
                    Email = email,
                    Login = login,
                    Password = hashedPass
                });

                db.SaveChanges();
                MessageBox.Show("Вы успешно зарегистрировались");

                NameField.Text = "";
                SurnameField.Text = "";
                PatronymicField.Text = "";
                PhoneField.Text = "";
                emailField.Text = "";
                RegLoginField.Text = "";
                RegPasswordField.Text = "";
                PasswordApplyField.Text = "";
            }
            catch
            {
                MessageBox.Show("Ошибка в данных, введи корректные данные");
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginField.Text.Trim();
            string pass = PasswordField.Text.Trim();

            User userFound = db.Users.FirstOrDefault(x => x.Login == login);

            bool isUserFound = userFound != null && BCrypt.Net.BCrypt.Verify(pass, userFound.Password);

            if (isUserFound) MessageBox.Show("Вы вошли!");
            else MessageBox.Show("Вы не вошли!");
        }


        private bool CheckEmail(string email, string emailPattern)
        {
            if (!Regex.IsMatch(email, emailPattern)) return false;

            bool isUserFound = db.Users.Any(x => x.Email == email);

            if (isUserFound) return false;

            return true;
        }
    }
}
