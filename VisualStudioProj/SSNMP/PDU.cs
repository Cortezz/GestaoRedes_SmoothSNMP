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

        public int getIndex()  { return this.index;}

        public byte[] buildPDU(int requestID, int type, string community, string[] mibs)
        {
            int MIBlength = 0;
            List<byte[]> oids = ConvertOIDsToBytes(mibs);
            foreach (byte[] b in oids)
                MIBlength += b.Length;
            

            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(25 + community.Length + MIBlength + (2 * mibs.Length)); //Length of the PDU
            //Version 2C
            FillInVersion2C();
            //Community
            FillInCommunity(community);
            //PDU Type
            pdu[index++] = Convert.ToByte(160 + type); //Type of the SNMP PDU
            pdu[index++] = Convert.ToByte(18 + MIBlength + 2 * mibs.Length); //Length
            //Request ID
            FillInRequestID(requestID);
            //Error Code and Status
            FillInErrorCodeAndStatus();
            //Varbinds
            FillInVarbindList(mibs, MIBlength, oids);

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
            this.pdu[index++] = 0x04; //Length: 4
            this.pdu[index++] = Convert.ToByte(request); //Request ID
            this.pdu[index++] = Convert.ToByte(request+1);
            this.pdu[index++] = Convert.ToByte(request+2);
            this.pdu[index++] = Convert.ToByte(request+3);
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

        private void FillInVarbindList(string[] mibs, int MIBlength, List<byte[]> oids)
        {
            int i;

            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(4 + MIBlength + 2 * mibs.Length); // Length
            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(2 + MIBlength + 2 * mibs.Length); // Length
            for (i = 0; i<oids.Count;i++)
            {
                byte[] varbind = oids.ElementAt(i);
                pdu[index++] = 0x06;
                pdu[index++] = Convert.ToByte(varbind.Length); //MIB's length
                foreach (byte b in varbind)
                    pdu[index++] = b;
            }
            pdu[index++] = 0x05; //Type: Null
            pdu[index] = 0x00; 
        }

        public List<byte[]> ConvertOIDsToBytes(string[] mibs)
        {
            int i, j;
            List<byte[]> res = new List<byte[]>();
            foreach (string s in mibs)
            {
                i = 0;
                byte[] b = new byte[s.Length - (s.Length / 2)-1];
                b[i++] = 0x2b;
                string[] withoutDots = s.Split('.');
                for (j=2;j<withoutDots.Length;j++)
                {
                    if (withoutDots[j] != "")
                        b[i++] = Convert.ToByte(Convert.ToInt16(withoutDots[j]));
                }
                res.Add(b);
            }
            return res;
        }



    }
}
