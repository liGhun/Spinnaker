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
        

        public Preferences()
        {
            InitializeComponent();
            listview_accounts.ItemsSource = AppController.accounts;

        }

        public void register_hotkey()
        {
            HotKeyHost hotKeyHost = new HotKeyHost((HwndSource)HwndSource.FromVisual(this));
            HotKey hotkey = new HotKey(Key.P, ModifierKeys.Shift|ModifierKeys.Control, true);
            hotkey.HotKeyPressed += new EventHandler<HotKeyEventArgs>(delegate(Object o, HotKeyEventArgs e)
            {
                Compose compose = new Compose();
                compose.Show();
            });
            hotKeyHost.AddHotKey(hotkey);
        }

       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Compose compose = new Compose();
            compose.Show();
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
            App.Current.Shutdown();
        }
    }
}
