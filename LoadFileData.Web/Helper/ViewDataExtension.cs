using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoadFileData.Web.Helper
{
    public static class ViewDataExtension
    {
        public static void DisplayContext<T>(this ViewDataDictionary viewData, T instance, string key = null)
        {
            var keyValue = (string.IsNullOrEmpty(key)) ? typeof(T).FullName : typeof(T).FullName + "." + key;
            viewData[keyValue] = instance;
        }
        public static T DisplayContext<T>(this ViewDataDictionary viewData, string key = null)
        {
            var keyValue = (string.IsNullOrEmpty(key)) ? typeof(T).FullName : typeof(T).FullName + "." + key;
            return (T) viewData.Eval(keyValue);
        }
    }
}