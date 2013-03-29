using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppNetDotNet.Model;
using AppNetDotNet.ApiCalls;

namespace Spinnaker.Model
{
    public class Account
    {
        public string access_token { get; set; }
        public Token token { get; set; }
        public User user { get; set; }

        public string username
        {
            get
            {
                if (user != null)
                {
                    return user.username;
                }
                else
                {
                    return "Unauthorized";
                }
            }

        }


        public bool send_post(string text, string local_file_to_embed = "")
        {
            if (!string.IsNullOrEmpty(text))
            {
                Posts.create(this.access_token, text);
            }
            return false;
        }
    }
}
