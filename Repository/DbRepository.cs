﻿using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DbRepository:PlugInsEntities
    {
        public override int SaveChanges()
        {
            try
            {
                var entries = from e in this.ChangeTracker.Entries()
                              where e.State != EntityState.Unchanged
                              select e;   //过滤所有修改了的实体，包括：增加 / 修改 / 删除

                foreach (var entry in entries)
                {
                    InitObject(entry);
                }

                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
                ////并发冲突数据
                //if (ex.GetType() == typeof(DbUpdateConcurrencyException))
                //{
                //    return -1;
                //}
                //return 0;
            }

        }
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="entry">entry对象</param>
        private void InitObject(DbEntityEntry entry)
        {
            if (entry.Entity is BaseEntity)
            {
                var entity = entry.Entity as BaseEntity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        //初始化这些值，如果这些值为null时，自动赋值
                        if (entity.CreatedTime == new DateTime())
                            entity.CreatedTime = DateTime.Now;
                        if (entity.UpdatedTime == new DateTime())
                            entity.UpdatedTime = DateTime.Now;
                        if (string.IsNullOrEmpty(entity.ID))
                            entity.ID = Guid.NewGuid().ToString("N");
                        break;
                    case EntityState.Modified:
                        //初始化这些值，如果这些值为null时，自动赋值
                        if (entity.UpdatedTime == new DateTime())
                            entity.UpdatedTime = DateTime.Now;
                        break;

                    case EntityState.Deleted:
                        //初始化这些值，如果这些值为null时，自动赋值
                        if (entity.UpdatedTime == new DateTime())
                            entity.UpdatedTime = DateTime.Now;
                        break;
                }
            }
        }


    }

    public class PlugInsEntities : DbContext
    {
        public PlugInsEntities()
           : base("name=Entities")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PlugInsEntities>());

            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.AutoDetectChangesEnabled = false;//关闭自动跟踪对象的属性变化
            //this.Configuration.ValidateOnSaveEnabled = false; //关闭保存时的实体验证
            //this.Configuration.UseDatabaseNullSemantics = false;//关闭数据库null比较行为


        }




        public  DbSet<MeetPlan> MeetPlan { get; set; }
        public  DbSet<MeetTopic> MeetTopic { get; set; }
        public  DbSet<MeetUserJoin> MeetUserJoin { get; set; }

        public  DbSet<Meet> Meet { get; set; }


        public  DbSet<User> User { get; set; }
        public  DbSet<Room> Room { get; set; }
        public  DbSet<Speaker> Speaker { get; set; }
        public  DbSet<TopicUserJoin> TopicUserJoin { get; set; }

        public DbSet<PlanVoteUserJoin> TopicVoteUserJoin { get; set; }
    }

}
