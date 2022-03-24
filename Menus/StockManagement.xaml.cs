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
using System.Windows.Shapes;

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

        private void OpenLayoutEditor(object sender, RoutedEventArgs e)
        {
            Utility.StockLayoutEditor SLE = new();
            SLE.ShowDialog();
            
            if (SLE.DialogResult == true) { this.Close(); } // Close to refresh view
        }
    }
}
