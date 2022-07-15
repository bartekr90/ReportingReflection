using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReportingReflection
{
    public class CSVGenerator<T>
    {
        private IEnumerable<T> _data;
        private string _filename;
        private Type _type;

        public CSVGenerator(IEnumerable<T> data, string filename)
        {
            _data = data;
            _filename = filename;

            _type = typeof(T);
        }

        public void Generate()
        {
            var rows = new List<string>();

            rows.Add(CreateHeader());

            foreach (var item in _data)
                rows.Add(CreateRow(item));

            File.WriteAllLines($"{_filename}.csv", rows, Encoding.UTF8);
            //GenericTextFileProcessor.SaveToTextFile(rows, $"{_filename}.csv");
            //To jest podobna fukcja, co CSVGenerator, ale z różnicami
        }

        private string CreateHeader()
        {
            var properties = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //zwraca właściwości, ograniczone tylko do publicznych i nie statycznych 
            //można też użyć GetMembers - zostanie wtedy zwrócone wszystki, metody, pola i właściwości 

            var orderedProps = properties.OrderBy(p => p.GetCustomAttribute<ReportItemAttribute>().ColumnOrder);
            //tworzymy nową listę posortowanych właściwości

            var bob = new StringBuilder();

            foreach (var prop in orderedProps)
            {
                var attr = prop.GetCustomAttribute<ReportItemAttribute>();
                //tworzymy obiekt attr typu ReportItemAttribute 

                bob.Append(attr.Heading ?? prop.Name).Append(",");
                //dodajemy nagłowek z obiektu attr, lub jeśli jest to NULL to z prop 
            }

            return bob.ToString()[..^1];
            //sub-range of string, usuwanie przecinka
            //zwraca string z pominięciem ostatniego znaku. Są to zakresy z c# 8.
        }

        private string CreateRow(T item)
        {
            var properties = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var orderedProps = properties.OrderBy(p => p.GetCustomAttribute<ReportItemAttribute>().ColumnOrder);

            var bob = new StringBuilder();

            foreach (var prop in orderedProps)
            {
                bob.Append(CreateItem(prop, item)).Append(",");
                //GetProperties zwraca właściwość klasy, GetValue operuje na obiekcie, więc trzeba do tej metody podać obiekt
                //Jest tu dynamic overloading dla metody CreateItem. Bez tego zawsze zostanie wywołana CreateItem(object item)
            }

            return bob.ToString()[..^1];            
        }

        private string CreateItem(PropertyInfo prop, T item)
        {
            var att = prop.GetCustomAttribute<ReportItemAttribute>();
            //otrzymamy z powrotem instancję ReportItemAttribute dla tej konkretnej właściwości, lub NULL jeśli nie znajdzie.

            return string.Format($"{{0:{att.Format}}}{att.Units}", prop.GetValue(item));
            //uciekamy z interpolacji za pomocą podwójnego {{ i }} 
            //można to też zapisać $"{prop.GetValue(item)}:{att.Format}", 
        }
    }

}
