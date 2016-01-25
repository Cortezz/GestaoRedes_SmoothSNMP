using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothSNMP
{
    /// <summary>
    /// Class which represents a session, in which SNMP requests can be made.
    /// </summary>
    public class Session
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
        public Session()
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
        public Session (string host, string community)
        {
            this.host = host;
            this.community = community;
        }



        public byte[] get()
        {
            PDU pdu = new PDU(100);
            //Needs to be implemented
            return new byte[25];
        }
        
    }
}
