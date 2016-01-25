using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothSNMP
{
    /// <summary>
    /// Class which represents a session, in which SNMPv2c requests can be made.
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

        /// <summary>
        /// Changes the community associated with the session.
        /// </summary>
        /// <param name="community">Community to be associated with the session.</param>
        public void setCommunity (string community)
        {
            this.community = community;
        }

        /// <summary>
        /// Changes the host associated with the session.
        /// </summary>
        /// <param name="host">Host to be associated with the session.</param>
        public void setHost (string host)
        {
            this.host = host;
        }

        /// <summary>
        /// Changes the port associated with the session.
        /// </summary>
        /// <param name="port">Port to be associated with the session.</param>
        public void setPort (int port)
        {
            this.port = port;
        }


        /// <summary>
        /// Builds and sends an SNMP GetRequest PDU.
        /// </summary>
        /// <param name="mibs">OIDs to be attached to the GetRequest.</param>
        /// <returns>The response to the GetRequest from the Agent.</returns>
        public byte[] get(string[] mibs)
        {
            
            PDU pdu = new PDU(1024);
            byte[] data =  pdu.buildPDU(0, 1, this.community, mibs);
            for (int i = 0; i < data.Length; i++)
                Console.WriteLine(i + " - " + data[i].ToString("X"));
            return data;

        }

        public static void main (String[] args)
        {
            string[] mibs = new string[1] { "1.2.3.4.5.6.7" };
            Session session = new Session();
            byte[] pdu = session.get(mibs);
            for (int i = 0; i<pdu.Length;i++)
                Console.WriteLine(i+" - "+pdu[i].ToString("X"));
        }
        
    }
}
