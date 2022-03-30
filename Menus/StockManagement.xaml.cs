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

// Known Stock Bugs
// - When editing the same cell multiple times, the 'WasValue' is not updated

namespace OnlyEPOS.Menus
{
    public partial class StockManagement : Window
    {
        #region [*] Edit Selected Product

        // [*] Product Information
        public static string ProductName { get; set; }
        public static string ProductQuantity { get; set; }
        public static string ProductPackSize { get; set; }
        public static string ProductBarcode { get; set; }
        public static string ProductCaseBarcode { get; set; }
        public static string ProductWeight { get; set; }
        public static string ProductSellingPrice { get; set; }
        public static string ProductSupplierCost { get; set; }
        public static string ProductSupplierCode { get; set; }
        public static string ProductInternalReferenceCode { get; set; }
        public static string ProductWarehouseLocation { get; set; }
        public static string ProductMin { get; set; }
        public static string ProductMax { get; set; }
        public static string ProductType { get; set; }
        public static string ProductSubtype { get; set; }
        public static string ProductDiscontinued { get; set; }
        public static string ProductAgeRestriction { get; set; }
        public static string ProductUUID { get; set; }

        /// <summary>
        /// Called When A Product Is Selected In The Stock Manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        DataTable DataForControls = new();
        private async void UpdateStockInformation(object sender, SelectionChangedEventArgs e) 
        {
            // Obtain Its Values
            try 
            {
                DataGrid grid = (DataGrid)sender;
                DataRowView row_selected = grid.SelectedItem as DataRowView;
                if (row_selected is not null)
                {
                    // [*] Set Values For Other Methods
                    ProductName = row_selected["Name of Item"].ToString();
                    ProductQuantity = row_selected["Quantity"].ToString();
                    ProductPackSize = row_selected["Pack Size"].ToString();
                    ProductBarcode = row_selected["Barcode"].ToString();
                    ProductCaseBarcode = row_selected["Case Barcode"].ToString();
                    ProductWeight = row_selected["Weight"].ToString();
                    ProductSellingPrice = row_selected["Selling Price"].ToString();
                    ProductSupplierCost = row_selected["Supplier Cost"].ToString();
                    ProductSupplierCode = row_selected["Supplier Code"].ToString();
                    ProductInternalReferenceCode = row_selected["Internal Reference Code"].ToString();
                    ProductWarehouseLocation = row_selected["Warehouse Location"].ToString();
                    ProductMin = row_selected["Min"].ToString();
                    ProductMax = row_selected["Max"].ToString();
                    ProductType = row_selected["Type"].ToString();
                    ProductSubtype = row_selected["Subtype"].ToString();
                    ProductDiscontinued = row_selected["Discontinued"].ToString();
                    ProductAgeRestriction = row_selected["Age Restricted"].ToString();
                    ProductUUID = row_selected["StockUUID"].ToString();

                    // [*] Set Local Values
                    #region [1] Change Product Information
                    foreach (TextBox ProductInfo in ProductInformationGrid.Children.OfType<TextBox>()) 
                    {
                        if (ProductInfo.Name is not null && ProductInfo.Name != "")
                        {
                            // Values are replace to stop XAML name collisions
                            ProductInfo.Text = row_selected[ProductInfo.Name.Replace("9", "").Replace("_", " ")].ToString();
                        }
                    }
                    #endregion
                    
                    // [*] Check Active Controls (If A Tab Is Docked / Floating) & Fill Its Data If It Is
                    if (ProductHistory.IsVisible) 
                    {
                        // [*] Fill Product History
                        await Task.Run(async () =>
                        {
                            // [*] Get Product History
                            DataForControls = await Utility.SQL.GetSQLData("SELECT [Product Name], [Field Changed] as 'What Changed', [Was], [Now], [Changed By] as 'Changed By', [DateChanged] as 'Date' FROM ProductHistory WHERE StockUUID = '" + ProductUUID + "' Order By DateChanged Desc", "OnlyEPOS ");

                            // [*] Set Product History
                            ProductHistoryGrid.Dispatcher.Invoke(() =>
                            {
                                ProductHistoryGrid.ItemsSource = DataForControls.DefaultView;
                            });
                        });
                    }
                    if (BarcodeDataGrid.IsVisible)
                    {
                        // [*] Fill Barcode Data
                        await Task.Run(async () =>
                        {
                            // [*] Get Product History
                            DataForControls = await Utility.SQL.GetSQLData($"Select [Barcode] as 'Product Barcodes' From ProductBarcodes where StockUUID = '{ProductUUID}'", "OnlyEPOS");

                            // [*] Set Product History
                            BarcodeDataGrid.Dispatcher.Invoke(() =>
                            {
                                BarcodeDataGrid.ItemsSource = DataForControls.DefaultView;
                            });
                        });
                    }
                    if (SupplierDataGrid.IsVisible)
                    {
                        // [*] Fill Barcode Data
                        await Task.Run(async () =>
                        {
                            // [*] Get Product History
                            DataForControls = await Utility.SQL.GetSQLData($"Select [SupplierName] as 'Supplier Name',[SupplierCode] as 'Suppliers Code',[Case Cost],[Individual Cost],[Supplier Discount],[Pack Size],[Primary Supplier],[SupplierUUID] From ProductSuppliers where StockUUID = '{ProductUUID}' Order by [Primary Supplier] Desc", "OnlyEPOS");

                            // [*] Set Product History
                            SupplierDataGrid.Dispatcher.Invoke(() =>
                            {
                                SupplierDataGrid.ItemsSource = DataForControls.DefaultView;
                            });
                        });
                    }
                    if (StatsSection.IsVisible)
                    {
                        // [*] Fill Values
                        ProductNameField.Content = ProductName;
                    }
                }
            }
            catch (Exception ex) { Logs.LogError(ex.Message); }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                        "Name: " + ProductName + "\n" +
                        "Quantity: " + ProductQuantity + "\n" +
                        "Pack Size: " + ProductPackSize + "\n" +
                        "Barcode: " + ProductBarcode + "\n" +
                        "Case Barcode: " + ProductCaseBarcode + "\n" +
                        "Weight: " + ProductWeight + "\n" +
                        "Selling Price: " + ProductSellingPrice + "\n" +
                        "Supplier Cost: " + ProductSupplierCost + "\n" +
                        "Supplier Code: " + ProductSupplierCode + "\n" +
                        "Internal Reference Code: " + ProductInternalReferenceCode + "\n" +
                        "Warehouse Location: " + ProductWarehouseLocation + "\n" +
                        "Min: " + ProductMin + "\n" +
                        "Max: " + ProductMax + "\n" +
                        "Type: " + ProductType + "\n" +
                        "Subtype: " + ProductSubtype + "\n" +
                        "Discontinued: " + ProductDiscontinued + "\n" +
                        "Age Restricted: " + ProductAgeRestriction + "\n" +
                        "StockUUID: " + ProductUUID + "\n"
                        );
        }
        private void DoubleClickToEditRows(object sender, MouseButtonEventArgs e)
        {
            // Update PKey & Details
            DataGridCell cell = sender as DataGridCell;
            if (cell is not null) 
            { 
                cell.IsEditing = true;

                // get column name
                try
                {
                    string columnName = cell.Column.Header.ToString();
                    DataRowView row = (DataRowView)cell.DataContext;
                    string value = row[columnName].ToString();
                    WasValue = value;
                }
                catch (Exception ex) { Logs.LogError("Failed To Obtain Row Value From Stock Manager: " + ex.Message); }
            }
        }
        
