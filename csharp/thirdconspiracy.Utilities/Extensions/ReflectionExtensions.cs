using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace thirdconspiracy.Utilities.Extensions
{

    public static class ReflectionExtensions
    {
        private static readonly BindingFlags[] DEFAULT_FLAGS = new BindingFlags[]
        {
            BindingFlags.Public,
            BindingFlags.Instance,
            BindingFlags.GetProperty,
            BindingFlags.SetField
        };

        // Object <-> Dictionary property bag
        public static Dictionary<string, T> GetAsPropertyValuesDictionaryWithReturnType<T>(this object o, params BindingFlags[] args)
        {
            return o.GetPropertiesWithReturnType<T>(args).ToDictionary(p => p.Name, p => (T)p.GetValue(o));
        }

        public static Dictionary<string, object> GetAsPropertyValuesDictionary(this object o, params BindingFlags[] args)
        {
            return o.GetAllProperties(args).ToDictionary(p => p.Name, p => p.GetValue(o));
        }

        public static T ToObjectWithPropertyValuesWithReturnType<T, R>(this Dictionary<string, R> propertyValues, params BindingFlags[] args)
        {
            return Activator.CreateInstance<T>().SetPropertyValuesWithReturnType(propertyValues, args);
        }

        public static T SetPropertyValuesWithReturnType<T, R>(this T o, Dictionary<string, R> propertyValues, params BindingFlags[] args)
        {
            foreach (var property in o.GetPropertiesWithReturnType<R>(args).Where(p => propertyValues.ContainsKey(p.Name)))
            {
                property.SetValue(o, propertyValues[property.Name]);
            }
            return o;
        }

        public static T ToObjectWithPropertyValues<T>(this Dictionary<string, object> propertyValues, params BindingFlags[] args)
        {
            return Activator.CreateInstance<T>().SetPropertyValues(propertyValues, args);
        }

        public static T SetPropertyValues<T>(this T o, Dictionary<string, object> propertyValues, params BindingFlags[] args)
        {
            foreach (var property in o.GetAllProperties(args).Where(p => propertyValues.ContainsKey(p.Name)))
            {
                property.SetValue(o, propertyValues[property.Name]);
            }
            return o;
        }

        public static T SetFieldValues<T>(this T o, Dictionary<string, object> FieldValues, params BindingFlags[] args)
        {
            var type = o.GetType();

            foreach (var kvp in FieldValues)
            {
                var bindingFlags = GetFilter(args);
                var info = type.GetField( kvp.Key, bindingFlags);

                if (info != null)
                {
                    info.SetValue(o, kvp.Value);
                }
                else
                {
                    var ex = new Exception("Can not find field on object");
                    ex.Data.Add("FieldName", kvp.Key);
                    ex.Data.Add("ObjType", type.FullName);
                }
            }
            return o;
        }


        // OBJECT EXTENSIONS: Properties
        public static IEnumerable<string> GetPropertyNamesWithReturnType<T>(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetPropertyNamesWithReturnType<T>(args);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithReturnType<T>(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetPropertiesWithReturnType<T>(args);
        }

        public static IEnumerable<string> GetAllPropertyNames(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetAllPropertyNames(args);
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetAllProperties(args);
        }

        public static T GetPropertyValue<T>(this object o, string propertyName)
        {
            return (T)(o.GetType().GetProperty(propertyName).GetValue(o));
        }

        // OBJECT EXTENSIONS: Methods
        public static IEnumerable<string> GetMethodNamesWithReturnType<T>(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetMethodNamesWithReturnType<T>(args);
        }

        public static IEnumerable<MethodInfo> GetMethodsWithReturnType<T>(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetMethodsWithReturnType<T>(args);
        }

        public static IEnumerable<string> GetAllMethodNames(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetAllMethodNames(args);
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this object o, params BindingFlags[] args)
        {
            return o.GetType().GetAllMethods(args);
        }

        // TYPE EXTENSIONS: Properties
        public static IEnumerable<string> GetPropertyNamesWithReturnType<T>(this Type t, params BindingFlags[] args)
        {
            return t.GetPropertiesWithReturnType<T>(args).Select(p => p.Name);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithReturnType<T>(this Type t, params BindingFlags[] args)
        {
            return t.GetAllProperties(args).Where(p => p.PropertyType == typeof(T));
        }

        public static IEnumerable<string> GetAllPropertyNames(this Type t, params BindingFlags[] args)
        {
            return t.GetAllProperties(args).Select(p => p.Name);
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type t, params BindingFlags[] args)
        {
            return t.GetProperties(GetFilter(args));
        }

        // TYPE EXTENSIONS: Methods
        public static IEnumerable<string> GetMethodNamesWithReturnType<T>(this Type t, params BindingFlags[] args)
        {
            return t.GetMethodsWithReturnType<T>(args).Select(p => p.Name);
        }

        public static IEnumerable<MethodInfo> GetMethodsWithReturnType<T>(this Type t, params BindingFlags[] args)
        {
            return t.GetAllMethods(args).Where(m => m.ReturnType == typeof(T));
        }

        public static IEnumerable<string> GetAllMethodNames(this Type t, params BindingFlags[] args)
        {
            return t.GetAllMethods(args).Select(m => m.Name);
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this Type t, params BindingFlags[] args)
        {
            return t.GetMethods(/*GetFilter(args)*/);
        }

        #region Private helpers

        static BindingFlags GetFilter(BindingFlags[] args)
        {
            var flagsToUse = args?.Length > 0 ? args : DEFAULT_FLAGS;

            var filter = flagsToUse[0];
            for (var i = 1; i < flagsToUse.Length; i++)
            {
                filter |= flagsToUse[i];
            }

            return filter;
        }

        #endregion
    }
}
