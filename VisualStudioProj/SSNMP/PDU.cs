using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothSNMP
{
    internal class PDU
    {
        private byte[] pdu;
        private int index;

        public PDU(int size)
        {
            pdu = new byte[size];
            index = 0;
        }

        public byte[] buildPDU(int requestID, int type, string community, string[] mibs)
        {
            int MIBlength = 0;
            foreach (string s in mibs)
                MIBlength += s.Length;


            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(22 + community.Length + MIBlength + (2 * mibs.Length)); //Length of the PDU
            //Version 2C
            FillInVersion2C();
            //Community
            FillInCommunity(community);
            //PDU Type
            pdu[index++] = Convert.ToByte(160 + type); //Type of the SNMP PDU
            pdu[index++] = Convert.ToByte(15 + MIBlength + 2 * mibs.Length); //Length
            //Request ID
            FillInRequestID(requestID);
            //Error Code and Status
            FillInErrorCodeAndStatus();
            //OIDS
            FillInVarbindList(mibs, MIBlength);

            return this.pdu;
        }

        private void FillInVersion2C()
        {
            pdu[index++] = 0x02; //Type: Integer
            pdu[index++] = 0x01; //Length: 1
            pdu[index++] = 0x01; // Version: 2C
        }

        private void FillInCommunity(string community)
        {
            byte[] comBytes = Encoding.ASCII.GetBytes(community);

            pdu[index++] = 0x04; //Type: Octet String
            pdu[index++] = Convert.ToByte(community.Length); //Length
            foreach (Byte b in comBytes)
                pdu[index++] = b;  
        }

        private void FillInRequestID (int request)
        {
            this.pdu[index++] = 0x02; //Type: Integer
            this.pdu[index++] = 0x01; //Length: 4
            this.pdu[index++] = Convert.ToByte(request); //Request ID
        }


        private void FillInErrorCodeAndStatus()
        {
            //Error code
            this.pdu[index++] = 0x02; //Type: Integer
            this.pdu[index++] = 0x01; //Length: 1
            this.pdu[index++] = 0x00; //No error
            //Error index
            this.pdu[index++] = 0x02; //Type: Integer
            this.pdu[index++] = 0x01; //Length: 1
            this.pdu[index++] = 0x00; //Since there is no error, the index points to "nowhere".
        }

        private void FillInVarbindList(string[] mibs, int MIBlength)
        {
            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(4 + MIBlength + 2 * mibs.Length); // Length
            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(2 + MIBlength + 2 * mibs.Length); // Length
            foreach (string s in mibs)
            {
                pdu[index++] = 0x06;
                pdu[index++] = Convert.ToByte(s.Length); //MIB's length
                pdu[index++] = 0x2b; //Beginning of MIB
                byte[] mib = Encoding.ASCII.GetBytes(s);
                foreach (byte b in mib)
                    pdu[index++] = b;
            }
            pdu[index++] = 0x05; //Type: Null
            pdu[index++] = 0x00; 
        }

        

    }
}
