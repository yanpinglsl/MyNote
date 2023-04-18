using System.Net;
using System.Text;
using WebCoreExtend.CommonModels;

namespace DemoProject.Utility
{
    /// <summary>
    /// 封装下调用远程WebAPI 获取JWT的token
    /// </summary>
    public static class JWTTokenHelper
    {
        /// <summary>
        /// 调用远程校验服务
        /// </summary>
        /// <param name="userUrl"></param>
        /// <returns></returns>
        public static AjaxResult<string> IssueToken(string name, string password)
        {
            string url = "http://localhost:7200/api/jwt/login";
            AjaxResult<string> ajaxResult = null;

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                 {"name",name },
                {"password",password }
            };
            string sResult = PostClient(url, dic);

            if (string.IsNullOrWhiteSpace(sResult))
            {
                ajaxResult = new AjaxResult<string>()
                {
                    Result = false,
                    Message = "调用接口验证失败"
                };
            }
            else
            {
                ajaxResult = Newtonsoft.Json.JsonConvert.DeserializeObject<AjaxResult<string>>(sResult);
            }
            return ajaxResult;
        }

        /// <summary>
        /// 返回2个token  在TValue和OtherValue属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static AjaxResult<string> IssueTokenWithRefresh(string name, string password)
        {
            string url = "http://localhost:7200/api/jwt/LoginWithRefresh";
            AjaxResult<string> ajaxResult = null;
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                 {"name",name },
                {"password",password }
            };
            string sResult = PostClient(url, dic);

            if (string.IsNullOrWhiteSpace(sResult))
            {
                ajaxResult = new AjaxResult<string>()
                {
                    Result = false,
                    Message = "调用接口验证失败"
                };
            }
            else
            {
                ajaxResult = Newtonsoft.Json.JsonConvert.DeserializeObject<AjaxResult<string>>(sResult);
            }
            return ajaxResult;
        }

        public static AjaxResult<string> IssueTokenByRefresh(string refreshToken)
        {
            string url = "http://localhost:7200/api/jwt/RefreshToken";
            AjaxResult<string> ajaxResult = null;
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                 {"refreshToken",refreshToken }
            };
            string sResult = PostClient(url, dic);

            if (string.IsNullOrWhiteSpace(sResult))
            {
                ajaxResult = new AjaxResult<string>()
                {
                    Result = false,
                    Message = "调用接口验证失败"
                };
            }
            else
            {
                ajaxResult = Newtonsoft.Json.JsonConvert.DeserializeObject<AjaxResult<string>>(sResult);
            }
            return ajaxResult;
        }

        #region HttpClient实现Post请求
        /// <summary>
        /// HttpClient实现Post请求
        /// </summary>
        private static string PostClient(string url, Dictionary<string, string> dic)
        {

            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                using (var http = new HttpClient(handler))
                {
                    //new MultipartFormDataContent
                    var content = new FormUrlEncodedContent(dic);//使用FormUrlEncodedContent做HttpContent
                    var response = http.PostAsync(url, content).Result;
                    Console.WriteLine(response.StatusCode); //确保HTTP成功状态值
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region  HttpWebRequest实现post请求
        /// <summary>
        /// HttpWebRequest实现post请求
        /// </summary>
        private static string PostWebQuest(string url, string name, string password)
        {
            try
            {


                var user = new
                {
                    name = name,
                    password = password
                };
                var postData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

                var request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 30 * 1000;//设置30s的超时
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36";
                request.ContentType = "application/json";
                request.Method = "POST";
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = data.Length;
                Stream postStream = request.GetRequestStream();
                postStream.Write(data, 0, data.Length);
                postStream.Close();

                string result = "";
                using (var res = request.GetResponse() as HttpWebResponse)
                {
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                        result = reader.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion
    }
}
