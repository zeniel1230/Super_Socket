﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Config;

namespace EchoServer
{
    class MainServer : AppServer<NetworkSession, EFBinaryRequestInfo>
    {
        public static SuperSocket.SocketBase.Logging.ILog MainLogger;

        Dictionary<int, Action<NetworkSession, EFBinaryRequestInfo>> HandlerMap = new Dictionary<int, Action<NetworkSession, EFBinaryRequestInfo>>();
        PacketHandler packetHandler = new PacketHandler();

        IServerConfig m_Config;

        public MainServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFBinaryRequestInfo>())
        {
            // 3개 다 SuperSocket에서 만들어진 Deligate(C++에서 함수 포인터와 비슷한 개념이다.)
            NewSessionConnected += new SessionHandler<NetworkSession>(OnConnected);
            SessionClosed += new SessionHandler<NetworkSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<NetworkSession, EFBinaryRequestInfo>(RequestReceived);
        }

        // SuperSocket이 동작하기 위해 정보 전달
        public void InitConfig(ServerOption option)
        {
            //m_Config = new ServerConfig
            //{
            //    Port = option.Port,
            //    Ip = "Any",
            //    MaxConnectionNumber = option.MaxConnectionNumber,
            //    Mode = SocketMode.Tcp,
            //    Name = option.Name
            //};

            m_Config = new ServerConfig
            {
                Port = 12021,
                Ip = "Any",
                MaxConnectionNumber = 64,
                Mode = SocketMode.Tcp,
                Name = "EchoServer"
            };
        }

        // SUperSocket을 실행시키는 부분
        public void CreateServer()
        {
            try
            {
                // 먼저 Config로 Setup
                bool bResult = Setup(new RootConfig(), m_Config, logFactory: new NLogLogFactory());

                if (bResult == false)
                {
                    Console.WriteLine("[ERROR] 서버 네트워크 설정 실패");
                    return;
                }
                else
                {
                    MainLogger = base.Logger;
                }

                RegistHandler();

                MainLogger.Info("서버 생성 성공");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 서버 생성 실패: {ex.ToString()}");
            }
        }

        // SuperSocket 상태 확인용
        public bool IsRunning(ServerState eCurState)
        {
            if (eCurState == ServerState.Running)
            {
                return true;
            }

            return false;
        }

        void OnConnected(NetworkSession session)
        {
            MainLogger.Info($"세션 번호 {session.SessionID} 접속");
        }

        void OnClosed(NetworkSession session, CloseReason reason)
        {
            MainLogger.Info($"세션 번호 {session.SessionID} 접속해제: {reason.ToString()}");
        }

        void RequestReceived(NetworkSession session, EFBinaryRequestInfo reqInfo)
        {
            //MainLogger.Info($"세션 번호 {session.SessionID}, 받은 데이터 크기: {reqInfo.Body.Length}");

            var PacketID = reqInfo.PacketId;

            if (HandlerMap.ContainsKey(PacketID))
            {
                HandlerMap[PacketID](session, reqInfo);
            }
            else
            {
                MainLogger.Info($"세션 번호 {session.SessionID}, 받은 데이터 크기: {reqInfo.Body.Length}");
            }
        }

        void RegistHandler()
        {
            HandlerMap.Add((int)PACKETID.REQ_ECHO, packetHandler.RequestEcho);

            MainLogger.Info("핸들러 등록 완료");
        }
    }

    public class NetworkSession : AppSession<NetworkSession, EFBinaryRequestInfo>
    {
    }
}
