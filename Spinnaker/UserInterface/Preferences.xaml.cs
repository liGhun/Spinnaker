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
        private System.Windows.Forms.ContextMenu m_notifyMenu;

        public Preferences()
        {
            InitializeComponent();
            listview_accounts.ItemsSource = AppController.accounts;

            #region Tray icon init
            // from my Desktop Google Reader project

            try
            {
                m_notifyIcon = new System.Windows.Forms.NotifyIcon();
                m_notifyIcon.Text = "Spinnaker";

                System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Spinnaker;component/Images/spinnaker_32.ico")).Stream;
                m_notifyIcon.Icon = new System.Drawing.Icon(iconStream);
                m_notifyIcon.DoubleClick += new EventHandler(m_notifyIcon_Click);

                m_notifyMenu = new System.Windows.Forms.ContextMenu();
                m_notifyMenu.MenuItems.Add("Spinnaker");
                m_notifyMenu.MenuItems.Add("-");
                m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Open Prefernces", new System.EventHandler(trayContextShow)));
                m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Compose new post", new System.EventHandler(trayContextPost)));
                m_notifyMenu.MenuItems.Add("-");
                m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Quit", new System.EventHandler(trayContextQuit)));

                m_notifyIcon.ContextMenu = m_notifyMenu;
            }
            catch (Exception exp)
            {
                // you might have changed the binary name or the icon path - please update you m_notifyIcon code
                Console.WriteLine(exp.Message);
            }

            #endregion

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
            App.Current.Shutdown();
        }

        private void button_add_another_account_Click(object sender, RoutedEventArgs e)
        {
            AppController.Current.add_new_account();
        }

        private void button_hide_preferences_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void button_exit_app_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void border_with_roundes_edges_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
