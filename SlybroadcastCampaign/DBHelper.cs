using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SlybroadcastCampaign
{
    public class DBHelper
    {
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
                Campaign_Name=row["Campaign_Name"]?.ToString()
            }).ToList();
            return resp;
        }
    }
}
