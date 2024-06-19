using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMEE.AOI.FTP.Service;

namespace SMEE.AOI.FTP.UT
{
    [TestClass]
    public class UTOfMappingDiskAPI
    {
        [TestMethod]
        public void TestUploadIsUpload()
        {
            MappingDiskAPI.UploadFileToLocalComputerMappingDisk(null, 500,
                new FileInfo(@"D:\wangtx3870\_\1q23e4r5t.txt"),
                new FileInfo(@"E:\wangtx3870\_\23e4r5t.txt"));
        }
    }
}
