using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spinnaker.UserInterface;

namespace Spinnaker
{
    public class AppController
    {
        public static AppController Current;
        public Preferences preferences;
        public static List<string> SavedUsernames;
        public static List<string> SavedHashtags;
        
        private AppController()
        {
            SavedUsernames = new List<string>();
            SavedHashtags = new List<string>();
            preferences = new Preferences();
            preferences.Show();
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
