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
namespace oms.DataAccessLayer
{
    public static class UsersDL
    {
        public static User Get(string loginId)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from dbo.users where loginid = '{loginId}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return User.Create(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static User Get(int userId)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from dbo.users where id = {userId}";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return User.Create(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static List<User> Get()
        {
            List<User> returnList = new List<User>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select * from dbo.users";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(User.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }

        public static int Add(User user)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"insert into dbo.users(firstname, lastname, email, loginid, password) values(" +
                        $"'{user.FirstName}','{user.LastName}','{user.Email}','{user.LoginId}','{user.Password}'" +
                        $")";
                    int returnValue = cmd.ExecuteNonQuery();
                    return returnValue;
                }
            }
        }

        public static int Delete(int userId)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"delete from dbo.users where id = {userId}";
                    int returnValue = cmd.ExecuteNonQuery();
                    return returnValue;
                }
            }
        }

        public static int Update(User user)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"update dbo.users set " +
                        $"firstname='{user.FirstName}', " +
                        $"lastname='{user.LastName}', " +
                        $"email='{user.Email}', " +
                        $"loginid='{user.LoginId}', " +
                        $"password='{user.Password}' " +
                        $"where id = {user.Id}";
                    int returnValue = cmd.ExecuteNonQuery();
                    return returnValue;
                }
            }
        }
    }
}
