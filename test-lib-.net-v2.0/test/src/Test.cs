using System;
using System.Collections.Generic;
using System.Text;
using AndyLib.Util;
namespace Test
{
    class Test
    {
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

        public static String MD5()
        {
            String password = "123456";
            //MD5加密
            return md5.getMd5Hash(password);
        }

        public static String RSASign()
        {
            //签名参数
            String userName = "李兵";
            String Str = "123456";
            //构造java数据签名
            String sign = utilRSASign.RSASign(Properties.Settings.Default.privateKey, userName + Str);
            //返回结果
            return sign;
        }

        public static String RSAEncrypt()
        {
            //参数
            String userName = "李兵";
            String Str = "123456";
            //构造java数据加密
            String encrypt = utilRSASign.RSAEncrypt(Properties.Settings.Default.publicKey, userName + Str);
            //返回结果
            return encrypt;
        }

        public static String SM3()
        {
            //参数
            String Str = "123456";
            //构造数据加密
            String encrypt = SM3Digest.SM3(Str);
            //返回结果
            return encrypt;
        }
    }
}
