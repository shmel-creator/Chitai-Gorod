using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class НаселенныеПункты
{
    public int IdНаселенногоПункта { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Улицы> Улицыs { get; set; } = new List<Улицы>();
}
