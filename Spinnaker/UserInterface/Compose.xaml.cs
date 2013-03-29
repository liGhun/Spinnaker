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
        }

        private void button_send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(autoCompeteTextbox_post.textBoxContent.Text))
            {
                Account account = combobox_accounts.SelectedItem as Account;
                if (account != null)
                {
                    if (account.send_post(autoCompeteTextbox_post.textBoxContent.Text))
                    {
                        Close();
                    }
                }
            }
        }
    }
}
