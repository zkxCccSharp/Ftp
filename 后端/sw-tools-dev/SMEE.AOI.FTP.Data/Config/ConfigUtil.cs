using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Config
{
    public class ConfigUtil
    {
        /// <summary>
        /// 获取单数据结构的Xml配置文件的数据，注：配置文件数据类必须标记FileData特性，并声明其配置文件名及相对程序根目录的路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadXmlConfigData<T>() where T : class
        {
            return ConfigManager.Factory.ReadXmlConfigData<T>();
        }

        /// <summary>
        /// 获取单数据结构的Json配置文件的数据，注：配置文件数据类必须标记FileData特性，并声明其配置文件名及相对程序根目录的路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadJsonConfigData<T>() where T : class
        {
            return ConfigManager.Factory.ReadJsonConfigData<T>();
        }
        /// <summary>
        /// 将数据写入XML文件，注：数据类必须标记FileData特性，并声明其配置文件名及相对程序根目录的路径
        /// </summary>
        /// <param name="configData"></param>
        /// <returns></returns>
        public static bool WriteXmlConfigData<T>(T configData) where T : class
        {
            return ConfigManager.Factory.WriteXmlConfigData(configData);
        }
        /// <summary>
        /// 将数据写入Json文件，注：数据类必须标记FileData特性，并声明其配置文件名及相对程序根目录的路径
        /// </summary>
        /// <param name="configData"></param>
        /// <returns></returns>
        public static bool WriteJsonConfigData<T>(T configData) where T : class
        {
            return ConfigManager.Factory.WriteJsonConfigData(configData);
        }
    }
}
