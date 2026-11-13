using System;
using System.Collections.Generic;

namespace Chitai_Gorod.Models;

public partial class Пользователи
{
    public int IdПользователЯ { get; set; }

    public int IdРоли { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }

    public string Логин { get; set; } = null!;

    public string Пароль { get; set; } = null!;

    public virtual Роли IdРолиNavigation { get; set; } = null!;

    public virtual ICollection<Заказы> Заказыs { get; set; } = new List<Заказы>();
}
