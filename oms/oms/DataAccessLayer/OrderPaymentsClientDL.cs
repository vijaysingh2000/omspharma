using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using oms.Model;
using System.Runtime.CompilerServices;
using System.Configuration;
using DevExpress.Utils.Filtering.Internal;

namespace oms.DataAccessLayer
{
    public static class OrderPaymentsClientDL
    {
        public static List<OrderPaymentClient> Get(Guid orderId)
        {
            List<OrderPaymentClient> returnList = new List<OrderPaymentClient>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from {tableName} WITH(NOLOCK) order by chequedate desc";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderPaymentClient.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }
        public static int AddOrUpdate(List<OrderPaymentClient> models)
        {
            int returnValue = 0;
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = transaction;

                        if (models.Count > 0)
                        {
                            cmd.CommandText = $"delete from {tableName} where oid = '{models[0].OrderId}'";
                            returnValue = cmd.ExecuteNonQuery();

                            models.ForEach(model =>
                            {
                                cmd.CommandText = GetInsertScript(model);
                                returnValue += cmd.ExecuteNonQuery();
                            });
                        }
                    }

                    transaction.Commit();
                }
            }

            return returnValue;
        }

        private static string GetExistsScript(OrderPaymentClient model)
        {
            return $"select 1 from {tableName} where orderid = '{model.OrderId}' and chequenumber = '{model.ChequeNumber}'";
        }
        private static string GetInsertScript(OrderPaymentClient model)
        {
            return $"insert into {tableName}(oid,chequenumber,chequedate,amount,notes,path,filename) values(" +
                    $"'{model.OrderId}','{model.ChequeNumber}','{model.ChequeDate}',{model.Amount},'{model.Notes}','{model.Path}','{model.FileName}'" +
                    $")";
        }
        private static string GetUpdateScript(OrderPaymentClient model)
        {
            return $"update {tableName} set " +
                    $"chequedate='{model.ChequeDate}', " +
                    $"amount={model.Amount} " +
                    $"notes='{model.Notes}', " +
                    $"path='{model.Path}', " +
                    $"filename='{model.FileName}', " +
                    $"where oid = '{model.OrderId}' and chequenumber = '{model.ChequeNumber}'";
        }
        private static string tableName { get { return "dbo.orderpaymentsclient"; } }
    }
}
