using DevExpress.Utils.DirectXPaint;
using DevExpress.Xpo.DB.Helpers;
using oms.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static DevExpress.XtraEditors.RoundedSkinPanel;

namespace oms.DataAccessLayer
{
    public static class OrdersDL
    {
        public static Order Get(string orderNumber, bool includeDetails = false)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GetOrderScript(includeDetails) + $" where ordernumber = '{orderNumber}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order = Order.Create(reader);                           
                            return order;
                        }
                    }
                }
            }

            return null;
        }

        public static Order Get(Guid orderId, bool includeDetails = false)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GetOrderScript(includeDetails) + $" where oid = '{orderId}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order = Order.Create(reader);
                            return order;
                        }
                    }
                }
            }

            return null;
        }

        public static List<OrderWithPatient> GetByPatientId(Guid patientId)
        {
            List<OrderWithPatient> returnList = new List<OrderWithPatient>();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GetOrderPatientScript() +
                        $"where p.pid='{patientId}' " +
                        $"order by o.confirmeddos desc";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderWithPatient.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }

        public static List<OrderWithPatient> GetCallList(int numberofdays)
        {
            List<OrderWithPatient> returnList = new List<OrderWithPatient>();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GetOrderPatientScript() +
                        $"where o.nextcalldate < DATEADD(DAY,{numberofdays},GETDATE()) " +
                        $"order by o.confirmeddos desc";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderWithPatient.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }

        public static List<OrderWithPatient> GetOrdersInProgress()
        {
            List<OrderWithPatient> returnList = new List<OrderWithPatient>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GetOrderPatientScript() +
                        $"where (o.orderstatus is null or o.orderstatus = 1) " +
                        $"order by o.confirmeddos desc";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(OrderWithPatient.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }

        public static int Add(Order order, List<OrderAssay> orderAssays, List<Tasks> tasks)
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

                        cmd.CommandText = GetInsertScript(order);
                        returnValue = cmd.ExecuteNonQuery();

                        orderAssays.ForEach(model =>
                        {
                            cmd.CommandText = OrderAssayDL.GetInsertScript(model);
                            returnValue += cmd.ExecuteNonQuery();
                        });

                        tasks.ForEach(model =>
                        {
                            cmd.CommandText = $"insert into dbo.ordertasks(oid,taskcode,idx,taskstatus,notes,lastupdatedby,lastupdatedttm,createddate) values(" +
                                    $"'{order.Id}','{model.Code}',{tasks.IndexOf(model)},{(int)E_TaskStatus.InProgress},'','{ApplicationVariables.LoggedInUser.Id}',GETDATE(),GETDATE()" +
                                    $")";
                            returnValue += cmd.ExecuteNonQuery();
                        });

                        cmd.CommandText = $"update dbo.patients set orderinprogress = orderinprogress + 1 where pid = (select pid from dbo.orders where oid = '{order.Id}')";
                        returnValue += cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            return returnValue;
        }

        public static int Update(Order order, List<OrderAssay> orderAssays)
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

                        cmd.CommandText = GetUpdateScript(order);
                        returnValue = cmd.ExecuteNonQuery();

                        cmd.CommandText = OrderAssayDL.GetDeleteScript(order.Id);
                        returnValue += cmd.ExecuteNonQuery();

                        orderAssays.ForEach(model =>
                        {
                            cmd.CommandText = OrderAssayDL.GetInsertScript(model);
                            returnValue += cmd.ExecuteNonQuery();
                        });
                    }

                    transaction.Commit();
                }
            }

            return returnValue;
        }

        private static string GetInsertScript(Order order)
        {
            return $"insert into dbo.orders" +
                           $"(oid,pid,ordernumber,dos,confirmeddos,confirmeddeliverydate,estimateddeliverydate,orderstatus," +
                           $"deliveryaddress,nextcalldate,dateordered,prophyorprn," +
                           $"drugid,manufacturerid,insuranceid,providerid,acceptableoutdatesid,id340B," +
                           $"totalunitprescribed,dosecount,cogperunit,billperunit," +
                           $"confirmationnumber,createdby,createddttm,lastupdatedby,lastupdatedttm, otherdetails) " +
                           $"values(" +
                           $"'{order.Id}','{order.PatientId}','{order.OrderNumber}','{order.DOS}','{order.ConfirmedDOS}','{order.ConfirmedDeliveryDate}','{order.EstimatedDeliveryDate}',{(int)order.OrderStatus}," +
                           $"'{order.DeliveryAddress}','{order.NextCallDate}','{order.DateOrdered}','{order.ProphyOrPRN}'," +
                           $"'{order.DrugId}','{order.ManufacturerId}','{order.InsuranceId}','{order.ProviderId}','{order.AcceptableOutdatesId}','{order.Id340B}'," +
                           $"{order.TotalPrescribedUnit},{order.DoseCount},{order.CogPerUnit},{order.BillPerUnit}," +
                           $"'{order.ConfirmationNumber}','{ApplicationVariables.LoggedInUser.Id}',GETDATE(),'{ApplicationVariables.LoggedInUser.Id}',GETDATE(),'{order.Details.ToString()}'" +
                           $")";
        }

        private static string GetUpdateScript(Order order)
        {
            return $"update dbo.orders set " +
                            $"ordernumber='{order.OrderNumber}', " +
                            $"dos='{order.DOS}'," +
                            $"confirmeddos='{order.ConfirmedDOS}'," +
                            $"confirmeddeliverydate='{order.ConfirmedDeliveryDate}'," +
                            $"estimateddeliverydate='{order.EstimatedDeliveryDate}'," +
                            $"deliveryaddress='{order.DeliveryAddress}'," +
                            $"nextcalldate='{order.NextCallDate}'," +
                            $"dateordered='{order.DateOrdered}'," +
                            $"prophyorprn='{order.ProphyOrPRN}', " +
                            $"drugid='{order.DrugId}', " +
                            $"manufacturerid='{order.ManufacturerId}', " +
                            $"insuranceid='{order.InsuranceId}', " +
                            $"providerid='{order.ProviderId}', " +
                            $"acceptableoutdatesid='{order.AcceptableOutdatesId}', " +
                            $"id340B='{order.Id340B}', " +
                            $"totalunitprescribed={order.TotalPrescribedUnit}, " +
                            $"dosecount={order.DoseCount}, " +
                            $"cogperunit={order.CogPerUnit}, " +
                            $"billperunit={order.BillPerUnit}, " +
                            $"confirmationnumber='{order.ConfirmationNumber}', " +
                            $"lastupdatedby='{ApplicationVariables.LoggedInUser.Id}', " +
                            $"lastupdatedttm=GETDATE()," +
                            $"otherdetails='{order.Details.ToString()}' " +
                            $"where oid = '{order.Id}'";
        }

        private static string GetOrderScript(bool includeDetails)
        {
            string otherDetailsExp = includeDetails ? "o.otherdetails," : "'' as otherdetails,";

            string returnScript = $"select o.oid,o.pid,o.ordernumber,o.dos,o.confirmeddos,o.confirmeddeliverydate,o.estimateddeliverydate," +
                   $"o.orderstatus,o.deliveryaddress,o.nextcalldate,o.dateordered,o.prophyorprn,o.confirmationnumber," +
                   $"o.totalunitprescribed,o.dosecount,o.cogperunit,o.billperunit," +
                   $"o.createdby,o.createddttm,{otherDetailsExp}o.lastupdatedby,o.lastupdatedttm," +
                   $"d.id as drugid,d.name as drugname," +
                   $"m.id as manufacturerid,m.name as manufacturername," +
                   $"i.id as insuranceid,i.name as insurancename," +
                   $"pr.id as providerid,pr.name as providername," +
                   $"o.acceptableoutdatesid,o.id340B," +
                   $"((select sum(assay) from dbo.orderassay oa WITH(NOLOCK) where oa.oid = o.oid) * o.dosecount) as totalunitsbilled, " +
                   $"(select sum(amount) from dbo.orderpaymentsinsurance opi WITH(NOLOCK) where opi.oid = o.oid) as totalpayments " +
                   $"from dbo.orders o WITH(NOLOCK) " +
                   $"inner join dbo.patients p WITH(NOLOCK) on (p.pid = o.pid and p.cid = '{ApplicationVariables.WorkingClient.Id}') " +
                   $"left join dbo.drugs d WITH(NOLOCK) on (o.drugid = d.id) " +
                   $"left join dbo.manufacturers m WITH(NOLOCK) on (o.manufacturerid = m.id) " +
                   $"left join dbo.insurances i WITH(NOLOCK) on (o.insuranceid = i.id) " +
                   $"left join dbo.providers pr WITH(NOLOCK) on (o.providerid = pr.id) ";

            return returnScript;    
        }

        private static string GetOrderPatientScript()
        {
            return $"select p.pid,p.mrn,p.dob,p.firstname,p.lastname,p.email,o.insuranceid,i.name as insurancename,o.oid," +
                   $"o.ordernumber,o.dos,o.confirmeddos,o.confirmeddeliverydate,o.estimateddeliverydate," +
                   $"o.orderstatus,o.deliveryaddress,o.nextcalldate,o.dateordered,o.prophyorprn," +
                   $"o.confirmationnumber,o.deliveryaddress,o.nextcalldate,o.prophyorprn," +
                   $"o.totalunitprescribed,o.dosecount,o.cogperunit,o.billperunit," +
                   $"o.totalunitprescribed,o.cogperunit,o.billperunit," +
                   $"o.createdby,o.createddttm,o.lastupdatedby,o.lastupdatedttm,'' as otherdetails," +
                   $"d.id as drugid,d.name as drugname," +
                   $"m.id as manufacturerid,m.name as manufacturername," +
                   $"pr.id as providerid,pr.name as providername," +
                   $"o.acceptableoutdatesid,o.id340B," +
                   $"(select top 1 os.taskcode from dbo.ordertasks os WITH(NOLOCK) where os.oid = o.oid and os.taskstatus <> 2 order by os.idx) as lasttaskcode, " +
                   $"((select sum(assay) from dbo.orderassay oa WITH(NOLOCK) where oa.oid = o.oid) * o.dosecount) as totalunitsbilled, " +
                   $"(select sum(amount) from dbo.orderpaymentsinsurance opi WITH(NOLOCK) where opi.oid = o.oid) as totalpayments " +
                   $"from dbo.orders o WITH(NOLOCK) " +
                   $"inner join dbo.patients p WITH(NOLOCK) on (p.pid = o.pid and p.cid = {ApplicationVariables.WorkingClient.Id}) " +
                   $"left join dbo.drugs d WITH(NOLOCK) on (o.drugid = d.id) " +
                   $"left join dbo.insurances i WITH(NOLOCK) on (o.insuranceid = i.id) " +
                   $"left join dbo.manufacturers m WITH(NOLOCK) on (o.manufacturerid = m.id) " +
                   $"left join dbo.providers pr WITH(NOLOCK) on (o.providerid = pr.id) ";
        }
    }
}
