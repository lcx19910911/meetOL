
using Core.Code;
using Core.Extensions;
using Core.Helper;
using Core.Model;
using EnumPro;
using Extension;
using IService;
using Model;
using MPUtil.UserMng;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Service
{
    /// <summary>
    /// 微信用户
    /// </summary>
    public class UserService : BaseService, IUserService
    {

        public UserService()
        {
            base.ContextCurrent = HttpContext.Current;
        }


        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public WebResult<User> Login(string account, string password)
        {
            using (DbRepository entities = new DbRepository())
            {
                string md5Password = Core.Util.CryptoHelper.MD5_Encrypt(password);

                return Result(Find<User>(x => x.Account == account && x.Password == md5Password && !x.IsDelete));
            }
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public User FindByOpenId(string openId)
        {
            if (string.IsNullOrEmpty(openId))
                return null;
            return Find<User>(x => x.OpenId == openId);
        }


        /// <summary>
        /// 编辑管理用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(WXUser model)
        {
            var user = FindByOpenId(model.openid.Trim());
            if (user == null)
            {
                var addEntity = new User()
                {
                    OpenId = model.openid.Trim(),
                    NickName = model.nickname,
                    Country = model.country,
                    Province = model.province,
                    City = model.city,
                    Sex = model.sex.GetInt(),
                    HeadImgUrl = model.headimgurl,
                    CreatedTime = DateTime.Now
                };

                Add(addEntity);
            }
            else
            {
                user.NickName = model.nickname;
                user.Country = model.country;
                user.Province = model.province;
                user.City = model.city;
                user.Sex = model.sex.GetInt();
                user.HeadImgUrl = model.headimgurl;
                Update(user);
            }
            return Result(true);

        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <returns></returns>
        public PageList<User> GetPageList(int pageIndex, int pageSize, string name, UserType? type)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.User.Where(x => !x.IsDelete);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.RealName.Contains(name));
                }
                if (type != null)
                {
                    query = query.Where(x => x.Type == type);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    if (x != null)
                    {
                    }
                });

                return CreatePageList(list, pageIndex, pageSize, count);

            }
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<User> GetList(Expression<Func<User, bool>> predicate)
        {
            return GetList<User>(predicate).ToList();
        }

        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<User, bool>> predicate)
        {
            return IsExits<User>(predicate);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add(User model)
        {
            if (model.Type != UserType.User)
            {
                if (IsExits(x => x.Account == model.Account))
                    return Result(false, ErrorCode.system_name_already_exist);
                model.Password = CryptoHelper.MD5_Encrypt(model.Password);
            }
            Add<User>(model);
            return Result(true);
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(User model)
        {
            if (IsExits(x => x.Account == model.Account && x.ID != model.ID))
                return Result(false, ErrorCode.user_phone_already_exist);
            var user = Find(model.ID);
            if(user==null)
                return Result(false, ErrorCode.system_name_already_exist);
            user.RealName = model.RealName;
            user.MobilePhone = model.MobilePhone;
            Update<User>(user.ID, user);
            return Result(true);

        }


        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> ChangePassword(string oldPassword, string newPassword, string cfmPassword, string id)
        {
            if (newPassword.IsNullOrEmpty() || cfmPassword.IsNullOrEmpty() || id.IsNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            if (!cfmPassword.Equals(newPassword))
            {
                return Result(false, ErrorCode.user_password_notequal);

            }
            var user = Find(id);
            if (user == null)
                return Result(false, ErrorCode.user_not_exit);
            if (oldPassword == "")
            {
                oldPassword = CryptoHelper.MD5_Encrypt(oldPassword);
                if (!user.Password.Equals(oldPassword))
                    return Result(false, ErrorCode.user_password_nottrue);
            }
            newPassword = CryptoHelper.MD5_Encrypt(newPassword);
            user.Password = newPassword;
            Update(user);
            return Result(true);
        }


        public User Find(string id)
        {
            return Find<User>(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 删除会议厅
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            Delete<User>(IDs);
            return Result(true);
        }

        public WebResult<bool> Bind(string phone, string position, string realName, string compnay, string userId)
        {
            if (phone.IsNullOrEmpty() || position.IsNullOrEmpty() || realName.IsNullOrEmpty() || userId.IsNullOrEmpty())
                return Result(false, ErrorCode.sys_param_format_error);

            var user = Find(userId);
            if (user == null)
                return Result(false, ErrorCode.sys_param_format_error);
            user.MobilePhone = phone;
            user.Position = position;
            user.RealName = realName;
            user.Compnay = compnay;
            Update(userId, user);
            Client.LoginUser = user;
            return Result(true);
        }
    }
}
