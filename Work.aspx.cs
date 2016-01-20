using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;


public partial class Work : System.Web.UI.Page
{
    DataTable StockList;//取得有最近120筆資料的股票列表
    DataTable StockTime;//取得最近120比日期
    DataTable StockData;
    string Start_time_5;//週線頭
    string Start_time_20;//月線頭
    string Start_time_60;//季線頭
    string Start_time_15;//碎形用
    string End_time;//最後一日
    DataTable Stock_AVG_5;//5日平均
    DataTable Stock_AVG_20;//20日平均
    DataTable Stock_AVG_60;//60日平均
    DataTable Stock_AVG_15;//15日平均
    DataTable Stock_Week_20AVG;//20週平均
    //DataTable 月突破計算表;//月突破各股列表
    Database.MSSQL DB = new Database.MSSQL("web");
    int Day_Count;

    protected void Page_Load(object sender, EventArgs e)
    {

        //StockTime = DB.QuerySQL("SELECT TOP 1 convert(char(10) ,資料日期,111) as 資料日期 from [dbo].PYS ORDER BY PYS.資料日期 DESC");
        StockTime = DB.QuerySQL("SELECT TOP 120 convert(char(10) ,資料日期,111) as 資料日期 FROM [Analysis].[dbo].[Sourcebook] GROUP BY 資料日期 ORDER BY 資料日期 DESC");
        End_time = StockTime.Rows[0][0].ToString();
        Start_time_5 = StockTime.Rows[4][0].ToString();
        Start_time_15 = StockTime.Rows[14][0].ToString();
        Start_time_20 = StockTime.Rows[19][0].ToString();
        Start_time_60 = StockTime.Rows[59][0].ToString();
        //StockList = DB.QuerySQL("SELECT 證券代號 FROM [dbo].PYS WHERE 資料日期 = '" + End_time + "' ORDER BY 證券代號 ASC");


        List<SqlParameter> PMS = new List<SqlParameter>();
        //取得有最近120筆資料的股票列表
        //PMS = new List<SqlParameter>();
        //PMS.Add(new SqlParameter("@End_Time", End_time));
        //StockList = DB.QueryStoredProcedure("[Get_Stock_List]", PMS.ToArray());
        StockList = DB.QueryStoredProcedure("[Get_Basis_StockList]", PMS.ToArray());


        //計算5日平均
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@Start_Time", Start_time_5));
        PMS.Add(new SqlParameter("@End_Time", End_time));
        Stock_AVG_5 = DB.QueryStoredProcedure("[Stock_AVG]", PMS.ToArray());

        //計算20日平均
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@Start_Time", Start_time_20));
        PMS.Add(new SqlParameter("@End_Time", End_time));
        Stock_AVG_20 = DB.QueryStoredProcedure("[Stock_AVG]", PMS.ToArray());

        //計算60日平均
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@Start_Time", Start_time_60));
        PMS.Add(new SqlParameter("@End_Time", End_time));
        Stock_AVG_60 = DB.QueryStoredProcedure("[Stock_AVG]", PMS.ToArray());

        //計算15日平均
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@Start_Time", Start_time_15));
        PMS.Add(new SqlParameter("@End_Time", End_time));
        Stock_AVG_15 = DB.QueryStoredProcedure("[Stock_AVG]", PMS.ToArray());

        //計算20週平均
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@End_Time", End_time));
        Stock_Week_20AVG = DB.QueryStoredProcedure("[Get_20Week_AVG]", PMS.ToArray());

        //int TS = new TimeSpan((Convert.ToDateTime(StockTime.Rows[4][0].ToString())).Ticks - (Convert.ToDateTime(StockTime.Rows[0][0].ToString())).Ticks).Days;

        //DataTime_Source_lbl.Text = TS.ToString();
        DataTime_Source_lbl.Text = End_time;
        //Label1.Text = StockList.Rows[1][0].ToString();
    }
    protected void Work_btn_Click(object sender, EventArgs e)//找出碎形高低價
    {
        string[,] Templist = new string[StockList.Rows.Count, 7];
        for (int i = 0; i < StockList.Rows.Count; i++)
        {
            //StockData.Rows[i][0] = StockList.Rows[i][0];
            Templist[i, 0] = StockList.Rows[i][0].ToString();
            //Database.MSSQL DB = new Database.MSSQL("web");
            List<SqlParameter> PMS = new List<SqlParameter>();
            PMS = new List<SqlParameter>();
            PMS.Add(new SqlParameter("@證券代號", StockList.Rows[i][0].ToString()));
            DataTable StockData_Temp = DB.QueryStoredProcedure("[Get_Stock_Data]", PMS.ToArray());
            int high_fiag = 0;
            int low_fiag = 0;
            int x = 0;
            int y = 0;
            float DAY_AVG_20 = 0;
            float DAY_AVG_15 = 0;
            float DAY_AVG_15_Temp = 0;
            string Break_Date_High = "";
            string Break_Date_Low = "";

            for (int j = 2; j < (StockData_Temp.Rows.Count - 2); j++)//2015-4-14
            {

                if ((Convert.ToSingle(StockData_Temp.Rows[j][1].ToString()) > Convert.ToSingle(StockData_Temp.Rows[j - 1][1].ToString())) && (Convert.ToSingle(StockData_Temp.Rows[j][1].ToString()) > Convert.ToSingle(StockData_Temp.Rows[j - 2][1].ToString())) && (high_fiag == 0))
                {
                    if ((Convert.ToSingle(StockData_Temp.Rows[j][1].ToString()) > Convert.ToSingle(StockData_Temp.Rows[j + 1][1].ToString())) && (Convert.ToSingle(StockData_Temp.Rows[j][1].ToString()) > Convert.ToSingle(StockData_Temp.Rows[j + 2][1].ToString())))
                    {
                        DAY_AVG_15_Temp = 0;
                        for (int AVG_15_Temp = 0; AVG_15_Temp < 15; AVG_15_Temp++)
                        {
                            DAY_AVG_15_Temp += Convert.ToSingle(StockData_Temp.Rows[j + AVG_15_Temp][3].ToString());
                        }
                        if (Convert.ToSingle(StockData_Temp.Rows[j][1].ToString()) > (DAY_AVG_15_Temp / 15))
                        {
                            //high_price = Convert.ToInt32(StockData_Temp.Rows[j][1].ToString());
                            Templist[i, 1] = StockData_Temp.Rows[j][1].ToString();
                            high_fiag = 1;
                            Break_Date_High = StockData_Temp.Rows[j][5].ToString();
                        }
                    }
                }

                if ((Convert.ToSingle(StockData_Temp.Rows[j][2].ToString()) < Convert.ToSingle(StockData_Temp.Rows[j - 1][2].ToString())) && (Convert.ToSingle(StockData_Temp.Rows[j][2].ToString()) < Convert.ToSingle(StockData_Temp.Rows[j - 2][2].ToString())) && (low_fiag == 0))
                {
                    if ((Convert.ToSingle(StockData_Temp.Rows[j][2].ToString()) < Convert.ToSingle(StockData_Temp.Rows[j + 1][2].ToString())) && (Convert.ToSingle(StockData_Temp.Rows[j][2].ToString()) < Convert.ToSingle(StockData_Temp.Rows[j + 2][2].ToString())))
                    {
                        DAY_AVG_15_Temp = 0;
                        for (int AVG_15_Temp = 0; AVG_15_Temp < 15; AVG_15_Temp++)
                        {
                            DAY_AVG_15_Temp += Convert.ToSingle(StockData_Temp.Rows[j + AVG_15_Temp][3].ToString());
                        }
                        if (Convert.ToSingle(StockData_Temp.Rows[j][1].ToString()) < (DAY_AVG_15_Temp / 15))
                        {
                            //low_price = Convert.ToInt32(StockData_Temp.Rows[j][2].ToString());
                            Templist[i, 2] = StockData_Temp.Rows[j][2].ToString();
                            low_fiag = 1;
                            Break_Date_Low = StockData_Temp.Rows[j][5].ToString();
                        }
                    }
                }
                //Response.Write("Hello " + Server.HtmlEncode(Request.QueryString["UserName"]) + "<br>");
                if (x < 20)
                {
                    DAY_AVG_20 = DAY_AVG_20 + Convert.ToSingle(StockData_Temp.Rows[x][3].ToString());
                    x = x + 1;
                }
                if (y < 15)
                {
                    DAY_AVG_15 = DAY_AVG_15 + Convert.ToSingle(StockData_Temp.Rows[y][3].ToString());
                    y = y + 1;
                }


            }
            //2015-4-14

            //if (Convert.ToSingle(StockData_Temp.Rows[0][3].ToString()) > (DAY_AVG_20/20))
            //{
            string totle = string.Format("證券代號：{0} ，碎形高點：{1:f2} ，碎形低點：{2:f2} ，20日均價：{3:f2} ，15日均價：{4:f2}", Templist[i, 0], Convert.ToSingle(Templist[i, 1]), Convert.ToSingle(Templist[i, 2]), (DAY_AVG_20 / 20), (DAY_AVG_15 / 15));
            Response.Write(totle);
            if (Convert.ToSingle(StockData_Temp.Rows[0][3].ToString()) > Convert.ToSingle(Templist[i, 1]))
            {
                Response.Write("<font size='-1' color='#FF0000' style='font-size: 18pt'>突破碎形</font>");
            }
            else if (Convert.ToSingle(StockData_Temp.Rows[0][3].ToString()) < Convert.ToSingle(Templist[i, 2]))
            {
                Response.Write("<font size='-1' color='#00FF00' style='font-size: 18pt'>跌破碎形</font>");
            }
            else
            {
                Response.Write("<font size='-1' color='#000000' >箱型盤整</font>");
            }
            Response.Write("突破日期" + Break_Date_High);
            Response.Write("跌破日期" + Break_Date_Low);
            //Response.Write(totle + "<br />");
            Response.Write("<br />");
            //}
        }
        //DataTable C_D = DB.QueryStoredProcedure("[Select_Work_Date]", PMS.ToArray());
    }


