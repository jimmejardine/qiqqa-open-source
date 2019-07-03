using System.Reflection;

namespace Utilities.Reflection
{
    public class COMWrapper
    {
        public static object GetProperty(object com_object, string property_name)
        {
            return com_object.GetType().InvokeMember(property_name, BindingFlags.GetProperty, null, com_object, null);
        }

        public static object GetProperty(object com_object, string property_name, object value)
        {
            object[] parameters = new object[1];
            parameters[0] = value;
            return com_object.GetType().InvokeMember(property_name, BindingFlags.GetProperty, null, com_object, parameters);
        }

        public static object GetProperty(object com_object, string property_name, object value1, object value2)
        {
            object[] parameters = new object[2];
            parameters[0] = value1;
            parameters[0] = value2;
            return com_object.GetType().InvokeMember(property_name, BindingFlags.GetProperty, null, com_object, parameters);
        }

        public static void SetProperty(object com_object, string property_name, object value)
        {
            object[] parameters = new object[1];
            parameters[0] = value;
            com_object.GetType().InvokeMember(property_name, BindingFlags.SetProperty, null, com_object, parameters);
        }

        public static object InvokeMethod(object com_object, string method_name)
        {
            return com_object.GetType().InvokeMember(method_name, BindingFlags.InvokeMethod, null, com_object, null);
        }

        public static object InvokeMethod(object com_object, string method_name, object value)
        {
            object[] oParam = new object[1];
            oParam[0] = value;
            return com_object.GetType().InvokeMember(method_name, BindingFlags.InvokeMethod, null, com_object, oParam);
        }

        public static object InvokeMethod(object com_object, string method_name, object[] parameters)
        {
            return com_object.GetType().InvokeMember(method_name, BindingFlags.InvokeMethod, null, com_object, parameters);
        }
    }
}
