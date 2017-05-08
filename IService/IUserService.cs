using Core.Model;
using Domain;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IService
{
    /// <summary>
    /// 会议厅接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <returns></returns>
        PageList<User> GetPageList(int pageIndex, int pageSize, string name, UserType? type);

        WebResult<User> Login(string account, string password);

        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        List<User> GetList(Expression<Func<User, bool>> predicate = null);

        bool IsExits(Expression<Func<User, bool>> predicate);

        WebResult<bool> Add(User model);


        WebResult<bool> Update(User model);


        WebResult<bool> Delete(string ids);


        User Find(string id);

        User FindByOpenId(string openid);

        WebResult<bool> Bind(string phone, string position, string realName,string compnay, string userId);

        WebResult<bool> ChangePassword(string oldPassword, string newPassword, string cfmPassword, string id);
    }
}
