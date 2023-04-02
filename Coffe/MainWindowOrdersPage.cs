using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;

namespace Coffe
{
    public partial class MainWindow : Window
    {
        private void OrdersPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ordersPage.Visibility == Visibility.Visible)
            {
                title.Content = "Заказы";

                try
                {
                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        using (SqlDataAdapter ad = new SqlDataAdapter("exec get_orders", conn))
                        {
                            DataTable orders = new DataTable();
                            ad.Fill(orders);

                            foreach (DataRow r in orders.Rows)
                            {
                                this.orders.RowDefinitions.Add(new RowDefinition()
                                {
                                    Height = new GridLength(60)
                                });

                                Label v = new Label()
                                {
                                    FontFamily = new FontFamily("Comic Sans MS"),
                                    BorderBrush = new SolidColorBrush(Colors.Black),
                                    BorderThickness = new Thickness(1, 1, 1, 1),
                                    FontSize = 15,
                                    VerticalContentAlignment = VerticalAlignment.Center,
                                    Content = r[1]
                                };

                                Grid.SetColumn(v, 0);
                                Grid.SetRow(v, this.orders.RowDefinitions.Count - 1);
                                this.orders.Children.Add(v);

                                v = new Label()
                                {
                                    FontFamily = new FontFamily("Comic Sans MS"),
                                    BorderBrush = new SolidColorBrush(Colors.Black),
                                    BorderThickness = new Thickness(1, 1, 1, 1),
                                    Foreground = new SolidColorBrush(Colors.Blue),
                                    FontSize = 15,
                                    Cursor = Cursors.Hand,
                                    VerticalContentAlignment = VerticalAlignment.Center,
                                    Content = "Открыть"
                                };

                                v.MouseDown += new MouseButtonEventHandler(OpenOrder);

                                Grid.SetColumn(v, 1);
                                Grid.SetRow(v, this.orders.RowDefinitions.Count - 1);
                                this.orders.Children.Add(v);

                                _ordersRows.Add(new object[] { r[0], v });
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                ClearTable(orders);
        }

        private void OpenOrder(object sender, MouseEventArgs e)
        {
            var row = from r in _ordersRows
                      where r[1].Equals(sender)
                      select r[0];

            _orderId = int.Parse(row.ToArray()[0].ToString());
            
            _lastPage = ordersPage;
            OpenPage(productsPage);
        }
    }
}