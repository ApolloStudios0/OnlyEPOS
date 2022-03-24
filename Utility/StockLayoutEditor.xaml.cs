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

namespace OnlyEPOS.Utility
{
    public partial class StockLayoutEditor : Window
    {
        DataTable Data = new();
        public StockLayoutEditor()
        {
            InitializeComponent();
            GetData();
        }
        
        public async void GetData()
        {
            // Load Pre-Checked
            Data = await Utility.SQL.GetSQLData("SELECT * FROM StockColumns Where Active = 1", "OnlyEPOS");

            foreach (CheckBox Box in MainGrid.Children.OfType<CheckBox>())
            {
                foreach (DataRow Row in Data.Rows)
                {
                    if (Box.Name.ToString().Replace("_", " ") == Row["StockColumn"].ToString())
                    {
                        Box.IsChecked = true;
                    }
                }
            }
        }

        private void SaveLayout(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox Box in MainGrid.Children.OfType<CheckBox>())
            {
                if ((bool)Box.IsChecked)
                {
                    // Save
                    Utility.SQL.ExecuteThisQuery($"UPDATE StockColumns SET Active = 1 where StockColumn = '{Box.Name.ToString().Replace("_", " ")}'");
                }
                else
                {
                    // Save
                    Utility.SQL.ExecuteThisQuery($"UPDATE StockColumns SET Active = 0 where StockColumn = '{Box.Name.ToString().Replace("_", " ")}'");
                }
            }
            
            MessageBox.Show("Layout Saved. The Stock Window Will Now Close.");
            DialogResult = true;
        }
    }
}
