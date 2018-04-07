using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CrawlerService
{
    public class NewsOperator
    {
        public void InsertNews(News news)
        {
            Database db = new Database();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into dbo.News (Id,Title,Content,Editor,Address,Source,CrawlerConfigId,InsertTime, SpendTime, ThreadId, PublishTime) ");
                sb.Append(" values (@Id,@Title,@Content,@Editor,@Address,@Source,@CrawlerConfigId,@InsertTime,@SpendTime,@ThreadId, @PublishTime) ");

                SqlParameter[] parameters =
                {
                    db.MakeParam("Id", SqlDbType.UniqueIdentifier, 0, ParameterDirection.Input, news.Id),
                    db.MakeParam("Title", SqlDbType.NVarChar, 200, ParameterDirection.Input, news.Title),
                    db.MakeParam("Content", SqlDbType.NVarChar, 10000, ParameterDirection.Input, news.Content),
                    db.MakeParam("Editor", SqlDbType.NVarChar, 50, ParameterDirection.Input, news.Editor),
                    db.MakeParam("Address", SqlDbType.VarChar, 300, ParameterDirection.Input, news.Address),
                    db.MakeParam("Source", SqlDbType.NVarChar, 50, ParameterDirection.Input, news.Source),
                    db.MakeParam("CrawlerConfigId", SqlDbType.UniqueIdentifier, 0, ParameterDirection.Input, news.CrawlerConfigId),
                    db.MakeParam("InsertTime", SqlDbType.DateTime, 0, ParameterDirection.Input, DateTime.Now),
                    db.MakeParam("SpendTime", SqlDbType.BigInt, 0, ParameterDirection.Input, news.SpendTime),
                    db.MakeParam("ThreadId", SqlDbType.Int, 0, ParameterDirection.Input, news.ThreadId),
                    db.MakeParam("PublishTime", SqlDbType.DateTime, 0, ParameterDirection.Input, news.PublishTime)
                };

                db.ExeQuery(sb.ToString(), parameters);
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

        public void DeleteNews(DateTime deadline)
        {
            Database db = new Database();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("delete from dbo.News where PublishTime < @DeadLine; ");
                sb.Append("delete from dbo.ErrorLog where InsertTime < @DeadLine; ");

                SqlParameter[] parameters =
                {
                    db.MakeParam("DeadLine", SqlDbType.DateTime, 0, ParameterDirection.Input, deadline),
                };

                db.ExeQuery(sb.ToString(), parameters);
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
