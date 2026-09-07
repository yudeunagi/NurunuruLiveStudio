using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using Unage;

/**
 * 参考：
 * 
 * https://blog.sky-net.pw/article/36
 * https://hakase0274.hatenablog.com/entry/2019/08/11/235621
 * https://blog.applibot.co.jp/2018/08/13/socket-communication-with-unity/
 * 
 */



public class TestTCPServer : MonoBehaviour
{

    //自分自身を指すIPアドレス
    public string MY_IP_ADDRESS = "127.0.0.1";
    //受信するポート番号
    [SerializeField]
    private int myPortNumber = 50002;
    //転送するポート番号
    [SerializeField]
    private int sendPortNumber = 50001;
    //棒読みちゃんへ転送するか
    [SerializeField]
    private bool isTransport = true;
    // 詳細なTCPログを出力するか
    [SerializeField]
    private bool enableVerboseTcpLog = false;

    private TcpListener myListener;
    private TcpClient myClient;

    //マネージャクラス包括オブジェクト
    [SerializeField]
    private GameObject _Managers;

    //トリガーマネージャ
    private TriggerManager trigger;

    //イベントマネージャ
    private EventManager eventManager;
    //環境設定データ
    private EnvironmentConfigData environmentConfigData;

    private const int BOUYOMI_HEADER_LENGTH = 15;
    private const int BOUYOMI_ENCODING_OFFSET = 10;
    private const int BOUYOMI_BODY_LENGTH_OFFSET = 11;
    private const int CLIENT_RECEIVE_TIMEOUT_MS = 10000;
    private const int MAX_BODY_LENGTH_BYTES = 1024 * 1024;
    private const int RELAY_CONNECT_TIMEOUT_MS = 3000;

    private int connectionSequence;
    private bool isShuttingDown;
    private readonly Queue<ReceivedMessageContext> receivedMessageQueue = new Queue<ReceivedMessageContext>();
    private readonly object receivedMessageQueueLock = new object();

    private class ReceivedMessageContext
    {
        public int ConnectionId;
        public int MessageId;
        public string Message;
    }

    void Start()
    {
        trigger = _Managers.GetComponent<TriggerManager>();
        eventManager = _Managers.GetComponent<EventManager>();
        ResolveEnvironmentConfigData();
        ApplyEnvironmentConfig(environmentConfigData);
        StartServer();
    }

    private void Update()
    {
        while (true)
        {
            ReceivedMessageContext context = null;
            lock (receivedMessageQueueLock)
            {
                if (receivedMessageQueue.Count == 0)
                {
                    break;
                }

                context = receivedMessageQueue.Dequeue();
            }

            ProcessReceivedMessageOnMainThread(context);
        }
    }

    /// <summary>
    /// 環境設定データ参照を解決する。
    /// </summary>
    private void ResolveEnvironmentConfigData()
    {
        if (environmentConfigData != null)
        {
            return;
        }

        environmentConfigData = UnityEngine.Object.FindFirstObjectByType<EnvironmentConfigData>();
        if (environmentConfigData == null)
        {
            Debug.LogWarning("EnvironmentConfigData が見つからないため、既定のポート設定を使用します。");
        }
    }

    /// <summary>
    /// 環境設定を TCP サーバー設定へ反映する。
    /// </summary>
    public void ApplyEnvironmentConfig(EnvironmentConfigData configData)
    {
        if (configData == null)
        {
            return;
        }

        environmentConfigData = configData;
        myPortNumber = configData.MyPortNumber;
        sendPortNumber = configData.SendPortNumber;
        isTransport = configData.IsTransport;
    }

    // ソケット接続準備、待機
    public void StartServer()
    {
        isShuttingDown = false;

        if (myListener != null)
        {
            myListener.Stop();
        }

        // TCPリスナー設定
        var ip = IPAddress.Parse(MY_IP_ADDRESS);
        myListener = new TcpListener(ip, myPortNumber);
        myListener.Start();
        Debug.Log($"[TCP] Listening on {MY_IP_ADDRESS}:{myPortNumber}");

        BeginAccept();
    }

