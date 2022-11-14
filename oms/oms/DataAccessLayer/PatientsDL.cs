using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo.DB.Helpers;
using oms.Model;
namespace oms.DataAccessLayer
{
    public static class PatientsDL
    {
        public static Patient Get(string mrn)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select *,i.name as insurancename from dbo.patients p " +
                        $"left join dbo.insurances i on (p.insuranceid = i.id) " +
                        $"where p.mrn = '{mrn}' and " +
                        $"p.cid = '{ApplicationVariables.WorkingClient.Id}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Patient.Create(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static Patient Get(Guid patientId)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select *,i.name as insurancename from dbo.patients p " +
                        $"left join dbo.insurances i on (p.insuranceid = i.id) " +
                        $"where p.pid = '{patientId}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Patient.Create(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static List<Patient> Get()
        {
            List<Patient> returnList = new List<Patient>();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select *, i.name as insurancename from dbo.patients p " +
                        $"left join dbo.insurances i on (p.insuranceid = i.id) " +
                        $"where p.cid = '{ApplicationVariables.WorkingClient.Id}'";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnList.Add(Patient.Create(reader));
                        }
                    }
                }
            }

            return returnList;
        }

        public static int AddOrUpdate(List<Patient> models)
        {
            int returnValue = 0;
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    models.ForEach(model =>
                    {
                        cmd.CommandText = CommonFunctions.GetAddOrUpdateScript(GetExistsScript(model), GetInsertScript(model), GetUpdateScript(model));
                        returnValue += cmd.ExecuteNonQuery();
                    });
                }
            }

            return returnValue;
        }

        public static int Delete(Guid pid)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString()))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"delete from dbo.patients where pid = '{pid}'";
                    int returnValue = cmd.ExecuteNonQuery();
                    return returnValue;
                }
            }
            return 0;
        }

        private static string GetExistsScript(Patient patient)
        {
            return $"select 1 from dbo.patients where mrn = '{patient.MRN}' and cid = {ApplicationVariables.WorkingClient.Id}";
        }
        
        private static string GetInsertScript(Patient patient)
        {
            return $"insert into dbo.patients(pid,mrn, cid,dob, firstname, lastname, email, phone1, phone2, address1, address2, address3, defaultaddresstype, insuranceid, notes, guardiandetails) values(" +
                        $"'{patient.Id}','{patient.MRN}',{ApplicationVariables.WorkingClient.Id},'{patient.DOB}','{patient.FirstName}','{patient.LastName}','{patient.Email}','{patient.Phone1}','{patient.Phone2}','{patient.Address1}','{patient.Address2}','{patient.Address3}',{patient.DefaultAddressType},{patient.InsuranceId},'{patient.Notes}','{patient.GuardianDetails}'" +
                        $")";
        }

        private static string GetUpdateScript(Patient patient)
        {
            return $"update dbo.patients set " +
                        $"mrn='{patient.MRN}'," +
                        $"dob='{patient.DOB}'," +
                        $"firstname='{patient.FirstName}'," +
                        $"lastname='{patient.LastName}'," +
                        $"email='{patient.Email}'," +
                        $"phone1='{patient.Phone1}'," +
                        $"phone2='{patient.Phone2}'," +
                        $"defaultaddresstype={patient.DefaultAddressType}," +
                        $"address1='{patient.Address1}'," +
                        $"address2='{patient.Address2}'," +
                        $"address3='{patient.Address3}'," +
                        $"insuranceid='{patient.InsuranceId}'," +
                        $"notes='{patient.Notes}', " +
                        $"guardiandetails='{patient.GuardianDetails}' " +
                        $"where pid = '{patient.Id}'";
        }
    }
}
