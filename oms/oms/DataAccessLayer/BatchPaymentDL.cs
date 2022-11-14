using oms.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oms.DataAccessLayer
{
    public static class BatchPaymentDL
    {
        public static List<Batch> Get()
        {
            List<Batch> returnList = new List<Batch>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select b.*," +
                        $"(select sum(amount) from dbo.orderpaymentsinsurance opi where opi.bid = b.bid) as totalamount," +
                        $"u.lastname,u.firstname from {tableName} b " +
                        $"left join dbo.users u on ( u.id = b.lastupdatedby ) " +
                        $"where b.cid = {ApplicationVariables.WorkingClient.Id}" +
                        $"order by createddate desc";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Batch batch = Batch.Create(reader);
                            batch.TotalAmount = CommonFunctions.GetFloatSafely(reader["totalamount"]);
                            returnList.Add(batch);
                        }
                    }
                }
            }

            return returnList;
        }
        public static Batch Get(string batchName)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select b.*,u.* " +
                        $"from {tableName} b WITH(NOLOCK) " +
                        $"left join dbo.users u WITH(NOLOCK) on ( u.id = b.lastupdatedby ) " +
                        $"where b.cid = {ApplicationVariables.WorkingClient.Id} " +
                        $"and name = '{batchName}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Batch.Create(reader);
                        }
                    }
                }
            }

            return null;
        }
        public static Batch Get(Guid batchId)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select b.*,u.* " +
                        $"from {tableName} b WITH(NOLOCK) " +
                        $"left join dbo.users u WITH(NOLOCK) on ( u.id = b.lastupdatedby ) " +
                        $"where bid = '{batchId}' " +
                        $"order by createddate desc";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Batch.Create(reader);
                        }
                    }
                }
            }

            return null;
        }
        public static int Delete(Guid batchId)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"delete from {tableName} where bid = '{batchId}'";
                    int returnValue = cmd.ExecuteNonQuery();
                    return returnValue;
                }
            }
        }
        public static int UpdateBatchPayments(Batch model, List<OrderPaymentInsurance> batchPayments)
        {
            int returnValue = 0;

            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tran;

                        cmd.CommandText = CommonFunctions.GetAddOrUpdateScript(GetExistsScript(model), GetInsertScript(model), GetUpdateScript(model));
                        returnValue = cmd.ExecuteNonQuery();

                        cmd.CommandText = $"delete from dbo.orderpaymentsinsurance where bid = '{model.Id}'";
                        returnValue += cmd.ExecuteNonQuery();

                        foreach (OrderPaymentInsurance batchPayment in batchPayments)
                        {
                            cmd.CommandText = OrderPaymentsInsuranceDL.GetInsertScript(batchPayment);
                            returnValue  += cmd.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                }
            }

            return returnValue;
        }
        public static int AddOrUpdate(List<Batch> models)
        {
            int returnValue = 0;
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        if (models.Count > 0)
                        {
                            models.ForEach(model =>
                            {
                                cmd.CommandText = CommonFunctions.GetAddOrUpdateScript(GetExistsScript(model), GetInsertScript(model), GetUpdateScript(model));
                                returnValue += cmd.ExecuteNonQuery();
                            });
                        }
                    }

                    tran.Commit();
                }
            }

            return returnValue;
        }

        private static string GetExistsScript(Batch model)
        {
            return $"select 1 from {tableName} where bid = '{model.Id}'";
        }
        private static string GetInsertScript(Batch model)
        {
            return $"insert into {tableName}(bid,cid,name,notes,emaildate,reportdate,createddate,createdby,lastupdatedby,lastupdateddate) values(" +
                    $"'{model.Id}','{ApplicationVariables.WorkingClient.Id}','{model.Name}','{model.Notes}','{model.EmailDate}','{model.ReportDate}',GETDATE(),{ApplicationVariables.LoggedInUser.Id},{ApplicationVariables.LoggedInUser.Id},GETDATE()" +
                    $")";
        }
        private static string GetUpdateScript(Batch model)
        {
            return $"update {tableName} set " +
                    $"name='{model.Name}', " +
                    $"notes='{model.Notes}', " +
                    $"emaildate='{model.EmailDate}', " +
                    $"reportdate='{model.ReportDate}', " +
                    $"lastupdatedby={ApplicationVariables.LoggedInUser.Id}, " +
                    $"lastupdateddate=GETDATE() " +
                    $"where bid = '{model.Id}'";
        }
        private static string tableName { get { return "dbo.batchpayments"; } }
    }
}
