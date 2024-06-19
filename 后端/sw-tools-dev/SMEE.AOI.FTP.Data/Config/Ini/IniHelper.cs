using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using SMEE.AOI.FTP.Data.ConfigModel;

namespace SMEE.AOI.FTP.Data.Config
{
    public class IniHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(IniHelper));
        private static readonly string path;
        private static readonly uint defalutSectionBuffSize = 1024;
        private static readonly uint defalutItemsBuffSize = 65535;
        private static readonly int defalutItemBuffSize = 1024;
        private static readonly string headerStr = "Header";
        private static readonly string versionStr = "VersionNo";
        private static readonly string updateStrategyStr = "UpdateStrategy";

        static IniHelper()
        {
            try
            {
                var baseDirectory = AppDomain.CurrentDomain?.BaseDirectory;
                if (baseDirectory == null) throw new Exception(
                    "Get AppDomain.CurrentDomain.BaseDirectory is Null!");
                path = GetConfigFilePathByAttr(typeof(IniConfigData),
                    Directory.GetParent(baseDirectory).FullName);
                if (!File.Exists(path))
                {
                    IniOperator.CreateFile(path);
                    InitIniFile(path);
                }
                else
                {
                    UpdateIniFile(path);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Ini file read initialization failed, path:{path}", ex);
                string errMsg = IniOperator.GetLastErrorMsg();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    throw new Exception(errMsg, ex);
                }
                throw ex;
            }
        }

        #region private

        private static string GetConfigFilePathByAttr(Type t, string basePath)
        {
            var attrType = typeof(FileDataAttribute);
            if (!t.IsDefined(attrType))
            {
                throw new Exception($"This Class: '{t.FullName}' must be marked with 'FileDataAttribute'.");
            }
            var attr = t.GetCustomAttribute<FileDataAttribute>();
            return Path.Combine(basePath, attr.RelativePath, attr.FileName);
        }

        private static void InitIniFile(string path)
        {
            var dicInitAllSectionAndItems = GetIniInitData<IniConfigData>();
            foreach (var section in dicInitAllSectionAndItems)
            {
                foreach (var item in section.Value)
                {
                    IniOperator.WriteIniValue(section.Key, item.Key, item.Value, path);
                }
            }
        }

        private static void UpdateIniFile(string path)
        {
            var dicInitAllSectionAndItems = GetIniInitData<IniConfigData>();
            string currVersion = GetVersion(dicInitAllSectionAndItems);
            string fileVersion = GetVersionFromFile(path);
            if (!fileVersion.Equals(currVersion))
            {
                string updateStrategy = GetUpdateStrategyFromFile(path);
                if ("1".Equals(updateStrategy))
                {
                    UpdateIniFileByCoverage(dicInitAllSectionAndItems);
                }
                else
                {
                    UpdateIniFileByNoCoverage(dicInitAllSectionAndItems);
                }
                UpdateVersionToFile(currVersion, path);
            }
        }

        private static void UpdateIniFileByCoverage(Dictionary<string, Dictionary<string, string>> dicInitAllSectionAndItems)
        {
            foreach (var section in dicInitAllSectionAndItems)
            {
                foreach (var item in section.Value)
                {
                    IniOperator.WriteIniValue(section.Key, item.Key, item.Value, path);
                }
            }
        }

        private static void UpdateIniFileByNoCoverage(Dictionary<string, Dictionary<string, string>> dicInitAllSectionAndItems)
        {
            var dicFileAllSectionAndItems = ReadAllDataFromFile();
            foreach (var section in dicInitAllSectionAndItems)
            {
                if (!dicFileAllSectionAndItems.ContainsKey(section.Key))
                {
                    foreach (var item in section.Value)
                    {
                        IniOperator.WriteIniValue(section.Key, item.Key, item.Value, path);
                    }
                }
                else
                {
                    foreach (var item in section.Value)
                    {
                        if (!dicFileAllSectionAndItems[section.Key].ContainsKey(item.Key))
                        {
                            IniOperator.WriteIniValue(section.Key, item.Key, item.Value, path);
                        }
                    }
                }
            }
        }

        private static string GetUpdateStrategyFromFile(string path)
        {
            return IniOperator.ReadIniValue(headerStr, updateStrategyStr, "0", defalutItemBuffSize, path);
        }

        private static string GetVersionFromFile(string path)
        {
            return IniOperator.ReadIniValue(headerStr, versionStr, "", defalutItemBuffSize, path);
        }

        private static void UpdateVersionToFile(string version, string path)
        {
            IniOperator.WriteIniValue(headerStr, versionStr, version, path);
        }

        private static string GetVersion(Dictionary<string, Dictionary<string, string>> dicInitAllSectionAndItems)
        {
            if (!dicInitAllSectionAndItems.ContainsKey(headerStr) || !dicInitAllSectionAndItems[headerStr].ContainsKey(versionStr))
            {
                return string.Empty;
            }
            return dicInitAllSectionAndItems[headerStr][versionStr];
        }

        private static Dictionary<string, Dictionary<string, string>> GetIniInitData<T>()
        {
            return ReflectHelper.GetNestedTypesFields(new IniConfigData());
        }

        private static Dictionary<string, Dictionary<string, string>> ReadAllDataFromFile()
        {
            try
            {
                var dicAllSectionAndItems = new Dictionary<string, Dictionary<string, string>>();
                string[] sections = IniOperator.ReadIniSections(path, defalutSectionBuffSize);
                foreach (var section in sections)
                {
                    string[] items = IniOperator.ReadIniItemsBySection(section, path, defalutItemsBuffSize);
                    var dicKeyAndValue = ConvertItemArrayToDictionary(items);
                    if (!dicAllSectionAndItems.ContainsKey(section))
                    {
                        dicAllSectionAndItems.Add(section, dicKeyAndValue);
                    }
                }
                return dicAllSectionAndItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static Dictionary<string, string> ConvertItemArrayToDictionary(string[] items)
        {
            var dicKeyAndValue = new Dictionary<string, string>();
            foreach (var item in items)
            {
                var itemStr = item.Trim();
                int spiltIndex = itemStr.IndexOf('=');
                if (spiltIndex <= 0)
                {
                    continue;
                }
                string key = itemStr.Substring(0, spiltIndex);
                string value = string.Empty;
                if (spiltIndex + 1 < itemStr.Length)
                {
                    value = itemStr.Substring(spiltIndex + 1, itemStr.Length - 1 - spiltIndex);
                }
                if (!dicKeyAndValue.ContainsKey(key))
                {
                    dicKeyAndValue.Add(key, value);
                }
            }
            return dicKeyAndValue;
        }

        #endregion

        #region Public

        public static string ReadValueFromIni(string section, string key, string defaultValue)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new Exception("Ini file does not exist.");
                }
                if (string.IsNullOrWhiteSpace(section))
                {
                    throw new Exception("Section is null or emtry.");
                }
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new Exception("Key is null or emtry.");
                }
                section = section.Trim();
                key = key.Trim();
                return IniOperator.ReadIniValue(section, key, defaultValue, defalutItemBuffSize, path).Trim();
            }
            catch (Exception ex)
            {
                logger.Error($"Read ini config failed, section:{section} key:{key} path:{path}", ex);
                return defaultValue;
            }
        }

        public static bool SaveValueToIni(string section, string key, string value)
        {
            try
            {
                return IniOperator.WriteIniValue(section, key, value, path);
            }
            catch (Exception ex)
            {
                logger.Error($"Save ini config failed, section:{section} key:{key} path:{path}", ex);
                return false;
            }
        }

        #endregion
    }
}

