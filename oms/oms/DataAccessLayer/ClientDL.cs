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
    public static class ClienttDL
    {
        public static BasicModel Get(Guid id)
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
        public static List<BasicModel> Get()
        {
            List<BasicModel> returnList = new List<BasicModel>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from {tableName} WITH(NOLOCK)";
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
        public static List<BasicModel> GetActive()
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
        public static int Delete(Guid id)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"delete from {tableName} WITH(NOLOCK) where id = '{id}'";
                    int returnValue = cmd.ExecuteNonQuery();
                    return returnValue;
                }
            }
        }

        private static string tableName = "dbo.clients";
    }
}
