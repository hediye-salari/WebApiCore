using Common.Utilities;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        //راه دوم برای تعریف DbContext
        //چون در اپ ستینگ  ما تعریف کردیم باید یک سازنده اینجا تعریف کنیم و آپشن را به آن پاس بدهیم 
        //وگرنه در ماگریشن و لانچ پروژه به  مشکل بر میخوریم 
        //base(options):آپشن را به سازنده پدر پاس میدهیم 
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {

        }
        //DbContextدو راه برای تعریف 
        // راه اول برای تعریف DbContext
        //short key: override OnConfiguring

        ////protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        ////{
        ////    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MyApiDb;Integrated Security=true");
        ////    base.OnConfiguring(optionsBuilder);
        ////}

        //-------------------------------------------------------------------------------------
        //متد  زیر وظیفه ساخت جدول از روی مدل ها را دارد به جای تعریف 
        //public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //نوع تایپ را اینترفیس تعریف میکنیم در  اسمبلی تعیین میکنیم برای اضافه کردن تیبیل 
            var entitiesAssembly = typeof(IEntity).Assembly;
            //RegisterAllEntities یک متد در لایه کامان تعریف شده که اگر هر کلاسی که از کلاس بیس اینتیتی 
            //ارث بری کرده بود بیاد تیبل های آن را در دیتا بیس بسازد 

            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            //modelBuilder.ApplyConfiguration();
            //اگر از اپلای کانفیگوریشن استفاده کنیم باید به ازای هر کانفیگوریشن دستی بیایم تعریف کنیم 
            //==> یک متد تعریف میکینیم  و در اینجا  صدا میزنیم .
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            //برای جایی که رابطه پرنت چایلد وچود دارد در هنگام حذف اول چایلد را حذف کند .
            modelBuilder.AddRestrictDeleteBehaviorConvention();
            // هر جا که آیدی ما از نوع جیو آیدی بود برو به 
            //sequential guid 
            //تبدیل کن .
            modelBuilder.AddSequentialGuidForIdConvention();

            modelBuilder.AddPluralizingTableNameConvention();
        }

        public override int SaveChanges()
        {
            _cleanString();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _cleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(cancellationToken);
        }


        //قبل آپدیت و ادد میاید مقادیر استرینگ را از لحاظ فارسی و عربی و انگلیسی و ... بررسی میکند 
        private void _cleanString()
        {
            //یکی از پراپرتی های کلاس DbContext، پراپرتی ChangeTracker می باشد که
            //وظیفه آن مدیریت تغییرات entityها و اعمال تغییرات در دیتابیس می باشد.
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }
    }
}
