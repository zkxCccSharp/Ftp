using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace SMEE.AOI.FTP.Data.Config
{
    internal class ConfigManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ConfigManager));

        public static ConfigManager Factory
        {
            get
            {
                return new ConfigManager();
            }
        }

        public T ReadXmlConfigData<T>() where T : class
        {
            try
            {
                var instance = Activator.CreateInstance(typeof(T));
                var configHelper = new ConfigHelper<T>((T)instance, new XmlOperator());
                return configHelper.ReadConfigData();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                logger.Error(s);
                return null;
            }
        }

        public T ReadJsonConfigData<T>() where T : class
        {
            try
            {
                var instance = Activator.CreateInstance(typeof(T));
                var configHelper = new ConfigHelper<T>((T)instance, new JsonOperator());
                return configHelper.ReadConfigData();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                logger.Error(s);
                return null;
            }
        }

        public bool WriteXmlConfigData<T>(T configData) where T : class
        {
            try
            {
                var configHelper = new ConfigHelper<T>(configData, new XmlOperator());
                configHelper.WriteConfigData(configData);
                return true;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                logger.Error(s);
                return false;
            }
        }

        public bool WriteJsonConfigData<T>(T configData) where T : class
        {
            try
            {
                var configHelper = new ConfigHelper<T>(configData, new JsonOperator());
                configHelper.WriteConfigData(configData);
                return true;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                logger.Error(s);
                return false;
            }
        }
    }
}
