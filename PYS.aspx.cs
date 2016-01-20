using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.POIFS.FileSystem;
using NPOI.Util;


public partial class Index : System.Web.UI.Page
{
    //畫面上要先擺一個FileUpload控制項
    HSSFWorkbook workbook = null;
    HSSFSheet u_sheet = null;
    DateTime dt = DateTime.Now;
    DataTable Stock_Num = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        Database.MSSQL DB = new Database.MSSQL("Web");
        Stock_Num = DB.QuerySQL("SELECT [C_ID] FROM [Analysis].[dbo].[Company] ORDER BY [C_ID]");
    }

    #region 載入資料
    protected void GSD_btn_Click(object sender, EventArgs e)
    {
        try
        {
            this.workbook = new HSSFWorkbook(PYS_FileUpload.FileContent);
            this.u_sheet = (HSSFSheet)workbook.GetSheetAt(0);  //取得第0個Sheet
            //不同於Microsoft Object Model，NPOI都是從索引0開始算起
            //從第一個Worksheet讀資料        
            SaveOrInsertSheet(this.u_sheet);
            ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.Page), "匯入完成", "alert('匯入完成');", true);
        }
        catch (Exception)
        {
            ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.Page), "匯入失敗", "alert('匯入失敗');", true);
        }
        finally
        {
            //釋放 NPOI的資源 
            if (this.workbook != null) this.workbook = null;
            if (this.u_sheet != null) this.u_sheet = null;

            //是否刪除Server上的Excel檔(預設true)
            /*bool isDeleteFileFromServer = false;
            if (isDeleteFileFromServer)
            {
                System.IO.File.Delete(excel_filePath);
            }*/
            GC.Collect();
        }
    }

    #endregion

    #region 驗證資料，送入資料庫
    private void SaveOrInsertSheet(HSSFSheet u_sheet)
    {
        Database.MSSQL DB = new Database.MSSQL("Web");
        List<SqlParameter> PMS = new List<SqlParameter>();
        //因為要讀取的資料列不包含標頭，所以i從u_sheet.FirstRowNum + 1開始讀
        /*一列一列地讀取資料*/
        int Company_check = 0;

        for (int i = u_sheet.FirstRowNum + 1; i <= u_sheet.LastRowNum; i++)
        {
            HSSFRow row = (HSSFRow)u_sheet.GetRow(i);//取得目前的資料列 2015/03/18[新增資料區段]
            for (int j = 0; j < Stock_Num.Rows.Count; j++ )
            {
                if (Equals(Stock_Num.Rows[j][0], row.GetCell(0).ToString()))
                {
                    Company_check = 1;
                }   
            }
            if (Company_check == 0)
            {
                PMS = new List<SqlParameter>();
                PMS.Add(new SqlParameter("@C_ID", row.GetCell(0).ToString()));
                PMS.Add(new SqlParameter("@C_Name", row.GetCell(1).ToString()));
                DB.ExecutionStoredProcedure("[GetStockNum_Insert]", PMS.ToArray());
            }

            PMS = new List<SqlParameter>();
            PMS.Add(new SqlParameter("@證券代號", row.GetCell(0).ToString()));
            PMS.Add(new SqlParameter("@本益比", Convert.ToSingle(row.GetCell(2).ToString())));
            PMS.Add(new SqlParameter("@殖利率",  Convert.ToSingle(row.GetCell(3).ToString())));
            PMS.Add(new SqlParameter("@股價淨值比",  Convert.ToSingle(row.GetCell(4).ToString())));
            PMS.Add(new SqlParameter("@資料日期", Convert.ToDateTime(DataTime_PYS_txt.Text)));
            PMS.Add(new SqlParameter("@下載日期", dt));
            DB.ExecutionStoredProcedure("[PYS_Insert]", PMS.ToArray());
            Company_check = 0;
        }
    }
    #endregion
}