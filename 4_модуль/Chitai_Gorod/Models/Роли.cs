using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Роли
{
    public int IdРоли { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Пользователи> Пользователиs { get; set; } = new List<Пользователи>();
}
