using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;

namespace Coffe
{
    public partial class MainWindow : Window
    {
        private void CraftPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (craftPage.Visibility == Visibility.Visible)
            {
                _ingredientsRows.Clear();
                LoadIngredientsRecipe();

                productCountCaption.Visibility = _isInsert ? Visibility.Hidden : Visibility.Visible;
                incr.Visibility = _isInsert ? Visibility.Hidden : Visibility.Visible;
                productCount.Visibility = _isInsert ? Visibility.Hidden : Visibility.Visible;
                decr.Visibility = _isInsert ? Visibility.Hidden : Visibility.Visible;
                summary.Visibility = _isInsert ? Visibility.Hidden : Visibility.Visible;

                CalculationSummary();

                recipeText.IsReadOnly = !_isInsert;
            }
        }

        private void Incr_Click(object sender, RoutedEventArgs e)
        {
            decr.Visibility = Visibility.Visible;
            productCount.Text = (int.Parse(productCount.Text) + 1).ToString();
            CalculationSummary();
        }

        private void Decr_Click(object sender, RoutedEventArgs e)
        {
            decr.Visibility = (int.Parse(productCount.Text) - 1) > 1 ? Visibility.Visible : Visibility.Hidden;
            productCount.Text = (int.Parse(productCount.Text) - 1).ToString();
            CalculationSummary();
        }

        private void CalculationSummary()
        {
            var row = from r in _productsDT.AsEnumerable()
                      where _productId == int.Parse(r[0].ToString())
                      select r[2];

            summary.Content = $"Общая стоимость: {int.Parse(productCount.Text) * decimal.Parse(row.ToArray()[0].ToString())} руб.";

        }

