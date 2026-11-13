using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Остатки
{
    public int IdОстатка { get; set; }

    public string IdТовара { get; set; } = null!;

    public int Количество { get; set; }

    public virtual Товары IdТовараNavigation { get; set; } = null!;
}
