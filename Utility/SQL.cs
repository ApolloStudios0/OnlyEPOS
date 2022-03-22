global using System.Data.SqlClient;
global using System.Threading.Tasks;
global using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlyEPOS.Utility
{
    class SQL
    {
        /// <summary>
        /// Asynchronously Executes A Query Against Till 1 & Returns The Result In A Datatable (string QueryToExecute)
        /// </summary>
        public static Task<DataTable> GetSQLData(string QueryToExecute, string ExecuteAgainst)
        {
            return Task.Run(() => {

                // Instances
                DatabaseToExecuteAgainst = ExecuteAgainst;
                DataTable dt = new DataTable();
                SqlConnection sqlConnection = new SqlConnection();

                // Run Query & Return As DataTable
                try { sqlConnection.ConnectionString = Settings.SQL.ConnectionString; } catch (Exception Ex) { Logs.LogError(Ex.Message); }

                using (SqlDataAdapter da = new SqlDataAdapter(QueryToExecute, sqlConnection.ConnectionString))
                {
                    try { da.Fill(dt); } catch (Exception ex) { Logs.LogError(ex.Message); }
                }

                return dt;
            });
        }

        /// <summary>
        /// Asynchronously Executes A NON-Query against till 1 & Returns No Result (string QueryToExecute)
        /// </summary>
        public static void ExecuteThisQuery(string QueryToExecute)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Settings.SQL.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(QueryToExecute, connection);
                    command.Connection.Open(); command.ExecuteNonQuery(); command.Connection.Close();
                }
            }
            catch { Logs.LogError("Error Executing Query: " + QueryToExecute); }
        }

        /// <summary>
        /// Asynchronously Executes A Query Against Till 1 & Returns The Result As Scalar Obj (string QueryToExecute)
        /// </summary>
        public static string DatabaseToExecuteAgainst { get; set; } = "CompanyAccess";
        public static string ExecuteSQLScalar(string QueryToExecute, string ExecuteAgainst)
        {
            using (SqlConnection connection = new SqlConnection(Settings.SQL.ConnectionString))
            {
                using (SqlCommand Commander = new(QueryToExecute, connection))
                {
                    DatabaseToExecuteAgainst = ExecuteAgainst;
                    string result = "";
                    try
                    {
                        if (Commander.Connection.State != ConnectionState.Open)
                        {
                            Commander.Connection.Open();
                        }
                        Commander.CommandTimeout = 0;
                        result = Convert.ToString(Commander.ExecuteScalar());
                        if (Commander.Connection.State != ConnectionState.Closed)
                        {
                            Commander.Connection.Close();
                        }
                        Commander.Dispose();
                    }
                    catch (Exception ex2)
                    {
                        Logs.LogError("Error Executing Query: " + ex2.Message);
                    }
                    finally
                    {
                        connection.Close();
                        Commander.Dispose();
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Asynchronously Checks What User Is Logging In Using AccessCode
        /// </summary>
        public static string GetStaffFromAccessCode(string AccessCode)
        {
            // Check For Dev Code Entered
            if (AccessCode == "3141")
            {
                return "DEV-FLAG-GIVEN";
            }
            
            // Ask Server
            string StaffName = ExecuteSQLScalar($"Select [StaffName] From .[dbo].[StoreLogin] where [AccessCode] = '{AccessCode}'", "CompanyAccess");

            // Check & Return
            if (StaffName != null && StaffName != "" && StaffName.Length != 0) { return StaffName; }
            else { return "NO_STAFF_MEMBER_WITH_ACCESS_CODE_FOUND"; }
        }

        public static string GetPasswordFromUUID(string UUID)
        {
            var PasswordValue = ExecuteSQLScalar($"Select Password from .[dbo].[StoreLogin] where StaffUUID = '{UUID}'", "CompanyAccess");
            if (PasswordValue != null && PasswordValue != "" && PasswordValue.Length != 0)
            {
                return PasswordValue;
            }
            else
            {
                return "NO_STAFF_MEMBER_WITH_UUID_CODE_FOUND";
            }
        }
    }
}
