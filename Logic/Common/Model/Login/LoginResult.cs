namespace Logic
{
    public class Result
    {
        /// <summary>
        /// 结果
        /// </summary>
        public bool is_success;
        /// <summary>
        /// 提示文字
        /// </summary>
        public string prompt;
    }
    /// <summary>
    /// 登录结果
    /// </summary>
    public class LoginResult : TokenResult
    {
        /// <summary>
        /// 当前密码
        /// </summary>
        public string password;
        /// <summary>
        /// 服务器列表
        /// </summary>
        public Server server;
        /// <summary>
        /// 游戏更新地址
        /// </summary>
        public UpdateUrl update_url;
    }
    /// <summary>
    /// 游戏服Token数据
    /// </summary>
    public class TokenResult : Result
    {
        /// <summary>
        /// 指定服务器UID
        /// </summary>
        public long uid;
        /// <summary>
        /// 指定服务器Token
        /// </summary>
        public byte[] token;
    }
    /// <summary>
    /// 检测版本结果
    /// </summary>
    public class VersionResult : Result
    {
        /// <summary>
        /// 游戏更新地址
        /// </summary>
        public string update_url;
        /// <summary>
        /// 基础数据更新地址
        /// </summary>
        public string basedata_url;
    }
}
