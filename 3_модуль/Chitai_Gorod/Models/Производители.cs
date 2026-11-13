using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Производители
{
    public int IdПроизводителя { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Товары> Товарыs { get; set; } = new List<Товары>();
}
