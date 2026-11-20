using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Paragraph = iTextSharp.text.Paragraph;

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for AuditLogs.xaml
    /// </summary>
    public partial class AuditLogs : Page
    {
        private List<int> selectedItemsID = new List<int>();
        public AuditLogs()
        {
            InitializeComponent();
            DataContext = new AuditLogsVM();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (recordLogs.Items.Count <= 0)
                {
                    topRibbonBtns.Visibility = Visibility.Collapsed;
                }
                else
                {
                    topRibbonBtns.Visibility = Visibility.Visible;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void PrintToPDF_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Audit Log Records {DateTime.Now.ToLocalTime().ToString("yyyyMMdd_HHmmss")}",
                Filter = "PDF documents (.pdf)|*.pdf",
                DefaultExt = ".pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    ConvertToPDF(recordLogs, saveDialog.FileName);
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

        private void ConvertToPDF(DataGrid dataGrid, string filePath)
        {
            // Backup and temporarily remove checkbox column (assumed first column)
            var originalColumns = new List<DataGridColumn>(dataGrid.Columns);
            if (dataGrid.Columns.Count > 0)
            {
                dataGrid.Columns.RemoveAt(0);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (Document document = new Document(PageSize.A4, 30f, 30f, 40f, 40f))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                // Add Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
                var title = new Paragraph("AUDIT LOG RECORDS", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                document.Add(title);

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
                var itemsSource = dataGrid.ItemsSource as IEnumerable<AuditLogsModel>;
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
                var footer = new Paragraph($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}", footerFont)
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



        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in recordLogs.Items)
            {
                if (item is AuditLogsModel selectedItem)
                {
                    // Set CheckBoxChecked to true for all items
                    selectedItem.CheckBoxChecked = true;
                    selectedItemsID.Add(selectedItem.Log_id);
                }
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in recordLogs.Items)
            {
                if (item is AuditLogsModel selectedItem)
                {
                    // Set CheckBoxChecked to true for all items
                    selectedItem.CheckBoxChecked = false;
                    selectedItemsID.Remove(selectedItem.Log_id);
                }
            }
        }


        //Delete all logs
        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this log?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                AuditLogsVM auditLogsVM = new();
                auditLogsVM.DeleteSelectedLogs(selectedItemsID);
            }
            DataContext = new AuditLogsVM();
            if (recordLogs.Items.Count <= 0)
            {
                topRibbonBtns.Visibility = Visibility.Collapsed;
            }
            else
            {
                topRibbonBtns.Visibility = Visibility.Visible;
            }
        }

        private void SelectedItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is AuditLogsModel selectedItem)
            {
                selectedItemsID.Add(selectedItem.Log_id);
            }
            else
            {
                MessageBox.Show("No item selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SelectedItem_Unchecked(object sender, RoutedEventArgs e)
        {
            AuditLogsModel selectedItem = (AuditLogsModel)((CheckBox)sender).Tag;

            if (selectedItem != null) {
                selectedItemsID.Remove(selectedItem.Log_id);
            }
        }
    }
}
