using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class СтатусыЗаказа
{
    public int IdСтатусаЗаказа { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Заказы> Заказыs { get; set; } = new List<Заказы>();
}
