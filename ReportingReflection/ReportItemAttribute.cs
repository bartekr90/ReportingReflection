using System;

namespace ReportingReflection
{
    [AttributeUsage(AttributeTargets.Property)]
    class ReportItemAttribute : Attribute
    {
        //musimy tutaj zdecydować które cechy będą obowiązkowe, a które opcjonalne
        //obowiązkowe muszą być inicjowane w konstruktorze i będą tylko jako GET
        //opcjonale będą zwykłą właściwością, możemy je ustawiać w konstruktorze, 
        public string Heading { get; set; }
        //właściwość dodatkowa
        public string Units { get; set; }
        public string Format { get; set; }
        public int ColumnOrder { get; }
        //właściwość obowiązkowa
        public ReportItemAttribute(int columnOrder)
        {
            ColumnOrder = columnOrder;
        }
    }
}
