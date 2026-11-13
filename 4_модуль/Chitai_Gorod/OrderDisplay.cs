using System;

namespace Chitai_Gorod
{
    public class OrderDisplay
    {
        public int IdЗаказа { get; set; }
        public int НомерЗаказа { get; set; }
        public string ТоварАртикул { get; set; }
        public string Статус { get; set; }
        public string АдресПункта { get; set; }
        public DateTime ДатаЗаказа { get; set; }
        public DateTime ДатаДоставки { get; set; }
        public int КоличествоТоваров { get; set; }
        public string ФИОКлиента { get; set; }
    }

    public class PunktDisplay
    {
        public int IdПункта { get; set; }
        public string Адрес { get; set; }
    }
}