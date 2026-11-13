using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Товары
{
    public string IdТовара { get; set; } = null!;

    public int IdЕдиницыИзмерения { get; set; }

    public int IdПоставщика { get; set; }

    public int IdПроизводителя { get; set; }

    public int IdКатегории { get; set; }

    public string Наименование { get; set; } = null!;

    public decimal Цена { get; set; }

    public int Скидка { get; set; }

    public string? Описание { get; set; }

    public string Фото { get; set; } = null!;

    public virtual ЕдиницыИзмерения IdЕдиницыИзмеренияNavigation { get; set; } = null!;

    public virtual Категории IdКатегорииNavigation { get; set; } = null!;

    public virtual Поставщики IdПоставщикаNavigation { get; set; } = null!;

    public virtual Производители IdПроизводителяNavigation { get; set; } = null!;

    public virtual ICollection<Остатки> Остаткиs { get; set; } = new List<Остатки>();

    public virtual ICollection<ПозицииЗаказа> ПозицииЗаказаs { get; set; } = new List<ПозицииЗаказа>();
}
