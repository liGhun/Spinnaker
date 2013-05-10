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
        public static RoutedCommand insert_link_command = new RoutedCommand();
        public static RoutedCommand add_image_command = new RoutedCommand();
        public static RoutedCommand account_scroll_command = new RoutedCommand();
        public static RoutedCommand cancel_command = new RoutedCommand();

        public Compose()
        {
            InitializeComponent();
            
            combobox_accounts.ItemsSource = AppController.accounts;
            if (AppController.accounts.Count > 0)
            {
                combobox_accounts.SelectedIndex = 0;
            }
            if (AppController.last_used_account != null)
            {
                combobox_accounts.SelectedItem = AppController.last_used_account;
            }
            autoCompeteTextbox_post.textBoxContent.TextChanged += textBoxContent_TextChanged;
            autoCompeteTextbox_post.textBoxContent.KeyDown += textBoxContent_KeyDown;

            insert_link_command.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
            add_image_command.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));
            cancel_command.InputGestures.Add(new KeyGesture(Key.Escape, ModifierKeys.None));
            account_scroll_command.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
        }

        void textBoxContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key.Return & Key.LeftCtrl) || e.Key == (Key.Enter & Key.LeftCtrl) || e.Key == (Key.Return & Key.RightCtrl) || e.Key == (Key.Enter & Key.RightCtrl))
            {
                if (!string.IsNullOrWhiteSpace(autoCompeteTextbox_post.textBoxContent.Text))
                {
                    button_send_Click(null,null);
                }
            }
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
                remaining_chars -= autoCompeteTextbox_post.NumberOfChars;
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
                    AppNetDotNet.Model.Entities entities = null;
                    string toBePostedText = autoCompeteTextbox_post.textBoxContent.Text;
                    if (autoCompeteTextbox_post.MarkdownLinksInText.Count() > 0)
                    {
                        entities = new AppNetDotNet.Model.Entities();
                        entities.links = new List<AppNetDotNet.Model.Entities.Link>();
                        entities.hashtags = null;
                        entities.mentions = null;
                        foreach (KeyValuePair<string, string> link in autoCompeteTextbox_post.MarkdownLinksInText)
                        {
                            AppNetDotNet.Model.Entities.Link linkEntity = new AppNetDotNet.Model.Entities.Link();
                            linkEntity.text = link.Value;
                            linkEntity.url = link.Key;
                            int startPosition = toBePostedText.IndexOf(string.Format("[{0}]({1})",linkEntity.text, linkEntity.url));
                            linkEntity.pos = startPosition;
                            linkEntity.len = linkEntity.text.Length;
                            toBePostedText = toBePostedText.Replace(string.Format("[{0}]({1})", linkEntity.text, linkEntity.url), linkEntity.text);
                            entities.links.Add(linkEntity);
                        }
                    }


                    if (account.send_post(toBePostedText, path_to_be_uploaded_image, entities:entities))
                    {
                        AppController.last_used_account = account;
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
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && Mouse.DirectlyOver != autoCompeteTextbox_post.textBoxContent)
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
                    autoCompeteTextbox_post.textBoxContent.Text = autoCompeteTextbox_post.textBoxContent.Text.Insert(autoCompeteTextbox_post.textBoxContent.CaretIndex, "[" + System.IO.Path.GetFileNameWithoutExtension(path_to_be_uploaded_image) + "](photos.app.net/{post_id}/1)");
                    button_upload_photo.ToolTip = "Right click to remove the image";
                }
            }
        }

        private void button_upload_photo_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            image_upload_photo.Opacity = 0.7;
            if (autoCompeteTextbox_post.textBoxContent.Text.Contains("[" + System.IO.Path.GetFileNameWithoutExtension(path_to_be_uploaded_image) + "](photos.app.net/{post_id}/1)"))
            {
                autoCompeteTextbox_post.textBoxContent.Text = autoCompeteTextbox_post.textBoxContent.Text.Replace("[" + System.IO.Path.GetFileNameWithoutExtension(path_to_be_uploaded_image) + "](photos.app.net/{post_id}/1)", "");
            }
            path_to_be_uploaded_image = "";
            button_upload_photo.ToolTip = "Upload an image";
        }

        private void button_upload_photo_Drop_1(object sender, DragEventArgs e)
        {
            if (e.Data is System.Windows.DataObject &&
  ((System.Windows.DataObject)e.Data).ContainsFileDropList())
            {
                foreach (string filePath in ((System.Windows.DataObject)e.Data).GetFileDropList())
                {
                    Console.WriteLine(e);
                }
            } 
            
            Console.WriteLine(e);
        }

        private void button_insert_link_Click(object sender, RoutedEventArgs e)
        {
            UserInterface.AddLink add_link_window = new AddLink();
            add_link_window.Left = this.Left;
            add_link_window.Top = this.Top;
            add_link_window.Width = this.Width;
            add_link_window.Height = this.Height;
            add_link_window.InsertLink += add_link_window_InsertLink;
            add_link_window.Show();
        }

        void add_link_window_InsertLink(object sender, AddLink.InsertLinkEventArgs e)
        {
            if (e != null)
            {
                if (e.success)
                {
                    autoCompeteTextbox_post.textBoxContent.Text = autoCompeteTextbox_post.textBoxContent.Text.Insert(autoCompeteTextbox_post.textBoxContent.CaretIndex, e.insert_string);
                }
            }
        }



        #region keyboard shortcuts

        private void command_insert_link_executed(object sender, ExecutedRoutedEventArgs e)
        {
            button_insert_link_Click(null, null);
        }

        private void command_add_image_executed(object sender, ExecutedRoutedEventArgs e)
        {
            button_upload_photo_Click_1(null, null);
        }

        private void command_account_scroll_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (combobox_accounts.Items.Count > 1)
            {
                int selected_index = combobox_accounts.SelectedIndex;
                selected_index--;
                if (selected_index < 0)
                {
                    selected_index = combobox_accounts.Items.Count - 1;
                }
                combobox_accounts.SelectedIndex = selected_index;
            }
        }

        private void command_cancel_executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
