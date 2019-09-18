using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WxDemo.Models;

namespace WxDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //ASP.net Core微信配置Token
        /// <summary>
        /// 定义Token，与微信公共平台上的Token保持一致
        /// </summary>
        private const string Token = "admin";
        public void Valid(WeChatRequestModel model)
        {
            //获取请求来的 echostr 参数
            string echoStr = model.echostr;
            //通过验证，出于安全考虑。（也可以跳过）
            if (CheckSignature(model))
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    //将随机生成的 echostr 参数 原样输出
                    Response.WriteAsync(echoStr);
                    //截止输出流
                    //Response.end();
                }
            }
        }

        /// <summary>
        /// 验证签名，检验是否是从微信服务器上发出的请求
        /// </summary>
        /// <param name="model">请求参数模型 Model</param>
        /// <returns>是否验证通过</returns>
        private bool CheckSignature(WeChatRequestModel model)

        {
            string signature, timestamp, nonce, tempStr;
            //获取请求来的参数
            signature = model.signature;
            timestamp = model.timestamp;
            nonce = model.nonce;
            //创建数组，将 Token, timestamp, nonce 三个参数加入数组
            string[] array = { Token, timestamp, nonce };
            //进行排序
            Array.Sort(array);
            //拼接为一个字符串
            tempStr = String.Join("", array);
            //对字符串进行 SHA1加密
            tempStr = Get_SHA1_Method2(tempStr);
            //判断signature 是否正确
            if (tempStr.Equals(signature))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string Get_SHA1_Method2(string strSource)
        {
            string strResult = "";
            //Create 
            System.Security.Cryptography.SHA1 md5 = System.Security.Cryptography.SHA1.Create();
            //注意编码UTF8、UTF7、Unicode等的选择 
            byte[] bytResult = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strSource));
            //字节类型的数组转换为字符串 
            for (int i = 0; i < bytResult.Length; i++)
            {
                //16进制转换 
                strResult = strResult + bytResult[i].ToString("X");
            }
            return strResult.ToLower();
        }

    }
    public class WeChatRequestModel
    {
        public string signature { get; set; }
        public string timestamp { get; set; }
        public string nonce { get; set; }
        public string echostr { get; set; }
    }

}
