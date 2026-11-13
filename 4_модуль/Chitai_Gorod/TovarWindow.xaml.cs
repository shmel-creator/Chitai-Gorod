using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Chitai_Gorod.Models;
using System.IO;

namespace Chitai_Gorod
{
    public partial class TovarWindow : Window
    {
        private DBContext db = new DBContext();
        private string _tovarId = null;
        private bool _isReadOnly = false;

        public TovarWindow(string tovarId = null, bool isReadOnly = false)
        {
            InitializeComponent();
            _tovarId = tovarId;
            _isReadOnly = isReadOnly;

            LoadComboBoxes();

            if (string.IsNullOrEmpty(_tovarId))
            {
                TbTitle.Text = "Добавление товара";
                TbId.Text = "Автоматически";
                BtnDeleteTovar.Visibility = Visibility.Collapsed;
                TbPhotoPath.Text = "picture.png";
                try
                {
                    ImgPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/picture.png"));
                }
                catch { }
            }
            else
            {
                TbTitle.Text = _isReadOnly ? "Просмотр товара" : "Редактирование товара";
                TbId.Text = _tovarId;
                LoadTovarData();

                if (!_isReadOnly)
                {
                    BtnDeleteTovar.Visibility = Visibility.Visible;
                }
            }

            // Если окно открыто в режиме просмотра (для Менеджера), блокируем редактирование
            if (_isReadOnly)
            {
                BtnSave.Visibility = Visibility.Collapsed;
                TbName.IsReadOnly = true;
                TbDescription.IsReadOnly = true;
                TbPrice.IsReadOnly = true;
                TbDiscount.IsReadOnly = true;
                TbQuantity.IsReadOnly = true;
                CbCategory.IsEnabled = false;
                CbManufacturer.IsEnabled = false;
                CbSupplier.IsEnabled = false;
                CbUnit.IsEnabled = false;
                BtnLoadPhoto.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadComboBoxes()
        {
            CbCategory.ItemsSource = db.Категорииs.ToList();
            CbManufacturer.ItemsSource = db.Производителиs.ToList();
            CbSupplier.ItemsSource = db.Поставщикиs.ToList();
            CbUnit.ItemsSource = db.ЕдиницыИзмеренияs.ToList();
        }

        private void LoadTovarData()
        {
            var tovar = db.Товарыs
                .Include(t => t.IdКатегорииNavigation)
                .Include(t => t.IdПроизводителяNavigation)
                .Include(t => t.IdПоставщикаNavigation)
                .Include(t => t.IdЕдиницыИзмеренияNavigation)
                .FirstOrDefault(t => t.IdТовара == _tovarId);

            if (tovar == null)
            {
                MessageBox.Show("Товар не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            TbName.Text = tovar.Наименование;
            TbDescription.Text = tovar.Описание;
            TbPrice.Text = tovar.Цена.ToString("F2");
            TbDiscount.Text = tovar.Скидка.ToString();
            TbPhotoPath.Text = tovar.Фото;

            CbCategory.SelectedValue = Convert.ToInt32(tovar.IdКатегории);
            CbManufacturer.SelectedValue = Convert.ToInt32(tovar.IdПроизводителя);
            CbSupplier.SelectedValue = Convert.ToInt32(tovar.IdПоставщика);
            CbUnit.SelectedValue = Convert.ToInt32(tovar.IdЕдиницыИзмерения);

            if (!string.IsNullOrWhiteSpace(tovar.Фото))
            {
                try
                {
                    ImgPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri($"pack://application:,,,/import/{tovar.Фото}"));
                }
                catch { }
            }

            var qty = db.Остаткиs.Where(o => o.IdТовара == _tovarId).Sum(o => o.Количество);
            TbQuantity.Text = qty.ToString();
        }

        private void TbPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0) && e.Text != "," && e.Text != ".";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text) ||
                string.IsNullOrWhiteSpace(TbPrice.Text) ||
                CbCategory.SelectedItem == null ||
                CbManufacturer.SelectedItem == null ||
                CbSupplier.SelectedItem == null ||
                CbUnit.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                decimal price = Convert.ToDecimal(TbPrice.Text.Replace('.', ','));
                int discount = Convert.ToInt32(TbDiscount.Text);
                int quantity = Convert.ToInt32(TbQuantity.Text);

                if (price < 0) throw new Exception("Цена не может быть отрицательной.");
                if (discount < 0 || discount > 100) throw new Exception("Скидка должна быть от 0 до 100.");
                if (quantity < 0) throw new Exception("Количество не может быть отрицательным.");

                int categoryId = Convert.ToInt32(CbCategory.SelectedValue);
                int manufacturerId = Convert.ToInt32(CbManufacturer.SelectedValue);
                int supplierId = Convert.ToInt32(CbSupplier.SelectedValue);
                int unitId = Convert.ToInt32(CbUnit.SelectedValue);

                if (string.IsNullOrEmpty(_tovarId))
                {
                    string newId = "T" + new Random().Next(1000, 9999).ToString();

                    var newTovar = new Товары
                    {
                        IdТовара = newId,
                        Наименование = TbName.Text,
                        Описание = TbDescription.Text,
                        Цена = price,
                        Скидка = discount,
                        Фото = TbPhotoPath.Text,
                        IdКатегории = categoryId,
                        IdПроизводителя = manufacturerId,
                        IdПоставщика = supplierId,
                        IdЕдиницыИзмерения = unitId
                    };
                    db.Товарыs.Add(newTovar);
                    db.Остаткиs.Add(new Остатки { IdТовара = newId, Количество = quantity });
                }
                else
                {
                    var tovar = db.Товарыs.FirstOrDefault(t => t.IdТовара == _tovarId);
                    if (tovar != null)
                    {
                        tovar.Наименование = TbName.Text;
                        tovar.Описание = TbDescription.Text;
                        tovar.Цена = price;
                        tovar.Скидка = discount;
                        tovar.Фото = TbPhotoPath.Text;
                        tovar.IdКатегории = categoryId;
                        tovar.IdПроизводителя = manufacturerId;
                        tovar.IdПоставщика = supplierId;
                        tovar.IdЕдиницыИзмерения = unitId;

                        var ostatok = db.Остаткиs.FirstOrDefault(o => o.IdТовара == _tovarId);
                        if (ostatok != null)
                            ostatok.Количество = quantity;
                        else
                            db.Остаткиs.Add(new Остатки { IdТовара = _tovarId, Количество = quantity });
                    }
                }

                db.SaveChanges();
                MessageBox.Show("Сохранено успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Проверьте правильность ввода чисел (цена, скидка, количество).", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteTovar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_tovarId)) return;

            // Проверка ссылочной целостности: нельзя удалить товар, если на него ссылаются в заказах
            bool isInOrders = db.ПозицииЗаказаs.Any(p => p.IdТовара == _tovarId);
            if (isInOrders)
            {
                MessageBox.Show("Невозможно удалить товар, так как он присутствует в заказах!",
                                "Запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот товар?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Сначала удаляем связанные записи из таблицы остатков
                    var ostatki = db.Остаткиs.Where(o => o.IdТовара == _tovarId).ToList();
                    if (ostatki.Any())
                    {
                        db.Остаткиs.RemoveRange(ostatki);
                    }

                    var tovar = db.Товарыs.FirstOrDefault(t => t.IdТовара == _tovarId);
                    if (tovar != null)
                    {
                        db.Товарыs.Remove(tovar);
                    }

                    db.SaveChanges();
                    MessageBox.Show("Товар успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnLoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == true)
            {
                // В учебном проекте сохраняется только имя файла, без копирования в папку приложения
                TbPhotoPath.Text = Path.GetFileName(ofd.FileName);
                MessageBox.Show("В этом примере мы просто запоминаем имя файла.", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}