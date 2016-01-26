using System;
using System.Net;
using System.Net.Sockets;
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
        private int requestCounter;

        /// <summary>
        /// Creates a session whose default values are:
        ///     - Community : public
        ///     - port : 161
        ///     - host : localhost
        /// </summary>
        public Session()
        {
            this.host = "127.0.0.1";
            this.port = 161;
            this.community = "public";
            this.requestCounter = 0;
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
            this.port = 161;
            this.requestCounter = 0;
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
            byte[] data =  pdu.buildPDU(requestCounter, 0, this.community, mibs);
            byte[] response = SendPDU(data, pdu.getIndex()+1);
            return response;

        }

        /// <summary>
        /// Sends a PDU to the SNMP Agent. 
        /// Timeout is specified to be 5 seconds.
        /// </summary>
        /// <param name="PDU">SNMP PDU to be sent.</param>
        /// <param name="size">Size of the PDU.</param>
        /// <returns>The response from the SNMP Agent.</returns>
        private byte[] SendPDU(byte[] PDU, int size)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(host), 161);
            UdpClient udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 5000;
            udpClient.Connect(iep);
            udpClient.Send(PDU, size);
            
            try
            {
                
                byte[] response = udpClient.Receive(ref iep);
                requestCounter++;
                return response;
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return null; }
        }
        
    }
}
