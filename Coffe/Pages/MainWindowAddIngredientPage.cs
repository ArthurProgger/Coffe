using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace Coffe
{
    public partial class MainWindow : Window
    {
        private void AddIngredientPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (addIngredientPage.Visibility == Visibility.Visible)
            {
                foreach (object tb in addIngredientContent.Children)
                    if (tb.GetType() == typeof(TextBox))
                        (tb as TextBox).Clear();

                LoadUnits(units);

                addIngr.Content = _isInsert ? "Добавить" : "Изменить";
                title.Content = _isInsert ? "Добавить ингредиент" : "Изменить ингредиент";
                ingredientCountCaption.Visibility = _isInsert ? Visibility.Hidden : Visibility.Visible;
                ingredientCount.Visibility = _isInsert ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private void AddIngr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    SqlCommand comm = _isInsert ?
                    new SqlCommand("insert into ingredients values" +
                        $"('{ingredientName.Text}', {decimal.Parse(ingredientPrice.Text).ToString().Replace(',', '.')}, {float.Parse(ingredientCountUnit.Text).ToString().Replace(',', '.')}, '{units.SelectedItem}')", conn)
                        :
                    new SqlCommand("update " +
                                    "ingredients " +
                                    "set " +
                                    $"ingr_name = '{ingredientName.Text}', " +
                                    $"price = {(decimal.Parse(ingredientPrice.Text)).ToString().Replace(',' , '.')}, " +
                                    $"count_of_price = {(float.Parse(ingredientCountUnit.Text)).ToString().Replace(',' , '.')}, " +
                                    $"unit = '{units.SelectedItem}' " +
                                    "where " +
                                    $"id = {_currentIngredientId}", conn);

                    using (comm)
                        comm.ExecuteNonQuery();

                    if (!_isInsert)
                        using (comm = new SqlCommand(
                            $"insert into ingredients_coming values ('{DateTime.Now}', {_currentIngredientId}, {float.Parse(ingredientCount.Text).ToString().Replace(',', '.')})", conn))
                            comm.ExecuteNonQuery();
                }

                string infoText = _isInsert ? $"Ингредиент \"{ingredientName.Text}\" добавлен!" : $"Ингредиент \"{ingredientName.Text}\" изменен!";
                MessageBox.Show(infoText, _infoCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                OpenPage(_lastPage);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}