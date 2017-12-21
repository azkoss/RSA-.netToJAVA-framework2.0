using System;
using System.Collections.Generic;
using System.Text;
using AndyLib.Util;
namespace Test
{
    class Test
    {    
        /// <summary>
        /// 测试类集合
        /// </summary>      
        /// <remarks>
        /// 2017.12.01: 创建. 李兵 <br/>
        /// </remarks>

        /// <summary>
        /// httpPost提交
        /// </summary>
        /// <returns></returns>
        public static String httpPost()
        {
            //提交网址设置
            String URL = "https://www.baidu.com";
            String userName = "libing";
            String postStr = "&userName=" + userName;
            //推送
            String returnMsg = HttpPost.PostData(URL, postStr);
            //返回结果
            return returnMsg;
        }
        //MD5加密
        public static String MD5()
        {
            String userName = "李兵";
            String password = "123456";
            //MD5加密
            return md5.getMd5Hash(userName + password);
        }
        //RSA签名
        public static String RSASign()
        {
            //签名参数
            String userName = "李兵";
            String password = "123456";
            //构造java数据签名
            String sign = utilRSASign.RSASign(Properties.Settings.Default.privateKey, userName + password);
            //返回结果
            return sign;
        }
        //RSA加密
        public static String RSAEncrypt()
        {
            //参数
            String userName = "李兵";
            String password = "123456";
            //构造java数据加密
            String encrypt = utilRSASign.RSAEncrypt(Properties.Settings.Default.publicKey, userName + password);
            //返回结果
            return encrypt;
        }
        //SM3加密
        public static String SM3()
        {
            //参数
            String str = "李兵123456";
            //构造数据加密
            String encrypt = SM3Digest.SM3(str);
            //返回结果
            return encrypt;
        }
    }
}
