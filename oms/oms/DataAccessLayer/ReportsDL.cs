using oms.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oms.DataAccessLayer
{
    public static class ReportsDL
    {
        public static List<Report_MonthlyInvoice_Model> GetMonthlyInvoiceReport(DateTime startDate, DateTime endDate)
        {
            DateTime sDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            DateTime eDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            List<Report_MonthlyInvoice_Model> returnList = new List<Report_MonthlyInvoice_Model>();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select o.*,oa.*," +
                        $"p.lastname as lastname,p.firstname as firstname," +
                        $"d.id as drugid,d.name as drugname," +
                        $"m.id as manufacturerid,m.name as manufacturername," +
                        $"i.id as insuranceid, i.name as insurancename " +
                        $"from dbo.orders o " +
                        $"left join dbo.patients p on (p.pid = o.pid) " +
                        $"left join dbo.orderassay oa on (oa.oid = o.oid) " +
                        $"left join dbo.drugs d on (o.drugid = d.id) " +
                        $"left join dbo.manufacturers m on (o.manufacturerid = m.id) " +
                        $"left join dbo.insurances i on (o.insuranceid = i.id) " +
                        $"where o.dos >= '{sDate}' and o.dos <= '{eDate}' " +
                        $"and oa.assay <> 0 " +
                        $"and p.cid = {ApplicationVariables.WorkingClient.Id}";

                    using(SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while(rdr.Read())
                        {
                            Report_MonthlyInvoice_Model report_MonthlyInvoice_Model = returnList.FirstOrDefault(x => x.OrderId.Equals(CommonFunctions.GetGuidSafely(rdr["oid"])));

                            if (report_MonthlyInvoice_Model == null)
                            {
                                report_MonthlyInvoice_Model = new Report_MonthlyInvoice_Model()
                                {
                                    DOS = CommonFunctions.GetDateTimeSafely(rdr["dos"]),
                                    DrugId = CommonFunctions.GetIntSafely(rdr["drugid"]),
                                    DrugName = CommonFunctions.GetStringSafely(rdr["drugname"]),
                                    InsuranceId = CommonFunctions.GetIntSafely(rdr["insuranceid"]),
                                    InsuranceName = CommonFunctions.GetStringSafely(rdr["insurancename"]),
                                    OrderId = CommonFunctions.GetGuidSafely(rdr["oid"]),
                                    PatientId = CommonFunctions.GetGuidSafely(rdr["pid"]),
                                    OrderNumber = CommonFunctions.GetStringSafely(rdr["ordernumber"]),
                                    PatientName = CommonFunctions.GetStringSafely(rdr["lastname"]) + ", " + CommonFunctions.GetStringSafely(rdr["firstname"]),
                                    TotalPrescribed = CommonFunctions.GetFloatSafely(rdr["totalunitprescribed"]) * CommonFunctions.GetFloatSafely(rdr["dosecount"])
                                };

                                returnList.Add(report_MonthlyInvoice_Model);
                            }

                            if (report_MonthlyInvoice_Model != null)
                            {
                                string assayId = CommonFunctions.GetStringSafely(rdr["assayid"]);
                                if (assayId.IsStringEqual("assay1"))
                                {
                                    report_MonthlyInvoice_Model.RX1 = CommonFunctions.GetStringSafely(rdr["rxnumber"]);
                                }
                                if (assayId.IsStringEqual("assay2"))
                                {
                                    report_MonthlyInvoice_Model.RX2 = CommonFunctions.GetStringSafely(rdr["rxnumber"]);
                                }
                                if (assayId.IsStringEqual("assay3"))
                                {
                                    report_MonthlyInvoice_Model.RX3 = CommonFunctions.GetStringSafely(rdr["rxnumber"]);
                                }

                                report_MonthlyInvoice_Model.TotalUnitsBilled += CommonFunctions.GetFloatSafely(rdr["assay"]) * CommonFunctions.GetFloatSafely(rdr["qty"]);
                                report_MonthlyInvoice_Model.AmountBilled += CommonFunctions.GetFloatSafely(rdr["billperunit"]) * CommonFunctions.GetFloatSafely(rdr["assay"]) * CommonFunctions.GetFloatSafely(rdr["qty"]);
                            }
                        }
                    }

                }
            }

            return returnList;
        }

        public static List<Report_Purchasing_Model> GetPurchasingReport(DateTime startDate, DateTime endDate)
        {
            DateTime sDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            DateTime eDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            List<Report_Purchasing_Model> returnList = new List<Report_Purchasing_Model>();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select o.*, oa.*," +
                        $"d.name as drugname," +
                        $"m.name as manufacturername," +
                        $"o.id340B " +
                        $"from dbo.orders o " +
                        $"left join dbo.patients p on (p.pid = o.pid) " +
                        $"left join dbo.orderassay oa on (o.oid = oa.oid) " +
                        $"left join dbo.drugs d on (o.drugid = d.id) " +
                        $"left join dbo.manufacturers m on (o.manufacturerid = m.id) " +
                        $"where o.dateordered >= '{sDate}' and o.dateordered <= '{eDate}' " +
                        $"and oa.assay <> 0 " +
                        $"and p.cid = {ApplicationVariables.WorkingClient.Id}";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Report_Purchasing_Model report = new Report_Purchasing_Model()
                            {
                                Assay = CommonFunctions.GetFloatSafely(reader["Assay"]),
                                COGPerUnit = CommonFunctions.GetFloatSafely(reader["cogperunit"]),
                                ConfirmationNumber = CommonFunctions.GetStringSafely(reader["confirmationnumber"]),
                                DateOrdered = CommonFunctions.GetDateTimeSafely(reader["dateordered"]),
                                DrugId = CommonFunctions.GetIntSafely(reader["drugid"]),
                                DrugName = CommonFunctions.GetStringSafely(reader["drugname"]),
                                Id340B = CommonFunctions.GetIntSafely(reader["id340B"]),
                                Id340BName = StaticListDL.GetName(Constants.TableName_id340B, CommonFunctions.GetIntSafely(reader["id340B"])),
                                ExpDate = CommonFunctions.GetDateTimeSafely(reader["expdate"]),
                                Lot = CommonFunctions.GetStringSafely(reader["lot"]),
                                ManufacturerId = CommonFunctions.GetIntSafely(reader["manufacturerid"]),
                                ManufacturerName = CommonFunctions.GetStringSafely(reader["manufacturername"]),
                                NDC = CommonFunctions.GetStringSafely(reader["ndc"]),
                                OrderId = CommonFunctions.GetGuidSafely(reader["oid"]),
                                OrderNumber = CommonFunctions.GetStringSafely(reader["ordernumber"]),
                                ProphyPRN = CommonFunctions.GetStringSafely(reader["prophyorprn"]),
                                Quantity = CommonFunctions.GetFloatSafely(reader["qty"]),
                                TransactionType = "",
                                RCDate = CommonFunctions.GetDateTimeSafely(reader["dateordered"]).AddDays(1),
                                PatientId = CommonFunctions.GetGuidSafely(reader["pid"])
                            };

                            returnList.Add(report);
                        }
                    }
                }
            }

            return returnList;
        }
    }

    public class Report_Purchasing_Model
    {
        public Guid OrderId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime DateOrdered { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }   
        public string ConfirmationNumber { get; set; }
        public string OrderNumber { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public float Assay { get; set; }
        public string NDC { get; set; }
        public string Lot { get; set; }
        public DateTime ExpDate { get; set; }
        public string TransactionType { get; set; }
        public DateTime RCDate { get; set; }
        public float Quantity { get; set; }
        public string ProphyPRN { get; set; }
        public float COGPerUnit { get; set; }
        public int Id340B { get; set; }
        public string Id340BName { get; set; }
    }

    public class Report_MonthlyInvoice_Model
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public int InsuranceId { get; set; }
        public string InsuranceName { get; set; }
        public DateTime DOS { get; set; }
        public double AmountBilled { get; set; }
        public string RX1 { get; set; }
        public string RX2 { get; set; }
        public string RX3 { get; set; }
        public double TotalPrescribed { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public double TotalUnitsBilled { get; set; }
    }
}
