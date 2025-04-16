using Common;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramwork.Api
{
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public ApiResultStatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
    //اینجوری هم زمانی که دیتا داریم و هم نداریم را کنترل میکنیم 
    public class ApiResult<TData> : ApiResult
       where TData : class
    {
        //ba implicit operation api ra bar migardanim.
        public TData Data { get; set; }

        public static implicit operator ApiResult<TData>(TData value)
        {
            //return new ApiResult<List<TData>>
            return new ApiResult<TData>

            {
                IsSuccess = true,
                StatusCode  =ApiResultStatusCode.Success,
                Message="عملیات با موفقیت انجام شد ",
                //Data = new List<TData> { value }
                Data = value
            };
        }
    }

}
