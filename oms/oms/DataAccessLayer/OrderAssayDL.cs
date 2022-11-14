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
    public static class OrderAssayDL
    {
        public static List<OrderAssay> Get(Guid orderId)
        {
            List<OrderAssay> returnList = new List<OrderAssay>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from {tableName} WITH(NOLOCK) where oid = '{orderId}' order by assayid desc";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderAssay.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }
        public static int AddOrUpdate(List<OrderAssay> models)
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

        private static string GetExistsScript(OrderAssay model)
        {
            return $"select 1 from {tableName} where orderid = '{model.OrderId}' and assayid = '{model.AssayId}'";
        }
        public static string GetInsertScript(OrderAssay model)
        {
            return $"insert into {tableName}(oid,assayid,assay,ndc,qty,expdate,lot,rxnumber) values(" +
                    $"'{model.OrderId}','{model.AssayId}',{model.Assay},'{model.NDC}',{model.Qty},'{model.ExpDate}','{model.Lot}','{model.RxNumber}'" +
                    $")";
        }
        public static string GetDeleteScript(Guid orderId)
        {
            return $"delete from {tableName} where oid = '{orderId}'";
        }

        private static string GetUpdateScript(OrderAssay model)
        {
            return $"update {tableName} set " +
                    $"assay={model.Assay}, " +
                    $"ndc='{model.NDC}', " +
                    $"qty={model.Qty}, " +
                    $"expdate='{model.ExpDate}', " +
                    $"lot='{model.Lot}', " +
                    $"rxnumber='{model.RxNumber}' " +
                    $"where oid = '{model.OrderId}' and assayid = '{model.AssayId}'";
        }
        private static string tableName { get { return "dbo.orderassay"; } }
    }
}
