using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Net;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Policy;
using Newtonsoft.Json;
using System.Linq;

namespace F2.Common
{
    public static class CommonTools
    {
        private static long Jan1st1970Ms = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UnBase64String(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBase64String(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 写日志，后续用Log4
        /// </summary>
        /// <param name="input"></param>
        public static void WriteLogFile(string input)
        {
            string logPath = ConfigurationManager.AppSettings["LogFilePath"].ToString();
            //判断该路径下文件夹是否存在，不存在的情况下新建文件夹
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            //指定日志文件的目录
            string fname = logPath + "\\LogFile" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            //定义文件信息对象
            FileInfo finfo = new FileInfo(fname);

            if (!finfo.Exists)
            {
                FileStream fs;
                fs = File.Create(fname);
                fs.Close();
                finfo = new FileInfo(fname);
            }

            //判断文件是否存在以及是否大于2K
            //if (finfo.Length > 1024 * 1024 * 10)
            //{
            //    //文件超过10MB则重命名
            //    File.Move(Directory.GetCurrentDirectory() + "\\LogFile.txt", Directory.GetCurrentDirectory() + DateTime.Now.TimeOfDay + "\\LogFile.txt");
            //}

            //创建只写文件流
            using (FileStream fs = finfo.OpenWrite())
            {
                //根据上面创建的文件流创建写数据流
                StreamWriter w = new StreamWriter(fs);

                //设置写数据流的起始位置为文件流的末尾
                w.BaseStream.Seek(0, SeekOrigin.End);

                //写入“Log Entry : ”
                w.Write("\n\rLog Entry : ");

                //写入当前系统时间并换行
                w.Write("{0} {1} \n\r", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());

                //写入日志内容并换行
                w.Write(input + "\n\r");

                //写入------------------------------------“并换行
                w.Write("------------------------------------\n\r");

                //清空缓冲区内容，并把缓冲区内容写入基础流
                w.Flush();

                //关闭写数据流
                w.Close();
            }
        }
        /// <summary>
        /// 返回当前时间的毫秒数, 这个毫秒其实就是自1970年1月1日0时起的毫秒数
        /// </summary>
        public static long currentTimeMillis()
        {
            return (System.DateTime.UtcNow.Ticks - Jan1st1970Ms) / 10000;
        }

        /// <summary>
        /// 从一个代表自1970年1月1日0时起的毫秒数，转换为DateTime (北京时间)
        /// </summary>
        public static System.DateTime getDateTime(long timeMillis)
        {
            return new System.DateTime((long)((timeMillis + 28800000L) * 10000 + Jan1st1970Ms));
        }

        /// <summary>
        /// 从一个代表自1970年1月1日0时起的毫秒数，转换为DateTime (UTC时间)
        /// </summary>
        public static System.DateTime getDateTimeUTC(long timeMillis)
        {
            return new System.DateTime(timeMillis * 10000 + Jan1st1970Ms);
        }

        public static string getimages(string content, string path, string fileName)
        {
            try
            {
                //文件保存路径
                string fileFullPath = ConfigurationManager.AppSettings["VideoPicPath"].ToString() + path;
                string suffix = "";
                if (!string.IsNullOrEmpty(content))
                {
                    if (content.Contains("data:image/png;base64"))
                    {
                        //如果没有文件夹,则创建
                        if (!Directory.Exists(fileFullPath))
                        {
                            Directory.CreateDirectory(fileFullPath);
                        }

                        //获取文件后缀
                        string i = content.Trim().Substring(0, content.IndexOf(",") + 1);
                        suffix = i.Substring(i.IndexOf("/") + 1, i.IndexOf(";") - i.IndexOf("/") - 1);

                        //suffix = "jpg";

                        //将,以前的多余字符串删除
                        string strbase64 = content.Trim().Substring(content.IndexOf(",") + 1);
                        //将指定的字符串（它将二进制数据编码为 Base64 数字）转换为等效的 8 位无符号整数数组
                        MemoryStream stream = new MemoryStream(Convert.FromBase64String(strbase64));
                        //文件读写
                        FileStream fs = new FileStream(fileFullPath + "\\" + fileName + "." + suffix, FileMode.OpenOrCreate, FileAccess.Write);
                        //将流写入数组
                        byte[] b = stream.ToArray();
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                    }
                    else
                    {
                        suffix = "jpg";
                        //创建一个request 同时可以配置requst其余属性  
                        System.Net.WebRequest imgRequst = System.Net.WebRequest.Create(content);
                        //在这里我是以流的方式保存图片  
                        System.Drawing.Image downImage = System.Drawing.Image.FromStream(imgRequst.GetResponse().GetResponseStream());
                        if (!System.IO.Directory.Exists(fileFullPath))
                        {
                            System.IO.Directory.CreateDirectory(fileFullPath);
                        }
                        downImage.Save(fileFullPath + "\\" + fileName + "." + suffix);
                        downImage.Dispose();//用完一定要释放  
                    }
                    return path+@"\" + fileName + "." + suffix;
                }
                else
                {
                    CommonTools.WriteLogFile("getimages：无图片");
                    return "";
                }
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("getimages：" + ex.ToString());
                return "";
            }
        }
    }
}