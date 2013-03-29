using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spinnaker.Model
{
    public class Draft
    {
        public string  text { get; set; }

        public bool send_now(Account account)
        {
            if (account != null)
            {
                return account.send_post(text);
            }
            return false;
        }
    }
}
