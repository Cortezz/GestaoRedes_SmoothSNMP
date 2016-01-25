using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSNMP
{
    class session
    {
        private string host;
        private int port;
        private string community;

        /// <summary>
        /// Creates a session whose default values are:
        ///     - Community : public
        ///     - port : 161
        ///     - host : localhost
        /// </summary>
        public session()
        {
            this.host = "localhost";
            this.port = 161;
            this.community = "public";
        }


        /// <summary>
        /// Creates a session with a specified host and community.
        /// </summary>
        /// <param name="host">Host associated with the session.</param>
        /// <param name="community">Community associated with the session.</param>
        public session (string host, string community)
        {
            this.host = host;
            this.community = community;
        }

    }
}
