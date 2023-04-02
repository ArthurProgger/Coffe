using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace Coffe
{
    public partial class MainWindow : Window
    {
        private void AddProductPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (addProductPage.Visibility == Visibility.Visible)
            {
                foreach (Control el in addProductContent.Children)
                    if (el.GetType() == typeof(TextBox))
                        (el as TextBox).Clear();

                LoadProductsTypes(productTypeAdd);
                addProd.Content = _isInsert ? "Добавить" : "Изменить";
                title.Content = _isInsert ? "Добавить блюдо" : "Изменить блюдо";

                _lastPage = productsPage;
            }
        }

        private void FindFile_Click(object sender, RoutedEventArgs e)
        {
            FileDialog fg = new OpenFileDialog();
            fg.Filter = "Фото PNG|*.png|Фото JPG|*.jpg|Фото JPEG|*.jpeg";
            fg.ShowDialog();

            imgPath.Text = fg.FileName;
        }

        private void AddProd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    SqlCommand comm = _isInsert ?
                        new SqlCommand(
                            "insert into products values " +
                            $"('{productName.Text}', '{productTypeAdd.SelectedItem}', {productPrice.Text.Replace(',', '.')}, '{imgPath.Text}', null, '{productCookingTime.Text}')", conn)
                        :
                        new SqlCommand(
                            "update products " +
                            "set " +
                            $"prod_name = '{productName.Text}'," +
                            $"prod_type = '{productTypeAdd.SelectedItem}'," +
                            $"price = {productPrice.Text.Replace(',', '.')}," +
                            $"img_path = '{imgPath.Text}'," +
                            $"cooking_time = '{productCookingTime.Text}' " +
                            $"where id = {_productId}", conn);

                    using (comm)
                        comm.ExecuteNonQuery();

                    string infoText = _isInsert ? $"Блюдо \"{ingredientName.Text}\" добавлено!" : $"Блюдо \"{ingredientName.Text}\" изменено!";
                    MessageBox.Show(infoText, _infoCaption, MessageBoxButton.OK, MessageBoxImage.Information);

                    OpenPage(productsPage);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
