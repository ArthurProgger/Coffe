using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;

namespace Coffe
{
    public partial class MainWindow : Window
    {
        private const int _maxRows = 10;
        private int _currentIngredientsPageNum, _currentIngredientId;
        private DataSet _ingredientsPages = new DataSet();

        private void IngredientsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ingredientsPage.Visibility == Visibility.Visible)
            {
                title.Content = "Список ингредиентов";

                _lastPage = _lastPage == ordersPage ? productsPage : null;

                _currentIngredientsPageNum = 0;
                _ingredientsPages.Clear();
                LoadIngredients();
            }
            else
                ClearTable(ingredients);
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            _lastPage = ingredientsPage;
            _isInsert = true;
            OpenPage(addIngredientPage);
        }

        private void LoadIngredients(int pageIndex = -1)
        {
            try
            {
                ClearTable(ingredients);

                using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    DataTable ingredients;

                    if (_ingredientsPages.Tables.Count < 1 || pageIndex == -1)
                        using (SqlDataAdapter ad = new SqlDataAdapter("exec get_ingredients", conn))
                        {
                            ingredients = new DataTable();
                            ad.Fill(ingredients);

                            _currentIngredientsPageNum = 0;

                            firstIngrPage.Visibility = Visibility.Hidden;
                            backIngrPage.Visibility = Visibility.Hidden;

                            _ingredientsPages.Tables.Clear();
                        }
                    else
                    {
                        ingredients = _ingredientsPages.Tables[pageIndex];

                        firstIngrPage.Visibility = _currentIngredientsPageNum > 0 ? Visibility.Visible : Visibility.Hidden;
                        backIngrPage.Visibility = _currentIngredientsPageNum > 0 ? Visibility.Visible : Visibility.Hidden;
                        nextIngrPage.Visibility = _currentIngredientsPageNum < _ingredientsPages.Tables.Count - 1 ? Visibility.Visible : Visibility.Hidden;
                        lastIngrPage.Visibility = _currentIngredientsPageNum < _ingredientsPages.Tables.Count - 1 ? Visibility.Visible : Visibility.Hidden;
                    }

                    int i = 0;
                    foreach (DataRow r in ingredients.Rows)
                    {
                        if (i < _maxRows)
                        {
                            DataTable ingredientCount = new DataTable();

                            using (SqlDataAdapter ad2 = new SqlDataAdapter($"exec get_ingredients_count {r[0]}", conn))
                            {
                                ingredientCount = new DataTable();
                                ad2.Fill(ingredientCount);
                                ingredientCount.Rows[0][0] = string.IsNullOrEmpty(ingredientCount.Rows[0][0].ToString()) ? 0 : float.Parse(ingredientCount.Rows[0][0].ToString());
                            }

                            this.ingredients.RowDefinitions.Add(new RowDefinition()
                            {
                                Height = new GridLength(70)
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
                            Grid.SetRow(v, this.ingredients.RowDefinitions.Count - 1);
                            this.ingredients.Children.Add(v);

                            v = new Label()
                            {
                                FontFamily = new FontFamily("Comic Sans MS"),
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                FontSize = 15,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = $"{r[2]} руб. за {r[5]} {r[4]}"
                            };

                            Grid.SetColumn(v, 1);
                            Grid.SetRow(v, this.ingredients.RowDefinitions.Count - 1);
                            this.ingredients.Children.Add(v);

                            v = new Label()
                            {
                                FontFamily = new FontFamily("Comic Sans MS"),
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                FontSize = 15,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = $"{ingredientCount.Rows[0][0]} {r[4]}"
                            };

                            Grid.SetColumn(v, 2);
                            Grid.SetRow(v, this.ingredients.RowDefinitions.Count - 1);
                            this.ingredients.Children.Add(v);

                            v = new Label()
                            {
                                FontFamily = new FontFamily("Comic Sans MS"),
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                FontSize = 15,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Content = new Grid()
                                {
                                    Height = 65
                                }
                            };

                            (v.Content as Grid).Children.Add(new Label()
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Foreground = new SolidColorBrush(Colors.Blue),
                                Cursor = Cursors.Hand,
                                Content = "Редактировать"
                            });

                            (v.Content as Grid).Children.Add(new Label()
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Foreground = new SolidColorBrush(Colors.Red),
                                Cursor = Cursors.Hand,
                                Content = "Удалить"
                            });

                            (v.Content as Grid).Children[0].MouseDown += new MouseButtonEventHandler(EditClick);
                            (v.Content as Grid).Children[1].MouseDown += new MouseButtonEventHandler(DeleteClick);

                            _ingredientsRows.Add(new object[] { r[0], (v.Content as Grid).Children[0], (v.Content as Grid).Children[1] });

                            Grid.SetColumn(v, 3);
                            Grid.SetRow(v, this.ingredients.RowDefinitions.Count - 1);
                            this.ingredients.Children.Add(v);

                            ingredientsCount.Content = $"{r[6]} наименований";
                            ingredientsSummary.Content = $"Запасов в холодильнике на сумму (руб.): {r[7]}";

                        }

                        if (pageIndex == -1)
                        {
                            if (i % _maxRows == 0)
                            {
                                _ingredientsPages.Tables.Add(new DataTable());

                                foreach (DataColumn c in ingredients.Columns)
                                    _ingredientsPages.Tables[_ingredientsPages.Tables.Count - 1].Columns.Add(c.ColumnName, c.DataType);
                            }

                            _ingredientsPages.Tables[_ingredientsPages.Tables.Count - 1].Rows.Add(r.ItemArray);

                        }

                        i++;
                    }
                }

                pagesCount.Content = $"{_currentIngredientsPageNum + 1} / {_ingredientsPages.Tables.Count}";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FirstIngrPage_Click(object sender, RoutedEventArgs e)
        {
            _currentIngredientsPageNum = 0;
            LoadIngredients(_currentIngredientsPageNum);
        }

        private void BackIngrPage_Click(object sender, RoutedEventArgs e)
        {
            _currentIngredientsPageNum--;
            LoadIngredients(_currentIngredientsPageNum);
        }
        private void NextIngrPage_Click(object sender, RoutedEventArgs e)
        {
            _currentIngredientsPageNum++;
            LoadIngredients(_currentIngredientsPageNum);
        }

        private void LastIngrPage_Click(object sender, RoutedEventArgs e)
        {
            _currentIngredientsPageNum = _ingredientsPages.Tables.Count - 1;
            LoadIngredients(_currentIngredientsPageNum);
        }

        // редактирование ингредиента

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            _lastPage = ingredientsPage;
            _isInsert = false;
            OpenPage(addIngredientPage);

            foreach (object[] r in _ingredientsRows)
                if (sender.Equals(r[1]))
                    _currentIngredientId = int.Parse(r[0].ToString());

            var row = from r in _ingredientsPages.Tables[_currentIngredientsPageNum].AsEnumerable()
                      where r.Field<int>(0) == _currentIngredientId
                      select r;

            ingredientName.Text = row.ToArray()[0][1].ToString();
            ingredientPrice.Text = row.ToArray()[0][2].ToString();
            ingredientCount.Text = string.IsNullOrEmpty(row.ToArray()[0][3].ToString()) ? "0" : row.ToArray()[0][3].ToString();
            units.SelectedItem = row.ToArray()[0][4].ToString();
            ingredientCountUnit.Text = row.ToArray()[0][5].ToString();
        }
        
        // удаление ингредиента
        private void DeleteClick(object sender, MouseButtonEventArgs e)
        {
            foreach (object[] r in _ingredientsRows)
                if (sender.Equals(r[2]))
                    _currentIngredientId = int.Parse(r[0].ToString());

            if (MessageBox.Show("Удалить ингредиент?", _warningCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                    {
                        conn.Open();

                        using (SqlCommand comm = new SqlCommand(
                            "delete from ingredients " +
                            $"where id = {_currentIngredientId}", conn))

                            comm.ExecuteNonQuery();

                        MessageBox.Show("Ингредиент удален!", _infoCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    LoadIngredients();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}