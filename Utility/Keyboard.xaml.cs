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
    public partial class Keyboard : Window
    {
        public Keyboard()
        {
            InitializeComponent();
            UserInputBox.Clear();
            UserInputBox.Focus();
        }

        /// <summary>
        /// Handle Keyboard Presses
        /// </summary>
        public bool CapsEnabled { get; set; } = false;
        private void ButtonHandler(object sender, RoutedEventArgs e)
        {
            // Name
            Button b = sender as Button;

            // Content Handler
            switch (b.Content.ToString())
            {
                // Delete Key
                case "⬅️ Del":
                    if (UserInputBox.Text.Length > 0)
                    {
                        UserInputBox.Text = UserInputBox.Text.Substring(0, UserInputBox.Text.Length - 1);
                    }
                    break;

                default:

                    if (b.Tag != null && b.Tag.ToString() != "")
                    {
                        // Check If Caps Lock Was Pressed
                        if (b.Tag.ToString() == "CapsLockButton")
                        {
                            if (CapsEnabled) { CapsEnabled = false; CapsLockButtonObject.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#22d3ee"); }
                            else if (!CapsEnabled) { CapsEnabled = true; CapsLockButtonObject.BorderBrush = Brushes.Red; }
                        }

                        // Enter Pressed - Close Window
                        else if (b.Tag.ToString() == "EnterButton") { if (UserInputBox.Text != "") { DialogResult = true; } else { this.Close(); } }

                        // Space Button
                        else if (b.Tag.ToString() == "SpaceButton") { UserInputBox.Text += " "; }
                    }

                    // Handle All Other Buttons
                    else
                    {
                        if (CapsEnabled) { UserInputBox.Text += b.Content.ToString().ToUpper(); }
                        else { UserInputBox.Text += b.Content.ToString().ToLower(); }
                    }
                    break;
            }
        }
    }
}
