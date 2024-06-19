using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    public static class MappingDiskBusiness
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(MappingDiskBusiness));

        public static void ValidateFileExisted(FileInfo fileInfo)
        {
            MappingDiskUtil.ValidateObjectNotNull(fileInfo, "testFileInfo");
            if (!File.Exists(fileInfo.FullName))
            {
                throw new Exception(
                    $"File [{fileInfo.FullName}] is NOT Existed!");
            }    
        }

        public static void CopyToMappingDisk(
            Action<string> funcOfCreateUserProcess,
            int waitUploadMS, FileInfo sourceFile,
            FileInfo targetFile, int maxRepeatCount)
        {
            maxRepeatCount = maxRepeatCount < 0 ? 0 : maxRepeatCount;
            if (IsTargetFolderAccessible(targetFile))
            {
                CopyCycling(sourceFile, targetFile, maxRepeatCount);
            }
            else
            {
                CopyCyclingByAnotherProcess(funcOfCreateUserProcess,
                    waitUploadMS, sourceFile, targetFile, maxRepeatCount);
            }
        }

        private static bool IsTargetFolderAccessible(FileInfo targetFile)
        {
            try
            {
                MappingDiskUtil.ValidateObjectNotNull(
                    targetFile, nameof(targetFile));
                var folder = targetFile.Directory.FullName;
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                return Directory.Exists(folder);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Can NOT Access Target Folder!");
                return false;
            }
        }

        public static void CopyCycling(
            FileInfo sourceFile, FileInfo targetFile, int maxRepeatCount)
        {
            for (int i = 0; ; i++)
            {
                if (i > maxRepeatCount)
                {
                    var msg = $"Copy Repeat Count [{i}] > " +
                        $"Max Repeat Count [{maxRepeatCount}]!";
                    logger.Error(msg);
                    throw new Exception(msg);
                }
                try
                {
                    CopyTo(sourceFile, targetFile);
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    logger.Error("Single Copy Failed!");
                }
            }
        }

        private static void CopyCyclingByAnotherProcess(
            Action<string> funcOfCreateUserProcess, int waitUploadMS,
            FileInfo sourceFile, FileInfo targetFile, int maxRepeatCount)
        {
            try
            {
                ProcessCopy.StartCopyProcessIfNotFound(
                    funcOfCreateUserProcess);
                var newCopyTask = ProcessCopy.CreateNewCopyTask(
                    sourceFile, targetFile, maxRepeatCount);
                ProcessCopy.SendCopyTask(newCopyTask, waitUploadMS);
                ProcessCopy.WaitForCopyCompleted(
                    newCopyTask.UID, waitUploadMS);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Repeat Copy By Another Process Failed!");
                throw ex;
            }
        }

        /// <summary>
        /// 映射盘的访问权未知
        /// 使用Exist接口判断是否复制到位可能会产生额外的报错
        /// 出现Exist判断出错时仅打印Warning
        /// 
        /// 调用此方法前，需要确保目标文件夹的存在性
        /// </summary>
        private static void CopyTo(FileInfo sourceFile, FileInfo targetFile)
        {
            MappingDiskUtil.ValidateObjectNotNull(sourceFile, "sourceFileInfo");
            MappingDiskUtil.ValidateObjectNotNull(targetFile, "targetFileInfo");
            File.Copy(sourceFile.FullName, targetFile.FullName, true);
            try
            {
                ValidateFileExisted(targetFile);
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                logger.Warn("Copy File Completed. But Check Existed Failed!");
            }
        }

    }
}
