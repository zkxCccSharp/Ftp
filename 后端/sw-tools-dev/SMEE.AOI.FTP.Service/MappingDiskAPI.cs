using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    /// <summary>
    /// 将文件上传至映射盘的API
    /// </summary>
    public static class MappingDiskAPI
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(MappingDiskAPI));

        /// <summary>
        /// 上传文件到本地计算机的映射盘
        /// </summary>
        public static void UploadFileToLocalComputerMappingDisk(
            Action<string> funcOfCreateUserProcess, int waitUploadMS,
            FileInfo sourceFile, FileInfo targetFile, int maxRepeatCount = 5)
        {
            try
            {
                MappingDiskBusiness.ValidateFileExisted(sourceFile);
                MappingDiskBusiness.CopyToMappingDisk(
                    funcOfCreateUserProcess, waitUploadMS,
                    sourceFile, targetFile, maxRepeatCount);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Upload to Local Computer Mapping Disk Failed!");
                throw ex;
            }
        }

    }
}
