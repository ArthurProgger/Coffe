using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace Coffe
{
    public partial class MainWindow : Window
    {
        private List<object[]> _productsRows = new List<object[]>();
        private DataTable _productsDT;

        private void ProductsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (productsPage.Visibility == Visibility.Visible)
            {
                title.Content = "Список блюд";

                productTypes.Items.Clear();
                productTypes.Items.Add("<Все типы>");

                try
                {
                    // типы блюд
                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        using (SqlDataAdapter ad = new SqlDataAdapter("exec get_products_types", conn))
                        {
                            DataTable types = new DataTable();
                            ad.Fill(types);

                            foreach (DataRow r in types.Rows)
                                productTypes.Items.Add(r.ItemArray[0]);
                        }
                    }

                    recipeText.Clear();
                    _orderId = ordersPage == _lastPage ? _orderId : -1;
                    LoadProducts();

                    productTypes.SelectedIndex = 0;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadProducts()
        {
            products.Items.Clear();

            try
            {
                // блюда
                using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    string prodType = productTypes.SelectedIndex < 1 ? null : productTypes.SelectedItem.ToString();
                    int onlyHave = this.onlyHave.IsChecked.Value ? 1 : 0;

                    using (SqlDataAdapter ad = new SqlDataAdapter($"exec get_products '{prodType}', '{search.Text}', {onlyHave}, {_orderId}", conn))
                    {
                        _productsDT = new DataTable();
                        ad.Fill(_productsDT);

                        // верстка плиток для блюд

                        foreach (DataRow r in _productsDT.Rows)
                        {
                            ListViewItem productContent = new ListViewItem()
                            {
                                BorderThickness = new Thickness(1),
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                Margin = new Thickness(10, 10, 0, 0),
                                Cursor = Cursors.Hand,
                                Width = 175,
                                Height = 140,
                                HorizontalContentAlignment = HorizontalAlignment.Center
                            };

                            BitmapImage bmi = null;
                            if (File.Exists(r[3].ToString()))
                            {
                                bmi = new BitmapImage();
                                bmi.BeginInit();
                                bmi.UriSource = new Uri(r[3].ToString(), UriKind.Absolute);
                                bmi.EndInit();
                            }

                            if (bmi == null)
                                productContent.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c8c8c8"));
                            else
                                productContent.Background = new ImageBrush(bmi);

                            productContent.Content = new Grid()
                            {
                                Width = 165,
                                VerticalAlignment = VerticalAlignment.Bottom
                            };

                            (productContent.Content as Grid).Children.Add(new TextBlock()
                            {
                                Padding = new Thickness(5, 10, 5, 10),
                                FontFamily = new FontFamily("Comic Sans MS"),
                                TextAlignment = TextAlignment.Center,
                                FontSize = 15,
                                TextWrapping = TextWrapping.Wrap,
                                VerticalAlignment = VerticalAlignment.Top,
                                Foreground = new SolidColorBrush(Colors.White),
                                Margin = new Thickness(-4, 10, -4, -28),
                                Height = 39,
                                Text = r.ItemArray[1].ToString()
                            });

                            (productContent.Content as Grid).Children.Add(new TextBlock()
                            {
                                Padding = new Thickness(5, 0, 5, 0),
                                FontFamily = new FontFamily("Comic Sans MS"),
                                TextAlignment = TextAlignment.Center,
                                FontSize = 10,
                                TextWrapping = TextWrapping.Wrap,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Foreground = new SolidColorBrush(Colors.White),
                                Margin = new Thickness(-4, 0, -4, -58),
                                Height = 30,
                                Text = $"1 порция = {r.ItemArray[2].ToString()} руб."
                            });

                            productContent.MouseDown += new MouseButtonEventHandler(OpenProductClick);
                            productContent.AddHandler(MouseDownEvent, new MouseButtonEventHandler(OpenProductClick), true);

                            _productsRows.Add(new object[] { r[0], productContent, false });
                            products.Items.Add(productContent);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProductTypes_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadProducts();

        private void Search_TextChanged(object sender, TextChangedEventArgs e) => LoadProducts();

        private void OnlyHave_Checked(object sender, RoutedEventArgs e) => LoadProducts();

        private void OnlyHave_Unchecked(object sender, RoutedEventArgs e) => LoadProducts();

        //открытие рецепта товара или формирование заказа
        private void OpenProductClick(object sender, MouseEventArgs e)
        {
            if (productsPage.ContextMenu.Visibility == Visibility.Visible)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _isInsert = false;
                    OpenProduct(sender);
                }
            }
            else if (MessageBox.Show("Заказать блюдо?", _warningCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var row = from r in _productsRows
                           where r[1].Equals(sender)
                           select r;

                row.ToArray()[0][2] = true;
                (row.ToArray()[0][1] as ListViewItem).BorderBrush = new SolidColorBrush(Colors.Green);
                (row.ToArray()[0][1] as ListViewItem).BorderThickness = new Thickness(3, 3, 3, 3);

                products.SelectedIndex = -1;
            }
        }

        private void ProductEditCM_Click(object sender, RoutedEventArgs e)
        {
            _isInsert = false;
            OpenPage(addProductPage);

            var row = from r in _productsDT.AsEnumerable()
                      where int.Parse(r[0].ToString()) == _productId
                      select r;

            productName.Text = row.ToArray()[0][1].ToString();
            productPrice.Text = row.ToArray()[0][2].ToString();
            imgPath.Text = row.ToArray()[0][3].ToString();
            productTypeAdd.SelectedItem = row.ToArray()[0][4].ToString();
            productCookingTime.Text = row.ToArray()[0][5].ToString();
        }

        private void RecipeEditCM_Click(object sender, RoutedEventArgs e)
        {
            _isInsert = true;
            OpenProduct(products.SelectedItem);
        }

        private void DropProductCM_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить блюдо?", _warningCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        var row = from r in _productsRows
                                 where products.SelectedItem.Equals(r[1])
                                 select r;

                        using (SqlCommand comm = new SqlCommand(
                            $"delete from products where id = {row.ToArray()[0]}", conn))
                            comm.ExecuteNonQuery();

                        MessageBox.Show($"Блюдо \"{(((row.ToArray()[0][1] as ListViewItem).Content as Grid).Children[0] as TextBlock).Text}\"", _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddProductCM_Click(object sender, RoutedEventArgs e)
        {
            _isInsert = true;
            OpenPage(addProductPage);
        }

        private void OpenProduct(object sender)
        {
            var row = from r in _productsDT.AsEnumerable()
                      where r.Field<int>(0) == _productId
                      select r;

            title.Content = $"Рецепт для \"{row.ToArray()[0][1]}\"";
            productType.Content = $"Категория: {row.ToArray()[0][4]}";
            craftingTime.Content = $"Время на приготовление: {row.ToArray()[0][5]}";

            cooking.Content = _isInsert ? "Сохранить" : "Готовим!";

            OpenPage(craftPage);
        }

        private void ProductsPage_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            dropProductCM.IsEnabled = products.SelectedItem != null;
            recipeEditCM.IsEnabled = products.SelectedItem != null;
            productEditCM.IsEnabled = products.SelectedItem != null;
        }

        private void Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (products.SelectedIndex != -1)
                foreach (object[] r in _productsRows)
                    if (products.SelectedItem.Equals(r[1]))
                        _productId = int.Parse(r[0].ToString());
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Создать заказ?", _warningCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        var rows = from r in _productsRows
                                   where bool.Parse(r[2].ToString())
                                   select r[0];

                        using (SqlCommand comm = new SqlCommand(
                            $"insert into orders values ('{_userName}')", conn))
                            comm.ExecuteNonQuery();

                        int orderId;
                        using (SqlDataAdapter ad = new SqlDataAdapter($"exec get_last_order '{_userName}'", conn))
                        {
                            DataTable id = new DataTable();
                            ad.Fill(id);
                            orderId = int.Parse(id.Rows[0][0].ToString());
                        }

                        string values = null;
                        foreach (object v in rows)
                            values += $"({orderId}, {v} , 0),";

                        values = values.Substring(0, values.Length - 1);

                        using (SqlCommand comm = new SqlCommand(
                            $"insert into orders_products values {values}", conn))
                            comm.ExecuteNonQuery();

                        MessageBox.Show("Заказ сформирован! Ожидайте.", _infoCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts();
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}