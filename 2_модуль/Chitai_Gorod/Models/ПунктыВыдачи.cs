using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class ПунктыВыдачи
{
    public int IdПункта { get; set; }

    public int IdИндекса { get; set; }

    public int НомерДома { get; set; }

    public virtual Индексы IdИндексаNavigation { get; set; } = null!;

    public virtual ICollection<Заказы> Заказыs { get; set; } = new List<Заказы>();
}
