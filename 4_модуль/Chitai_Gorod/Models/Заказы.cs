using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Заказы
{
    public int IdЗаказа { get; set; }

    public int IdПункта { get; set; }

    public int IdПользователя { get; set; }

    public int IdСтатусаЗаказа { get; set; }

    public int НомерЗаказа { get; set; }

    public DateOnly ДатаЗаказа { get; set; }

    public DateOnly ДатаДоставки { get; set; }

    public int Количество { get; set; }

    public int КодДляПолучения { get; set; }

    public virtual Пользователи IdПользователяNavigation { get; set; } = null!;

    public virtual ПунктыВыдачи IdПунктаNavigation { get; set; } = null!;

    public virtual СтатусыЗаказа IdСтатусаЗаказаNavigation { get; set; } = null!;

    public virtual ICollection<ПозицииЗаказа> ПозицииЗаказаs { get; set; } = new List<ПозицииЗаказа>();
}
