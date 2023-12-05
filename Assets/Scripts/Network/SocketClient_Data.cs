using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

public partial class SocketClient
{
    public static int seq = 0;

    public enum IOCommand : ushort
    {
        Syn = 0x0001,
        Fin = 0x0002,
        UniCast = 0x0003,
        Ack = 0x0100,
        Err = 0x0200,
        Redundant = 0x0201,
        
        SynAck = Ack | Syn,
        FinAck = Ack | Fin,
    }
    
    
    public enum ErrorCode: int
    {
        Success = 0,
        Fail = 1,

        /// <summary>
        /// At Codec with streamBuffer
        /// </summary>
        NotEnoughStream = 101,
        MalformedStream = 102,
        
        /// <summary>
        /// At GatewayServer with Header Parse
        /// </summary>
        HeaderVersionIsNotSupported = 201,
        FlagIsNotSupported = 202,
        PayloadLengthNotSufficient = 203,

        AlreadyExistsSession = 204,
        NotAvailable = 205,
        NotFoundGameServer = 206,
        DuplicateConnection = 207,
        /// <summary>
        /// At Game Server 
        /// </summary>
    }
    
    

    public enum HeaderSize : int
    {
        None = 0,
        IOHeader = 19,
        GameHeader = 8
    }

    // 서버로 데이터 전송시 사용할 구조체
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WebSocketHeader
    {
        public static readonly int Size = Marshal.SizeOf<WebSocketHeader>();
        //public static readonly int Size = 27;

        #region IO Header
        public byte headerVersion;
        public short flag;

        public int dataLengh; //Game Header + Game Payload Size
        public int sequenceNumber;
        public long uid;
        #endregion


        #region Game Header
        public int messageId;

        public int payLoadLengh;
        #endregion

        public WebSocketHeader(short commandId, long uid, int messageId, int payLoadLengh, HeaderSize gameHeaderSize = HeaderSize.GameHeader)
        {
            this.headerVersion = 0x1;

            this.flag = (short)IPAddress.HostToNetworkOrder((short)commandId);

            this.dataLengh = IPAddress.HostToNetworkOrder((int)gameHeaderSize + payLoadLengh);

            this.sequenceNumber = IPAddress.HostToNetworkOrder(seq++);

            this.uid = IPAddress.HostToNetworkOrder(uid);

            this.messageId = IPAddress.HostToNetworkOrder(messageId);

            this.payLoadLengh = IPAddress.HostToNetworkOrder(payLoadLengh);
        }

        public void WriteTo(MemoryStream stream)
        {
            byte[] flagBytes = BitConverter.GetBytes(this.flag);
            byte[] dataBytes = BitConverter.GetBytes(this.dataLengh);
            byte[] sequenceBytes = BitConverter.GetBytes(this.sequenceNumber);
            byte[] uidBytes = BitConverter.GetBytes(this.uid);
            byte[] messageBytes = BitConverter.GetBytes(this.messageId);
            byte[] payloadBytes = BitConverter.GetBytes(this.payLoadLengh);

            stream.WriteByte(this.headerVersion);
            stream.Write(flagBytes);
            stream.Write(dataBytes);
            stream.Write(sequenceBytes);
            stream.Write(uidBytes);
            stream.Write(messageBytes);
            stream.Write(payloadBytes);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("flag : {0}", flag).AppendLine();

            builder.Append("data : ");

            return builder.ToString();
        }
    }
}