        private void EnterPressedSearchProducts(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) { StockSearchAdvisor(StockSearchButton, null); } }
        
        #endregion

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

            foreach (TextBox Search in ProductSearchGrid.Children.OfType<TextBox>())
            {
                switch (Sender.Name)
                {
                    // Search Stock Fields
                    case "StockSearchButton":
                        if (Search.Text != "")
                        {
                            // Check Which Values Are Being Used
                            BaseSearch += $" AND [{Search.Name.Replace("_", " ")}] LIKE '%{Search.Text}%'";
                        }

                        // Get Data From Database
                        StockData = await Utility.SQL.GetSQLData(BaseSearch, "OnlyEPOS");

                        // Set View
                        StockDataGrid.ItemsSource = StockData.DefaultView;

                        // Dynamically Check Column Data
                        CheckDataColumns();
                        break;

                    // Clear All Fields
                    case "ClearStockSearches":
                        StockDataGrid.ItemsSource = null;
                        Search.Text = "";
                        foreach (var row in ProductInformationGrid.Children.OfType<TextBox>())
                        {
                            if (row.Name is not null && row.Name != "")
                            {
                                row.Text = "";
                            }
                        }
                        break;
                }
            }
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
        /// Dynmically Edit & Save DataGrid Edits To The Stock Table (Also Controls StockProductHistory Updates)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static string WasValue { get; set; }
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

            // Make Sure Value Is Actually Different
            if (WasValue != CellValue)
            {

                // Send Parameterized Query
                SqlCommand ProductUpdate = new($"UPDATE Stock SET [{CurrentColumn}] = @StockValue WHERE StockUUID = @StockUUID", new SqlConnection(SQL.ConnectionString))
                {
                    Parameters =
                    {
                        new SqlParameter("@StockValue", CellValue),
                        new SqlParameter("@StockUUID", StockUUID),
                    }
                };

                SqlCommand ProductHistory = new($"Insert Into .[dbo].[ProductHistory] VALUES (@ProductName, @FieldChanged, @Was, @Now, @ChangedBy, @StockUUID, GetDate())", new SqlConnection(SQL.ConnectionString))
                {
                    Parameters =
                    {
                        new SqlParameter("@ProductName", ProductName),
                        new SqlParameter("@FieldChanged", CurrentColumn),
                        new SqlParameter("@Was", WasValue),
                        new SqlParameter("@Now", CellValue),
                        new SqlParameter("@ChangedBy", Utility.CurrentStaffInformation.StaffMemberName),
                        new SqlParameter("@StockUUID", StockUUID),
                    }
                };

                // Execute With Try
                try
                {
                    if (ProductUpdate.Connection.State == ConnectionState.Closed) { ProductUpdate.Connection.Open(); }
                    if (ProductHistory.Connection.State == ConnectionState.Closed) { ProductHistory.Connection.Open(); }
                    ProductUpdate.ExecuteNonQuery();
                    ProductHistory.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logs.LogError(ex.Message);
                }
                finally
                {
                    if (ProductUpdate.Connection.State == ConnectionState.Open) { ProductUpdate.Connection.Close(); }
                    if (ProductHistory.Connection.State == ConnectionState.Open) { ProductHistory.Connection.Close(); }
                    ProductUpdate.Dispose();
                    ProductHistory.Dispose();
                }
            }
        }

        /// <summary>
        /// Dynmically Change Product Information On Left Side-Stock Manager When Pressing Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeProductInformation(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (e.Key == Key.Return)
            {
                // Values are replaced to stop XAML name collisions 
                SqlCommand cmd = new($"UPDATE Stock SET [{tb.Name.Replace("9", "").Replace("_", " ")}] = @StockValue WHERE StockUUID = @StockUUID", new SqlConnection(SQL.ConnectionString))
                {
                    Parameters =
                    {
                        new SqlParameter("@StockValue", tb.Text),
                        new SqlParameter("@StockUUID", ProductUUID), // Product UUID changed on row click
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
                    StockSearchButton.Focus();
                    StockSearchAdvisor(StockSearchButton, null);
                }
            }
        }

        public StockManagement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dynmically Handles All Button Clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static string SelectedValue { get; set; }
        private void ObtainRowValue(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            DataRowView row_selected = grid.SelectedItem as DataRowView;
            if (row_selected is not null)
            {
                switch (grid.Name)
                {
                    case "BarcodeDataGrid":
                        SelectedValue = row_selected["Product Barcodes"].ToString();
                        break;

                    case "SupplierDataGrid":
                        SelectedValue = row_selected["SupplierUUID"].ToString();
                        break;
                }
            }
        }
        private void GlobalButtonAdvisor(object sender, RoutedEventArgs e)
        {
            Button Sender = sender as Button;

            // Direct
            switch (Sender.Name)
            {
                // -- Add Barcode
                case "AddBarcode":
                    AddBarcode();
                    break;

                // -- Remove Barcode
                case "RemoveBarcode":
                    RemoveBarcode();
                    break;

                // -- Add Product Supplier
                case "AddSupplier":
                    AddSupplier();
                    break;

                // -- Remove Product Supplier
                case "RemoveSupplier":
                    RemoveSupplier();
                    break;
            }

            // -- Barcode Controls
            async void AddBarcode()
            {
                // Show keyboard
                Utility.Keyboard KB = new();
                KB.ShowDialog();

                if (KB.DialogResult == true)
                {
                    SqlCommand AddBarcode = new($"Insert Into ProductBarcodes VALUES (@StockUUID, @Barcode)", new SqlConnection(SQL.ConnectionString))
                    {
                        Parameters =
                    {
                        new SqlParameter("@StockUUID", ProductUUID),
                        new SqlParameter("@Barcode", KB.UserInputBox.Text),
                    }
                    };
                    try
                    {
                        if (AddBarcode.Connection.State == ConnectionState.Closed) { AddBarcode.Connection.Open(); }
                        AddBarcode.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logs.LogError(ex.Message);
                    }
                    finally
                    {
                        if (AddBarcode.Connection.State == ConnectionState.Open) { AddBarcode.Connection.Close(); }
                        AddBarcode.Dispose();

                        // Refresh Barcode List
                        BarcodeDataGrid.ItemsSource = null;
                        StockData = await Utility.SQL.GetSQLData($"Select [Barcode] as 'Product Barcodes' From ProductBarcodes where StockUUID = '{ProductUUID}'", "OnlyEPOS");
                        BarcodeDataGrid.ItemsSource = StockData.DefaultView;
                    }
                }
            }
            async void RemoveBarcode()
            {
                var result = MessageBox.Show("Are You Sure You Want To Remove This Barcode?", "Remove Barcode", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedValue != "NO_VALUE")
                    {
                        SqlCommand RemoveBarcode = new($"Delete From ProductBarcodes where StockUUID = @StockUUID and Barcode = @Barcode", new SqlConnection(SQL.ConnectionString))
                        {
                            Parameters =
                    {
                        new SqlParameter("@StockUUID", ProductUUID),
                        new SqlParameter("@Barcode", SelectedValue),
                    }
                        };
                        try
                        {
                            if (RemoveBarcode.Connection.State == ConnectionState.Closed) { RemoveBarcode.Connection.Open(); }
                            RemoveBarcode.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logs.LogError(ex.Message);
                        }
                        finally
                        {
                            if (RemoveBarcode.Connection.State == ConnectionState.Open) { RemoveBarcode.Connection.Close(); }
                            RemoveBarcode.Dispose();

                            // Refresh Barcode List
                            BarcodeDataGrid.ItemsSource = null;
                            StockData = await Utility.SQL.GetSQLData($"Select [Barcode] as 'Product Barcodes' From ProductBarcodes where StockUUID = '{ProductUUID}'", "OnlyEPOS");
                            BarcodeDataGrid.ItemsSource = StockData.DefaultView;
                            SelectedValue = "NO_VALUE";
                        }
                    }
                    else { MessageBox.Show("Please Select A Barcode To Remove", "Remove Barcode", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
            
            // -- Supplier Controls
            async void AddSupplier()
            {
                Sub_Windows.AddNewSupplier AddSupplier = new();
                AddSupplier.ShowDialog();
                
                if (AddSupplier.DialogResult == true) 
                {
                    // Refresh Supplier List
                    SupplierDataGrid.ItemsSource = null;
                    StockData = await Utility.SQL.GetSQLData($"Select [SupplierName] as 'Supplier Name',[SupplierCode] as 'Suppliers Code',[Case Cost],[Individual Cost],[Supplier Discount],[Pack Size],[Primary Supplier],[SupplierUUID] From ProductSuppliers where StockUUID = '{ProductUUID}' Order by [Primary Supplier] Desc", "OnlyEPOS");
                    SupplierDataGrid.ItemsSource = StockData.DefaultView;
                }
            }
            async void RemoveSupplier()
            {
                var result = MessageBox.Show("Are You Sure You Want To Remove This Supplier?", "Remove Supplier", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedValue != "NO_VALUE")
                    {
                        SqlCommand RemoveSupplier = new($"Delete From ProductSuppliers where StockUUID = @StockUUID and SupplierUUID = @SupplierUUID", new SqlConnection(SQL.ConnectionString))
                        {
                            Parameters =
                    {
                        new SqlParameter("@StockUUID", ProductUUID),
                        new SqlParameter("@SupplierUUID", SelectedValue),
                    }
                        };
                        try
                        {
                            if (RemoveSupplier.Connection.State == ConnectionState.Closed) { RemoveSupplier.Connection.Open(); }
                            RemoveSupplier.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logs.LogError(ex.Message);
                        }
                        finally
                        {
                            if (RemoveSupplier.Connection.State == ConnectionState.Open) { RemoveSupplier.Connection.Close(); }
                            RemoveSupplier.Dispose();

                            // Refresh Barcode List
                            SupplierDataGrid.ItemsSource = null;
                            StockData = await Utility.SQL.GetSQLData($"Select [SupplierName] as 'Supplier Name',[SupplierCode] as 'Suppliers Code',[Case Cost],[Individual Cost],[Supplier Discount],[Pack Size],[Primary Supplier],[SupplierUUID] From ProductSuppliers where StockUUID = '{ProductUUID}' Order by [Primary Supplier] Desc", "OnlyEPOS");
                            SupplierDataGrid.ItemsSource = StockData.DefaultView;
                            SelectedValue = "NO_VALUE";
                        }
                    }
                    else { MessageBox.Show("Please Select A Supplier To Remove", "Remove Supplier", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
        }
    }
}
