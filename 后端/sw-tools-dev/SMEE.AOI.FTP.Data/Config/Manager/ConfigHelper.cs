using System;
using System.Collections;
using System.IO;
using System.Reflection;
using log4net;
using Newtonsoft.Json;

namespace SMEE.AOI.FTP.Data.Config
{
    internal class ConfigHelper<T> where T : class
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(ConfigHelper<T>));
        private readonly string path;
        private readonly IConfigOperator configOperator;
        private bool updateIsCoverage;

        public ConfigHelper(T configData, IConfigOperator configOperator, bool isUpdate = false, bool updateIsCoverage = false)
        {
            try
            {
                CheckParams(configData, configOperator);
                this.configOperator = configOperator;
                var baseDirectory = AppDomain.CurrentDomain?.BaseDirectory;
                if (baseDirectory == null) throw new Exception(
                    "Get AppDomain.CurrentDomain.BaseDirectory is Null!");
                path = GetConfigFilePathByAttr(configData.GetType(),
                    Directory.GetParent(baseDirectory).FullName);
                if (!File.Exists(path))
                {
                    configOperator.CreateFile(path);
                    InitConfigFile(configData, path);
                }
                else if (isUpdate)
                {
                    this.updateIsCoverage = updateIsCoverage;
                    UpdateConfigFile(configData, path, updateIsCoverage);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Config file read initialization failed, path:{path}", ex);
                throw ex;
            }
        }

        #region Private

        private void CheckParams(T configData, IConfigOperator configOperator)
        {
            if (configData == null)
            {
                throw new Exception("ConfigData is null.");
            }
            if (configOperator == null)
            {
                throw new Exception("ConfigOperator is null.");
            }
        }

        private void InitConfigFile(T configData, string path)
        {
            configOperator.WriteToFile(configData, path);
        }

        private void UpdateConfigFile(T configData, string path, bool isCoverage)
        {
            var configFileData = GetConfigFileData(path);
            if (configFileData == null)
            {
                InitConfigFile(configData, path);
                return;
            }
            if (isCoverage)
            {
                UpdateConfigFileByCoverage(configData, configFileData);
            }
            else
            {
                UpdateConfigFileByNoCoverage(configData, configFileData);
            }
        }

        private void UpdateConfigFileByCoverage(T configData, T configFileData)
        {
            configOperator.WriteToFile(configData, path);
        }

        private void UpdateConfigFileByNoCoverage(T configData, T configFileData)
        {
            try
            {
                TraversalObject(configData, configFileData);
                configOperator.WriteToFile(configData, path);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateConfigFileByNoCoverage failed.", ex);
                BackUpOldFile(path);
                UpdateConfigFileByCoverage(configData, configFileData);
            }
        }

        private void BackUpOldFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            string pathBackUp = GenerateBackUpPath(path);
            File.Copy(path, pathBackUp);
        }

        private string GenerateBackUpPath(string path)
        {
            int dotIndex = path.IndexOf('.');
            string filePathAndName = path.Substring(0, dotIndex);
            string fileType = path.Substring(dotIndex, path.Length - dotIndex);
            return $" {filePathAndName}_BackUp{fileType}";
        }

        private T GetConfigFileData(string path)
        {
            try
            {
                return configOperator.ReadFromFile<T>(path);
            }
            catch (Exception ex)
            {
                logger.Error($"Deserialize read Config failed, path:{path}", ex);
            }
            return null;
        }

        private string GetConfigFilePathByAttr(Type t, string basePath)
        {
            var attrType = typeof(FileDataAttribute);
            if (!t.IsDefined(attrType))
            {
                throw new Exception($"This Class: '{t.FullName}' must be marked with 'FileDataAttribute'.");
            }
            var attr = t.GetCustomAttribute(attrType) as FileDataAttribute;
            return Path.Combine(basePath, attr.RelativePath, attr.FileName);
        }

        private void TraversalObject<T1>(T1 destData, T1 dataSource) where T1 : class
        {
            if (destData == null || dataSource == null)
            {
                return;
            }
            var properties = dataSource.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(destData);
                var valueSource = prop.GetValue(dataSource);
                if (valueSource == null)
                {
                    continue;
                }
                if (value == null)
                {
                    prop.SetValue(destData, valueSource);
                }
                if (IsPrimitive(prop.PropertyType) || !IsSmeeCustomizeType(prop.PropertyType))
                {
                    if (valueSource != null && !valueSource.Equals(value))
                    {
                        prop.SetValue(destData, valueSource);
                    }
                }
                else
                {
                    if (value != null && (value is IList))
                    {
                        var valueArray = value as IList;
                        var valueSourceArray = valueSource as IList;
                        for (int i = 0; i < valueSourceArray.Count; i++)
                        {
                            if (valueSourceArray[i] == null)
                            {
                                continue;
                            }
                            if (i >= valueArray.Count)
                            {
                                valueArray.Add(valueSourceArray[i]);
                                continue;
                            }
                            if (valueArray[i] == null || (valueSourceArray[i] != null && IsPrimitive(valueSourceArray[i].GetType())))
                            {
                                if (!valueSourceArray[i].Equals(valueArray[i]))
                                {
                                    valueArray[i] = valueSourceArray[i];
                                }
                            }
                            else
                            {
                                TraversalObject(valueArray[i], valueSourceArray[i]);
                            }
                        }
                    }
                    else if (value != null && (value is IDictionary))
                    {
                        var valueDic = value as IDictionary;
                        var valueSourceDic = valueSource as IDictionary;
                        foreach (var keySource in valueSourceDic.Keys)
                        {
                            object key = null;
                            if (!IsDicContainsKey(valueDic, keySource, out key))
                            {
                                valueDic.Add(keySource, valueSourceDic[keySource]);
                                continue;
                            }
                            if (valueDic[key] == null || (valueSourceDic[keySource] != null && IsPrimitive(valueSourceDic[keySource].GetType())))
                            {
                                if (!valueSourceDic[keySource].Equals(valueDic[key]))
                                {
                                    valueDic[key] = valueSourceDic[keySource];
                                }
                            }
                            else
                            {
                                TraversalObject(valueDic[key], valueSourceDic[keySource]);
                            }
                        }
                    }
                    else
                    {
                        TraversalObject(value, valueSource);
                    }
                }
            }
        }

        private bool IsPrimitive(Type propertyType)
        {
            if (propertyType.IsPrimitive || propertyType.IsEnum ||
                propertyType.Equals(typeof(decimal)) || propertyType.Equals(typeof(string)) ||
                propertyType.Equals(typeof(decimal)))
            {
                return true;
            }
            return false;
        }

        private bool IsSmeeCustomizeType(Type propertyType)
        {
            return propertyType.Namespace.Contains("SMEE");
        }

        private bool IsDicContainsKey(IDictionary dic, object key, out object key1)
        {
            key1 = null;
            foreach (var item in dic.Keys)
            {
                if (IsEqualsObject(item, key))
                {
                    key1 = item;
                    return true;
                }
            }
            return false;
        }

        private bool IsEqualsObject<T1>(T1 data1, T1 data2)
        {
            if (data1 == null || data2 == null)
            {
                return false;
            }
            string json1 = JsonConvert.SerializeObject(data1);
            string json2 = JsonConvert.SerializeObject(data2);
            if (string.IsNullOrEmpty(json1) || string.IsNullOrEmpty(json2))
            {
                return false;
            }
            return json1.Equals(json2);
        }
        #endregion

        #region Public

        public T ReadConfigData()
        {
            return configOperator.ReadFromFile<T>(path);
        }

        public void WriteConfigData(T configData)
        {
            configOperator.WriteToFile(configData, path);
        }

        public void UpdateConfigFile(T configData)
        {
            UpdateConfigFile(configData, path, updateIsCoverage);
        }
        #endregion
    }
}
