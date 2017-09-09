using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QUANLYTIEC.Models
{
    public class Database
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionEFDataFirst"].ConnectionString;
        private SqlConnection _sqlconnect;
        public DataTable mn_Table { get; set; }
        public string mn_Error { get; set; }

        public Database()
        {
            mn_Table = new DataTable();
            mn_Error = "";
        }

        /// <summary>
        /// Mở kết nối
        /// </summary>
        /// <returns></returns>
        private bool _OpenConnection()
        {
            try
            {
                _sqlconnect = new SqlConnection(_connectionString);
                _sqlconnect.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Lấy dữ liệu sử dụng select command
        /// </summary>
        /// <param name="SqlSelect"></param>
        /// <returns></returns>
        public bool fn_GetData(string SqlSelect)
        {
            if (!this._OpenConnection())
                return false;
            SqlCommand cmd = new SqlCommand(SqlSelect, _sqlconnect);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            mn_Table = new DataTable();
            try
            {
                da.Fill(mn_Table);
            }
            catch (SqlException ex)
            {
                this.mn_Error = ex.Message;
                return false;
            }
            finally
            {
                _sqlconnect.Close();
            }
            return true;
        }

        /// <summary>
        /// Lấy dữ liệu sử dụng store procedure
        /// </summary>
        /// <param name="ProName">Tên thủ tục</param>
        /// <returns>true/false</returns>
        public bool fn_GetData_Pro(string ProName, params SqlParameter[] Parameter)
        {
            if (!this._OpenConnection())
                return false;
            SqlCommand cmd = new SqlCommand(ProName, _sqlconnect);
            cmd.Parameters.AddRange(Parameter);
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            mn_Table = new DataTable();
            try
            {
                da.Fill(mn_Table);
            }
            catch (SqlException ex)
            {
                this.mn_Error = ex.Message;
                return false;
            }
            finally
            {
                _sqlconnect.Close();
            }
            return true;
        }

        /// <summary>
        /// Thực thi lấy 1 giá trị nào đó trong csdl.
        /// </summary>
        /// <param name="sql">Câu truy vấn sử dụng select command</param>
        /// <returns>'-1' nếu không có giá trị</returns>
        public string fn_Execute_Scalar(string sql)
        {
            if (!this._OpenConnection())
                return "--1";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, _sqlconnect);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                try
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                        return obj.ToString();
                    return "";
                }
                catch (Exception)
                {
                    return "";
                }
            }
            catch (SqlException ex)
            {
                this.mn_Error = ex.Message;
                return "--1";
            }
            finally
            {
                _sqlconnect.Close();
            }
        }

        /// <summary>
        /// Thực thi lấy 1 giá trị nào đó trong csdl.
        /// </summary>
        /// <param name="ProName">Tên thủ tục</param>
        /// <param name="Parameters">Danh sách tham số</param>
        /// <returns>'--1' nếu không có giá trị</returns>
        public string fn_Execute_Scalar(string ProName, params SqlParameter[] Parameters)
        {
            if (!this._OpenConnection())
                return "--1";
            try
            {
                SqlCommand cmd = new SqlCommand(ProName, _sqlconnect);
                cmd.Parameters.AddRange(Parameters);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    return cmd.ExecuteScalar().ToString();
                }
                catch
                {
                    return "--1";
                }
            }
            catch (SqlException ex)
            {
                this.mn_Error = ex.Message;
                return "--1";
            }
            finally
            {

                _sqlconnect.Close();
            }
        }

        /// <summary>
        /// Sử dụng ExecuteReader để lấy dữ liệu
        /// </summary>
        /// <param name="ProName">Tên procedure</param>
        /// <param name="Parameters">Danh sách tham số</param>
        /// <returns></returns>
        public string fn_Execute_Reader(string ProName, params SqlParameter[] Parameters)
        {
            if (!this._OpenConnection())
                return "-1";
            try
            {
                SqlCommand cmd = new SqlCommand(ProName, _sqlconnect);
                cmd.Parameters.AddRange(Parameters);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                mn_Table = new DataTable();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return reader["TableName"].ToString();
                }
                return "";
            }
            catch (SqlException ex)
            {
                this.mn_Error = ex.Message;
                return "-1";
            }
            finally
            {
                _connectionString.Clone();
            }
        }

        /// <summary>
        /// Thực thi câu truy vấn sử dụng store procedure
        /// </summary>
        /// <param name="ProName">Tên store procedure</param>
        /// <param name="Parameters">Danh sách các tham số</param>
        /// <returns>true: thành công. False: không thành công</returns>
        public bool fn_Execute_Pro(string ProName, params SqlParameter[] Parameters)
        {
            if (!this._OpenConnection())
                return false;

            SqlCommand cmd = new SqlCommand(ProName, _sqlconnect);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddRange(Parameters);
            int resuls = 0;
            try
            {
                resuls = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                this.mn_Error = ex.Message;
                return false;
            }
            finally
            {
                _sqlconnect.Close();
            }
            //if (resuls < 0)
            //    return false;
            return true;
        }

        /// <summary>
        /// Thực thi câu truy vấn sử dụng select command
        /// </summary>
        /// <param name="sqlQuerry">Câu truy vấn</param>
        /// <returns>True: thành công. False: không thành công</returns>
        public bool fn_Execute_Sql(string sqlQuerry)
        {
            if (!this._OpenConnection())
                return false;
            SqlCommand cmd = new SqlCommand(sqlQuerry, _sqlconnect);
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
            int resuls = 0;
            try
            {
                resuls = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                this.mn_Error = ex.Message;
                return false;
            }
            finally
            {
                _sqlconnect.Close();
            }
            if (resuls <= 0)
                return false;
            return true;
        }

        /// <summary>
        /// Thực thi mệnh lệnh với điều kiện là thao tác trên 2 bảng cùng lúc, master and detail.
        /// </summary>
        /// <param name="ProName1">Tên thủ tục cần tao tác trên bảng cha</param>
        /// <param name="ProName2">Tên thủ tục cần thao tác trên bảng con</param>
        /// <param name="pra1">Danh sách các tham số cần thao tác đến bảng cha</param>
        /// <param name="pra2">Danh sách các tham số cần thao tác đến bảng con</param>
        /// <returns></returns>
        public bool fn_Execute_ProWithTransaction(string ProName1, string ProName2, SqlParameter[] pra1, List<SqlParameter[]> pra2)
        {
            if (!this._OpenConnection())
                return false;
            SqlCommand cmd1 = new SqlCommand("dbo." + ProName1, _sqlconnect);
            cmd1.CommandTimeout = 0;
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlTransaction tranS;
            tranS = _sqlconnect.BeginTransaction();
            cmd1.Transaction = tranS;
            try
            {
                foreach (SqlParameter item in pra1)
                {
                    cmd1.Parameters.Add(item);
                }
                cmd1.ExecuteNonQuery();
                foreach (SqlParameter[] item in pra2)
                {
                    SqlCommand cmd2 = new SqlCommand("dbo." + ProName2, _sqlconnect);
                    cmd2.CommandType = 0;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Transaction = tranS;
                    foreach (SqlParameter pr in item)
                    {
                        cmd2.Parameters.Add(pr);
                    }
                    cmd2.ExecuteNonQuery();
                }
                tranS.Commit();
            }
            catch (Exception ex)
            {
                this.mn_Error = ex.Message;
                try
                {
                    tranS.Rollback();
                }
                catch (Exception x)
                {
                    this.mn_Error = x.Message;
                    return false;
                }
                return false;
            }
            finally
            {
                _sqlconnect.Close();
            }
            return true;
        }


        /// <summary>
        /// Thực thi mệnh lệnh với điều kiện là thao tác trên 2 bảng cùng lúc, master and detail.
        /// </summary>
        /// <param name="ProName1">Tên thủ tục cần tao tác trên bảng cha</param>
        /// <param name="ProName2">Tên thủ tục cần thao tác trên bảng con</param>
        /// <param name="pra1">Danh sách các tham số cần thao tác đến bảng cha</param>
        /// <param name="pra2">Danh sách các tham số cần thao tác đến bảng con</param>
        /// <param name="pra3">Danh sách các tham số cần thao tác đến bảng con</param>
        /// <returns></returns>
        public bool fn_Execute_ProWithTransaction(string ProName1, string ProName2, string ProName3, SqlParameter[] pra1, List<SqlParameter[]> pra2, List<SqlParameter[]> pra3)
        {
            if (!this._OpenConnection())
                return false;
            SqlCommand cmd1 = new SqlCommand("dbo." + ProName1, _sqlconnect);
            cmd1.CommandTimeout = 0;
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlTransaction tranS;
            tranS = _sqlconnect.BeginTransaction();
            cmd1.Transaction = tranS;
            try
            {
                foreach (SqlParameter item in pra1)
                {
                    cmd1.Parameters.Add(item);
                }
                cmd1.ExecuteNonQuery();
                foreach (SqlParameter[] item in pra2)
                {
                    SqlCommand cmd2 = new SqlCommand("dbo." + ProName2, _sqlconnect);
                    cmd2.CommandTimeout = 0;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Transaction = tranS;
                    foreach (SqlParameter pr in item)
                    {
                        cmd2.Parameters.Add(pr);
                    }
                    cmd2.ExecuteNonQuery();
                }
                foreach (SqlParameter[] item in pra3)
                {
                    SqlCommand cmd3 = new SqlCommand("dbo." + ProName3, _sqlconnect);
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.CommandTimeout = 0;
                    cmd3.Transaction = tranS;
                    foreach (SqlParameter pr in item)
                    {
                        cmd3.Parameters.Add(pr);
                    }
                    cmd3.ExecuteNonQuery();
                }
                tranS.Commit();
            }
            catch (Exception ex)
            {
                this.mn_Error = ex.Message;
                try
                {
                    tranS.Rollback();
                }
                catch (Exception x)
                {
                    this.mn_Error = x.Message;
                    return false;
                }
                return false;
            }
            finally
            {
                _sqlconnect.Close();
            }
            return true;
        }
    }
}