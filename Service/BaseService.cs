using Core.Code;
using Core.Model;
using Core.Util;
using Core.Web;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Extension;
using Model;
using System.Linq.Expressions;
using Core;
using Core.Extensions;

namespace Service
{
    public class BaseService
    {
        /// <summary>
        /// 请求context
        /// </summary>
        public HttpContext ContextCurrent { get; set; }

        public WebClient Client
        {
            get
            {
                return new WebClient(ContextCurrent);
            }
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void ClearCache()
        {
            LogHelper.WriteCustom(string.Format("ClearCache At {0} \r\n", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")), @"ClearCache\");
            CacheHelper.Clear();
        }



        public WebResult<T> Result<T>(T model)
        {
            return Result(model, ErrorCode.sys_success);
        }

        public WebResult<T> Result<T>(T model, ErrorCode code)
        {
            return new WebResult<T> { Code = code, Result = model,Append=code.GetDescription() };
        }

        public WebResult<T> Result<T>(T model, string append)
        {
            return new WebResult<T> { Code = ErrorCode.sys_fail, Result = model, Append = append };
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="source">实体</param>
        /// <returns>影响条数</returns>
        public void Add<T>(T source) where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                var addEntity = source.AutoMap<T>();
                db.Entry(addEntity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        /// <returns></returns>
        public User GetCurrentUser()
        {
                return LoginHelper.GetCurrentUser();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="IDs">ID，多个id用逗号分隔</param>
        /// <returns>影响条数</returns>
        public int Delete<T>(string IDs) where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                //按逗号分隔符分隔开得到ID列表
                var IDArray = IDs.Split(',');
                //遍历ID列表逐个删除
                foreach (var ID in IDArray)
                {
                    DbSet<T> dbSet = db.Set<T>();
                    var entity = dbSet.Find(ID);
                    if (entity != null)
                    {
                        entity.IsDelete = true;
                    }
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns>影响条数</returns>
        public int Delete<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            if (predicate == null)
                return 0;
            using (DbRepository db = new DbRepository())
            {
                DbSet<T> dbSet = db.Set<T>();
                var list = dbSet.Where(predicate).ToList();
                list.ForEach(entity =>
                {
                    entity.IsDelete = true;
                });
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 获取用户所有的会议
        /// </summary>
        /// <returns></returns>
        public bool IsExits<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
           return GetList<T>().AsQueryable().Any(predicate);
        }

        public T Find<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                DbSet<T> dbSet = db.Set<T>();
                return dbSet.Where(x => !x.IsDelete).Where(predicate).FirstOrDefault();
            }
        }


        /// <summary>
        /// 查找所有
        /// </summary>
        /// <returns></returns>
        public int GetCount<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                DbSet<T> dbSet = db.Set<T>();
                return dbSet.Where(x => !x.IsDelete).Where(predicate).Count();
            }
        }

        /// <summary>
        /// 查找所有
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll<T>() where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                DbSet<T> dbSet = db.Set<T>();
                return dbSet.ToList();
            }
        }

        /// <summary>
        /// 查找所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAllQuery<T>() where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                DbSet<T> dbSet = db.Set<T>();
                return dbSet.Where(x=>x.IsDelete);
            }
        }



        /// <summary>
        /// 查找所有
        /// </summary>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> predicate=null) where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                DbSet<T> dbSet = db.Set<T>();
                if(predicate != null)
                    return dbSet.Where(x=>!x.IsDelete).Where(predicate).ToList();
                else
                    return dbSet.Where(x => !x.IsDelete).ToList();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="source">实体</param>
        public void Update<T>(string ID, T source) where T : BaseEntity
        {
            using (DbRepository db = new DbRepository())
            {
                DbSet<T> dbSet = db.Set<T>();
                var sourceEntity = dbSet.Find(ID);
                if (sourceEntity != null)
                {
                    source.AutoMap<T>(sourceEntity);
                }
                db.SaveChanges();
            }
        }


        /// <summary>
        /// 创建分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected PageList<T> CreatePageList<T>(IQueryable<T> queryable, int pageIndex, int pageSize) where T : BaseEntity
        {
            int recordCount = 0;
            recordCount = queryable.Count();
            List<T> list = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<T>(list, pageIndex, pageSize, recordCount);

        }

        /// <summary>
        /// 创建分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">查询对象</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected PageList<T> CreatePageList<T>(List<T> list, int pageIndex, int pageSize, int recordCount)
        {
            try
            {
                return new PageList<T>(list, pageIndex, pageSize, recordCount);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected List<T> CreateList<T>(IQueryable<T> queryable, int pageIndex, int pageSize) 
        {
            try
            {
                List<T> list = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