    protected void Analysis_Click(object sender, EventArgs e)
    {
        //Database.MSSQL DB = new Database.MSSQL("web");
        List<SqlParameter> PMS = new List<SqlParameter>();
        DataTable 月線上揚配布林表 = DB.QueryStoredProcedure("[月線上揚配布林]", PMS.ToArray());
        Response.Write("月線上揚搭配布林<br />");

        Response.Write("證券代號<br />");
        if (月線上揚配布林表 != null)
        {
            for (int i = 0; i < 月線上揚配布林表.Rows.Count; i++)
            {
                Response.Write(月線上揚配布林表.Rows[i][0] + "<br />");
            }
        }


        DataTable 出量訊號表 = DB.QueryStoredProcedure("[出量訊號]", PMS.ToArray());
        Response.Write("出量訊號表<br />");

        Response.Write("證券代號<br />");
        if (出量訊號表 != null)
        //if (出量訊號表.Rows.Count != 0) 
        {
            for (int i = 0; i < 出量訊號表.Rows.Count; i++)
            {
                Response.Write(出量訊號表.Rows[i][0] + "<br />");

            }
        }


    }


    /*================================
    * 1.收盤價要在月均價以下
    * 2.收盤量要在五日均量以下
    * 3.收盤價*1.1要大於月均價
    * 4.計算預期突破價格提供參考
    ================================*/
    protected void WeekLine_Click(object sender, EventArgs e)
    {
        /*Database.MSSQL DB = new Database.MSSQL("web");
        List<SqlParameter> PMS = new List<SqlParameter>();

        //取得有最近120筆資料的股票列表
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@End_Time", End_time));
        DataTable StockList_WeekLine = DB.QueryStoredProcedure("[Get_Stock_List]", PMS.ToArray());
        string[] underweek_List = new string[StockList_WeekLine.Rows.Count];
        for (int i = 0; i < StockList_WeekLine.Rows.Count; i++) 
        {
            PMS = new List<SqlParameter>();
            PMS.Add(new SqlParameter("@證券代號", StockList_WeekLine.Rows[i][0]));
            DataTable Stock_Data_single = DB.QueryStoredProcedure("[Get_Stock_Data]", PMS.ToArray());
            //DataSet ds = new DataSet();
            //ds.Tables.Add(Stock_Data_single);
        }2015/10/23*/
        //Database.MSSQL DB = new Database.MSSQL("web");
        List<SqlParameter> PMS = new List<SqlParameter>();
        DataTable 月突破計算表 = DB.QueryStoredProcedure("[月突破計算]", PMS.ToArray());
        Response.Write("月突破計算<br />");
        Response.Write("<table>");// style=" + '"' + "width:100%" + '"' + "
        Response.Write("<tr>");
        Response.Write("<th>證券代號</th>");
        Response.Write("<th>成交量</th>");
        Response.Write("<th>收盤價</th>");
        Response.Write("<th>平均價</th>");
        Response.Write("<th>平均量</th>");
        Response.Write("<th>隔日漲停價</th>");
        Response.Write("<th>突破價</th>");
        Response.Write("<th>資料日期</th>");
        Response.Write("</tr>");
        if (月突破計算表 != null)
        {
            for (int i = 0; i < 月突破計算表.Rows.Count; i++)
            {
                Response.Write("<tr>");
                Response.Write("<th>" + 月突破計算表.Rows[i][0] + "</th>");
                Response.Write("<th>" + 月突破計算表.Rows[i][1] + "</th>");
                Response.Write("<th>" + Convert.ToSingle(月突破計算表.Rows[i][2]).ToString("0.00") + "</th>");
                Response.Write("<th>" + Convert.ToSingle(月突破計算表.Rows[i][3]).ToString("0.00") + "</th>");
                Response.Write("<th>" + 月突破計算表.Rows[i][4] + "</th>");
                Response.Write("<th>" + Convert.ToSingle(月突破計算表.Rows[i][5]).ToString("0.00") + "</th>");
                Response.Write("<th>" + Convert.ToSingle(月突破計算表.Rows[i][6]).ToString("0.00") + "</th>");
                Response.Write("<th>" + Convert.ToDateTime(月突破計算表.Rows[i][7]).ToString("yyyy/MM/dd") + "</th>");
                Response.Write("</tr>");
            }
        }
        Response.Write("</table>");
    }
    protected void FeedBack_Test_Click(object sender, EventArgs e)
    {
        int test_total = Convert.ToInt16(FeedBack_txt.Text.ToString());
        //輸入投資N天的N
        Day_Count = Convert.ToInt16(NDay_Count.Text.ToString());
        //Database.MSSQL DB = new Database.MSSQL("web");
        double[] Final_Avg = new double[test_total];
        int Avg_Tmp = 0;
        //驗證次數
        for (int test_total_tmp = test_total + Day_Count; test_total_tmp >= (Day_Count+1); test_total_tmp--)
        //for (int test_total_tmp = test_total + 1; test_total_tmp >= 2; test_total_tmp--)
        {
            int Conform_Count = 0;//總個符合個數
            int Test_Count = 0;//總測試個數
            List<SqlParameter> PMS = new List<SqlParameter>();
            PMS = new List<SqlParameter>();
            PMS.Add(new SqlParameter("@N", test_total_tmp));
            //將回測日期的標的篩選出來
            DataTable 回測暫存資料表 = DB.QueryStoredProcedure("[月突破計數回測]", PMS.ToArray());
            //Response.Write("回測次數倒數：" + (test_total_tmp - 1) + "，資料日期：" + Convert.ToDateTime(回測暫存資料表.Rows[0][7]).ToString("yyyy/MM/dd") + "，系統總撈出筆數：" + 回測暫存資料表.Rows.Count + "<br />");
            Response.Write("回測次數倒數：" + (test_total_tmp - Day_Count) + "，資料日期：" + Convert.ToDateTime(回測暫存資料表.Rows[0][7]).ToString("yyyy/MM/dd") + "，系統總撈出筆數：" + 回測暫存資料表.Rows.Count + "<br />");
            //進行標的測試
            for (int i = 0; i < 回測暫存資料表.Rows.Count; i++)
            {
                //找出當日的基本面資料
                if (FeedBack_Fundamental(回測暫存資料表.Rows[i][0].ToString(), test_total_tmp))
                {
                    //找出標的後一日的資料
                    string[] array_tmp = FeedBack_testing(回測暫存資料表.Rows[i][0].ToString(), test_total_tmp - 1);
                    //string[] array_tmp = FeedBack_testing(回測暫存資料表.Rows[i][0].ToString(), test_total_tmp - 3);
                                       
                    
                    //若資料為空則不計算
                    if (array_tmp != null)
                    {
                        //先找出有超過突破價者
                        if (Convert.ToSingle(array_tmp[2]) >= Convert.ToSingle(回測暫存資料表.Rows[i][6]))
                        {
                            //計算持有N天內最高價
                            Single highPrice = FeedBack_ThreeDayHighPrice(回測暫存資料表.Rows[i][0].ToString(), test_total_tmp);
                            if (highPrice != 0) 
                            {
                                //比較後N日的最高價是否有達到預期利潤
                                if ((highPrice - Convert.ToSingle(回測暫存資料表.Rows[i][6])) / Convert.ToSingle(回測暫存資料表.Rows[i][6]) >= Convert.ToSingle(Return_value.Text))
                                //if ((Convert.ToSingle(array_tmp[2]) - Convert.ToSingle(回測暫存資料表.Rows[i][6])) / Convert.ToSingle(回測暫存資料表.Rows[i][6]) >= Convert.ToSingle(Return_value.Text))
                                {
                                    Response.Write("代號：" + 回測暫存資料表.Rows[i][0] + "<br />");
                                    Conform_Count++;
                                }
                                else
                                {
                                    Response.Write("<font color='#FF0000'>代號：</font>" + 回測暫存資料表.Rows[i][0] + "<br />");
                                }
                                Test_Count++;
                            }
                        }
                    }
                }
            }
            //Response.Write("測試筆數：" + Test_Count + "，達成筆數：" + Conform_Count + "<br />");
            //if (Test_Count == 0) { Test_Count = 1; }//若沒有測試筆數,則設定測試為1避免無法計算

            if (Test_Count != 0)
            {
                Response.Write("測試筆數：" + Test_Count + "，達成筆數：" + Conform_Count + "<br />");
                double Export = (Convert.ToSingle(Conform_Count) / Convert.ToSingle(Test_Count)) * 100;
                Final_Avg[Avg_Tmp] = Export;
                Avg_Tmp++;
                Response.Write("成功比例：" + Export.ToString("0.00") + "%<br />");
                Response.Write("<br />");
            }
            else 
            {
                Response.Write("<br />");
            }
            //double Export = (Convert.ToSingle(Conform_Count) / Convert.ToSingle(Test_Count)) * 100;
            //Final_Avg[Avg_Tmp] = Export;
            //Avg_Tmp++;
            
            //Response.Write("成功比例：" + Export.ToString("0.00") + "%<br />");
            //Response.Write("<br />");
            回測暫存資料表.Clear();
        }
        double Final_AvgPrice_Tmp = 0;
        for (int x = 0; x < Avg_Tmp; x++)
        {
            Final_AvgPrice_Tmp = Final_AvgPrice_Tmp + Final_Avg[x];
        }

        double Final_AvgPrice = Final_AvgPrice_Tmp / Avg_Tmp;

        Response.Write("測試平均成功率：" + Final_AvgPrice.ToString("0.00") + "%<br />");

    }
    protected void Clear_btn_Click(object sender, EventArgs e)
    {
        //月突破計算表.Clear();
    }


