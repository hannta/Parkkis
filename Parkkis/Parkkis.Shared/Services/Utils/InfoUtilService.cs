using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel;

namespace Parkkis.Services.Utils
{
    /// <summary>
    /// App info util
    /// </summary>
    public class InfoUtilService
    {

        /// <summary>
        /// App version
        /// </summary>
        public string AppVersion
        {
            get
            {
                var ver = Package.Current.Id.Version;
                return ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();
            }
        }


        /// <summary>
        /// User aget string
        /// </summary>
        public string UserAgent
        {
            get
            {
                return "Parkkis " + this.AppVersion + " / WP";
            }
        }


    }
}
