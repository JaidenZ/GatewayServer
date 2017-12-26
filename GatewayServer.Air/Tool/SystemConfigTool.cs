namespace GatewayServer.Air.Tool
{
    using GatewayServer.Air.Configuration;
    using GatewayServer.Air.Model;
    using System;
    using System.Reflection;

    public class SystemConfigTool
    {
        private static string path = AppDomain.CurrentDomain.BaseDirectory + "gatewayConfig.ini";
        private static GatewayConfig config = null;

        public static GatewayConfig Current
        {
            get
            {
                if (config == null)
                    config = GetConfig();
                return config;
            }
        }

        private static GatewayConfig GetConfig()
        {
            GatewayConfig result = new GatewayConfig();
            INIDocument doc = new INIDocument(path);
            doc.Load();
            result = new SystemConfigTool().GetValue(doc);
            return result;
        }

        private GatewayConfig GetValue(INIDocument doc)
        {
            GatewayConfig config = new GatewayConfig();
            doc.Load();
            Type clazz = config.GetType();
            foreach (INISection section in doc.Sections)
            {
                foreach (INIKey key in section.Keys)
                {
                    PropertyInfo pi = clazz.GetProperty(key.Name);
                    object value = key.Value;
                    if (pi.PropertyType == typeof(int))
                        value = Convert.ToInt32(value);
                    if (pi != null && !pi.PropertyType.IsGenericType)
                    {
                        pi.SetValue(config, value, null);
                    }
                }
            }
            return config;
        }

    }
}
