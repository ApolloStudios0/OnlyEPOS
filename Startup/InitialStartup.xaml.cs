global using OnlyEPOS.Settings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
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

            // Load Pages
            LoadPages();
            
            // Loading Screen
            LoadingFrame.Content = new EntranceLoadScreen();
            FadeoutLoader();
        }
        
        async void FadeoutLoader()
        {
            // Fade opacity to zero
            await Task.Delay(3000);
            LoadingFrame.Opacity = 0.9;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.8;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.7;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.6;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.5;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.4;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.3;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.2;
            await Task.Delay(40);
            LoadingFrame.Opacity = 0.1;

            LoadingFrame.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Load The Buttons That Are Loaded On EPOS Startup (Sign In Screen)
        /// </summary>
        DataTable StaffMembers = new();
        public static int NumberOfStaffButtonsCreated { get; set; }
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
            NumberOfStaffButtonsCreated = StaffMembers.Rows.Count; // For Page Calculations

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
                Label StaffRole = new() { Content = Row["StaffRole"].ToString(), FontSize = 13, HorizontalAlignment = HorizontalAlignment.Center, Opacity = 0.5 };

                // -- Button Creation
                Button btn = new Button()
                {
                    Background = Brushes.White,
                    BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(Row["StaffColour"].ToString()),
                    BorderThickness = new Thickness(3),
                    Width = 200,
                    Height = 120,
                    Tag = pageNumber,
                    Uid = Row["StaffUUID"].ToString(),
                    Name = Row["StaffName"].ToString().Replace(" ", "_") + $"_{pageNumber}",
                };

                // Add DropShadow to button
                DropShadowEffect dse = new DropShadowEffect()
                {
                    Color = Brushes.Black.Color,
                    ShadowDepth = 0,
                    Opacity = 0.5,
                    BlurRadius = 5,
                };
                btn.Effect = dse;

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

            // Obtain StaffName
            string StaffPassword = Utility.SQL.GetPasswordFromUUID(btn.Uid);

            // Pop Keyboard
            Utility.Keyboard keyboard = new();
            keyboard.ShowDialog();
            
            // Match Password
            if (keyboard.DialogResult == true)
            {
                // Get Value
                string Password = keyboard.UserInputBox.Text.ToString();

                // Check Password
                if (Password == StaffPassword) { LoginUser(btn.Uid); }
                else { MessageBox.Show($"Incorrect Password"); }
            }
        }

        public void LoginUser(string UUID)
        {
            // Log To Server
            Utility.SQL.ExecuteThisQuery($"Insert Into .[dbo].[StaffLogs] VALUES ('1', GetDate(), '{UUID}')");

            // Log Info Locally
            Utility.CurrentStaffInformation.StaffUUID = UUID;
            Utility.CurrentStaffInformation.StaffMemberName = Utility.SQL.ExecuteSQLScalar($"Select [StaffName] From .[dbo].[StoreLogin] Where StaffUUID = '{UUID}'", "CompanyAccess");
            Utility.CurrentStaffInformation.StaffProfilePicture = Utility.SQL.ExecuteSQLScalar($"Select [StaffImage] From .[dbo].[StaffLoginColours] Where StaffUUID = '{UUID}'", "CompanyAccess");
            Utility.CurrentStaffInformation.StaffColor = Utility.SQL.ExecuteSQLScalar($"Select [StaffColour] From .[dbo].[StaffLoginColours] Where StaffUUID = '{UUID}'", "CompanyAccess");

            // Proceed
            Menus.MainMenu MainMenu = new();
            MainMenu.Show();
            this.Close();
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
                    if (UserLogin != "NO_STAFF_MEMBER_WITH_ACCESS_CODE_FOUND" && UserLogin != "DEV-FLAG-GIVEN") { LogTest.Content = UserLogin; KeypadEntryBox.Text = ""; }
                    else if (UserLogin == "DEV-FLAG-GIVEN") { Utility.Keyboard k = new(); k.ShowDialog(); KeypadEntryBox.Text = ""; }
                    else { LogTest.Content = "Invalid Access Code"; }
                    break;

                default:
                    KeypadEntryBox.Text += b.Content;
                    break;
            }
        }
        
        /// <summary>
        /// If More Than 12 Staff, Show Page Next Buttons
        /// </summary>
        public void LoadPages()
        {
            // Check If More Than 12 Buttons
            if (NumberOfStaffButtonsCreated <= 12)
            {
                NextPageOfStaff.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Staff Page Controls (Next/Previous)
        /// </summary>
        public static int CurrentPage { get; set; } = 0;
        private void GetNextPageOfStaffMembers(object sender, RoutedEventArgs e)
        {
            // Set Values
            CurrentPage++;

            // Show Next Buttons
            foreach (Object o in LoginButtonGrid.Children)
            {
                if (o is Button)
                {
                    Button b = o as Button;
                    if (b.Tag.ToString() != "NEXT-BUTTON-RESERVED" && b.Tag.ToString() != "NEXT-BUTTON-RESERVED")
                    {
                        if (b.Tag.ToString() == CurrentPage.ToString()) 
                        {
                            b.Visibility = Visibility.Visible;
                        }
                        else { b.Visibility = Visibility.Collapsed; }
                    }
                }
            }

            // Check How Many Buttons Are Visible
            int NumberOfButtonsVisible = 0;
            foreach (Object o in LoginButtonGrid.Children)
            {
                if (o is Button)
                {
                    Button b = o as Button;
                    if (b.Visibility == Visibility.Visible) { NumberOfButtonsVisible++; }
                }
            }

            // If Less Than 12 Buttons, Hide Next Button
            if (NumberOfButtonsVisible <= 12)
            {
                NextPageOfStaff.Visibility = Visibility.Collapsed;
                PreviousPageOfStaff.Visibility = Visibility.Visible;
            }
        }
        private void GetPreviousPageOfStaffMembers(object sender, RoutedEventArgs e)
        {
            // Set Values
            CurrentPage--;

            // Show Next Buttons
            foreach (Object o in LoginButtonGrid.Children)
            {
                if (o is Button)
                {
                    Button b = o as Button;
                    if (b.Tag.ToString() != "NEXT-BUTTON-RESERVED" && b.Tag.ToString() != "NEXT-BUTTON-RESERVED")
                    {
                        if (b.Tag.ToString() == CurrentPage.ToString())
                        {
                            b.Visibility = Visibility.Visible;
                        }
                        else { b.Visibility = Visibility.Collapsed; }
                    }
                }
            }

            // Check How Many Buttons Are Visible
            int NumberOfButtonsVisible = 0;
            foreach (Object o in LoginButtonGrid.Children)
            {
                if (o is Button)
                {
                    Button b = o as Button;
                    if (b.Visibility == Visibility.Visible) { NumberOfButtonsVisible++; }
                }
            }

            // If More Than 12 Buttons, Show Next Button
            if (NumberOfButtonsVisible >= 12)
            {
                NextPageOfStaff.Visibility = Visibility.Visible;
                PreviousPageOfStaff.Visibility = Visibility.Collapsed;
            }
        }
    }
}
