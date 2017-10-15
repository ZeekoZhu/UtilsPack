using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ZeekoUtilsPack.AspNetCore
{
    public class OperationResult
    {
        public string Because { get; set; }
        public bool Status { get; set; } = false;
        public string ErrorCode { get; set; }
        private readonly int _httpStatusCode;

        /// <summary>
        /// 获取操作失败结果
        /// </summary>
        [JsonIgnore]
        public JsonResult Error => new JsonResult(this) { StatusCode = _httpStatusCode };

        /// <summary>
        /// 获取操作成功结果
        /// </summary>
        [JsonIgnore]
        public JsonResult Success => new JsonResult(new { Status = true }) { StatusCode = 200 };

        /// <summary>
        /// 返回一个失败操作就调用这个
        /// </summary>
        /// <param name="because">失败原因</param>
        /// <param name="errorCode">系统错误代码</param>
        /// <param name="httpStatusCode">http 状态码</param>
        public OperationResult(string because, string errorCode = "error", int httpStatusCode = 400)
        {
            Status = false;
            Because = because;
            ErrorCode = errorCode;
            _httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// 返回一个成功操作就调用这个
        /// </summary>
        public OperationResult()
        { }
    }
}
