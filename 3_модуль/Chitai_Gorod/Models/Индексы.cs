using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Индексы
{
    public int IdИндекса { get; set; }

    public int IdУлицы { get; set; }

    public int ПочтовыйИндекс { get; set; }

    public virtual Улицы IdУлицыNavigation { get; set; } = null!;

    public virtual ICollection<ПунктыВыдачи> ПунктыВыдачиs { get; set; } = new List<ПунктыВыдачи>();
}
