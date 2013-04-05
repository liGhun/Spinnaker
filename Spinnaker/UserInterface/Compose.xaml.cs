using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Spinnaker.Model;

namespace Spinnaker.UserInterface
{
    /// <summary>
    /// Interaction logic for Compose.xaml
    /// </summary>
    public partial class Compose : Window
    {
        public Compose()
        {
            InitializeComponent();
            combobox_accounts.ItemsSource = AppController.accounts;
            if (AppController.accounts.Count > 0)
            {
                combobox_accounts.SelectedIndex = 0;
            }
            autoCompeteTextbox_post.textBoxContent.TextChanged += textBoxContent_TextChanged;
        }

        public string path_to_be_uploaded_image { get; set; }

        void textBoxContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (button_send == null)
            {
                return;
            }
            button_send.IsEnabled = true;
            int remaining_chars = 255;
            
            if (textblock_remaining_chars != null)
            {
                remaining_chars -= autoCompeteTextbox_post.textBoxContent.Text.Length;
                textblock_remaining_chars.Text = remaining_chars.ToString();
            }
            if (remaining_chars < 0)
            {
                textblock_remaining_chars.Foreground = Brushes.Red;
                button_send.IsEnabled = false;
            }
            else if (remaining_chars < 3)
            {
                textblock_remaining_chars.Foreground = Brushes.Yellow;
            }
            else
            {
                textblock_remaining_chars.Foreground = Brushes.White;
            }
        }

        public void open()
        {
            this.Show();
            this.Focus();
            autoCompeteTextbox_post.textBoxContent.Focus();
        }

        private void button_send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(autoCompeteTextbox_post.textBoxContent.Text))
            {
                Account account = combobox_accounts.SelectedItem as Account;
                if (account != null)
                {
                    if (account.send_post(autoCompeteTextbox_post.textBoxContent.Text,path_to_be_uploaded_image))
                    {
                        Close();
                    }
                }
            }
        }

        private void button_close_window_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        private void combobox_accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            autoCompeteTextbox_post.textBoxContent.Focus();
        }

        private void button_upload_photo_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(path_to_be_uploaded_image))
            {
                return;
            }
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Images (*.png,*.jpg,*.gif,*.tif)|*.png;*.jpeg;*.jpg;*.gif;*.tif;*.tiff"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string filename = dlg.FileName;
                if (System.IO.File.Exists(filename))
                {
                    path_to_be_uploaded_image = filename;
                    image_upload_photo.Opacity = 1.0;
                    autoCompeteTextbox_post.textBoxContent.Text += " photos.app.net/{post_id}/1";
                    button_upload_photo.ToolTip = "Right click to remove the image";
                }
            }
        }

        private void button_upload_photo_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            path_to_be_uploaded_image = "";
            image_upload_photo.Opacity = 0.7;
            if(autoCompeteTextbox_post.textBoxContent.Text.Contains("photos.app.net/{post_id}/1")) {
                autoCompeteTextbox_post.textBoxContent.Text = autoCompeteTextbox_post.textBoxContent.Text.Replace("photos.app.net/{post_id}/1","");
            }
            button_upload_photo.ToolTip = "Upload an image";
        }
    }
}
