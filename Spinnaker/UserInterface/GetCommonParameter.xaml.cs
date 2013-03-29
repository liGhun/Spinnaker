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

namespace Spinnaker.UserInterface
{
    /// <summary>
    /// Interaction logic for GetCommonParameter.xaml
    /// </summary>
    public partial class GetCommonParameter : Window
    {
        public GetCommonParameter()
        {
            InitializeComponent();
        }

        private void button_save_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.api_key = texbox_api_key.Text;
            Properties.Settings.Default.api_secret = texbox_api_secret.Text;
            Properties.Settings.Default.Save();

            AppController.Current.startup_completed();
            Close();
        }
    }
}
