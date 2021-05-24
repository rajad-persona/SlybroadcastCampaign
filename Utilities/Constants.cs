using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Constants
    {
        public const string Attentive_weekly_Job = "USP_GET_ATTENTIVE_WEEKLY_DATA";
        public const string WunderKind_Hourly_Job = @"select distinct first_name,last_name,email from [dbo].[TBL_BRAZE_CUSTOMER_DATA] where original_customer_id not in
                                                    (select distinct Customer_ID from TBL_DO_NOT_SEND_EMAIL_LIST where customer_id is not null ) and
                                                    original_customer_id not in (select distinct Customer_ID from VW_INVALID_CUSTOMERS where customer_id is not null )
                                                    and original_customer_id not in (select distinct Customer_ID from TBL_DO_NOT_CALL_LIST where customer_id is not null)
													and braze_date_added  >= DATEADD(HH, -1, GETDATE());";
    }
}
