using iTextSharp.text.pdf;
using iTextSharp.text;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.Teacher.Pages
{
    /// <summary>
    /// Interaction logic for ViewAttendanceRecord.xaml
    /// </summary>
    public partial class ViewAttendanceRecord : Page
    {
        ObservableCollection<StudentModel> studentModel = new();

        private int classId;
        private TeacherModel teacherModel;
        private DateTime _attendanceDate;
        private ClassModel _classModel;
        private StudentVM studentVM;
        public ViewAttendanceRecord(int classID, DateTime attendanceDate)
        {
            InitializeComponent();
            // Initialize view model and class information
            studentVM = new StudentVM();
            classId = classID;
            _attendanceDate = attendanceDate;
            ClassesOperations classOp = new();
            _classModel = classOp.GetClassById(classId);
            // Fetch students for the given class and date
            studentModel = studentVM.GetAllExistingStudentsByClassID(classId, attendanceDate);
            
            AttendanceOperations attendOp = new();
            if (!attendOp.CanUpdateAttendance(classID))
            {
                generatePdfBtn.Visibility = Visibility.Visible;
                foreach(var student in studentModel)
                {
                    student.CanUpdate = false;
                }
            }
            // Bind the fetched students to the DataGrid
            DataGridAttendance.ItemsSource = studentModel;
        }

        //Update Student Status
        private int classID;

        // Update Student Status
        private void UpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is StudentModel selectedStudent)
            {
                string newStatus = selectedStudent.Student_status;

                switch (selectedStudent.Student_status)
                {
                    case "Late":
                        newStatus = "Present";
                        button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#00C951");
                        break;
                    case "Present":
                        newStatus = "Absent";
                        button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FB2C36");
                        break;
                    case "Absent":
                        newStatus = "Pending";
                        button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA500");
                        break;
                    case "Pending":
                        newStatus = "Late";
                        button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#8f8e8e");
                        break;
                }

                // Update UI
                selectedStudent.Student_status = newStatus;

                // Call method to update DB
                AttendanceOperations attendanceOps = new();
                bool updated = attendanceOps.UpdateAttendanceStatus(selectedStudent.Student_id, classId, newStatus, _attendanceDate);

                if (!updated)
                {
                    MessageBox.Show("Failed to update attendance in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        //Generate to PDF
        private void GeneratePdfBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Want to export student record into PDF file?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"Overall Attendance Record {DateTime.Now.ToLocalTime().ToString("yyyyMMdd_HHmmss")}",
                    Filter = "PDF documents (.pdf)|*.pdf",
                    DefaultExt = ".pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    try
                    {
                        PrintToPDF(DataGridAttendance, saveDialog.FileName);
                        MessageBox.Show("PDF exported successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting PDF: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        //Convert to PDF method
        private void PrintToPDF(DataGrid dataGrid, string filePath)
        {
            var originalColumns = new List<DataGridColumn>(dataGrid.Columns);

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (Document document = new Document(PageSize.A4, 30f, 30f, 40f, 40f))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                // Add Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
                var title = new iTextSharp.text.Paragraph("ATTENDANCE RECORD", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                document.Add(title);

                // Add ClassDetails
                var termFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var currrentTerm = new iTextSharp.text.Paragraph($"Class Term: {_classModel.Class_term}", termFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 3f
                };
                document.Add(currrentTerm);

                // Add ClassDetails
                var semFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var currrentSem = new iTextSharp.text.Paragraph($"Semester: {_classModel.Class_semester}", semFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 3f
                };
                document.Add(currrentSem);

                var totalStudentsFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var totalStudents = new iTextSharp.text.Paragraph($"Total of Students: {DataGridAttendance.Items.Count.ToString()}", totalStudentsFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 20f
                };
                document.Add(totalStudents);

                // Initialize PdfPTable
                PdfPTable table = new PdfPTable(dataGrid.Columns.Count)
                {
                    WidthPercentage = 100f,
                    SplitLate = false,
                    SplitRows = true
                };

                float[] columnWidths = Enumerable.Repeat(1f, dataGrid.Columns.Count).ToArray();
                table.SetWidths(columnWidths);

                // Add Header Row
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    var headerText = column.Header?.ToString() ?? string.Empty;
                    PdfPCell headerCell = new PdfPCell(new Phrase(headerText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                    {
                        BackgroundColor = new BaseColor(230, 230, 230),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 6,
                        NoWrap = false
                    };
                    table.AddCell(headerCell);
                }

                foreach (var row in DataGridAttendance.ItemsSource)
                {
                    foreach (DataGridColumn column in dataGrid.Columns)
                    {
                        string cellText = string.Empty;

                        if (column is DataGridBoundColumn boundColumn)
                        {
                            var binding = boundColumn.Binding as Binding;
                            if (binding != null)
                            {
                                var propertyName = binding.Path.Path;
                                var prop = row.GetType().GetProperty(propertyName);
                                var value = prop?.GetValue(row, null);
                                cellText = value?.ToString() ?? string.Empty;
                            }
                        }
                        else if (column is DataGridTemplateColumn templateColumn)
                        {
                            // Assuming the template column is the last one
                            if (column.DisplayIndex == dataGrid.Columns.Count - 1)
                            {
                                // Create the content to get the button
                                var cellContent = templateColumn.GetCellContent(row);

                                if (cellContent is Button button)
                                {
                                    cellText = button.Content?.ToString() ?? string.Empty;
                                }
                                else if (cellContent != null)
                                {
                                    // If inside a Grid or StackPanel
                                    var btn = FindVisualChild<Button>(cellContent);
                                    if (btn != null)
                                    {
                                        cellText = btn.Content?.ToString() ?? string.Empty;
                                    }
                                }
                            }
                        }

                        PdfPCell dataCell = new PdfPCell(new Phrase(cellText, FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        {
                            Padding = 4,
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            NoWrap = false
                        };

                        table.AddCell(dataCell);
                    }
                }

                // Add Table to Document
                document.Add(table);

                // Add Footer
                var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var footer = new iTextSharp.text.Paragraph($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}", footerFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 20f
                };
                document.Add(footer);

                document.Close();
            }

            // Restore removed column (like checkbox)
            dataGrid.Columns.Clear();
            foreach (var column in originalColumns)
            {
                dataGrid.Columns.Add(column);
            }
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    return tChild;

                T result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
