using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Student_Attendance_Management_System.Data
{
    public class SectionOperations
    {
        //Get all section data
        public ObservableCollection<SectionModel> GetAllSection()
        {
            ObservableCollection<SectionModel> sections = new();
            string query = "SELECT * FROM sections";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sections.Add(new SectionModel
                        {
                            Section_id = reader.GetInt32("section_id"),
                            Course = reader.GetString("course"),
                            Year_level = reader.GetInt32("year_level"),
                            Section = reader.GetString("section"),
                            Created_at = reader.GetDateTime("created_at")
                        });
                    }
                    return sections;
                }
            }
        }

        //get list of section only by course
        public List<string> GetListSections(string course)
        {
            List<string> sections = new();
            string query = "SELECT section FROM sections WHERE course = @course";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", course);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sections.Add(reader.GetString("section"));
                    }
                    return sections;
                }
            }
        }

        //Get all section data by course
        public ObservableCollection<SectionModel> GetAllSectionByCourse(string course)
        {
            ObservableCollection<SectionModel> sections = new();
            string query = "SELECT * FROM sections WHERE course = @course";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", course);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sections.Add(new SectionModel
                        {
                            Section_id = reader.GetInt32("section_id"),
                            Course = reader.GetString("course"),
                            Year_level = reader.GetInt32("year_level")
                        });
                    }
                    return sections;
                }
            }
        }

        //Get all section data by course and year level
        public SectionModel GetAllSectionByCourseAndYearLevel(string course, int year_level)
        {
            SectionModel section = new();
            string query = "SELECT * FROM sections WHERE course = @course AND year_level = @year_level";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", course);
                cmd.Parameters.AddWithValue("@year_level", year_level);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        section.Section_id = reader.GetInt32("section_id");
                        section.Course = reader.GetString("course");
                        section.Year_level = reader.GetInt32("year_level");
                        section.Section = reader.GetString("section");
                    }
                    return section;
                }
            }
        }

        //get specified section data by course, year, and section
        public SectionModel GetIdByCourseSectionYear(string course, int year, string section)
        {
            string query = "SELECT * FROM sections WHERE course = @course AND year_level = @year AND section = @section";
            SectionModel sectionModel = new();
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", course);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@section", section);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        sectionModel = new SectionModel()
                        {
                            Section_id = reader.GetInt32("section_id"),
                            Course = reader.GetString("course"),
                            Year_level = reader.GetInt32("year_level"),
                            Section = reader.GetString("section"),
                        };
                    }
                    return sectionModel;
                }
            }
        }

        //Get all course available
        public List<string> GetAllCourse()
        {
            List<string> courses = new();
            string query = "SELECT DISTINCT course FROM sections";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(reader.GetString("course"));
                    }
                    return courses;
                }
            }
        }

        //Get all year level available by course
        public List<int> GetAllYearLevelByCourse(string course)
        {
            List<int> year_levels = new();
            string query = "SELECT DISTINCT year_level FROM sections WHERE course = @course";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", course);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        year_levels.Add(reader.GetInt32(reader.GetOrdinal("year_level")));
                    }
                    return year_levels;
                }
            }
        }

        //Add new course and section
        public bool AddNewSection(SectionModel section, int adminUserID)
        {
            if (string.IsNullOrEmpty(section.Course) ||
                string.IsNullOrEmpty(section.Year_level.ToString()) ||
                string.IsNullOrEmpty(section.Section))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string query = "INSERT INTO sections (course, year_level, section, created_at, updated_at) VALUES (@course, @year_level, @section, @created_at, @updated_at)";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", section.Course);
                cmd.Parameters.AddWithValue("@year_level", section.Year_level);
                cmd.Parameters.AddWithValue("@section", section.Section);
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("New section added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notifOP = new();
                        notifOP.AdminNonUserNotification(adminUserID, "Section", $"{section.Course} - {section.Year_level}{section.Section}", "Create section", "Super Admin");
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while adding new section: {ex.Message}");
                    return false;
                }
            }
        }

        //Restore course and section
        public bool RestoreCourseSection(SectionModel section, int adminUserID)
        {
            string query = "INSERT INTO sections (course, year_level, section, created_at, updated_at) VALUES (@course, @year_level, @section, @created_at, @updated_at)";

            string archiveQuery = "DELETE FROM archive WHERE original_id = @origID";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", section.Course);
                cmd.Parameters.AddWithValue("@year_level", section.Year_level);
                cmd.Parameters.AddWithValue("@section", section.Section);
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                cmd.ExecuteNonQuery();
                using MySqlCommand archiveCmd = new(archiveQuery, conn);
                archiveCmd.Parameters.AddWithValue("@origID", section.Section_id);
                try
                {
                    if (archiveCmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Course & section are restored successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notifOP = new();
                        notifOP.AdminNonUserNotification(adminUserID, "Section", $"{section.Course} - {section.Year_level}{section.Section}", "Restore section", "Super Admin");
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while adding new section: {ex.Message}");
                    return false;
                }
            }
        }

        //Get section data by id
        public SectionModel GetSectionData(int section_id)
        {
            string query = "SELECT * FROM sections WHERE section_id = @section_id";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@section_id", section_id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SectionModel
                        {
                            Section_id = reader.GetInt32("section_id"),
                            Course = reader.GetString("course"),
                            Year_level = reader.GetInt32("year_level"),
                            Section = reader.GetString("section"),
                            Created_at = reader.GetDateTime("created_at")
                        };
                    }
                    return null;
                }
            }
        }

        public SectionModel GetSectionDataByCourseYear(string course, int year)
        {
            string query = "SELECT * FROM sections WHERE course = @course AND year_level = @year";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", course);
                cmd.Parameters.AddWithValue("@year", year);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SectionModel
                        {
                            Section_id = reader.GetInt32("section_id"),
                            Course = reader.GetString("course"),
                            Year_level = reader.GetInt32("year_level"),
                            Section = reader.GetString("section"),
                            Created_at = reader.GetDateTime("created_at")
                        };
                    }
                    return null;
                }
            }
        }

        //Update course and section
        public bool UpdateSection(SectionModel section, int adminUserID)
        {
            
            string query = "UPDATE sections SET course = @course, year_level = @year_level, section = @section, updated_at = @updated_at WHERE section_id = @section_id";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                MessageBox.Show(section.Section);
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@section_id", section.Section_id);
                cmd.Parameters.AddWithValue("@course", section.Course);
                cmd.Parameters.AddWithValue("@year_level", section.Year_level);
                cmd.Parameters.AddWithValue("@section", section.Section);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Section updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notifOP = new();
                        notifOP.AdminNonUserNotification(adminUserID, "Section", $"{section.Course} - {section.Year_level}{section.Section}", "Update section", "Super Admin");
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while updating section: {ex.Message}");
                    return false;
                }
            }
        }

        //Delete section
        public bool DeleteSection(int section_id, int adminUserID)
        {
            SectionModel section = GetSectionData(section_id);

            string query = "DELETE FROM sections WHERE section_id = @section_id";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@section_id", section_id);
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Section deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notifOP = new();
                        notifOP.AdminNonUserNotification(adminUserID, "Section", $"{section.Course} - {section.Year_level}{section.Section}", "Create section", "Super Admin");
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting section: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
