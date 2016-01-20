using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


public partial class BackTest : System.Web.UI.Page
{
    string Start_time_5;//週線頭
    string Start_time_20;//月線頭
    string Start_time_60;//季線頭
    string Start_time_15;//碎形用
    string End_time;//最後一日
    protected void Page_Load(object sender, EventArgs e)
    {
        //DataTable.Rows(0)
        //DataTable.Rows(DataTable.Rows.Count-1) 
    }
    protected void BackTest_btn_Click(object sender, System.EventArgs e)
    {
        Database.MSSQL DB = new Database.MSSQL("web");
        List<SqlParameter> PMS = new List<SqlParameter>();
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@tmp", 1));
        PMS.Add(new SqlParameter("@tmp_N", 100));
        DataTable Time_Temp = DB.QueryStoredProcedure("[Get_Time_N]", PMS.ToArray());


        /*for (int i = 1; i < Time_Temp.Rows.Count; i++) 
        {
            Response.Write(Time_Temp.Rows[i][0].ToString() + "<br />");
        }*/
        DataTable StockTime = DB.QuerySQL("SELECT TOP 20 證券代號 ,收盤價 ,資料日期 FROM Sourcebook WHERE 證券代號 = '1101' ORDEr BY 資料日期 DESC");

        

    }

    //private 
}