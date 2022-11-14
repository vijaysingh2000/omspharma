using Microsoft.VisualBasic.ApplicationServices;
using oms.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static DevExpress.XtraEditors.RoundedSkinPanel;
using static System.Windows.Forms.AxHost;

namespace oms.DataAccessLayer
{
    public static class OrderTasksDL
    {
        public static OrderTask Get(Guid orderId, string taskCode)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select ot.*, u.firstname, u.lastname" +
                        $" from dbo.ordertasks ot " +
                        $" left join dbo.users u " +
                        $" on (ot.lastupdatedby = u.id ) " +
                        $" where ot.oid = '{orderId}' and ot.taskcode = '{taskCode}'";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return OrderTask.Create(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static List<OrderTask> Get(Guid orderId)
        {
            List<OrderTask> returnList = new List<OrderTask>();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select ot.*, u.firstname, u.lastname" +
                        $" from dbo.ordertasks ot WITH(NOLOCK) " +
                        $" left join dbo.users u WITH(NOLOCK) " +
                        $" on (ot.lastupdatedby = u.id )" +
                        $" where ot.oid = '{orderId}'";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderTask.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }

        public static int AddAllTasks(Guid orderId)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                int returnValue = 0;
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = transaction;

                        List<Tasks> tasks = StaticListDL.GetAllTasks();

                        foreach (Tasks item in tasks)
                        {
                            cmd.CommandText = $"insert into dbo.ordertasks(oid,taskcode,idx,taskstatus,notes,lastupdatedby,lastupdatedttm,createddate) values(" +
                                $"'{orderId}','{item.Code}',{tasks.IndexOf(item)},{(int)E_TaskStatus.InProgress},'','{ApplicationVariables.LoggedInUser.Id}',GETDATE(),GETDATE()" +
                                $")";
                            returnValue += cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }

                return returnValue;
            }
        }


        public static int Update(OrderTask orderTask)
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

                        cmd.CommandText = GetUpdateScript(orderTask);
                        returnValue += cmd.ExecuteNonQuery();

                        cmd.CommandText = $"select count(*) from dbo.ordertasks where taskstatus <> 2 and oid = '{orderTask.OrderId}'";
                        int inprogress = CommonFunctions.GetIntSafely(cmd.ExecuteScalar());
                        if (inprogress == 0)
                        {
                            cmd.CommandText = $"update dbo.orders set " +
                            $"orderstatus={(int)E_TaskStatus.Complete} " +
                            $"where oid = '{orderTask.OrderId}'";

                            returnValue += cmd.ExecuteNonQuery();

                            cmd.CommandText = $"update dbo.patients set ordercompleted = ordercompleted + 1, orderinprogress = orderinprogress -1 where pid = (select pid from dbo.orders where oid = '{orderTask.OrderId}')";
                            returnValue += cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }

            return returnValue;
        }

        public static string GetUpdateScript(OrderTask task)
        {
            return $"update dbo.ordertasks set " +
                            $"taskstatus={(int)task.TaskStatus}, " +
                            $"notes='{task.Notes}', " +
                            $"lastupdatedby='{ApplicationVariables.LoggedInUser.Id}', " +
                            $"lastupdatedttm=GETDATE() " +
                            $"where oid = '{task.OrderId}' and taskcode = '{task.TaskCode}'";
        }

        public static string GetInsertScript(OrderTask orderTask)
        {
            return $"insert into dbo.ordertasks(oid,taskcode,taskstatus,notes,lastupdatedby,lastupdatedttm,createddate) values(" +
                        $"'{orderTask.OrderId}','{orderTask.TaskCode}',{(int)orderTask.TaskStatus},'{orderTask.Notes}','{orderTask.LastUpdatedBy}',GETDATE(),GETDATE()" +
                        $")";
        }
    }
}
