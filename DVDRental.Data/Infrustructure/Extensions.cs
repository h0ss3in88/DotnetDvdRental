using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using Npgsql;

namespace DVDRental.Data.Infrustructure
{
    public static class Extensions
    {
        public static IEnumerable<dynamic> ToListDynamic(this IDataReader reader)
        {
            while (reader.Read())
            {
                yield return reader.ToExpando();
            }
        }
        public static dynamic ToExpando(this IDataReader reader)
        {
            dynamic item = new ExpandoObject();
            var dic = item as IDictionary<string, object>;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var textInfo = new CultureInfo("en-US",false).TextInfo;
                var replacedName = reader.GetName(i).Replace("_", " ").ToLower();
                var name = textInfo.ToTitleCase(replacedName).Replace(" ",string.Empty);
                dic.Add(name,DBNull.Value.Equals(reader.GetValue(i)) ? null : reader.GetValue(i));
            }
            return item;
        }
        public static T ToSingle<T>(this IDataReader reader) where T : new()
        {
            var item = new T();
            var properties = item.GetType().GetProperties();
            foreach (var property in properties)
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (property.Name.Equals(reader.GetName(i),StringComparison.CurrentCultureIgnoreCase))
                    {
                        property.SetValue(item,DBNull.Value.Equals(reader.GetValue(i)) ? null : reader.GetValue(i));
                    }
                }
            }
            return item;
        }

        public static List<T> ToList<T>(this IDataReader reader) where T : new()
        {
            var result = new List<T>();
            while (reader.Read())
            {
               result.Add(reader.ToSingle<T>());
            }
            return result;
        }

        public static void AddParameter(this NpgsqlCommand command, object arg)
        {
            var parameter = new NpgsqlParameter();
            parameter.ParameterName = string.Format("@{0}", command.Parameters.Count);
            if (arg == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                if (arg is string)
                {
                    parameter.Value = arg.ToString();
                    parameter.Size = arg.ToString().Length > 4000 ? -1 : 4000;
                }
                if (arg is Guid)
                {
                    parameter.DbType = DbType.Guid;
                    parameter.Value = arg.ToString();
                    parameter.Size = 4000;
                }
                else
                {
                    parameter.Value = arg;
                }
            }
            command.Parameters.Add(parameter);
        }
    }
}