using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class ПозицииЗаказа
{
    public int IdПозиции { get; set; }

    public int IdЗаказа { get; set; }

    public string IdТовара { get; set; } = null!;

    public int Количество { get; set; }

    public virtual Заказы IdЗаказаNavigation { get; set; } = null!;

    public virtual Товары IdТовараNavigation { get; set; } = null!;
}
