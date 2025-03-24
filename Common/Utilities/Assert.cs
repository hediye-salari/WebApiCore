using System;
using System.Collections;
using System.Linq;

namespace Common.Utilities
{
    public static class Assert//اعتبار سنجی آرگومان های ورودی  
    {
        //where : اعمال محدودیت 
        //نوع آرگومان باید یک نوع مرجع (reference type) باشد.
        public static void NotNull<T>(T obj, string name, string message = null)
            where T : class
        {
            if (obj is null)
                throw new ArgumentNullException($"{name} : {typeof(T)}" , message);
        }
        //نوع آرگومان باید یک نوع مقدار غیر قابل تهی (non-nullable) باشد.
        //از آنجایی که همه انواع مقادیر (value types)، یک سازنده بدون پارامتر قابل دسترسی دارند



        public static void NotNull<T>(T? obj, string name, string message = null)
            where T : struct
        {
            if (!obj.HasValue)
                throw new ArgumentNullException($"{name} : {typeof(T)}", message);

        }

        public static void NotEmpty<T>(T obj, string name, string message = null, T defaultValue = null)
            where T : class
        {
            if (obj == defaultValue
                || (obj is string str && string.IsNullOrWhiteSpace(str))
                || (obj is IEnumerable list && !list.Cast<object>().Any()))
            {
                throw new ArgumentException("Argument is empty : " + message, $"{name} : {typeof(T)}");
            }
        }
    }
}
