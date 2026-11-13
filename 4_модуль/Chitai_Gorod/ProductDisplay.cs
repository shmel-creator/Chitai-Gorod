using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Chitai_Gorod
{
    public class ProductDisplay
    {
        public string IdТовара { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Supplier { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string ImageUri { get; set; }

        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public int Discount { get; set; }
        public int TotalQuantity { get; set; }

        // Вычисляемые свойства для DataTrigger в XAML
        public bool HasDiscount => Discount > 0;
        public bool IsBigDiscount => Discount > 25;
        public bool IsOutOfStock => TotalQuantity <= 0;
    }
}