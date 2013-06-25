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
        public User user
        {
            get
            {
                if (token != null)
                {
                    return token.user;
                }
                else
                {

                    return null;
                }
            }
        }
        public override string ToString()
        {
            return "@" + username;
        }

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


        public bool send_post(string text, string local_file_to_embed = "", Entities entities = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                List<AppNetDotNet.Model.File> toBeAddedFiles = null;
                if (!string.IsNullOrEmpty(local_file_to_embed))
                {
                    if (System.IO.File.Exists(local_file_to_embed))
                    {
                        Tuple<AppNetDotNet.Model.File, ApiCallResponse> uploadedFile = AppNetDotNet.ApiCalls.Files.create(this.access_token, local_file_path:local_file_to_embed, type: "de.li-ghun.spinnaker.image");
                        if (uploadedFile.Item2.success)
                        {
                            toBeAddedFiles = new List<File>();
                            toBeAddedFiles.Add(uploadedFile.Item1);
                        }
                    }
                }
                Tuple<Post,ApiCallResponse> response = Posts.create(this.access_token, text, toBeEmbeddedFiles:toBeAddedFiles, entities:entities, parse_links:true);
                return response.Item2.success;
            }
            return false;
        }
    }
}
