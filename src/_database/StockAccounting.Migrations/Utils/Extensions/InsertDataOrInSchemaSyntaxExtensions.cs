using FluentMigrator.Builders.Insert;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace StockAccounting.Migrations.Utils.Extensions
{
    public static class InsertDataOrInSchemaSyntaxExtensions
    {
        public static IInsertDataSyntax Rows<T>(this IInsertDataOrInSchemaSyntax @this, IEnumerable<T> rows)
        {
            var props = GetProps<T>();
            foreach (var row in rows)
            {
                var dictionary = new Dictionary<string, object?>(props.Count);
                foreach (var keyValue in props)
                {
                    var value = GetValue(row, keyValue.Value);
                    dictionary.Add(keyValue.Key, value);
                }

                @this.Row(dictionary);
            }

            return @this;
        }
        private static IReadOnlyDictionary<string, PropertyInfo> GetProps<T>()
        {
            var props = typeof(T).GetProperties();
            var dictionary = new Dictionary<string, PropertyInfo>();

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<ColumnAttribute>() ?? throw new InvalidOperationException($"`{nameof(ColumnAttribute)}` is not defined on `{typeof(T).FullName}`");
                dictionary.Add(attr.Name ?? prop.Name, prop);
            }

            return dictionary;
        }

        private static object? GetValue<T>(this T @this, PropertyInfo prop)
        {
            var value = prop.GetValue(@this);
            if (prop.PropertyType.IsEnum)
                value = Convert.ChangeType(value, Enum.GetUnderlyingType(prop.PropertyType));

            return value;
        }
    }
}
