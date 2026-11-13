using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Chitai_Gorod.Models;

namespace Chitai_Gorod
{
    public partial class MainWindow : Window
    {
        private DBContext db = new DBContext();
        private string _fio;
        private int _roleId;
        private List<ProductDisplay> allProducts = new List<ProductDisplay>();

        public MainWindow(string fio, int roleId)
        {
            InitializeComponent();
            _fio = fio;
            _roleId = roleId;
            TbUserFio.Text = _fio;

            // Панель поиска/фильтров доступна только Менеджеру (2) и Админу (1)
            if (_roleId != 1 && _roleId != 2)
            {
                ToolbarPanel.Visibility = Visibility.Collapsed;
            }

            // Кнопка перехода к заказам доступна только Менеджеру и Админу
            if (_roleId == 1 || _roleId == 2)
            {
                BtnOrders.Visibility = Visibility.Visible;
            }

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = db.Товарыs
                    .Include(tovar => tovar.IdПроизводителяNavigation)
                    .Include(tovar => tovar.IdКатегорииNavigation)
                    .Include(tovar => tovar.IdПоставщикаNavigation)
                    .Include(tovar => tovar.IdЕдиницыИзмеренияNavigation)
                    .ToList();

                var ostatki = db.Остаткиs.ToList();
                allProducts = new List<ProductDisplay>();

                foreach (var tovar in products)
                {
                    int totalQty = ostatki.Where(o => o.IdТовара == tovar.IdТовара).Sum(o => o.Количество);
                    int discount = tovar.Скидка;
                    decimal finalPrice = tovar.Цена * (100 - discount) / 100;

                    string manufacturerName = "Не указан";
                    if (tovar.IdПроизводителяNavigation != null) manufacturerName = tovar.IdПроизводителяNavigation.Наименование;

                    string categoryName = "Без категории";
                    if (tovar.IdКатегорииNavigation != null) categoryName = tovar.IdКатегорииNavigation.Наименование;

                    string supplierName = "Не указан";
                    if (tovar.IdПоставщикаNavigation != null) supplierName = tovar.IdПоставщикаNavigation.Наименование;

                    string unitName = "шт.";
                    if (tovar.IdЕдиницыИзмеренияNavigation != null) unitName = tovar.IdЕдиницыИзмеренияNavigation.Наименование;

                    string description = "";
                    if (tovar.Описание != null) description = tovar.Описание;

                    // Если у товара нет фото в БД, подставляем заглушку из корня проекта. 
                    // Иначе ищем файл в папке import (папка с картинками из Приложения 2)
                    string imagePath = "pack://application:,,,/picture.png";
                    if (!string.IsNullOrWhiteSpace(tovar.Фото)) imagePath = "pack://application:,,,/import/" + tovar.Фото;

                    allProducts.Add(new ProductDisplay
                    {
                        IdТовара = tovar.IdТовара,
                        Manufacturer = manufacturerName,
                        Name = tovar.Наименование,
                        Category = categoryName,
                        Supplier = supplierName,
                        Description = description,
                        Unit = unitName,
                        TotalQuantity = totalQty,
                        ImageUri = imagePath,
                        BasePrice = tovar.Цена,
                        FinalPrice = finalPrice,
                        Discount = discount
                    });
                }

                ApplyFiltersAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void ApplyFiltersAndSort()
        {
            if (allProducts == null || LstProducts == null) return;

            List<ProductDisplay> filtered = new List<ProductDisplay>();
            string search = TxtSearch.Text.Trim().ToLower();

            string discountFilter = "Все диапазоны";
            if (CmbDiscountFilter.SelectedItem is ComboBoxItem selectedDiscount)
            {
                discountFilter = selectedDiscount.Content.ToString();
            }

            foreach (ProductDisplay product in allProducts)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    // Поиск идет по всем текстовым полям (название, описание, производитель, поставщик)
                    bool ok = product.Name.ToLower().Contains(search) ||
                              product.Description.ToLower().Contains(search) ||
                              product.Manufacturer.ToLower().Contains(search) ||
                              product.Supplier.ToLower().Contains(search);
                    if (!ok) continue;
                }

                if (discountFilter != "Все диапазоны")
                {
                    if (discountFilter == "0-12,99%" && !(product.Discount >= 0 && product.Discount <= 12)) continue;
                    if (discountFilter == "13-16,99%" && !(product.Discount >= 13 && product.Discount <= 16)) continue;
                    if (discountFilter == "17% и более" && product.Discount < 17) continue;
                }

                filtered.Add(product);
            }

            if (CmbSort.SelectedIndex == 1) filtered = filtered.OrderByDescending(p => p.FinalPrice).ToList();
            else if (CmbSort.SelectedIndex == 2) filtered = filtered.OrderBy(p => p.TotalQuantity).ToList();
            else if (CmbSort.SelectedIndex == 3) filtered = filtered.OrderByDescending(p => p.TotalQuantity).ToList();
            else filtered = filtered.OrderBy(p => p.FinalPrice).ToList();

            LstProducts.ItemsSource = filtered;
        }

        private void BtnAddTovar_Click(object sender, RoutedEventArgs e)
        {
            TovarWindow tovarWindow = new TovarWindow();
            if (tovarWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void TovarCard_Click(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border == null) return;

            ProductDisplay product = border.DataContext as ProductDisplay;
            if (product == null) return;

            // Админ (1) редактирует, Менеджер (2) смотрит (isReadOnly = true)
            bool isReadOnly = (_roleId != 1);

            TovarWindow tovarWindow = new TovarWindow(product.IdТовара, isReadOnly);
            if (tovarWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new OrdersWindow(_roleId);
            ordersWindow.ShowDialog();
            LoadProducts();
        }
    }
}