using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Coffe
{
    /// <summary>
    /// Контент приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _productId, _orderId;
        private bool _isInsert;
        private string _userName;
        private const string _errCaption = "Ошибка", _infoCaption = "Информация", _warningCaption = "Внимание";
        private Regex _onlyNumbers = new Regex("[^0-9,]+");
        private Grid _lastPage;
        private List<object[]> _ingredientsRows = new List<object[]>(), _ordersRows = new List<object[]>();

        private void Window_Loaded(object sender, RoutedEventArgs e) => OpenPage(authPage);

        public MainWindow() => InitializeComponent();

        // отмена действий
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Cancel() == MessageBoxResult.Yes)
                OpenPage(_lastPage);
        }
        private MessageBoxResult Cancel() => MessageBox.Show("Отменить?", _warningCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

        // открытие страниц
        private void OpenPage(Grid page)
        {
            foreach (object p in appContent.Children)
                if (p.GetType() == typeof(Grid))
                    (p as Grid).Visibility = (p as Grid).Name.Contains("Page") && !p.Equals(page) ? Visibility.Hidden : Visibility.Visible;

            page.Visibility = Visibility.Visible;
        }

        // загрузка единиц измерения
        private void LoadUnits(ComboBox cb)
        {
            cb.Items.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter ad = new SqlDataAdapter("exec get_units", conn))
                    {
                        DataTable units = new DataTable();
                        ad.Fill(units);

                        foreach (DataRow r in units.Rows)
                            cb.Items.Add(r.ItemArray[0]);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsTextAllowed(string text, Regex reg) => !reg.IsMatch(text);
        
        // выход из системы
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вернуться в окно авторизации?", _warningCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                OpenPage(authPage);
        }

        private void OpenProducts_Click(object sender, RoutedEventArgs e)
        {
            if (addIngredientPage.Visibility == Visibility.Visible || addProductPage.Visibility == Visibility.Visible)
            {
                if (Cancel() == MessageBoxResult.Yes)
                    OpenPage(productsPage);
            }
            else
                OpenPage(productsPage);
        }

        private void OpenIngredients_Click(object sender, RoutedEventArgs e)
        {
            if (addIngredientPage.Visibility == Visibility.Visible || addProductPage.Visibility == Visibility.Visible)
            {
                if (Cancel() == MessageBoxResult.Yes)
                    OpenPage(ingredientsPage);
            }
            else
                OpenPage(ingredientsPage);
        }

        private void ClearTable(Grid container)
        {
            for (int i = container.RowDefinitions.Count - 1; i > 0; i--)
                container.RowDefinitions.RemoveAt(i);

            for (int i = 0; i < container.Children.Count - 1; i++)
                if (string.IsNullOrEmpty((container.Children[i] as Control).Name))
                {
                    container.Children.Remove(container.Children[i]);
                    i = 0;
                }
        }

        private void IngredientValuePreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) => e.Handled = !IsTextAllowed((sender as TextBox).Text, _onlyNumbers);

        private void OpenOrders_Click(object sender, RoutedEventArgs e) => OpenPage(ordersPage);

        private void IngredientValuePasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed((sender as TextBox).Text, _onlyNumbers))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }

        private void LoadProductsTypes(ComboBox cb)
        {
            cb.Items.Clear();

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
                            cb.Items.Add(r.ItemArray[0]);
                    }
                }

                recipeText.Clear();
                LoadProducts();

                cb.SelectedIndex = cb.Items.Count > 0 ? 0 : -1;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, _errCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}