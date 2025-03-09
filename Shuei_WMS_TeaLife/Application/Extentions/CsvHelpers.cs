using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extentions
{
    public static class CsvHelpers<T> where T : class
    {
        //public static class CsvWriter
        //{
            public static void WriteInfoToCsv(List<T> data, string filePath)
            {
                var properties = typeof(ShippingInfoCSV).GetProperties()
                    .Select(prop => new
                    {
                        Property = prop,
                        Display = prop.GetCustomAttribute<DisplayAttribute>()
                    })
                    .Where(x => x.Display != null)
                    .OrderBy(x => GetOrderFromDisplayName(x.Display.Name))
                    .ToList();

                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    var header = string.Join(",", properties.Select(p => p.Display.Name));
                    writer.WriteLine(header);

                    foreach (var item in data)
                    {
                        var line = new List<string>();
                        foreach (var prop in properties)
                        {
                            var value = prop.Property.GetValue(item);
                            line.Add(FormatCsvValue(value, prop.Property));
                        }
                        writer.WriteLine(string.Join(",", line));
                    }
                }
            }

            private static int GetOrderFromDisplayName(string displayName)
            {
                var orderPart = displayName.Split(':').First();
                return int.Parse(orderPart);
            }

            private static string FormatCsvValue(object value, PropertyInfo property)
            {
                if (value == null)
                    return string.Empty;

                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                return type switch
                {
                    Type _ when type == typeof(DateTime) => ((DateTime)value).ToString("yyyy-MM-dd"),
                    Type _ when type == typeof(bool) => ((bool)value) ? "1" : "0",
                    Type _ when type == typeof(decimal) => ((decimal)value).ToString(CultureInfo.InvariantCulture),
                    Type _ when type == typeof(int) => ((int)value).ToString(CultureInfo.InvariantCulture),
                    _ => EscapeCsvField(value.ToString())
                };
            }

            private static string EscapeCsvField(string value)
            {
                if (string.IsNullOrEmpty(value)) return string.Empty;

                if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n"))
                {
                    return $"\"{value.Replace("\"", "\"\"")}\"";
                }
                return value;
            }

            public static List<T> ReadFromCsv<T>(string filePath) where T : new()
            {
                var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                if (lines.Length == 0) return new List<T>();

                // Get properties in order they are defined in the class
                var properties = typeof(T).GetProperties().Take(3).ToList();

                var result = new List<T>();
                foreach (var line in lines.Skip(1)) // Skip header row
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = SplitCsvLine(line);
                    var obj = new T();

                    // Map values to properties in order
                    for (int i = 0; i < Math.Min(values.Count, properties.Count); i++)
                    {
                        var prop = properties[i];
                        var valueStr = values[i].Trim();
                        
                        try
                        {
                            SetPropertyValue(obj, prop, valueStr);
                        }
                        catch (Exception ex)
                        {
                            // Log error and continue with next field
                            Console.WriteLine($"Error setting value '{valueStr}' for property {prop.Name}: {ex.Message}");
                            continue;
                        }
                    }

                    result.Add(obj);
                }

                return result;
            }

            // Kiểm tra kiểu nullable
            private static bool IsNullable(Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            // Tách dòng CSV thành các giá trị (xử lý quoted fields)
            private static List<string> SplitCsvLine(string line)
            {
                var result = new List<string>();
                var current = new StringBuilder();
                bool inQuotes = false;

                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (c == '"')
                    {
                        if (inQuotes && i < line.Length - 1 && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++; // Bỏ qua ký tự quote tiếp theo
                        }
                        else
                        {
                            inQuotes = !inQuotes;
                        }
                    }
                    else if (c == ',' && !inQuotes)
                    {
                        result.Add(current.ToString());
                        current.Clear();
                    }
                    else
                    {
                        current.Append(c);
                    }
                }

                result.Add(current.ToString());
                return result;
            }

            // Phân tích giá trị và gán vào property
            private static void SetPropertyValue<T>(T obj, PropertyInfo prop, string valueStr)
            {
                if (string.IsNullOrEmpty(valueStr))
                {
                    if (IsNullable(prop.PropertyType))
                        prop.SetValue(obj, null);
                    return;
                }

                try
                {
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    object value = type switch
                    {
                        Type _ when type == typeof(bool) => valueStr == "1", // Xử lý bool
                        Type _ when type == typeof(DateTime) => DateTime.ParseExact(valueStr, "yyyy-MM-dd", null),
                        Type _ when type == typeof(decimal) => decimal.Parse(valueStr, CultureInfo.InvariantCulture),
                        Type _ when type == typeof(int) => int.Parse(valueStr, CultureInfo.InvariantCulture),
                        Type _ when type == typeof(string) => valueStr.Trim('"').Replace("\"\"", "\""), // Unescape quotes
                        _ => Convert.ChangeType(valueStr, type, CultureInfo.InvariantCulture)
                    };
                    prop.SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Lỗi chuyển đổi giá trị '{valueStr}' sang kiểu {prop.PropertyType.Name} cho trường {prop.Name}", ex);
                }
            }
        }
    //}
}
