using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Utilities
{
    public class DBHelper
    {
        public List<string> GetSlyCampaigns()
        {
            DataTable table = new DataTable();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString))
            using (var cmd = new SqlCommand("USP_GET_SLYBROADCAST_SESSIONID", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                da.Fill(table);
            }
            var resp = table.AsEnumerable().Select(row =>
                row["Session_id"]?.ToString()?.Trim()
            ).ToList();
            return resp;
        }

        public List<Customer> GetCustomers()
        {
            DataTable table = new DataTable();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString))
            using (var cmd = new SqlCommand("USP_GET_SLYBROADCAST_CUSTOMERS_DETAILS", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                da.Fill(table);
            }
            var resp = table.AsEnumerable().Select(row => new Customer
            {
                CUSTOMER_ID = Convert.ToInt64(row["CUSTOMER_ID"]),
                CUST_SHIP_COUNTRY_CODE = row["CUST_SHIP_COUNTRY_CODE"]?.ToString(),
                EMAIL = row["EMAIL"]?.ToString(),
                FIRST_NAME = row["FIRST_NAME"]?.ToString(),
                LAST_NAME = row["LAST_NAME"]?.ToString(),
                Message = row["Message"]?.ToString(),
                MOBILE = row["MOBILE"]?.ToString(),
                Campaign_Name = row["Campaign_Name"]?.ToString()
            }).ToList();
            return resp;
        }

        public List<string> GetAttentiveCustomers()
        {
            var resp = new List<string>();
            try
            {
                DataTable table = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString))
                using (var cmd = new SqlCommand("[USP_GET_ATTENTIVE_CUSTOMERS_DETAILS]", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
                resp = table.AsEnumerable().Select(row => row["MOBILE"]?.ToString()).ToList();

            }
            catch (Exception ex) { }
            return resp;

        }

        public bool AddLog(string campaignResponse, string campaignName, string message, string SESSION_ID, string CAMPAIGN_RESULT)
        {
            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("[USP_ADD_SLYBROADCAST_LOGS]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@SLY_RESPONSE", SqlDbType.NVarChar));
                        cmd.Parameters["@SLY_RESPONSE"].Value = campaignResponse;
                        cmd.Parameters.Add(new SqlParameter("@CAMPAIGN_NAME", SqlDbType.NVarChar));
                        cmd.Parameters["@CAMPAIGN_NAME"].Value = campaignName;
                        cmd.Parameters.Add(new SqlParameter("@VOICE_MESSAGE", SqlDbType.NVarChar));
                        cmd.Parameters["@VOICE_MESSAGE"].Value = message;
                        cmd.Parameters.Add(new SqlParameter("@SESSION_ID", SqlDbType.NVarChar));
                        cmd.Parameters["@SESSION_ID"].Value = SESSION_ID;
                        cmd.Parameters.Add(new SqlParameter("@CAMPAIGN_RESULT", SqlDbType.NVarChar));
                        cmd.Parameters["@CAMPAIGN_RESULT"].Value = CAMPAIGN_RESULT;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public DataSet GetAttentiveWeeklyData()
        {
            DataSet ds = new DataSet();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString))
            using (var cmd = new SqlCommand("USP_GET_ATTENTIVE_WEEKLY_DATA", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                da.Fill(ds);
            }
            return ds;
        }

        public DataSet GetDataSet(string query, bool isStroedproc = true, int? timeOut = null)
        {
            DataSet ds = new DataSet();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString))
            using (var cmd = new SqlCommand(query, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (isStroedproc)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                if (timeOut != null)
                {
                    cmd.CommandTimeout = timeOut ?? 30;
                }

                da.Fill(ds);
            }
            return ds;
        }


    }
}
