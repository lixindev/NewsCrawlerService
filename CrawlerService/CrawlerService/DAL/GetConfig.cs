using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CrawlerService
{
    public class GetConfig
    {
        /// <summary>
        /// 获取爬虫配置信息
        /// </summary>
        /// <returns>网站层级配置信息</returns>
        public List<CrawlerConfig> GetCrawlerConfig()
        {
            Database db = new Database();
            List<CrawlerConfig> cc_list = new List<CrawlerConfig>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from dbo.CrawlerConfig where [state] = 1; ");
                DataSet ds = db.ExeQueryGetDataSet(sb.ToString());

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    CrawlerConfig cc = new CrawlerConfig()
                    {
                        CrawlerConfigId = new Guid(item["CrawlerConfigId"].ToString()),
                        StationName = item["StationName"].ToString(),
                        Address = item["Address"].ToString()
                    };
                    cc_list.Add(cc);
                }
                return cc_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Close();
                db.Dispose();
            }
        }

        /// <summary>
        /// 获取网站对应的区块设置
        /// </summary>
        /// <param name="CrawlerConfigId"></param>
        /// <returns>区块层级配置信息</returns>
        public List<CrawlerPartConfig> GetCrawlerPartConfig(Guid CrawlerConfigId)
        {
            Database db = new Database();
            List<CrawlerPartConfig> cpc_list = new List<CrawlerPartConfig>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from dbo.CrawlerPartConfig where CrawlerConfigId = @CrawlerConfigId; ");
                SqlParameter[] parameters =
                {
                    db.MakeParam("CrawlerConfigId", SqlDbType.UniqueIdentifier, 0, ParameterDirection.Input, CrawlerConfigId)
                };
                DataSet ds = db.ExeQueryGetDataSet(sb.ToString(), parameters);

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    CrawlerPartConfig cpc = new CrawlerPartConfig()
                    {
                        Id = new Guid(item["Id"].ToString()),
                        CrawlerConfigId = new Guid(item["CrawlerConfigId"].ToString()),
                        FetchModel = Convert.ToInt32(item["FetchModel"]),
                        IndexLink_XPath = item["IndexLink_XPath"] == DBNull.Value ? null : item["IndexLink_XPath"].ToString(),
                        LoadMoreButton_XPath = item["LoadMoreButton_XPath"] == DBNull.Value ? null : item["LoadMoreButton_XPath"].ToString(),
                        PageNumInput_XPath = item["PageNumInput_XPath"] == DBNull.Value ? null : item["PageNumInput_XPath"].ToString(),
                        GoToButton_XPath = item["GoToButton_XPath"] == DBNull.Value ? null : item["GoToButton_XPath"].ToString(),
                        FirstPage = item["FirstPage"] == DBNull.Value ? null : (int?)(Convert.ToInt32(item["FirstPage"])),
                        LastPage = item["LastPage"] == DBNull.Value ? null : (int?)(Convert.ToInt32(item["LastPage"])),
                        ExtentionData1 = item["ExtentionData1"] == DBNull.Value ? null : item["ExtentionData1"].ToString(),
                        ExtentionData2 = item["ExtentionData2"] == DBNull.Value ? null : item["ExtentionData2"].ToString(),
                        ExtentionData3 = item["ExtentionData3"] == DBNull.Value ? null : item["ExtentionData3"].ToString(),
                        ExtentionData4 = item["ExtentionData4"] == DBNull.Value ? null : item["ExtentionData4"].ToString(),
                        ExtentionData5 = item["ExtentionData5"] == DBNull.Value ? null : item["ExtentionData5"].ToString(),
                        Title_Xpath = item["Title_Xpath"] == DBNull.Value ? null : item["Title_Xpath"].ToString(),
                        Content_Xpath = item["Content_Xpath"] == DBNull.Value ? null : item["Content_Xpath"].ToString(),
                        Editor_Xpath = item["Editor_Xpath"] == DBNull.Value ? null : item["Editor_Xpath"].ToString(),
                        Source_Xpath = item["Source_Xpath"] == DBNull.Value ? null : item["Source_Xpath"].ToString(),
                        PublishTime_Xpath = item["PublishTime_Xpath"] == DBNull.Value ? null : item["PublishTime_Xpath"].ToString()
                    };
                    cpc_list.Add(cpc);
                }
                return cpc_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Close();
                db.Dispose();
            }
        }


        public bool CheckIsExists(string Address)
        {
            Database db = new Database();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select Id, Address from dbo.News where Substring(Address,0,CHARINDEX('?',Address)) = @Address; ");

                SqlParameter[] parameters =
                {
                    db.MakeParam("Address", SqlDbType.VarChar, 300, ParameterDirection.Input, Address)
                };
                DataSet ds = db.ExeQueryGetDataSet(sb.ToString(), parameters);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Close();
                db.Dispose();
            }
        }

        public Dictionary<string, string> GetSysConfig()
        {
            Database db = new Database();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from dbo.SysConfig; ");
                DataSet ds = db.ExeQueryGetDataSet(sb.ToString());

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    dic.Add(item["Key"].ToString(), item["Value"].ToString());
                }

                return dic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Close();
                db.Dispose();
            }
        }
    }
}
