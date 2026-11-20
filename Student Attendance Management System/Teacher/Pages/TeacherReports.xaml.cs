using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Teacher.Components;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Student_Attendance_Management_System.Teacher.Pages
{
    /// <summary>
    /// Interaction logic for TeacherReports.xaml
    /// </summary>
    public partial class TeacherReports : Page
    {
        private TeacherModel teacherData;
        private ReportSummary summary;
        public TeacherReports(TeacherModel teacher)
        {
            InitializeComponent();
            ClassesVM classVm = new();
            classVm.LoadClassesByTeacher(teacher.Teacher_id);
            ReportSummary reportSum = new(teacher);
            summary = reportSum;
            teacherData = teacher;
            PiePerformance piePerformance = new(teacher);
            StudentGenderPerformance GenderPerformance = new(teacher);
            summaryReport.Children.Add(reportSum);
            pieBox.Children.Add(piePerformance);
            genderPerformance.Children.Add(GenderPerformance);
            DataContext = classVm;
            dataGridList.ItemsSource = classVm.TeacherAllClasses;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dataGridList.Items.Count > 0)
                {
                    dataGridList.Visibility = Visibility.Visible;
                    assignedClassText.Visibility = Visibility.Collapsed;
                    return;
                }
                dataGridList.Visibility = Visibility.Collapsed;
                assignedClassText.Visibility = Visibility.Visible;
            }));
        }
        //View Button
        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            ClassModel selectedClass = (ClassModel)((Button)sender).Tag;
            if (selectedClass != null)
            {
                TeacherLayout? teacherPage = Application.Current.Windows.OfType<TeacherLayout>().FirstOrDefault();
                if (teacherPage != null)
                {
                    teacherPage.TeacherFrame.Navigate(new ViewClassStatus(selectedClass, teacherData));
                }
            }
        }

        //Print to PDF
        private void PrintToPDF(DataGrid dataGrid, string filePath)
        {
            var originalColumns = new List<DataGridColumn>(dataGrid.Columns);

            if (dataGrid.Columns.Count > 0)
            {
                dataGrid.Columns.RemoveAt(6);
            }
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (Document document = new Document(PageSize.A4, 30f, 30f, 40f, 40f))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                // Add Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
                var title = new iTextSharp.text.Paragraph("YOUR REPORT", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                document.Add(title);

                // Report DetailS
                var totalStudFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var totalStud = new iTextSharp.text.Paragraph($"Total Students: {summary.totalStudentsText.Text}", totalStudFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 3f
                };
                document.Add(totalStud);

                var totalClassFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var totalClass = new iTextSharp.text.Paragraph($"Total Classes: {summary.totalClassesText.Text}", totalClassFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 3f
                };
                document.Add(totalClass);

                var totalDropFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var totalDrop = new iTextSharp.text.Paragraph($"Total Drop Outs: {summary.totalDropText.Text}", totalDropFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 20f
                };
                document.Add(totalDrop);

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

                // Add Data Rows
                var itemsSource = dataGrid.ItemsSource as IEnumerable<ClassModel>;
                if (itemsSource != null)
                {
                    foreach (var row in itemsSource)
                    {
                        foreach (DataGridColumn column in dataGrid.Columns)
                        {
                            string cellText = string.Empty;
                            Binding binding = null;

                            if (column is DataGridBoundColumn boundColumn)
                                binding = boundColumn.Binding as Binding;

                            if (binding != null)
                            {
                                var propertyName = binding.Path.Path;
                                var prop = row.GetType().GetProperty(propertyName);
                                var value = prop?.GetValue(row, null);
                                cellText = value?.ToString() ?? string.Empty;
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
        private void ExportToPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Want to export data into PDF file?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"Reports {DateTime.Now.ToLocalTime().ToString("yyyyMMdd_HHmmss")}",
                    Filter = "PDF documents (.pdf)|*.pdf",
                    DefaultExt = ".pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    try
                    {
                        PrintToPDF(dataGridList, saveDialog.FileName);
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
    }
}
