using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Spinnaker.UserInterface;

namespace Spinnaker
{
    public class AppController
    {
        public static AppController Current;
        public static string api_key = "";
        public static string api_secret = "";


        public Preferences preferences;
        public static List<string> savedUsernames;
        public static List<string> savedHashtags;
        public static ObservableCollection<Model.Account> accounts;
        
        private AppController()
        {
            savedUsernames = new List<string>();
            savedHashtags = new List<string>();
            accounts = new ObservableCollection<Model.Account>();

            if (!string.IsNullOrEmpty(Common.api_key) && !string.IsNullOrEmpty(Common.api_secret))
            {
                AppController.api_key = Common.api_key;
                AppController.api_secret = Common.api_secret;
            }
            else if (!string.IsNullOrEmpty(Properties.Settings.Default.api_key) && !string.IsNullOrEmpty(Properties.Settings.Default.api_secret))
            {
                AppController.api_key = Properties.Settings.Default.api_key;
                AppController.api_secret = Properties.Settings.Default.api_secret;
            }
            else
            {
                UserInterface.GetCommonParameter parameter = new GetCommonParameter();
                parameter.Show();
                return;
            }

            startup_completed();
        }

        public void startup_completed()
        {
            load_stored_accounts();

            preferences = new Preferences();
            preferences.Show();
            
            if (accounts.Count == 0)
            {
                add_new_account();
            }
        }

        private void load_stored_accounts()
        {
            string[] delimiter = { "|||" };
            if (!string.IsNullOrEmpty(Properties.Settings.Default.access_tokens))
            {
                string[] access_tokens = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.access_tokens)).Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (access_tokens.Length > 0)
                {
                    foreach (string access_token in access_tokens)
                    {
                        Model.Account account = new Model.Account();
                        account.access_token = access_token;
                        Tuple<AppNetDotNet.Model.Token, AppNetDotNet.ApiCalls.ApiCallResponse> token_response = AppNetDotNet.ApiCalls.Tokens.get(account.access_token);
                        if (token_response.Item2.success)
                        {
                            account.token = token_response.Item1;
                            accounts.Add(account);
                            return;
                        }
                    }
                }
            }
        }

        public void add_new_account()
        {
            AppNetDotNet.Model.Authorization.clientSideFlow auth_window = new AppNetDotNet.Model.Authorization.clientSideFlow(api_key, Common.redirect_url, "basic write_post files");
            auth_window.AuthSuccess += auth_window_AuthSuccess;
            auth_window.showAuthWindow();
        }

        void auth_window_AuthSuccess(object sender, AppNetDotNet.AuthorizationWindow.AuthEventArgs e)
        {
            if (e.success)
            {
                Model.Account account = new Model.Account();
                account.access_token = e.accessToken;
                Tuple<AppNetDotNet.Model.Token, AppNetDotNet.ApiCalls.ApiCallResponse> token_response = AppNetDotNet.ApiCalls.Tokens.get(account.access_token);
                if (token_response.Item2.success)
                {
                    account.token = token_response.Item1;
                    accounts.Add(account);
                    return;
                }
                else
                {
                    account = null;
                }
            }
        }

        public static void Start()
        {
            if (Current == null)
            {
                Current = new AppController();
            }
        }
    }
}
