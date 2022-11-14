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
    public static class ListDL
    {
        public static BasicModel Get(string tableName, int id)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from {tableName} WITH(NOLOCK) where id = {id}";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return BasicModel.Create(reader);
                        }
                    }
                }
            }

            return null;
        }
        public static List<BasicModel> Get(string tableName)
        {
            List<BasicModel> returnList = new List<BasicModel>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from {tableName}  WITH(NOLOCK)";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(BasicModel.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }
        public static List<BasicModel> GetActive(string tableName)
        {
            List<BasicModel> returnList = new List<BasicModel>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from {tableName}  WITH(NOLOCK) where Active=1";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(BasicModel.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }
        public static int Delete(string tableName, Guid id)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"delete from {tableName} WITH(NOLOCK) where id = {id}";
                    int returnValue = cmd.ExecuteNonQuery();
                    return returnValue;
                }
            }
        }
        public static int AddOrUpdate(string tableName, List<BasicModel> models)
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
                        models.ForEach(model =>
                        {
                            cmd.CommandText = CommonFunctions.GetAddOrUpdateScript(tableName, model.Id, GetInsertScript(tableName, model), GetUpdateScript(tableName, model)); ;
                            returnValue += cmd.ExecuteNonQuery();
                        });
                    }

                    transaction.Commit();
                }
            }

            return returnValue;
        }

        private static string GetInsertScript(string tableName, BasicModel model)
        {
            return $"insert into {tableName}(name, description,active,perunitfees,flatfees) values(" +
                    $"'{model.Name}','{model.Description}',{CommonFunctions.GetBitSafely(model.Active)},{model.PerUnitFees},{model.FlatFees}" +
                    $")";
        }
        private static string GetUpdateScript(string tableName, BasicModel model)
        {
            return $"update {tableName} set " +
                    $"name='{model.Name}', " +
                    $"description='{model.Description}', " +
                    $"active={CommonFunctions.GetBitSafely(model.Active)}, " +
                    $"perunitfees={model.PerUnitFees}, " +
                    $"flatfees={model.FlatFees} " +
                    $"where id = {model.Id}";
        }
    }
}
