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

        public TData Data { get; set; }
    }

        public enum ApiResultStatusCode
    {
        [Display(Name = "عملیات با موفقیت انجام شد")]
        Success = 0,

        [Display(Name = "خطایی در سرور رخ داده است")]
        ServerError = 1,

        [Display(Name = "پارامتر های ارسالی معتبر نیستند")]
        BadRequest = 2,

        [Display(Name = "یافت نشد")]
        NotFound = 3,

        [Display(Name = "لیست خالی است")]
        ListEmpty = 4,

        [Display(Name = "خطایی در پردازش رخ داد")]
        LogicError = 5,

        [Display(Name = "خطای احراز هویت")]
        UnAuthorized = 6
    }
}
