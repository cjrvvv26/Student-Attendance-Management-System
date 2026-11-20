using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Data
{
    class DataHelper
    {
        private static string SQL_STRING = "server=localhost;uid=clarence;pwd=Clarence26.;database=student_attendance_management_system;";
        public static MySqlConnection connectDB()
        {
            MySqlConnection conn = new MySqlConnection(SQL_STRING);
            try
            {
                conn.Open();
            }catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while connecting in SQL database: {ex.Message}");
            }
            return conn;
        }

        public static void closeConnection(MySqlConnection conn)
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
