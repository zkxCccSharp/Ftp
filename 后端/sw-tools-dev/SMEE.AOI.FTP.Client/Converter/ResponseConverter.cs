using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Response;

namespace SMEE.AOI.FTP.Client.Converter
{
    public class ResponseConverter
    {
        internal static UploadDirectoryResponse UploadDirectoryResponseConvert(bool result)
        {
            return new UploadDirectoryResponse()
            {
                IsSuccess = result
            };
        }

        internal static UploadFileResponse UploadFileResponseConvert(bool result)
        {
            return new UploadFileResponse()
            {
                IsSuccess = result
            };
        }

        internal static DownloadDirectoryResponse DownloadDirectoryResponseConvert(bool result)
        {
            return new DownloadDirectoryResponse()
            {
                IsSuccess = result
            };
        }

        internal static DownloadFileResponse DownloadFileResponseConvert(bool result)
        {
            return new DownloadFileResponse()
            {
                IsSuccess = result
            };
        }
    }
}
