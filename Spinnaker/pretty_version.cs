using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spinnaker
{
    class pretty_version
    {
        /// <summary>
        /// Returns a version string without trailing .0 (so 3.1.0.0 will return 3.1 for example)
        /// </summary>
        /// <param name="version_string">The version you want to format (e. g. 3.1.0.0)</param>
        /// <returns></returns>
        public static string get_nice_version_string(string version_string = null)
        {
            if (string.IsNullOrEmpty(version_string))
            {
                version_string = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            string pretty_string = version_string;
            while (pretty_string.Length > 3 && pretty_string.EndsWith(".0"))
            {
                pretty_string = pretty_string.Substring(0, pretty_string.Length - 2);
            }
            return pretty_string;
        }
    }
}