    private void BeginAccept()
    {
        if (isShuttingDown || myListener == null)
        {
            return;
        }

        try
        {
            //コールバック設定　第二引数はコールバック関数に渡される。
            myListener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, myListener);
            LogVerbose("[TCP] Waiting for next connection...");
        }
        catch (ObjectDisposedException)
        {
            if (!isShuttingDown)
            {
                Debug.LogWarning("[TCP] Listener is already disposed while waiting for connection.");
            }
        }
        catch (InvalidOperationException e)
        {
            if (!isShuttingDown)
            {
                Debug.LogError($"[TCP] Failed to start accepting connection: {e}");
            }
        }
    }

    /// <summary>
    /// クライアントからの接続処理
    /// </summary>
    /// <param name="ar"></param>
    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        //渡されたものを取り出す
        TcpListener listener = (TcpListener)ar.AsyncState; //TODO:何やってんのかわかんね、しらべる
        TcpClient acceptedClient = null;
        int connectionId = 0;
        bool acceptRearmed = false;

        try
        {
            acceptedClient = listener.EndAcceptTcpClient(ar);
            myClient = acceptedClient;
            connectionId = Interlocked.Increment(ref connectionSequence);

            // 接続処理中でも次の接続受付を止めない
            BeginAccept();
            acceptRearmed = true;

            string remoteEndPoint = acceptedClient.Client.RemoteEndPoint != null
                ? acceptedClient.Client.RemoteEndPoint.ToString()
                : "(unknown)";
            LogVerbose($"[TCP:{connectionId}] connect: {remoteEndPoint}");

            using (acceptedClient)
            using (NetworkStream stream = acceptedClient.GetStream())
            {
                acceptedClient.ReceiveTimeout = CLIENT_RECEIVE_TIMEOUT_MS;

                byte[] header = new byte[BOUYOMI_HEADER_LENGTH];
                int messageId = 1;
                int headerBytesRead = ReadExact(stream, header, 0, BOUYOMI_HEADER_LENGTH);
                if (headerBytesRead == 0)
                {
                    LogVerbose($"[TCP:{connectionId}] disconnect by remote before header.");
                    return;
                }

                if (headerBytesRead < BOUYOMI_HEADER_LENGTH)
                {
                    Debug.LogWarning($"[TCP:{connectionId}] Incomplete header. expected={BOUYOMI_HEADER_LENGTH}, actual={headerBytesRead}");
                    return;
                }

                byte encodingType = header[BOUYOMI_ENCODING_OFFSET];

                //本文の長さ
                int len = BitConverter.ToInt32(header, BOUYOMI_BODY_LENGTH_OFFSET);

                if (len < 0 || len > MAX_BODY_LENGTH_BYTES)
                {
                    Debug.LogError($"[TCP:{connectionId}|msg:{messageId}] Invalid body length: {len}");
                    return;
                }

                //本文のバイトストリーム
                byte[] bs = new byte[len];
                int bodyBytesRead = ReadExact(stream, bs, 0, len);
                if (bodyBytesRead < len)
                {
                    Debug.LogWarning($"[TCP:{connectionId}|msg:{messageId}] Incomplete body. expected={len}, actual={bodyBytesRead}");
                    return;
                }

                string message = DecodeMessage(bs, encodingType);

                LogVerbose($"[TCP:{connectionId}|msg:{messageId}] received header={BOUYOMI_HEADER_LENGTH} body={len} encoding={encodingType}");
                LogVerbose($"[TCP:{connectionId}|msg:{messageId}] message={message}");

                //受信したバイト配列を棒読みちゃんへ送信する
                //ヘッダー＆本文の結合
                byte[] sendByte = new byte[header.Length + bs.Length];
                header.CopyTo(sendByte, 0);
                bs.CopyTo(sendByte, header.Length);

                EnqueueReceivedMessage(connectionId, messageId, message);
                LogVerbose($"[TCP:{connectionId}|msg:{messageId}] queued for main-thread processing.");

                //棒読みちゃんへ送信
                if (isTransport)
                {
                    LogVerbose($"[TCP:{connectionId}|msg:{messageId}] start relay to bouyomichan.");
                    sendMessage(sendByte, connectionId, messageId);
                    LogVerbose($"[TCP:{connectionId}|msg:{messageId}] relay call done.");
                }
                else
                {
                    LogVerbose($"[TCP:{connectionId}|msg:{messageId}] relay skipped by configuration.");
                }
            }
        }
        catch (IOException e)
        {
            if (isShuttingDown || IsExpectedShutdownIo(e))
            {
                LogVerbose($"[TCP:{connectionId}] IO interrupted by shutdown: {e.Message}");
            }
            else if (IsTimeoutIo(e))
            {
                Debug.LogWarning($"[TCP:{connectionId}] IO timeout while waiting data: {e.Message}");
            }
            else
            {
                Debug.LogError($"[TCP:{connectionId}] IO error: {e}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[TCP:{connectionId}] Unexpected error: {e}");
        }
        finally
        {
            if (!acceptRearmed)
            {
                BeginAccept();
            }
        }
    }

    private void EnqueueReceivedMessage(int connectionId, int messageId, string message)
    {
        ReceivedMessageContext context = new ReceivedMessageContext();
        context.ConnectionId = connectionId;
        context.MessageId = messageId;
        context.Message = message;

        lock (receivedMessageQueueLock)
        {
            receivedMessageQueue.Enqueue(context);
        }
    }

    private void ProcessReceivedMessageOnMainThread(ReceivedMessageContext context)
    {
        if (context == null)
        {
            return;
        }

        if (trigger == null || eventManager == null)
        {
            Debug.LogError($"[TCP:{context.ConnectionId}|msg:{context.MessageId}] TriggerManager/EventManager is not assigned.");
            return;
        }

        LogVerbose($"[TCP:{context.ConnectionId}|msg:{context.MessageId}] start trigger/equeue processing (main thread).");

        // その他いろいろな処理
        // 金額からイベント名取得
        List<EventData> moneyEventList = trigger.getAmount(context.Message);
        // 単語からイベント名取得
        List<EventData> triggerEventList = trigger.Trigger(context.Message);

        int moneyEventCount = moneyEventList != null ? moneyEventList.Count : -1;
        int triggerEventCount = triggerEventList != null ? triggerEventList.Count : -1;
        LogVerbose($"[TCP:{context.ConnectionId}|msg:{context.MessageId}] enqueue events money={moneyEventCount}, word={triggerEventCount}");

        if (moneyEventList != null)
        {
            eventManager.enqueEvent(moneyEventList);
        }
        else
        {
            Debug.LogWarning($"[TCP:{context.ConnectionId}|msg:{context.MessageId}] moneyEventList is null.");
        }

        if (triggerEventList != null)
        {
            eventManager.enqueEvent(triggerEventList);
        }
        else
        {
            Debug.LogWarning($"[TCP:{context.ConnectionId}|msg:{context.MessageId}] triggerEventList is null.");
        }

        LogVerbose($"[TCP:{context.ConnectionId}|msg:{context.MessageId}] event enqueue done (main thread).");
    }

    /// <summary>
    ///  終了処理
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        isShuttingDown = true;
        if (myListener != null) myListener.Stop();
        if (myClient != null) myClient.Close();
    }


    private int ReadExact(Stream stream, byte[] buffer, int offset, int count)
    {
        int totalRead = 0;

        while (totalRead < count)
        {
            int read = stream.Read(buffer, offset + totalRead, count - totalRead);
            if (read == 0)
            {
                break;
            }

            totalRead += read;
        }

        return totalRead;
    }

    private bool IsTimeoutIo(IOException e)
    {
        SocketException socketException = e.InnerException as SocketException;
        if (socketException == null)
        {
            return false;
        }

        return socketException.SocketErrorCode == SocketError.TimedOut;
    }

    private bool IsExpectedShutdownIo(IOException e)
    {
        SocketException socketException = e.InnerException as SocketException;
        if (socketException == null)
        {
            return false;
        }

        return socketException.SocketErrorCode == SocketError.Interrupted
            || socketException.SocketErrorCode == SocketError.OperationAborted;
    }

    private void LogVerbose(string message)
    {
        if (enableVerboseTcpLog)
        {
            Debug.Log(message);
        }
    }

    /// <summary>
    /// デコードされたメッセージを取得します。
    /// </summary>
    /// <param name="body">メッセージのバイト配列</param>
    /// <param name="encodingType">エンコーディングタイプ (0=UTF-8, 1=Unicode, 2=Shift-JIS)</param>
    /// <returns>デコードされた文字列</returns>
    private string DecodeMessage(byte[] body, byte encodingType)
    {
        Encoding encoding;
        switch (encodingType)
        {
            case 0:
                encoding = Encoding.UTF8;
                break;
            case 1:
                encoding = Encoding.Unicode;
                break;
            case 2:
                try
                {
                    encoding = Encoding.GetEncoding(932);
                }
                catch (ArgumentException)
                {
                    Debug.LogWarning("[TCP] Shift-JIS encoding is not available. fallback=UTF-8");
                    encoding = Encoding.UTF8;
                }
                break;
            default:
                Debug.LogWarning($"[TCP] Unknown encoding type={encodingType}. fallback=UTF-8");
                encoding = Encoding.UTF8;
                break;
        }

        return encoding.GetString(body);
    }

    /// <summary>
    /// メッセージを送信します。
    /// </summary>
    /// <param name="sendByte">送信するバイト配列</param>
    /// <param name="connectionId">接続ID</param>
    /// <param name="messageId">メッセージID</param>
    /// <returns>送信の成否</returns>
    private void sendMessage(byte[] sendByte, int connectionId, int messageId)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start(); //計測開始
        try
        {
            //棒読みちゃんへ接続
            //memo:TcpClientの引数に"localhost"を渡したら2秒ぐらいかかったけど"127.0.0.1"を渡したら0.005秒とかになった。
            using (var client = new TcpClient())
            {
                LogVerbose($"[TCP:{connectionId}|msg:{messageId}] relay connecting {MY_IP_ADDRESS}:{sendPortNumber}");
                IAsyncResult connectAsyncResult = client.BeginConnect(MY_IP_ADDRESS, sendPortNumber, null, null);
                bool isConnected = false;
                try
                {
                    isConnected = connectAsyncResult.AsyncWaitHandle.WaitOne(RELAY_CONNECT_TIMEOUT_MS);
                }
                finally
                {
                    connectAsyncResult.AsyncWaitHandle.Close();
                }

                if (!isConnected)
                {
                    Debug.LogWarning($"[TCP:{connectionId}|msg:{messageId}] relay connect timeout {RELAY_CONNECT_TIMEOUT_MS}ms");
                    return;
                }

                client.EndConnect(connectAsyncResult);
                LogVerbose($"[TCP:{connectionId}|msg:{messageId}] relay connected elapsed={sw.ElapsedMilliseconds}ms");

                sw.Stop(); //計測終了

                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(sendByte, 0, sendByte.Length);
                    LogVerbose($"[TCP:{connectionId}|msg:{messageId}] relayed bytes={sendByte.Length}");
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"[TCP:{connectionId}|msg:{messageId}] relay socket error: {e}");
        }
    }

}
