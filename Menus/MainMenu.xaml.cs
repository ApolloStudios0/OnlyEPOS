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
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
            LoadUserSettings();
        }

        public void LoadUserSettings()
        {
            Load_Colors_and_Picture();
            GetStaffName();

            // Load Profile Picture
            void Load_Colors_and_Picture()
            {
                // Load Picture 
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri(Utility.CurrentStaffInformation.StaffProfilePicture));
                ProfilePicture.Fill = imgBrush;
                ProfilePictureBorder.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(Utility.CurrentStaffInformation.StaffColor);
                
                // Load Colors
                PrintLastReceiptButton.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(Utility.CurrentStaffInformation.StaffColor);
            }
            
            void GetStaffName()
            {
                Welcome_StaffName.Content = $"Hello, {Utility.CurrentStaffInformation.StaffMemberName}";
            }
        }
    }
}
