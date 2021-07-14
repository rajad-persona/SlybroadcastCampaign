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
        public const string Persona_Sub_Hourly_Job = @"select distinct original_customer_id ID,first_name,last_name,email,customer_category customer_category from [dbo].[TBL_BRAZE_CUSTOMER_DATA] where original_customer_id not in
                                                    (select distinct Customer_ID from TBL_DO_NOT_SEND_EMAIL_LIST where customer_id is not null ) and
                                                    original_customer_id not in (select distinct Customer_ID from VW_INVALID_CUSTOMERS where customer_id is not null )
                                                    and original_customer_id not in (select distinct Customer_ID from TBL_DO_NOT_CALL_LIST where customer_id is not null)
													and braze_date_added  >= DATEADD(DD, -1, GETDATE());";
        public const string MarketPlace_Unsub_Daily_Job = @"select c.CUSTOMER_ID ID,C.FIRST_NAME,c.LAST_NAME,c.EMAIL,TL.CREATED_DATE
                                                            FROM BCENTRAL.DBO.TBL_CUSTOMER_LIST C WITH (NOLOCK) JOIN TBL_DO_NOT_SEND_EMAIL_LIST TL on
                                                            C.CUSTOMER_ID=TL.CUSTOMER_ID where TL.CREATED_DATE >= dateadd(day,datediff(day,1,GETDATE()),0)
                                                            AND TL.CREATED_DATE < dateadd(day,datediff(day,0,GETDATE()),0);";
        public const string Persona_Unsub_Daily_Job = @";WITH CTE_GET_VALID_TRANSACTION_CUSTOMERS AS
                                                        (SELECT  CT.TRANSACTION_CUSTOMER_ID FROM TBL_CUSTOMER_TRANSACTION  CT WITH (NOLOCK)
                                                               WHERE (CT.TRANSACTION_STATUS IN ( 3, 5, 8 )  OR ( CT.TRANSACTION_STATUS = 6  AND ( CT.DOCKET_NUMBER IS NOT NULL  OR CT.SHIP_DATE IS NOT NULL ))))
                                                        , CTE_AUTOSHIP_PAUSE_DATA AS 
                                                        (SELECT DISTINCT SO.TRANSACTION_CUSTOMER_ID, CAST(MAX(SOASS.CREADTED_DATE) AS DATE) AS AUTOSHIP_PAUSE_DATE FROM TBL_STANDING_ORDERS SO  WITH (NOLOCK) 
                                                               JOIN TBL_STANDING_ORDER_AUTO_SHIP_STATUS SOASS  WITH (NOLOCK) ON SO.STANDING_ORDER_ID = SOASS.STANDING_ORDER_ID WHERE SO.AUTO_SHIP = 4 GROUP BY SO.TRANSACTION_CUSTOMER_ID)
                                                        SELECT DISTINCT CL.CUSTOMER_ID ID,CL.FIRST_NAME,CL.LAST_NAME,CL.EMAIL,CTE.AUTOSHIP_PAUSE_DATE UnSubscribedDate  FROM TBL_CUSTOMER_LIST CL WITH (NOLOCK) 
                                                        JOIN CTE_AUTOSHIP_PAUSE_DATA CTE ON CL.CUSTOMER_ID = CTE.TRANSACTION_CUSTOMER_ID JOIN CTE_GET_VALID_TRANSACTION_CUSTOMERS CT ON CT.TRANSACTION_CUSTOMER_ID = CTE.TRANSACTION_CUSTOMER_ID
                                                        WHERE CTE.AUTOSHIP_PAUSE_DATE >= Getdate()-2 and CTE.AUTOSHIP_PAUSE_DATE < Getdate()-1 AND NOT EXISTS (SELECT CL.CUSTOMER_ID FROM VW_INVALID_CUSTOMERS CL WHERE CL.CUSTOMER_ID= CTE.TRANSACTION_CUSTOMER_ID) 
                                                        AND NOT EXISTS ( SELECT DSE.CUSTOMER_ID FROM TBL_DO_NOT_SEND_EMAIL_LIST DSE WHERE DSE.CUSTOMER_ID = CTE.TRANSACTION_CUSTOMER_ID ) ";
        public const string WunderKind_get_History = @"select [FileName] from [TBL_WUNDERKIND_SUBSCRIBERS_BATCHJOB_LOG]";


        public const string WunderKind_bulk_insert = @"Declare @distinctEmails as table (Email nvarchar(50)) 
                                                        insert into @distinctEmails
                                                        select [Name] from [dbo].[splitstring]('{0}','|') as t
                                                        where t.Name not in (select Email from TBL_WUNDERKIND_SUBSCRIBERS) and t.name not in (select email from TBL_BRAZE_CUSTOMER_DATA)
                                                        insert into TBL_WUNDERKIND_SUBSCRIBERS(Email) select * from @distinctEmails
                                                        select ID,Email from TBL_WUNDERKIND_SUBSCRIBERS where Email in (select * from @distinctEmails)";

    }
}
