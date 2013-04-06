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
        public static string version_string = "";
        public static Model.Account last_used_account;

        public Preferences preferences;
        private UserInterface.Startup startup;


        public static List<string> savedUsernames;
        public static List<string> savedHashtags;
        public static ObservableCollection<Model.Account> accounts;
        
        private AppController()
        {
            savedUsernames = new List<string>();
            savedHashtags = new List<string>();
            accounts = new ObservableCollection<Model.Account>();
            accounts.CollectionChanged += accounts_CollectionChanged;
            AppController.version_string = pretty_version.get_nice_version_string();

            startup = new Startup();
            startup.Show();

            try
            {
                if (!Properties.Settings.Default.settings_updated)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.settings_updated = true;
                }
            }
            catch
            {
                try
                {
                    Properties.Settings.Default.Reset();
                }
                catch { }
            }

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

        void accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Model.Account account in e.NewItems)
                {
                    AppNetDotNet.ApiCalls.Parameters parameters = new AppNetDotNet.ApiCalls.Parameters();
                    parameters.count = 200;
                    Tuple<List<AppNetDotNet.Model.User>,AppNetDotNet.ApiCalls.ApiCallResponse> followings = AppNetDotNet.ApiCalls.Users.getFollowingsOfUser(account.access_token,account.username,parameters);
                    if(followings.Item2.success) {
                        foreach(AppNetDotNet.Model.User following in followings.Item1) {
                            if(!savedUsernames.Contains(following.username)) {
                                savedUsernames.Add("@" + following.username);
                            }
                        }
                    }
                    Tuple<List<AppNetDotNet.Model.User>, AppNetDotNet.ApiCalls.ApiCallResponse> followers = AppNetDotNet.ApiCalls.Users.getFollowersOfUser(account.access_token, account.username, parameters);
                    if (followers.Item2.success)
                    {
                        foreach (AppNetDotNet.Model.User follower in followers.Item1)
                        {
                            if (!savedUsernames.Contains(follower.username))
                            {
                                savedUsernames.Add("@" + follower.username);
                            }
                        }
                    }
                }
            }
        }

        public void startup_completed()
        {
            load_stored_accounts();
            preferences = new Preferences();

            startup.Close();
            preferences.Show();
            preferences.register_hotkey();
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
                        }
                    }
                }
            }
        }

        public void add_new_account()
        {
            AppNetDotNet.Model.Authorization.registerAppInRegistry(AppNetDotNet.Model.Authorization.registerBrowserEmulationValue.IE8Always, alsoCreateVshostEntry: true);
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

        public void open_compose_window(string initial_text = "") 
        {
            UserInterface.Compose compose = new Compose();
            compose.open();
        }
    }
}
