using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmoothSNMP;
using System.Net;
using System.Net.Sockets;

namespace SSNMP
{
    class Program
    {
        static void Main(string[] args)
        {

            int datatype, datalength, datastart, commlength, miblength;
            string output;
            //byte[] response = new byte[1024];
            byte[] response = new byte[1024];
            Session s = new Session();
            String[] oids = new String[1] { "1.3.6.1.2.1.1.5.0" };
            response = s.get(oids);
            //response = PDU.get("get", "127.0.0.1", "public", "1.3.6.1.2.1.1.5.0");

            
            string str = GetStringFromResponse(response);
            Console.WriteLine(str);
            
            

        }
        public static string GetStringFromResponse(byte[] b)
        {
            int comL = b[6];
            int reqL = b[6 + comL + 4];
            int varbindL = b[6 + comL + 4 + reqL + 12];
            int sL = b[6 + comL + 4 + reqL + 12 + varbindL + 1 + 1];
            int offset = 6 + comL + 4 + reqL + 12 + varbindL + 1 + 1;

            return Encoding.ASCII.GetString(b, offset+1, sL);
        }

        public static List<byte[]> ConvertOIDsToBytes(string[] mibs)
        {
            int i;
            List<byte[]> res = new List<byte[]>();
            foreach (string s in mibs)
            {
                i = 0;
                byte[] b = new byte[s.Length - (s.Length / 2)];
                string[] withoutDots = s.Split('.');
                foreach (string s2 in withoutDots)
                {
                    if (s2 != "") 
                        b[i++] = Convert.ToByte(Convert.ToInt16(s2));
                }
                res.Add(b);
            }
            return res;
        }
    }

    class PDU
    {

        public static byte[] get(string request, string host, string community, string mibstring)
        {
            byte[] packet = new byte[1024];
            byte[] mib = new byte[1024];
            int snmplen;
            int comlen = community.Length;
            string[] mibvals = mibstring.Split('.');
            int miblen = mibvals.Length;
            int cnt = 0, temp, i;
            int orgmiblen = miblen;
            int pos = 0;

            // Convert the string MIB into a byte array of integer values
            // Unfortunately, values over 128 require multiple bytes
            // which also increases the MIB length
            for (i = 0; i < orgmiblen; i++)
            {
                temp = Convert.ToInt16(mibvals[i]);
                if (temp > 127)
                {
                    mib[cnt] = Convert.ToByte(128 + (temp / 128));
                    mib[cnt + 1] = Convert.ToByte(temp - ((temp / 128) * 128));
                    cnt += 2;
                    miblen++;
                }
                else
                {
                    mib[cnt] = Convert.ToByte(temp);
                    cnt++;
                }
            }
            snmplen = 29 + comlen + miblen - 1;  //Length of entire SNMP packet
            //The SNMP sequence start
            packet[pos++] = 0x30; //Sequence start
            packet[pos++] = Convert.ToByte(snmplen - 2);  //sequence size

            //SNMP version
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x01; //length
            packet[pos++] = 0x01; //SNMP version 2

            //Community name
            packet[pos++] = 0x04; // String type
            packet[pos++] = Convert.ToByte(comlen); //length
                                                    //Convert community name to byte array
            byte[] data = Encoding.ASCII.GetBytes(community);
            for (i = 0; i < data.Length; i++)
            {
                packet[pos++] = data[i];
            }

            //Add GetRequest or GetNextRequest value
            if (request == "get")
                packet[pos++] = 0xA0;
            else
                packet[pos++] = 0xA1;

            packet[pos++] = Convert.ToByte(20 + miblen - 1); //Size of total MIB

            //Request ID
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x04; //length
            packet[pos++] = 0x00; //SNMP request ID
            packet[pos++] = 0x00;
            packet[pos++] = 0x00;
            packet[pos++] = 0x01;

            //Error status
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x01; //length
            packet[pos++] = 0x00; //SNMP error status

            //Error index
            packet[pos++] = 0x02; //Integer type
            packet[pos++] = 0x01; //length
            packet[pos++] = 0x00; //SNMP error index

            //Start of variable bindings
            packet[pos++] = 0x30; //Start of variable bindings sequence

            packet[pos++] = Convert.ToByte(6 + miblen - 1); // Size of variable binding

            packet[pos++] = 0x30; //Start of first variable bindings sequence
            packet[pos++] = Convert.ToByte(6 + miblen - 1 - 2); // size
            packet[pos++] = 0x06; //Object type
            packet[pos++] = Convert.ToByte(miblen - 1); //length

            //Start of MIB
            packet[pos++] = 0x2b;
            //Place MIB array in packet
            for (i = 2; i < miblen; i++)
                packet[pos++] = Convert.ToByte(mib[i]);
            packet[pos++] = 0x05; //Null object value
            packet[pos++] = 0x00; //Null

            //foreach (byte b in packet)
            //  Console.WriteLine(b.ToString("X"));

            //Send packet to destination
            /*Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                             ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket,
                            SocketOptionName.ReceiveTimeout, 5000);
            IPHostEntry ihe = Dns.Resolve(host);
            IPEndPoint iep = new IPEndPoint(ihe.AddressList[0], 161);
            EndPoint ep = (EndPoint)iep;
            sock.SendTo(packet, snmplen, SocketFlags.None, iep);*/
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(host), 161);
            UdpClient udpClient = new UdpClient();
            Console.WriteLine("Connecting...");
            udpClient.Connect(iep);
            Console.WriteLine("Connected!");
            udpClient.Send(packet, snmplen);

            //Receive response from packet
            try
            {
                byte[] response =  udpClient.Receive(ref iep);

                return response;
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return null; }

            
        }
       
    }
}
