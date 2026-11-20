using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.Data
{
    public class TrashOperations
    {
        //Get all recorded trash by its type
        public ObservableCollection<TrashModel> GetRecordedTrash(string type)
        {
            ObservableCollection<TrashModel> record = new();
            string query = "SELECT * FROM archive WHERE type = @type ORDER BY archived_at DESC";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@type", type);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        record.Add(new TrashModel
                        {
                            Archive_id = reader.GetInt32("archive_id"),
                            Type = reader.GetString("type"),
                            Original_id = reader.GetInt32("original_id"),
                            Data = reader.GetString("data"),
                            Archived_at = reader.GetDateTime("archived_at")
                        });
                    }
                    return record;
                }
            }
        }

        public bool DeletePermanentTrash(int origID, string type)
        {
            string query = "DELETE FROM archive WHERE type = @type AND original_id = @origID";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);

                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@origID", origID);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }catch(MySqlException ex)
            {
                MessageBox.Show("An error occurred while deleting a trash: " + ex.Message);
                return false;
            }
        }
    }
}