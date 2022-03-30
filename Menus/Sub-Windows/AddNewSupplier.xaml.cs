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

namespace OnlyEPOS.Menus.Sub_Windows
{
    public partial class AddNewSupplier : Window
    {
        public AddNewSupplier()
        {
            InitializeComponent();
        }

        private void AddNew(object sender, RoutedEventArgs e)
        {
            // Add Supplier
            SqlCommand AddSupplier = new($"Insert Into ProductSuppliers VALUES (@StockUUID, @SupplierName, @SupplierCode, @CaseCost, @IndividualCost, @SupplierDiscount, @PackSize, @MainSupplier, newid())", new SqlConnection(SQL.ConnectionString))
            {
                Parameters =
                    {
                        new SqlParameter("@StockUUID", StockManagement.ProductUUID),
                        new SqlParameter("@SupplierName", SupplierName.Text),
                        new SqlParameter("@SupplierCode", SupplierCode.Text),
                        new SqlParameter("@CaseCost", CaseCost.Text),
                        new SqlParameter("@IndividualCost", IndividualCost.Text),
                        new SqlParameter("@SupplierDiscount", SupplierDiscountBox.Text),
                        new SqlParameter("@PackSize", PackSize.Text),
                        new SqlParameter("@MainSupplier", PrimarySupplierCheckbox.IsChecked == true ? 1 : 0)
                    }
            };
            try
            {
                if (AddSupplier.Connection.State == ConnectionState.Closed) { AddSupplier.Connection.Open(); }
                AddSupplier.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logs.LogError("Couldn't Upload Supplier! : " + ex.Message);
            }
            finally
            {
                if (AddSupplier.Connection.State == ConnectionState.Open) { AddSupplier.Connection.Close(); }
                AddSupplier.Dispose();
            }

            // If Primary Supplier, Remove All Other Suppliers
            if (PrimarySupplierCheckbox.IsChecked == true)
            {
                SqlCommand MakePrimarySup = new($"Update ProductSuppliers Set [Primary Supplier] = 0 Where StockUUID = @StockUUID AND SupplierName = @SuppliersName; Update ProductSuppliers set [Primary Supplier] = 1 where StockUUID = @StockUUID and [SupplierName] = @SuppliersName;", new SqlConnection(SQL.ConnectionString))
                {
                    Parameters =
                    {
                        new SqlParameter("@StockUUID", StockManagement.ProductUUID),
                        new SqlParameter("@SuppliersName", SupplierName.Text)
                    }
                };
                try
                {
                    if (MakePrimarySup.Connection.State == ConnectionState.Closed) { MakePrimarySup.Connection.Open(); }
                    MakePrimarySup.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logs.LogError("Couldn't Make Primary Supplier! : " + ex.Message);
                }
                finally
                {
                    if (MakePrimarySup.Connection.State == ConnectionState.Open) { MakePrimarySup.Connection.Close(); }
                    MakePrimarySup.Dispose();
                }
            }

            // Handoff 
            DialogResult = true;
        }
    }
}
