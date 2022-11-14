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
    public static class OrderPaymentsInsuranceDL
    {
        public static List<OrderPaymentInsurance> Get(Guid orderId)
        {
            List<OrderPaymentInsurance> returnList = new List<OrderPaymentInsurance>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select opi.*,o.ordernumber as ordernumber, " +
                        $"b.bid as batchid, b.name as batchname, " +
                        $"((select sum(assay) from dbo.orderassay oa WITH(NOLOCK) where oa.oid = o.oid) * o.dosecount * o.billperunit) as totalamountbilled, " +
                        $"(select sum(amount) from dbo.orderpaymentsinsurance opi2 WITH(NOLOCK) where opi2.oid = o.oid) as totalpayments " +
                        $"from {tableName} opi WITH(NOLOCK) " +
                        $"inner join dbo.orders o WITH(NOLOCK) on ( o.oid = opi.oid ) " +
                        $"inner join dbo.batchpayments b WITH(NOLOCK) on ( b.bid = opi.bid ) " +
                        $"where opi.oid = '{orderId}' " +
                        $"order by opi.chequedate desc";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderPaymentInsurance.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }
        public static List<OrderPaymentInsurance> GetByBatch(Guid batchId)
        {
            List<OrderPaymentInsurance> returnList = new List<OrderPaymentInsurance>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select opi.*,o.ordernumber as ordernumber, " +
                        $"b.bid as batchid, b.name as batchname, " +
                        $"((select sum(assay) from dbo.orderassay oa WITH(NOLOCK) where oa.oid = o.oid) * o.dosecount * o.billperunit) as totalamountbilled, " +
                        $"(select sum(amount) from dbo.orderpaymentsinsurance opi2 WITH(NOLOCK) where opi2.oid = o.oid) as totalpayments " +
                        $"from {tableName} opi WITH(NOLOCK) " +
                        $"inner join dbo.orders o WITH(NOLOCK) on ( o.oid = opi.oid ) " +
                        $"inner join dbo.batchpayments b WITH(NOLOCK) on ( b.bid = opi.bid ) " +
                        $"where opi.bid = '{batchId}' " +
                        $"order by opi.chequedate desc";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderPaymentInsurance.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }
        public static int AddOrUpdate(List<OrderPaymentInsurance> models)
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

        private static string GetExistsScript(OrderPaymentInsurance model)
        {
            return $"select 1 from {tableName} where orderid = '{model.OrderId}' and chequeNumber = '{model.ChequeNumber}'";
        }
        public static string GetInsertScript(OrderPaymentInsurance model)
        {
            return $"insert into {tableName}(oid,bid,rxnumber,chequenumber,chequedate,amount,notes,paymenttype,pap,path,filename) values(" +
                    $"'{model.OrderId}','{model.BatchId}','{model.RxNumber}','{model.ChequeNumber}','{model.ChequeDate}',{model.Amount},'{model.Notes}',{model.PaymentType},{CommonFunctions.GetBitSafely(model.IsPap)},'{model.Path}','{model.FileName}'" +
                    $")";
        }
        private static string GetUpdateScript(OrderPaymentInsurance model)
        {
            return $"update {tableName} set " +
                    $"rxnumber='{model.RxNumber}', " +
                    $"chequenumber='{model.ChequeNumber}', " +
                    $"chequedate='{model.ChequeDate}', " +
                    $"amount={model.Amount} " +
                    $"notes='{model.Notes}', " +
                    $"paymenttype={model.PaymentType}, " +
                    $"pap={CommonFunctions.GetBitSafely(model.IsPap)}, " +
                    $"path='{model.Path}', " +
                    $"filename='{model.FileName}' " +
                    $"where oid = '{model.OrderId}' and chequenumber = '{model.ChequeNumber}'";
        }
        private static string tableName { get { return "dbo.orderpaymentsinsurance"; } }
    }
}
