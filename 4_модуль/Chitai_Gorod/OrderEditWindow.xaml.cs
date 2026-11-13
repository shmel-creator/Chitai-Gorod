using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Chitai_Gorod.Models;

namespace Chitai_Gorod
{
    public partial class OrderEditWindow : Window
    {
        private DBContext db = new DBContext();
        // Используем 0 вместо null для режима добавления
        private int _orderId = 0;

        // Конструктор: по умолчанию 0 (добавление)
        public OrderEditWindow(int orderId = 0)
        {
            InitializeComponent();
            _orderId = orderId;

            LoadComboBoxes();

            if (_orderId == 0)
            {
                TbTitle.Text = "Добавление заказа";
                BtnDelete.Visibility = Visibility.Collapsed;
            }
            else
            {
                TbTitle.Text = "Редактирование заказа";
                BtnDelete.Visibility = Visibility.Visible;
                LoadOrderData();
            }
        }

        private void LoadComboBoxes()
        {
            CbTovar.ItemsSource = db.Товарыs.ToList();
            CbStatus.ItemsSource = db.СтатусыЗаказаs.ToList();

            var punkts = db.ПунктыВыдачиs.ToList();
            var indexes = db.Индексыs.ToList();
            var streets = db.Улицыs.ToList();

            var punktList = new List<PunktDisplay>();
            foreach (var p in punkts)
            {
                string addr = "Неизвестно";
                var index = indexes.FirstOrDefault(i => i.IdИндекса == p.IdИндекса);
                if (index != null)
                {
                    var street = streets.FirstOrDefault(s => s.IdУлицы == index.IdУлицы);
                    if (street != null) addr = street.Наименование + ", д. " + p.НомерДома;
                }
                punktList.Add(new PunktDisplay { IdПункта = p.IdПункта, Адрес = addr });
            }

            CbPunkt.ItemsSource = punktList;
        }

        private void LoadOrderData()
        {
            var order = db.Заказыs
                .Include(o => o.ПозицииЗаказаs)
                .FirstOrDefault(o => o.IdЗаказа == _orderId);

            if (order == null)
            {
                MessageBox.Show("Заказ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            if (order.ПозицииЗаказаs.Count > 0)
                CbTovar.SelectedValue = order.ПозицииЗаказаs.First().IdТовара;

            CbStatus.SelectedValue = order.IdСтатусаЗаказа;
            CbPunkt.SelectedValue = order.IdПункта;

            DpDateOrder.SelectedDate = order.ДатаЗаказа.ToDateTime(new TimeOnly(0, 0));
            DpDateDelivery.SelectedDate = order.ДатаДоставки.ToDateTime(new TimeOnly(0, 0));
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CbTovar.SelectedItem == null || CbStatus.SelectedItem == null ||
                CbPunkt.SelectedItem == null || DpDateOrder.SelectedDate == null || DpDateDelivery.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_orderId == 0) // Если 0, значит добавляем
                {
                    int maxNumber = 0;
                    if (db.Заказыs.Any()) maxNumber = db.Заказыs.Max(o => o.НомерЗаказа);

                    int code = new Random().Next(100, 999);

                    var firstUser = db.Пользователиs.FirstOrDefault();
                    int userId = 1;
                    if (firstUser != null) userId = firstUser.IdПользователЯ;

                    var newOrder = new Заказы
                    {
                        НомерЗаказа = maxNumber + 1,
                        IdПользователя = userId,
                        IdСтатусаЗаказа = (int)CbStatus.SelectedValue,
                        IdПункта = (int)CbPunkt.SelectedValue,
                        ДатаЗаказа = DateOnly.FromDateTime(DpDateOrder.SelectedDate.Value),
                        ДатаДоставки = DateOnly.FromDateTime(DpDateDelivery.SelectedDate.Value),
                        Количество = 1,
                        КодДляПолучения = code
                    };

                    db.Заказыs.Add(newOrder);
                    db.SaveChanges();

                    var newPos = new ПозицииЗаказа
                    {
                        IdЗаказа = newOrder.IdЗаказа,
                        IdТовара = CbTovar.SelectedValue.ToString(),
                        Количество = 1
                    };
                    db.ПозицииЗаказаs.Add(newPos);
                }
                else // Иначе редактируем
                {
                    var order = db.Заказыs.FirstOrDefault(o => o.IdЗаказа == _orderId);
                    if (order != null)
                    {
                        order.IdСтатусаЗаказа = (int)CbStatus.SelectedValue;
                        order.IdПункта = (int)CbPunkt.SelectedValue;
                        order.ДатаЗаказа = DateOnly.FromDateTime(DpDateOrder.SelectedDate.Value);
                        order.ДатаДоставки = DateOnly.FromDateTime(DpDateDelivery.SelectedDate.Value);
                    }
                }

                db.SaveChanges();
                MessageBox.Show("Заказ сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_orderId == 0) return; // Если 0, удалять нечего (новый заказ)

            var result = MessageBox.Show("Вы уверены, что хотите удалить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var positions = db.ПозицииЗаказаs.Where(p => p.IdЗаказа == _orderId).ToList();
                    db.ПозицииЗаказаs.RemoveRange(positions);

                    var order = db.Заказыs.FirstOrDefault(o => o.IdЗаказа == _orderId);
                    if (order != null) db.Заказыs.Remove(order);

                    db.SaveChanges();
                    MessageBox.Show("Заказ удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}