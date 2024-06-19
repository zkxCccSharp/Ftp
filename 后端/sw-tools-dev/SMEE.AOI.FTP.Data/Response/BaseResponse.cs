using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Response
{
    public class BaseResponse<T>
    {
        public int StatusCode { get; set; } = (int)HttpStatusCode.Success;
        public string Error { get; set; }
        public T Body { get; set; }

        public BaseResponse(T body)
        {
            Body = body;
        }
    }
}
