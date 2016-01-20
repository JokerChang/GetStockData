using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;


public class Database
{
    public class MSSQL
    {
        /// <summary>
        /// 資料庫連線名稱
        /// </summary>
        private string _ConnectionName = "DefaultConnection";
        public string ConnectionName { get { return this._ConnectionName; } set { this._ConnectionName = value; } }
        /// <summary>
        /// 資料庫連線字串
        /// </summary>
        private string _ConnectionString = string.Empty;

        /// <summary>
        /// 建構函數
        /// </summary>
        public MSSQL()
        {
            this._ConnectionString = ConfigurationManager.ConnectionStrings[_ConnectionName].ConnectionString;
        }
        /// <summary>
        /// 建構函數
        /// </summary>
        /// <param name="ConnectionName">資料庫連線名稱</param>
        public MSSQL(string ConnectionName)
        {
            this._ConnectionName = ConnectionName;
            this._ConnectionString = ConfigurationManager.ConnectionStrings[_ConnectionName].ConnectionString;
        }

        /// <summary>
        /// 建立資料庫連線
        /// </summary>
        /// <returns></returns>
        private SqlConnection GetConn()
        {
            return new SqlConnection(this._ConnectionString);
        }

        /// <summary>
        /// 資料表查詢
        /// </summary>
        /// <param name="SqlStr"></param>
        /// <returns></returns>
        public DataTable QuerySQL(string SQLString)
        {
            SqlConnection conn = GetConn();
            conn.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(SQLString, conn);
            DataTable DT = new DataTable();

            try
            {
                adapter.Fill(DT);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
            finally
            {
                adapter.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return DT;
        }

        /// <summary>
        /// //資料新增、刪除的指令
        /// </summary>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public bool ExecutionSQL(string SQLString)
        {
            SqlConnection conn = GetConn();
            conn.Open();

            SqlCommand comd = new SqlCommand(SQLString, conn);

            try
            {
                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);

                return false;
            }
            finally
            {
                comd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        ///  //資料新增、刪除的指令（帶PK值）
        /// </summary>
        /// <param name="QueryString"></param>        
        /// <returns></returns>
        public string ExecutionSQLIdentity(string SQLString)
        {
            SqlConnection conn = GetConn();
            conn.Open();
            SqlCommand comd = new SqlCommand(SQLString + " SELECT @@IDENTITY", conn);
            try
            {
                return comd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
                return string.Empty;
            }
            finally
            {
                comd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 執行Exection預存程序
        /// </summary>
        /// <param name="ProcedureName">預存程序名稱</param>
        /// <param name="Params">參數</param>
        /// <returns>True/False</returns>
        public bool ExecutionStoredProcedure(string ProcedureName, SqlParameter[] Params)
        {
            SqlConnection conn = GetConn();
            SqlCommand comd = new SqlCommand(ProcedureName, conn);

            try
            {
                conn.Open();
                comd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in Params)
                    comd.Parameters.Add(p);
                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
                return false;
            }
            finally
            {
                comd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 執行Exection預存程序回傳產生的主鍵
        /// </summary>
        /// <param name="ProcedureName">預存程序名稱</param>
        /// <param name="Params">參數</param>
        /// <returns>True/False</returns>
        public object ExecutionStoredProcedureIdentity(string ProcedureName, SqlParameter[] Params)
        {
            SqlConnection conn = GetConn();
            SqlCommand comd = new SqlCommand(ProcedureName, conn);
            SqlParameter retValParam = comd.Parameters.Add("@RETURN_VALUE", SqlDbType.VarChar, 250);
            retValParam.Direction = ParameterDirection.ReturnValue;

            try
            {
                conn.Open();
                comd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in Params)
                    comd.Parameters.Add(p);
                comd.ExecuteNonQuery();
                return retValParam.Value;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
                return null;
            }
            finally
            {
                comd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 執行Query預存程序
        /// </summary>
        /// <param name="ProcedureName">預存程序名稱</param>
        /// <param name="Params">參數</param>
        /// <returns>DataTable</returns>
        public DataTable QueryStoredProcedure(string ProcedureName, SqlParameter[] Params)
        {
            SqlConnection conn = GetConn();
            SqlCommand comd = new SqlCommand(ProcedureName, conn);
            DataTable DT = new DataTable();

            try
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(comd);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in Params)
                    comd.Parameters.Add(p);
                adapter.Fill(DT);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
                return null;
            }
            finally
            {
                comd.Parameters.Clear();
                comd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return DT;
        }

        /// <summary>
        /// 執行Query預存程序回傳資料和總筆數
        /// </summary>
        /// <param name="ProcedureName">預存程序名稱</param>
        /// <param name="Params">參數</param>
        /// <returns>DataTable</returns>
        public DataItem QueryStoredProcedureWithCount(string ProcedureName, SqlParameter[] Params)
        {
            SqlConnection conn = GetConn();
            SqlCommand comd = new SqlCommand(ProcedureName, conn);
            DataSet DS = new DataSet();
            DataItem DI = new DataItem();

            try
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(comd);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in Params)
                    comd.Parameters.Add(p);
                adapter.Fill(DS);
                DI = new DataItem(DS.Tables[0], int.Parse(DS.Tables[1].Rows[0][0].ToString()));
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
                return null;
            }
            finally
            {
                comd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return DI;
        }



        public class DataItem
        {
            public DataTable Data;
            public int Count;

            public DataItem() { }
            public DataItem(DataTable Data, int Count)
            {
                this.Data = Data;
                this.Count = Count;
            }
        }

    }
}

namespace DatabaseOperate
{
    class SqlOperateInfo
    {
        //Suppose your ServerName is "aa",DatabaseName is "bb",UserName is "cc", Password is "dd"
        private string sqlConnectionCommand = "Data Source=aa;Initial Catalog=bb;User ID=cc;Pwd=dd";

        //This table contains two columns:KeywordID int not null,KeywordName varchar(100) not null
        private string dataTableName = "Basic_Keyword_Test";
        private string storedProcedureName = "Sp_InertToBasic_Keyword_Test";
        private string sqlSelectCommand = "Select KeywordID, KeywordName From Basic_Keyword_Test";

        //sqlUpdateCommand could contain "insert" , "delete" , "update" operate
        private string sqlUpdateCommand = "Delete From Basic_Keyword_Test Where KeywordID = 1";

        public void UseSqlReader()
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionCommand);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = sqlSelectCommand;
            sqlConnection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                //Get KeywordID and KeywordName , You can do anything you like. Here I just output them.
                int keywordid = (int)sqlDataReader[0];

                //the same as: int keywordid = (int)sqlDataReader["KeywordID"]
                string keywordName = (string)sqlDataReader[1];

                //the same as: string keywordName = (int)sqlDataReader["KeywordName"]
                Console.WriteLine("KeywordID = " + keywordid + " , KeywordName = " + keywordName);
            }

            sqlDataReader.Close();
            sqlCommand.Dispose();
            sqlConnection.Close();
        }

        public void UseSqlStoredProcedure()
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionCommand);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = storedProcedureName;
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();

            //you can use reader here,too.as long as you modify the sp and let it like select * from ....
            sqlCommand.Dispose();
            sqlConnection.Close();
        }

        public void UseSqlDataSet()
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionCommand);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = sqlSelectCommand;
            sqlConnection.Open();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = sqlCommand;
            DataSet dataSet = new DataSet();

            //sqlCommandBuilder is for update the dataset to database
            SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            sqlDataAdapter.Fill(dataSet, dataTableName);

            //Do something to dataset then you can update it to 　Database.Here I just add a row
            DataRow row = dataSet.Tables[0].NewRow();
            row[0] = 10000;
            row[1] = "new row";
            dataSet.Tables[0].Rows.Add(row);
            sqlDataAdapter.Update(dataSet, dataTableName);
            sqlCommand.Dispose();
            sqlDataAdapter.Dispose();
            sqlConnection.Close();
        }
    }
}