        private void LoadIngredientsRecipe()
        {
            ClearTable(ingredientsRecipe);

            try
            {
                using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter ad = new SqlDataAdapter("exec get_ingredients", App.ConnectionString))
                    {
                        DataTable ingredients = new DataTable(), ingredientCount;
                        ad.Fill(ingredients);

                        // ингредиенты блюда
                        foreach (DataRow r in ingredients.Rows)
                        {
                            using (SqlDataAdapter ad2 = new SqlDataAdapter($"exec get_ingredients_count {r[0]}", conn))
                            {
                                ingredientCount = new DataTable();
                                ad2.Fill(ingredientCount);
                                ingredientCount.Rows[0][0] = string.IsNullOrEmpty(ingredientCount.Rows[0][0].ToString()) ? 0 : float.Parse(ingredientCount.Rows[0][0].ToString());
                            }

                            ingredientsRecipe.RowDefinitions.Add(new RowDefinition()
                            {
                                Height = new GridLength(40)
                            });

                            Label v = new Label()
                            {
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = new CheckBox()
                            };

                            (v.Content as CheckBox).IsEnabled = _isInsert;

                            Grid.SetColumn(v, 0);
                            Grid.SetRow(v, ingredientsRecipe.RowDefinitions.Count - 1);
                            ingredientsRecipe.Children.Add(v);

                            TextBox useIgredientsCount = new TextBox()
                            {
                                FontFamily = new FontFamily("Comic Sans MS"),
                                FontSize = 15,
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                HorizontalContentAlignment = HorizontalAlignment.Left,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                //Text = ingredientCount.Rows[0][0].ToString()
                            };

                            _ingredientsRows.Add(new object[] { r[0], v.Content, ingredientCount.Rows[0][0], useIgredientsCount, r[1] });

                            DataObject.AddPastingHandler(useIgredientsCount, IngredientValuePasting);
                            useIgredientsCount.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(IngredientValuePreviewTextInput);
                            useIgredientsCount.IsReadOnly = !_isInsert;

                            Grid.SetColumn(useIgredientsCount, 2);
                            Grid.SetRow(useIgredientsCount, ingredientsRecipe.RowDefinitions.Count - 1);
                            ingredientsRecipe.Children.Add(useIgredientsCount);

                            v = new Label()
                            {
                                FontFamily = new FontFamily("Comic Sans MS"),
                                FontSize = 15,
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                HorizontalContentAlignment = HorizontalAlignment.Left,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = r[1]
                            };

                            Grid.SetColumn(v, 1);
                            Grid.SetRow(v, ingredientsRecipe.RowDefinitions.Count - 1);
                            ingredientsRecipe.Children.Add(v);

                            v = new Label()
                            {
                                FontFamily = new FontFamily("Comic Sans MS"),
                                FontSize = 15,
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                HorizontalContentAlignment = HorizontalAlignment.Left,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = r[4]
                            };

                            Grid.SetColumn(v, 3);
                            Grid.SetRow(v, ingredientsRecipe.RowDefinitions.Count - 1);
                            ingredientsRecipe.Children.Add(v);

                            v = new Label()
                            {
                                FontFamily = new FontFamily("Comic Sans MS"),
                                FontSize = 15,
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                HorizontalContentAlignment = HorizontalAlignment.Left,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = float.Parse(r[2].ToString()) * float.Parse(ingredientCount.Rows[0][0].ToString())
                            };

                            Grid.SetColumn(v, 4);
                            Grid.SetRow(v, ingredientsRecipe.RowDefinitions.Count - 1);
                            ingredientsRecipe.Children.Add(v);
                        }
                    }

                    using (SqlDataAdapter ad = new SqlDataAdapter($"exec get_craft {_productId}", conn))
                    {
                        DataTable ingredients = new DataTable();
                        ad.Fill(ingredients);

                        foreach (DataRow r in ingredients.Rows)
                        {
                            var row = from rI in _ingredientsRows
                                      where int.Parse(rI[0].ToString()) == int.Parse(r[0].ToString())
                                      select rI;

                            (row.ToArray()[0][1] as CheckBox).IsChecked = true;
                            (row.ToArray()[0][3] as TextBox).Text = r[1].ToString();
                        }
                    }

                    using (SqlDataAdapter ad = new SqlDataAdapter($"exec get_product_recipe {_productId}", conn))
                    {
                        DataTable rec = new DataTable();
                        ad.Fill(rec);

                        recipeText.Text = rec.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // готовка
        private void Cooking_Click(object sender, RoutedEventArgs e)
        {
            var rows = from r in _ingredientsRows
                       where (r[1] as CheckBox).IsChecked.Value
                       select r;

            try
            {
                if (_isInsert)
                {
                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        using (SqlCommand comm = new SqlCommand(
                            $"delete from crafts where prod_id = {_productId}", conn))
                            comm.ExecuteNonQuery();

                            string values = null;

                        foreach (object[] r in rows.ToArray())
                            values += $"({_productId}, {r[0]}, {(r[3] as TextBox).Text.Replace(',' , '.')}),";

                        values = values.Substring(0, values.Length - 1);

                        using (SqlCommand comm = new SqlCommand(
                            $"insert into crafts values {values}", conn))
                            comm.ExecuteNonQuery();

                        using (SqlCommand comm = new SqlCommand(
                            "update products " +
                            $"set recipe = '{recipeText.Text}' " +
                            $"where id = {_productId}", conn))
                            comm.ExecuteNonQuery();

                        MessageBox.Show("Рецепт сохранен!", _infoCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        OpenPage(productsPage);
                    }
                }
                else
                {
                    foreach (object[] r in rows.ToArray())
                        if (float.Parse((r[3] as TextBox).Text) * int.Parse(productCount.Text) > float.Parse(r[2].ToString()))
                        {
                            MessageBox.Show($"Не хватает \"{r[4]}\" в количестве {float.Parse((r[3] as TextBox).Text) * int.Parse(productCount.Text) - float.Parse(r[2].ToString())}", _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        string values = null;

                        foreach (object[] r in rows.ToArray())
                            values += $"('{DateTime.Now}', {r[0]}, {float.Parse((r[3] as TextBox).Text) * int.Parse(productCount.Text)}),";

                        values = values.Substring(0, values.Length - 1);

                        using (SqlCommand comm = new SqlCommand(
                            $"insert into ingredients_using values {values}", conn))
                            comm.ExecuteNonQuery();

                        if (_orderId != -1)
                        {
                            using (SqlCommand comm = new SqlCommand(
                                "update orders_products " +
                                "set " +
                                "is_cooked = 1 " +
                                "where " +
                                $"order_id = {_orderId} and " +
                                $"product_id = {rows.ToArray()[0][0]}", conn))
                                comm.ExecuteNonQuery();
                        }

                        MessageBox.Show("Готово!", _infoCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        OpenPage(productsPage);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}