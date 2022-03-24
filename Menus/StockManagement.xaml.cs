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
using System.Windows.Media.Imaging;
using System.IO;

namespace OnlyEPOS.Menus
{
    public partial class StockManagement : Window
    {
        public StockManagement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle Stock Product Search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        DataTable DatabaseData = new();
        DataTable StockData = new();
        private async void StockSearchAdvisor(object sender, RoutedEventArgs e)
        {
            // Check Which Values Are Being Used
            string BaseSearch = "Select * From Stock Where 1=1";
            Button Sender = sender as Button;

            // Stop Parent Checking Itself
            foreach (TextBox Search in ProductSearchGrid.Children.OfType<TextBox>())
            {
                if (Search.Text != "")
                {
                    // Check Which Values Are Being Used
                    BaseSearch += $" AND [{Search.Name.Replace("_", " ")}] LIKE '%{Search.Text}%'";
                }
                else
                {
                    // Obtain All Products (No Search Criteria Provided)
                    StockData = await Utility.SQL.GetSQLData(BaseSearch, "OnlyEPOS");
                    StockDataGrid.ItemsSource = StockData.DefaultView;
                }
            }

            // Get Data From Database
            StockData = await Utility.SQL.GetSQLData(BaseSearch, "OnlyEPOS");

            // Set View
            StockDataGrid.ItemsSource = StockData.DefaultView;

            // Dynamically Check Column Data
            CheckDataColumns();
        }
        
        /// <summary>
        /// Dynamically Bind Columns Using Active Stock Columns
        /// </summary>
        private async void CheckDataColumns()
        {
            // Clear Old Columns
            StockDataGrid.Columns.Clear();

            // Get Users Columns
            DatabaseData = await Utility.SQL.GetSQLData("Select * From StockColumns Where Active = 1", "OnlyEPOS");

            // Generate Columns Dynamically
            foreach (DataRow row in DatabaseData.Rows)
            {
                StockDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = row["StockColumn"].ToString(),
                    Binding = new Binding(row["StockColumn"].ToString()),
                });
            }
        }

        /// <summary>
        /// Open Stock Layout Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLayoutEditor(object sender, RoutedEventArgs e)
        {
            Utility.StockLayoutEditor SLE = new();
            SLE.ShowDialog();
            
            if (SLE.DialogResult == true) { this.Close(); } // Close to refresh view
        }

        /// <summary>
        /// Dynmically Edit & Save DataGrid Edits To The Stock Table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Get Current Row
            DataRowView CurrentRow = e.Row.Item as DataRowView;

            // Get Current Column
            string CurrentColumn = e.Column.Header.ToString();

            // Get Cell Value
            string CellValue = e.EditingElement.GetValue(TextBox.TextProperty).ToString();

            // Get StockUUID From Current Row
            string StockUUID = CurrentRow["StockUUID"].ToString();

            // Send Parameterized Query
            SqlCommand cmd = new($"UPDATE Stock SET [{CurrentColumn}] = @StockValue WHERE StockUUID = @StockUUID", new SqlConnection(SQL.ConnectionString))
            {
                Parameters =
                    {
                        new SqlParameter("@StockValue", CellValue),
                        new SqlParameter("@StockUUID", StockUUID),
                    }
            };
            
            // Execute With Try
            try
            {
                if (cmd.Connection.State == ConnectionState.Closed) { cmd.Connection.Open(); }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logs.LogError(ex.Message);
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open) { cmd.Connection.Close(); }
                cmd.Dispose();
            }
        }
    }
}
