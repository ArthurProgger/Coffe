using System;
using System.IO;
using System.Windows;
using System.Data.SqlClient;

namespace Coffe
{
    /// <summary>
    /// страница авторизации
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _serverName;

        private void AuthPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (authPage.Visibility == Visibility.Visible)
            {
                title.Content = "Добро пожаловать!";

                userName.Clear();
                userPassword.Clear();

                openProducts.Visibility = Visibility.Hidden;
                openIngredients.Visibility = Visibility.Hidden;
                createOrder.Visibility = Visibility.Hidden;
                openOrders.Visibility = Visibility.Hidden;
                exit.Visibility = Visibility.Hidden;

                _serverName = null;

                using (StreamReader r = new StreamReader("Server name.txt"))
                    _serverName = r.ReadToEnd();
            }
            else
            {
                openProducts.Visibility = Visibility.Visible;
                exit.Visibility = Visibility.Visible;
            }
        }

        // попытка входа в систему
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            App.ConnectionString = $@"Data Source={_serverName};Initial Catalog=Coffe;Persist Security Info=True;User ID={userName.Text};Password={userPassword.Password}";

            try
            {
                using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    if (string.Equals(userName.Text, "sa"))
                    {
                        openIngredients.Visibility = Visibility.Visible;
                        productsPage.ContextMenu.Visibility = Visibility.Visible;
                        openOrders.Visibility = Visibility.Visible;
                        createOrder.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        _userName = userName.Text;

                        openIngredients.Visibility = Visibility.Hidden;
                        productsPage.ContextMenu.Visibility = Visibility.Hidden;
                        createOrder.Visibility = Visibility.Visible;
                    }
                }

                OpenPage(productsPage);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e) => OpenPage(regPage);

        // регистрация

        private void RegPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (regPage.Visibility == Visibility.Visible)
            {
                title.Content = "Регистрация";

                newUserName.Clear();
                newUserPassword.Clear();
                confirmPassword.Clear();

                openProducts.Visibility = Visibility.Hidden;
                openIngredients.Visibility = Visibility.Hidden;
                exit.Visibility = Visibility.Hidden;
            }
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(newUserPassword.Password, confirmPassword.Password))
            {
                App.ConnectionString = $"Data Source={_serverName};Initial Catalog=Coffe;Persist Security Info=True;User ID=sa;Password=qwerty123";

                try
                {
                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        using (SqlCommand comm = new SqlCommand(
                            $"create login {newUserName.Text} with password = '{newUserPassword.Password}' " +
                            $"create user {newUserName.Text} for login {newUserName.Text} " +
                            $"alter role client add member {newUserName.Text} " +
                            $"insert into users values ('{newUserName.Text}')", conn))
                            comm.ExecuteNonQuery();
                    }

                    MessageBox.Show("Вы успешно зарегистрировались!", _infoCaption, MessageBoxButton.OK, MessageBoxImage.Information);

                    OpenPage(authPage);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("Пароли не совпадают!", _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}