    private string[] FeedBack_testing(string Code_Num, int count)
    {
        List<SqlParameter> PMS = new List<SqlParameter>();
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@Stock_Code", Code_Num));
        PMS.Add(new SqlParameter("@N", count));
        DataTable 個股驗證結果 = DB.QueryStoredProcedure("[個股驗證]", PMS.ToArray());
        if (個股驗證結果.Rows.Count != 0)
        {
            String[] 結果陣列 = new string[] { 個股驗證結果.Rows[0][0].ToString(), 個股驗證結果.Rows[0][1].ToString(), 個股驗證結果.Rows[0][2].ToString(), 個股驗證結果.Rows[0][3].ToString(), 個股驗證結果.Rows[0][4].ToString(), 個股驗證結果.Rows[0][5].ToString(), 個股驗證結果.Rows[0][6].ToString() };
            return 結果陣列;
        }
        else
        {
            return null;
        }
    }

    private Single FeedBack_ThreeDayHighPrice(string Code_Num, int count)
    {
        int x = Day_Count;//天數
        List<SqlParameter> PMS = new List<SqlParameter>();
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@Stock_Code", Code_Num));
        PMS.Add(new SqlParameter("@N", count));
        PMS.Add(new SqlParameter("@Day_count", x));
        DataTable 個股最高價 = DB.QueryStoredProcedure("[N日最高價]", PMS.ToArray());
        if (個股最高價.Rows.Count != 0)
        {
            return Convert.ToSingle(個股最高價.Rows[0][1]);
        }
        else
        {
            return (0);
        }
    }

    private bool FeedBack_Fundamental(string Code_Num, int count) 
    {
        List<SqlParameter> PMS = new List<SqlParameter>();
        PMS = new List<SqlParameter>();
        PMS.Add(new SqlParameter("@Stock_Code", Code_Num));
        PMS.Add(new SqlParameter("@N", count));
        DataTable 個股驗證結果 = DB.QueryStoredProcedure("[基本面判斷]", PMS.ToArray());
        if (個股驗證結果.Rows.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}