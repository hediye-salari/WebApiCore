using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    //هر کلاسی که از این اینتیتی ترث بری کرده باشه ما به ازاش یه جدول میسازیم در دیتا بیس 
    //این اینترفیس بدون پیاده سازی هست و در اصل برای مارک کردن استفاده میشه 
    //سناریو : کلاسی داریم که میخوایم ازش ارث بری داشته باشیم ولی نمیخواهیم از یک پراپرتی که در آن است استفاده کنیم مثل آیدی 
    //میایم یک اینترفیس بدون پیاده سازی ایجاد میکنیم 
    public interface IEntity
    {
    }

    public abstract class BaseEntity<TKey> : IEntity
    {
        public TKey Id { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
    //یک کلاس بیس تعریف میکنیم برای فقط آیدی ها و کلاس های دیگر که نیاز به آیدی دارند از این کلاس
    //اینهریت میشوند و از نوع جنریک که هر کلاس تایپ خودش را تعیین کند مثلا 
    //int or guid 
}
