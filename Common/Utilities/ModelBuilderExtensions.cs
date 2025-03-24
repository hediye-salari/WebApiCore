using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pluralize.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Utilities
{
    public static class ModelBuilderExtensions // تنظیمات اینتیتی فریم وورک را توسط رفلکشن انجام میده 
    {
        /// <summary>
        /// Singularizin table name like Posts to Post or People to Person
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddSingularizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            Pluralizer pluralizer = new Pluralizer();
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Singularize(tableName));
            }
        }

        /// <summary>
        /// Pluralizing table name like Post to Posts or Person to People
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddPluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            //Pluralizer: برای جمع کردن یا مفرد کردن اسم تیبل ها در دیتا بیس از این لایبرلی استفاده میکنیم 
            //GetEntityTypes: به ازای همه اینتیتی ها که میخواهیم برای آن تیبل ایجاد کنیم 
            Pluralizer pluralizer = new Pluralizer();
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Singularize(tableName));
            }
        }

        /// <summary>
        /// Set NEWSEQUENTIALID() sql function for all columns named "Id"
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="mustBeIdentity">Set to true if you want only "Identity" guid fields that named "Id"</param>
        public static void AddSequentialGuidForIdConvention(this ModelBuilder modelBuilder)
        {
            // هر جا که آیدی ما از نوع جیو آیدی بود برو به 
            //sequential guid 
            //تبدیل کن .
            modelBuilder.AddDefaultValueSqlConvention("Id", typeof(Guid), "NEWSEQUENTIALID()");
        }

        /// <summary>
        /// Set DefaultValueSql for sepecific property name and type
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="propertyName">Name of property wants to set DefaultValueSql for</param>
        /// <param name="propertyType">Type of property wants to set DefaultValueSql for </param>
        /// <param name="defaultValueSql">DefaultValueSql like "NEWSEQUENTIALID()"</param>
        public static void AddDefaultValueSqlConvention(this ModelBuilder modelBuilder, string propertyName, Type propertyType, string defaultValueSql)
        {
            //IMutableEntityType: base type 
            //IMutableProperty:این اینترفیس اجازه میدهد هنگامی که یک مدل ساخته شد دیتاهای آن را تغییر دهیم د 
            //OrdinalIgnoreCase: رشته ها را مقایسه میکنند  به صورت باینری  که در اینجا یعنی مقدار آیدی
            //ClrType: typeof(Guid) تایپ آن 
            //DefaultValueSql:"NEWSEQUENTIALID()"

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                IMutableProperty property = entityType.GetProperties().SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

                if (property != null && property.ClrType == propertyType)
                    property.SetDefaultValueSql(defaultValueSql);
            }
        }

        /// <summary>
        /// Set DeleteBehavior.Restrict by default for relations
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddRestrictDeleteBehaviorConvention(this ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            //casecade:این حالت باعث می شود تا پس از UPDATE شدن مقدار مرجع ، مقدار ستون مربوطه نیز
            //آپدیت شده و پس از حذف شدن نیز سطر مربوطه حذف خواهد شد.
            foreach (IMutableForeignKey fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        /// <summary>
        /// Dynamicaly load all IEntityTypeConfiguration with Reflection
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterEntityTypeConfiguration(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            MethodInfo applyGenericMethod = typeof(ModelBuilder).GetMethods().First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic);

            foreach (Type type in types)
            {
                foreach (Type iface in type.GetInterfaces())
                //در شرط گفته هر جا اینترفیس جنریک داشت که تایپ آن 
                //IEntityTypeConfiguration
                //بود اونها را پیدا کن 
                //که GenericTypeArguments ارکومانش  صفر بود 
                // یعنی  post  در این پروژه 
                {
                    if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        MethodInfo applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(type) });
                    }
                }
            }
        }

        /// <summary>
        /// Dynamicaly register all Entities that inherit from specific BaseType
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="baseType">Base type that Entities inherit from this</param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        /// 



        // به صورت رفلکشن تعریف شده و اکستندد پراپرتی 
        //کلمه کلیدی params امكان ارسال تعداد دلخواه پارامترهاي همنوع و ذخیره آنها در یک آرایه ساده را فراهم می‌آورد
        //--------------------------------------
        //دسترسی به کلاس Modelbuilder با Override کردن متد OnModelCreating از کلاس DbContext صورت می گیرد
        //. پس از آن ما با استفاده از ModelBuilder و Fluent API می توانیم مدل خود را پیکربندی کنیم.


        public static void RegisterAllEntities<BaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {

            // GetExportedTypes: تایپ هایی که قابل صدا زدن هستند واز بیرون قابل دسترس هستند را بر میگرداند
            // typeof(BaseType): جایی که از کلاس بیس تایپ اینهریت شده 
            //IsAssignableForm : و به پارامترهایی از نوع بیس تایپ اختصاص میدهد 
            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(BaseType).IsAssignableFrom(c));

            foreach (Type type in types)
                modelBuilder.Entity(type);
        }
    }
}
