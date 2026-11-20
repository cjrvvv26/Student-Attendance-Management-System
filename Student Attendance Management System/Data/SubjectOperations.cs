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
    public class SubjectOperations
    {
        //Get all subject data
        public ObservableCollection<SubjectModel> GetAllSubjects ()
        {
            ObservableCollection<SubjectModel> subjects = new();
            string query = "SELECT * FROM subjects";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subjects.Add(new SubjectModel
                        {
                            Subject_id = reader.GetInt32("subject_id"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_name = reader.GetString("subject_name"),
                            Subject_semester = reader.GetString("subject_semester"),
                            Course_availability = reader.GetString("course_availability"),
                            Year_availability = reader.GetString("year_availability"),
                            Units = reader.GetInt32("units")
                        });
                    }
                    return subjects;
                }
            }
        }

        //Get all subject data by course, section, semester availability
        public ObservableCollection<SubjectModel> GetAllSubjectsByCourse(string course, string section, string semester)
        {
            ObservableCollection<SubjectModel> subjects = new();
            string query = "SELECT * FROM subjects WHERE course_availability = @course AND year_availability = @section AND subject_semester = @semester";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@course", course);
                cmd.Parameters.AddWithValue("@section", section);
                cmd.Parameters.AddWithValue("@semester", semester);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subjects.Add(new SubjectModel
                        {
                            Subject_id = reader.GetInt32("subject_id"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_name = reader.GetString("subject_name"),
                            Subject_semester = reader.GetString("subject_semester"),
                            Course_availability = reader.GetString("course_availability"),
                            Year_availability = reader.GetString("year_availability"),
                            Units = reader.GetInt32("units")
                        });
                    }
                    return subjects;
                }
            }
        }

        //Get all subject id that is available for the course and section
        public List<string> GetAllSubjectIDByCourseYear(string course, int year)
        {
            string query = "SELECT sub.subject_name FROM class c " +
                "JOIN sections sec ON c.section_id = sec.section_id " +
                "JOIN subjects sub ON c.subject_id = sub.subject_id " +
                "WHERE sec.course = @course AND sec.year_level = @yearLevel";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@yearLevel", year);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<string> subjectNames = new();
                        while (reader.Read())
                        {
                            subjectNames.Add(reader.GetString("subject_name"));
                        }
                        return subjectNames;
                    }
                }
            }catch (MySqlException ex)
            {
                MessageBox.Show("An error occurred while checking the available subjects for specified course and section", ex.Message);
                return null;
            }
        }

        //Get subject data by its name
        public SubjectModel GetSubjectByName(string name)
        {
            string query = "SELECT * FROM subjects WHERE subject_name = @subject_name";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@subject_name", name);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SubjectModel
                            {
                                Subject_id = reader.GetInt32("subject_id"),
                                Subject_code = reader.GetString("subject_code"),
                                Subject_name = reader.GetString("subject_name"),
                                Subject_semester = reader.GetString("subject_semester"),
                                Course_availability = reader.GetString("course_availability"),
                                Year_availability = reader.GetString("year_availability"),
                                Units = reader.GetInt32("units")
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }catch(MySqlException ex)
            {
                MessageBox.Show("An error occurred while fetching subject details: ", ex.Message);
                return null;
            }
        }
        //Add subject
        public bool AddSubject(SubjectModel subject, int adminUserID)
        {
            string query = "INSERT INTO subjects (subject_code, subject_name, subject_semester, units, course_availability, year_availability, created_at, updated_at) " +
                "VALUES (@subject_code, @subject_name, @subject_semester, @units, @course_availability, @year_availability, @created_at, @updated_at)";

            //Validate if the subject info is null

            if(subject.Subject_name == null || subject.Subject_code == null)
            {
                MessageBox.Show("All fields must be filled", "Information Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@subject_code", subject.Subject_code);
                cmd.Parameters.AddWithValue("@subject_name", subject.Subject_name);
                cmd.Parameters.AddWithValue("@subject_semester", subject.Subject_semester);
                cmd.Parameters.AddWithValue("@course_availability", subject.Course_availability);
                cmd.Parameters.AddWithValue("@year_availability", subject.Year_availability);
                cmd.Parameters.AddWithValue("@units", subject.Units);
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Subject added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    NotificationOperation notificationOP = new();
                    notificationOP.AdminNonUserNotification(adminUserID, "Subject", subject.Subject_name, "Create subject", "Super Admin");
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while adding subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        //Restore Subject
        public bool RestoreSubject(SubjectModel subject, int adminUserID)
        {
            string query = "INSERT INTO subjects (subject_code, subject_name, subject_semester, units, course_availability, year_availability, created_at, updated_at) " +
                "VALUES (@subject_code, @subject_name, @subject_semester, @units, @course_availability, @year_availability, @created_at, @updated_at)";

            string archiveQuery = "DELETE FROM archive WHERE original_id = @origID";

            //Validate if the subject info is null

            if (subject.Subject_name == null || subject.Subject_code == null)
            {
                MessageBox.Show("All fields must be filled", "Information Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@subject_code", subject.Subject_code);
                cmd.Parameters.AddWithValue("@subject_name", subject.Subject_name);
                cmd.Parameters.AddWithValue("@subject_semester", subject.Subject_semester);
                cmd.Parameters.AddWithValue("@course_availability", subject.Course_availability);
                cmd.Parameters.AddWithValue("@year_availability", subject.Year_availability);
                cmd.Parameters.AddWithValue("@units", subject.Units);
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                using MySqlCommand archiveCmd = new(archiveQuery, conn);
                archiveCmd.Parameters.AddWithValue("@origID", subject.Subject_id);
                try
                {
                    cmd.ExecuteNonQuery();
                    archiveCmd.ExecuteNonQuery();
                    MessageBox.Show("Subject restored successfully!", "Restored Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                    NotificationOperation notificationOP = new();
                    notificationOP.AdminNonUserNotification(adminUserID, "Subject", subject.Subject_name, "Restore subject", "Super Admin");
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while adding subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        //Get subject data by id
        public SubjectModel GetSubjectData(int subject_id)
        {
            string query = "SELECT * FROM subjects WHERE subject_id = @subject_id";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@subject_id", subject_id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SubjectModel
                        {
                            Subject_id = reader.GetInt32("subject_id"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_name = reader.GetString("subject_name"),
                            Subject_semester = reader.GetString("subject_semester"),
                            Course_availability = reader.GetString("course_availability"),
                            Year_availability = reader.GetString("year_availability"),
                            Units = reader.GetInt32("units")
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        //Update subject
        public bool UpdateSubject(SubjectModel subject, int adminUserID)
        {
            string query = "UPDATE subjects SET subject_code = @subject_code, subject_name = @subject_name, subject_semester = @subject_semester, units = @units, course_availability = @course_availability, year_availability = @year_availability, updated_at = @updated_at WHERE subject_id = @subject_id";
            using (MySqlConnection conn = DataHelper.connectDB())
            {

                if (subject.Subject_id == null)
                {
                    MessageBox.Show("no di");
                }

                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@subject_code", subject.Subject_code);
                cmd.Parameters.AddWithValue("@subject_name", subject.Subject_name);
                cmd.Parameters.AddWithValue("@subject_semester", subject.Subject_semester);
                cmd.Parameters.AddWithValue("@course_availability", subject.Course_availability);
                cmd.Parameters.AddWithValue("@year_availability", subject.Year_availability);
                cmd.Parameters.AddWithValue("@units", subject.Units);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@subject_id", subject.Subject_id);
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Subject updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notificationOP = new();
                        notificationOP.AdminNonUserNotification(adminUserID, "Subject", subject.Subject_name, "Update subject", "Super Admin");
                        AuditLogsOperations auditLogs = new();
                        auditLogs.AddLogs(subject.Subject_id, "Subject", "Update subject", "Update", "Success");
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); AuditLogsOperations auditLogs = new();
                    auditLogs.AddLogs(subject.Subject_id, "Subject", "Update subject", "Update", "Failed");
                    return false;
                }
            }
        }

        //Delete subject
        public bool DeleteSubject(int subject_id, int adminUserID)
        {
            SubjectModel subject = GetSubjectData(subject_id);
            if (subject == null)
            {
                MessageBox.Show("Subject not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            string query = "DELETE FROM subjects WHERE subject_id = @subject_id";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@subject_id", subject_id);
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Subject deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notificationOP = new();
                        notificationOP.AdminNonUserNotification(adminUserID, "Subject", subject.Subject_name, "Delete subject", "Super Admin");
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }
}
