using iTextSharp.text.pdf;
using iTextSharp.text;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Student_Attendance_Management_System.Data;

namespace Student_Attendance_Management_System.Teacher.Pages
{
    /// <summary>
    /// Interaction logic for OverallRecord.xaml
    /// </summary>
    public partial class OverallRecord : Page
    {
        private AttendanceVM _attendanceVM;
        private ClassModel _classModel;
        private bool pageLoaded = false;
        public OverallRecord(int classID, string term)
        {
            InitializeComponent();
            ClassesOperations classOp = new();
            _classModel = classOp.GetClassById(classID);
            GenerateTerm();
            AttendanceVM attendVm = new();
            _attendanceVM = attendVm;
            _attendanceVM.GetOverallClassRecord(classID);
            DataContext = _attendanceVM;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (listStudentsTable.Items.Count > 0)
                {
                    listStudentsTable.Visibility = Visibility.Visible;
                    noStudents.Visibility = Visibility.Collapsed;
                    return;
                }
                listStudentsTable.Visibility = Visibility.Collapsed;
                noStudents.Visibility = Visibility.Visible;
            }), System.Windows.Threading.DispatcherPriority.Background);

            pageLoaded = true;
            termFilterCombo.SelectedIndex = 0;
        }

        private void pdfBtn_Click(object sender, RoutedEventArgs e)
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
                        PrintToPDF(listStudentsTable, saveDialog.FileName);
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
        //Print to PDF
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
                var currrentTerm = new iTextSharp.text.Paragraph($"Class Term: {termFilterCombo.SelectedItem.ToString()}", termFont)
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
                var totalStudents = new iTextSharp.text.Paragraph($"Total of Students: {listStudentsTable.Items.Count.ToString()}", totalStudentsFont)
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

                // Add Data Rows
                var itemsSource = dataGrid.ItemsSource as IEnumerable<AttendanceModel>;
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

        private void GenerateTerm()
        {
            termFilterCombo.ItemsSource = new List<string>()
            {
                "All" ,"Prelim", "Midterm", "Finals"
            };
        }

        //top ribbon
        private void ViewAllClass_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TermFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!pageLoaded || _classModel == null)
                return;

            string selectedTerm = termFilterCombo.SelectedItem.ToString();

            if (selectedTerm == "All")
            {
                _attendanceVM.GetOverallClassRecord(_classModel.Class_id);
            }
            else
            {
                _attendanceVM.GetOverallClassRecordByTerm(_classModel.Class_id, selectedTerm);
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_attendanceVM.GetOverallRecord.Count > 0)
                {
                    listStudentsTable.Visibility = Visibility.Visible;
                    pdfBtn.Visibility = Visibility.Visible;
                    noStudents.Visibility = Visibility.Collapsed;
                }
                else
                {
                    listStudentsTable.Visibility = Visibility.Collapsed;
                    pdfBtn.Visibility = Visibility.Collapsed;
                    noStudents.Visibility = Visibility.Visible;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
