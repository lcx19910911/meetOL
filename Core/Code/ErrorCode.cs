using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Code
{
    public enum ErrorCode
    {
        #region 系统操作0-99之间
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功.")]
        sys_success = 0,

        /// <summary>
        /// 操作失败,请联系管理员
        /// </summary>
        [Description("服务器异常.")]
        sys_fail = 1,

        /// <summary>
        /// 参数值格式有误
        /// </summary>
        [Description("参数值格式有误.")]
        sys_param_format_error = 2,


        /// <summary>
        /// 授权码无效
        /// </summary>
        [Description("授权码无效.")]
        sys_token_invalid = 11,

        /// <summary>
        /// 用户角色权限不足
        /// </summary>
        [Description("用户角色权限不足.")]
        sys_user_role_error = 12,


        /// <summary>
        /// 微信openid不存在
        /// </summary>
        [Description("微信openid不存在.")]
        openid_no_exit = 13,


        #endregion

        #region 数据库操作 100-199
        /// <summary>
        /// 无法找到对应主键Id
        /// </summary>
        [Description("无法找到对应主键Id.")]
        datadatabase_primarykey_not_found = 101,


        /// <summary>
        /// 两次密码输入不一样
        /// </summary>
        [Description("两次密码输入不一样")]
        user_password_notequal = 110,


        /// <summary>
        /// 用户不存在
        /// </summary>
        [Description("用户不存在.")]
        user_not_exit = 105,


        /// <summary>
        /// 旧密码输入错误
        /// </summary>
        [Description("旧密码输入错误")]
        user_password_nottrue = 109,

        #endregion


        #region 共享的业务异常 200-299

        /// <summary>
        /// 请先注册
        /// </summary>
        [Description("请先注册.")]
        need_to_register = 200,

        /// <summary>
        /// 手机验证码错误
        /// </summary>
        [Description("手机验证码错误.")]
        phone_verificationCode_error = 201,


        /// </summary>
        [Description("名称已存在.")]
        system_name_already_exist = 202,


        /// </summary>
        [Description("已报名.")]
        meet_already_join = 203,



        /// </summary>
        [Description("人数已超过限制参加人数")]
        meet_join_max = 205,



        /// </summary>
        [Description("开始时间小于当前时间")]
        meet_start_time_error = 206,
        /// </summary>
        [Description("结束时间小于开始时间")]
        meet_end_time_error = 207,


        /// </summary>
        [Description("已审核")]
        join_had_audit = 208,


        /// </summary>
        [Description("已投票.")]
        meet_had_vote = 209,
        #endregion





        #region 用户

        /// <summary>
        /// 用户还未登录
        /// </summary>
        [Description("用户还未登录.")]
        user_login_error = 700,

        /// <summary>
        /// 用户已存在
        /// </summary>
        [Description("用户名已存在.")]
        user_phone_already_exist = 701,

        /// <summary>
        /// 用户密码错误
        /// </summary>
        [Description("用户名或密码错误.")]
        user_password_error = 702,

        /// <summary>
        /// 原始密码错误
        /// </summary>
        [Description("原始密码错误.")]
        user_oldpassword_error = 703,

        /// <summary>
        /// 已过验证时间
        /// </summary>
        [Description("已过验证时间.")]
        verification_time_out = 704,

        #endregion



        #region 项目


        /// <summary>
        /// 用户已存在
        /// </summary>
        [Description("用户已经关注.")]
        user_project_already_focus = 701,

        #endregion
    }
}
