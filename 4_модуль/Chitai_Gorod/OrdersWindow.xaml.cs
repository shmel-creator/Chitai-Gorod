using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Chitai_Gorod.Models;

namespace Chitai_Gorod
{
    public partial class OrdersWindow : Window
    {
        private DBContext db = new DBContext();
        private int _roleId;

        public OrdersWindow(int roleId)
        {
            InitializeComponent();
            _roleId = roleId;

            if (_roleId == 1)
            {
                BtnAddOrder.Visibility = Visibility.Visible;
            }

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                var orders = db.Заказыs
                    .Include(o => o.IdПользователяNavigation)
                    .Include(o => o.IdСтатусаЗаказаNavigation)
                    .Include(o => o.ПозицииЗаказаs)
                    .ToList();

                var tovars = db.Товарыs.ToList();
                var punkts = db.ПунктыВыдачиs.ToList();
                var indexes = db.Индексыs.ToList();
                var streets = db.Улицыs.ToList();

                var result = new List<OrderDisplay>();

                foreach (var o in orders)
                {
                    string tovarName = "Нет товаров";
                    var firstPos = o.ПозицииЗаказаs.FirstOrDefault();
                    if (firstPos != null)
                    {
                        var tovar = tovars.FirstOrDefault(t => t.IdТовара == firstPos.IdТовара);
                        if (tovar != null) tovarName = tovar.Наименование;
                    }

                    string address = "Не указан";
                    var punkt = punkts.FirstOrDefault(p => p.IdПункта == o.IdПункта);
                    if (punkt != null)
                    {
                        var index = indexes.FirstOrDefault(i => i.IdИндекса == punkt.IdИндекса);
                        if (index != null)
                        {
                            var street = streets.FirstOrDefault(s => s.IdУлицы == index.IdУлицы);
                            if (street != null) address = street.Наименование + ", д. " + punkt.НомерДома;
                        }
                    }

                    string clientFio = "Гость";
                    if (o.IdПользователяNavigation != null)
                    {
                        clientFio = o.IdПользователяNavigation.Фамилия + " " + o.IdПользователяNavigation.Имя;
                    }

                    string statusName = "Неизвестно";
                    if (o.IdСтатусаЗаказаNavigation != null)
                    {
                        statusName = o.IdСтатусаЗаказаNavigation.Наименование;
                    }

                    result.Add(new OrderDisplay
                    {
                        IdЗаказа = o.IdЗаказа,
                        НомерЗаказа = o.НомерЗаказа,
                        ФИОКлиента = clientFio,
                        ТоварАртикул = tovarName,
                        Статус = statusName,
                        АдресПункта = address,
                        // Конвертация DateOnly в DateTime для привязки к элементам управления WPF
                        ДатаЗаказа = o.ДатаЗаказа.ToDateTime(new TimeOnly(0, 0)),
                        ДатаДоставки = o.ДатаДоставки.ToDateTime(new TimeOnly(0, 0))
                    });
                }

                LstOrders.ItemsSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заказов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderEditWindow editWindow = new OrderEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadOrders();
            }
        }

        private void LstOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_roleId != 1) return;

            if (LstOrders.SelectedItem is OrderDisplay selectedOrder)
            {
                OrderEditWindow editWindow = new OrderEditWindow(selectedOrder.IdЗаказа);
                if (editWindow.ShowDialog() == true)
                {
                    LoadOrders();
                }
            }
        }
    }
}