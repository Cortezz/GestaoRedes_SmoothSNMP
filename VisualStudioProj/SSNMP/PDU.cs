﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothSNMP
{
    /// <summary>
    /// Class used to build and store an SNMP PDU.
    /// </summary>
    internal class PDU
    {
        private byte[] pdu;
        private int index;

        public PDU(int size)
        {
            pdu = new byte[size];
            index = 0;
        }

        /// <summary>
        /// Returns the current index of the PDU.
        /// </summary>
        /// <returns></returns>
        public int getIndex()  { return this.index;}

        /// <summary>
        /// Builds an SNMP PDU. 
        /// </summary>
        /// <param name="requestID">ID associated with this SNMP request.</param>
        /// <param name="type">Type of the SNMP PDU (e.g. Get, GetNext).</param>
        /// <param name="community">Name of the community.</param>
        /// <param name="mibs">OIDs used in the PDU.</param>
        /// <returns>A byte array which represents the built PDU.</returns>
        public byte[] buildPDU(int requestID, int type, string community, string[] mibs)
        {
            int MIBlength = 0;
            List<byte[]> oids = ConvertOIDsToBytes(mibs);
            foreach (byte[] b in oids)
                MIBlength += b.Length;
            

            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(25 + community.Length + MIBlength + (2 * mibs.Length)); //Length of the PDU
            //Version 2C
            InsertVersion2C();
            //Community
            InsertCommunity(community);
            //PDU Type
            pdu[index++] = Convert.ToByte(160 + type); //Type of the SNMP PDU
            pdu[index++] = Convert.ToByte(18 + MIBlength + 2 * mibs.Length); //Length
            //Request ID
            InsertRequestID(requestID);
            //Error Code and Status
            InsertErrorCodeAndStatus();
            //Varbinds
            InsertVarbindList(MIBlength, oids);

            return this.pdu;
        }

        /// <summary>
        /// Inserts the necessary bytes for the Version 2C into the PDU.
        /// </summary>
        private void InsertVersion2C()
        {
            pdu[index++] = 0x02; //Type: Integer
            pdu[index++] = 0x01; //Length: 1
            pdu[index++] = 0x01; // Version: 2C
        }

        /// <summary>
        /// Inserts the community name into the PDU.
        /// </summary>
        /// <param name="community">Name of the community.</param>
        private void InsertCommunity(string community)
        {
            byte[] comBytes = Encoding.ASCII.GetBytes(community);

            pdu[index++] = 0x04; //Type: Octet String
            pdu[index++] = Convert.ToByte(community.Length); //Length
            foreach (Byte b in comBytes)
                pdu[index++] = b;  
        }

        /// <summary>
        /// Inserts the request ID into the PDU.
        /// </summary>
        /// <param name="request">Request ID.</param>
        private void InsertRequestID (int request)
        {
            this.pdu[index++] = 0x02; //Type: Integer
            this.pdu[index++] = 0x04; //Length: 4
            this.pdu[index++] = Convert.ToByte(request); //Request ID
            this.pdu[index++] = Convert.ToByte(request+1);
            this.pdu[index++] = Convert.ToByte(request+2);
            this.pdu[index++] = Convert.ToByte(request+3);
        }


        /// <summary>
        /// Inserts the bytes associated with the Error Code and Error Status into the PDU.
        /// </summary>
        private void InsertErrorCodeAndStatus()
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

        /// <summary>
        /// Inserts the Varbinds into the PDU.
        /// </summary>
        /// <param name="MIBlength">Length of all of the OIDs combined.</param>
        /// <param name="oids">List with the OIDs.</param>
        private void InsertVarbindList(int MIBlength, List<byte[]> oids)
        {
            int i;
            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(4 + MIBlength + 2 * oids.Count); // Length
            pdu[index++] = 0x30; //Type: List
            pdu[index++] = Convert.ToByte(2 + MIBlength + 2 * oids.Count); // Length
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

        /// <summary>
        /// Converts a string array containing the OIDs into a list of byte arrays, each one representing one OID.
        /// </summary>
        /// <param name="mibs">String representation of OIDs to be converted.</param>
        /// <returns>List with all the oids written in their byte form.</returns>
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
