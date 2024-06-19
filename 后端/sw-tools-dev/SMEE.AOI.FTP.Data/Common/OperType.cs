using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace SMEE.AOI.FTP.Data.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OperType
    {
        DownloadDirectory,
        DownloadFile,
        UploadDirectory,
        UploadFile
    }
}