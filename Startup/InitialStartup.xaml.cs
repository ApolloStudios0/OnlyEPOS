global using OnlyEPOS.Settings;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlyEPOS.Startup
{
    public partial class InitialStartup : Window
    {
        public InitialStartup()
        {
            // Load
            InitializeComponent();

            // Create Users
            CreateButtons();
            
            // Loading Screen
            LoadingFrame.Content = new EntranceLoadScreen();
            LoadingFrame.Visibility = Visibility.Collapsed;
        }

        // Button Properties
        DataTable StaffMembers = new();

        // Dynmically Create Users
        // Includes:
        // - Roles
        // -- Business Director
        // -- Management
        // -- Store Supervisor
        // -- Retail Assistant

        // Create Buttons Into Grid
        // -- Show 12 Users Per Page
        public async void CreateButtons()
        {
            // Get List Of Staff
            StaffMembers = await Utility.SQL.GetSQLData("Select * From .[Dbo].[StaffLoginColours]", "CompanyAccess");

            // Dynamic Values
            int colNumber = 0;
            int rowNumber = 0;
            int pageNumber = 0;
            int i = 0;
            int staffCount = StaffMembers.Rows.Count;

            // Create Buttons
            foreach (DataRow Row in StaffMembers.Rows)
            {
                // -- Organize
                StackPanel SP = new();

                // -- Profile Picture
                Ellipse ellipse = new() { Width = 55, Height = 55 };
                ImageBrush imgBrush = new ImageBrush();

                // Check Image Works - If Failed - Resort To Default
                try { imgBrush.ImageSource = new BitmapImage(new Uri(Row["StaffImage"].ToString())); }
                catch { imgBrush.ImageSource = new BitmapImage(new Uri(Paths.CurrentDirectory + "DefaultImage.png", UriKind.Relative)); }
                
                ellipse.Fill = imgBrush;
                ellipse.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom(Row["StaffColour"].ToString());
                ellipse.StrokeThickness = 3;

                // -- Label Content
                Label StaffName = new() { Content = Row["StaffName"].ToString(), FontSize = 21, Height = 34, HorizontalAlignment = HorizontalAlignment.Center };
                Label StaffRole = new() { Content = Row["StaffRole"].ToString(), FontSize = 10, HorizontalAlignment = HorizontalAlignment.Center, Opacity = 0.5 };

                // -- Button Creation
                Button btn = new Button()
                {
                    Background = Brushes.Transparent,
                    BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(Row["StaffColour"].ToString()),
                    BorderThickness = new Thickness(2),
                    Width = 200,
                    Height = 120,
                    Tag = Row["StaffUUID"].ToString(),
                    Name = Row["StaffName"].ToString().Replace(" ", "_") + $"_{pageNumber}",
                };
                
                // Hide More Page Buttons
                if (pageNumber != 0) { btn.Visibility = Visibility.Collapsed; }

                // -- Add Stackpanel & Labels To Button
                btn.Content = SP;
                SP.Children.Add(ellipse);
                SP.Children.Add(StaffName);
                SP.Children.Add(StaffRole);

                // -- Show & Display
                Grid.SetRow(btn, rowNumber);
                Grid.SetColumn(btn, colNumber);
                btn.Click += new RoutedEventHandler(StaffButtonClicked);
                LoginButtonGrid.Children.Add(btn);

                // -- Find Next Button Placement
                if (rowNumber == 3 && colNumber == 2)
                {
                    // Reset Back To Start
                    colNumber = 0;
                    rowNumber = 0;
                    pageNumber++;
                }
                else if (colNumber == 2)
                {
                    colNumber = 0;
                    rowNumber++;
                }
                else
                {
                    colNumber++;
                }
                i++;
            }
        }
        
        /// <summary>
        /// Staff Button Clicked, Figure Out Who Clicked It
        /// </summary>
        public void StaffButtonClicked(object sender, RoutedEventArgs e)
        {
            // Convert UUID > StaffName
            Button btn = sender as Button;

            // You're Logged In As:
            MessageBox.Show(btn.Tag.ToString());
        }
        
        /// <summary>
        /// Handle Keypad Input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeypadAdvisor(object sender, RoutedEventArgs e)
        {
            // Name
            Button b = sender as Button;
            
            // Switch
            switch (b.Content.ToString())
            {
                case "X":
                    if (KeypadEntryBox.Text.Length > 0)
                    {
                        KeypadEntryBox.Text = KeypadEntryBox.Text.Substring(0, KeypadEntryBox.Text.Length - 1);
                    }
                    break;

                case "✅":
                    // Execute Against The Local Server & Check The Information
                    string UserLogin = Utility.SQL.GetStaffFromAccessCode(KeypadEntryBox.Text);

                    // Proceed With Login
                    if (UserLogin != "NO_STAFF_MEMBER_WITH_ACCESS_CODE_FOUND") { LogTest.Content = UserLogin; KeypadEntryBox.Text = ""; }
                    else { LogTest.Content = "Invalid Access Code"; }
                    break;

                default:
                    KeypadEntryBox.Text += b.Content;
                    break;
            }
        }
    }
}
