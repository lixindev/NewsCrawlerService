using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace CrawlerService
{
    public class WriteLog
    {
        public static void WriteLogs(string logstr)
        {
            try
            {
                string Adress = ConfigurationManager.AppSettings["LogAdress"];
                if (!Directory.Exists(Adress))
                {
                    Directory.CreateDirectory(Adress);
                }
                string FlieName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                object locker = new object();
                lock (locker)
                {
                    FileStream fs = new FileStream(Adress + FlieName, FileMode.Append);

                    StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312"));
                    //lock (sw)
                    //{
                    sw.Write("\r\n====================开始===========================\r\n");
                    sw.Write(logstr);
                    sw.Write("\r\n====================结束===========================\r\n");
                    //}
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("因此该进程无法访问此文件"))
                {
                    WriteLogs(logstr);
                }
                else
                {
                    throw ex;
                }
            }
        }

        public static void InsertLogs(string Address, string ErrorMessage)
        {
            Database db = new Database();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into dbo.ErrorLog (Id,InsertTime,Address,ErrorMessage) ");
                sb.Append(" values (@Id,@InsertTime,@Address,@ErrorMessage) ");

                SqlParameter[] parameters =
                {
                    db.MakeParam("Id", SqlDbType.UniqueIdentifier, 0, ParameterDirection.Input, Guid.NewGuid()),
                    db.MakeParam("InsertTime", SqlDbType.DateTime, 0, ParameterDirection.Input, DateTime.Now),
                    db.MakeParam("Address", SqlDbType.VarChar, 300, ParameterDirection.Input, Address),
                    db.MakeParam("ErrorMessage", SqlDbType.NVarChar, 500, ParameterDirection.Input, ErrorMessage)
                };

                db.ExeQuery(sb.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
