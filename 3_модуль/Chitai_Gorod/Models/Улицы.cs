using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Улицы
{
    public int IdУлицы { get; set; }

    public int IdНаселенногоПункта { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual НаселенныеПункты IdНаселенногоПунктаNavigation { get; set; } = null!;

    public virtual ICollection<Индексы> Индексыs { get; set; } = new List<Индексы>();
}
