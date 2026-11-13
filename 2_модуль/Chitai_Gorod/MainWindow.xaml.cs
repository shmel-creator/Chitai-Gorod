using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Chitai_Gorod.Models;

namespace Chitai_Gorod
{
    public partial class MainWindow : Window
    {
        private DBContext db = new DBContext();
        private string _fio;
        private int _roleId;

        public MainWindow(string fio, int roleId)
        {
            InitializeComponent();
            _fio = fio;
            _roleId = roleId;
            TbUserFio.Text = _fio;

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
                var result = new System.Collections.Generic.List<ProductDisplay>();

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

                    // Если фото нет в БД, подставляем заглушку из корня, иначе ищем в папке import
                    string imagePath = "pack://application:,,,/picture.png";
                    if (!string.IsNullOrWhiteSpace(tovar.Фото)) imagePath = "pack://application:,,,/import/" + tovar.Фото;

                    result.Add(new ProductDisplay
                    {
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

                ProductsList.ItemsSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}