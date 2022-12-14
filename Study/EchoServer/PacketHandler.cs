﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public class PacketData
    {
        public NetworkSession session;
        public EFBinaryRequestInfo reqInfo;
    }

    public enum PACKETID : int
    {
        REQ_ECHO = 101,
    }

    public class PacketHandler
    {
        // 단순한 Echo 기능
        public void RequestEcho(NetworkSession session, EFBinaryRequestInfo requestInfo)
        {
            var totalSize = (Int16)(requestInfo.Body.Length + EFBinaryRequestInfo.HEADERE_SIZE);

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes(totalSize));
            dataSource.AddRange(BitConverter.GetBytes((Int16)PACKETID.REQ_ECHO));
            dataSource.AddRange(new byte[1]);
            dataSource.AddRange(requestInfo.Body);

            session.Send(dataSource.ToArray(), 0, dataSource.Count);
        }
    }

    public class PK_ECHO
    {
        public string msg;
    }
}
