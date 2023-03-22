﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSBaseLib;
using MessagePack;

namespace ChatServer
{
    public class ServerPacketData
    {
        public Int16 packetSize;
        public string sessionID;
        public Int16 packetID;
        public SByte type;
        public byte[] bodyData;

        public void Assign(string _sessionID, Int16 _packetID, byte[] _packetBodyData)
        {
            sessionID = _sessionID;
            packetID = _packetID;

            if (_packetBodyData.Length > 0)
            {
                bodyData = _packetBodyData;
            }
        }

        public static ServerPacketData NotifyConnectOrDisConnectClientPacket(
            bool isConnect, string sessionID)
        {
            var packet = new ServerPacketData();

            if (isConnect)
            {
                packet.packetID = (Int32)PACKETID.NOTIFY_CONNECT;
            }
            else
            {
                packet.packetID = (Int32)PACKETID.NOTIFY_DISCONNECT;
            }

            packet.sessionID = sessionID;

            return packet;
        }
    }

    [MessagePackObject]
    public class PKTMake_NofityLeaveRoom
    {
        [Key(0)]
        public int RoomNumber;

        [Key(1)]
        public string UserID;
    }
}
