using Microsoft.Win32;

namespace MicroscopeGUI
{
    // Small wrapper for the Registry class, so I don't have to specify the KeyName all of the time
    static class RegistryManager
    {
        static readonly string KeyName = @"SOFTWARE\MicroscopeGUI";

        public static void SetValue(string ValName, object Value)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KeyName))
                key.SetValue(ValName, Value);
        }

        static object GetObjVal(string ValName, object DefaultVal)
        {
            // Create SubKey creates a new subkey or opens it if it exists
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KeyName))
                return key.GetValue(ValName, DefaultVal);
        }

        public static string GetStrVal(string ValName) =>
            GetObjVal(ValName, string.Empty).ToString();

        public static int GetIntVal(string ValName) =>
            (int)GetObjVal(ValName, -1);

        public static bool GetBoolVal(string ValName) =>
            bool.Parse(GetObjVal(ValName, true).ToString());
    }
}
