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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using Spinnaker.GlobalHotKeys;

namespace Spinnaker.UserInterface
{
        /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Preferences : Window
    {

        private WindowState m_storedWindowState = WindowState.Normal;
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenuStrip m_notifyMenu;

        public Preferences()
        {
            InitializeComponent();
            listview_accounts.ItemsSource = AppController.accounts;

            textblock_app_name.Text = Spinnaker.Common.app_name;
            textblock_version_and_license.Text = "";
            textblock_version_and_license.Inlines.Add(Spinnaker.AppController.version_string);

            textblock_version_and_license.Inlines.Add(" · ");

            Hyperlink link_to_license = new Hyperlink();
            link_to_license.Click += link_to_license_Click;
            link_to_license.TextDecorations = null;
            link_to_license.Foreground = Brushes.White;
            link_to_license.Cursor = Cursors.Hand;
            link_to_license.ToolTip = "Open license text (BSD 3)";
            link_to_license.Inlines.Add("BSD 3 License");

            textblock_version_and_license.Inlines.Add(link_to_license);


            #region Tray icon init
            // from my Desktop Google Reader project

            try
            {
                m_notifyIcon = new System.Windows.Forms.NotifyIcon();
                m_notifyIcon.Text = "Spinnaker";

                System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Spinnaker;component/Images/spinnaker_32.ico")).Stream;
                m_notifyIcon.Icon = new System.Drawing.Icon(iconStream);
                m_notifyIcon.DoubleClick += new EventHandler(m_notifyIcon_Click);

                #region loading icons

                System.Drawing.Image preferences_icon = null;
                System.Drawing.Image quit_icon = null;
                System.Drawing.Image write_icon = null;

                try {
                iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Spinnaker;component/Images/TrayIconMenu/preferences.png")).Stream;
                preferences_icon = System.Drawing.Image.FromStream(iconStream);

                iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Spinnaker;component/Images/TrayIconMenu/quit.png")).Stream;
                quit_icon = System.Drawing.Image.FromStream(iconStream);

                iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Spinnaker;component/Images/TrayIconMenu/write.png")).Stream;
                write_icon = System.Drawing.Image.FromStream(iconStream);
                }
                catch {}

                #endregion

                m_notifyMenu = new System.Windows.Forms.ContextMenuStrip();
                m_notifyMenu.Items.Add("Spinnaker");
                m_notifyMenu.Items.Add("-");
                m_notifyMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Open Preferences", preferences_icon, new System.EventHandler(trayContextShow)));
                System.Windows.Forms.ToolStripMenuItem menuitem_compose = new System.Windows.Forms.ToolStripMenuItem("Compose new post", write_icon, new System.EventHandler(trayContextPost));
                menuitem_compose.ToolTipText = "ctrl-shift-p";
                m_notifyMenu.Items.Add(menuitem_compose);
                m_notifyMenu.Items.Add("-");
                m_notifyMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Quit", quit_icon,new System.EventHandler(trayContextQuit)));

                m_notifyIcon.ContextMenuStrip = m_notifyMenu;
            }
            catch (Exception exp)
            {
                // you might have changed the binary name or the icon path - please update you m_notifyIcon code
                Console.WriteLine(exp.Message);
            }

            #endregion

        }

        void link_to_license_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Spinnaker.Common.license_url);
            }
            catch { }
        }

        #region Tray icon methods

        protected void trayContextShow(Object sender, System.EventArgs e)
        {
            Show();
        }

        protected void trayContextPost(Object sender, System.EventArgs e)
        {
            AppController.Current.open_compose_window();
        }

        protected void trayContextQuit(Object sender, System.EventArgs e)
        {
            this.Close();
        }

        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState != WindowState.Minimized)
            {
                m_storedWindowState = WindowState;
            }


        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }


        #endregion

        public void register_hotkey()
        {
            HotKeyHost hotKeyHost = new HotKeyHost((HwndSource)HwndSource.FromVisual(this));
            HotKey hotkey = new HotKey(Key.P, ModifierKeys.Shift|ModifierKeys.Control, true);
            hotkey.HotKeyPressed += new EventHandler<HotKeyEventArgs>(delegate(Object o, HotKeyEventArgs e)
            {
                AppController.Current.open_compose_window();
            });
            hotKeyHost.AddHotKey(hotkey);
        }

       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            grid_main.Focus();
            AppController.Current.open_compose_window();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string access_tokens = "";
            foreach (Model.Account account in AppController.accounts)
            {
                access_tokens += account.access_token + "|||";
            }
            access_tokens.TrimEnd('|');
            Properties.Settings.Default.access_tokens = Crypto.EncryptString(Crypto.ToSecureString(access_tokens));
            Properties.Settings.Default.Save();
            try
            {
                ShowTrayIcon(false);
            }
            catch { }
            App.Current.Shutdown();
        }

        private void button_add_another_account_Click(object sender, RoutedEventArgs e)
        {
            grid_main.Focus();
            AppController.Current.add_new_account();
        }

        private void button_hide_preferences_Click_1(object sender, RoutedEventArgs e)
        {
            grid_main.Focus();
            this.Hide();
        }

        private void button_exit_app_Click_1(object sender, RoutedEventArgs e)
        {
            grid_main.Focus();
            this.Close();
        }

        private void border_with_roundes_edges_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
