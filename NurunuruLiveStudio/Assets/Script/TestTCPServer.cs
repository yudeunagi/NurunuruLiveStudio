using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
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

    private TcpListener myListener;
    private TcpClient myClient;

    //棒読みちゃん転送用クライアント
    private TcpClient sendClient;

    //マネージャクラス包括オブジェクト
    [SerializeField]
    private GameObject _Managers;

    //スパチャマネージャ
    private SuperChatManager supacha;

    //トリガーマネージャ
    private TriggerManager trigger;

    void Start()
    {
        supacha = _Managers.GetComponent<SuperChatManager>();
        trigger = _Managers.GetComponent<TriggerManager>();
        StartServer();
    }

    // ソケット接続準備、待機
    public void StartServer()
    {
        // TCPリスナー設定
        var ip = IPAddress.Parse(MY_IP_ADDRESS);
        myListener = new TcpListener(ip, myPortNumber);
        myListener.Start();

        //コールバック設定　第二引数はコールバック関数に渡される。
        myListener.BeginAcceptSocket(DoAcceptTcpClientCallback, myListener);
    }


    /// <summary>
    /// クライアントからの接続処理
    /// </summary>
    /// <param name="ar"></param>
    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {

        //渡されたものを取り出す
        TcpListener listener = (TcpListener)ar.AsyncState; //TODO:何やってんのかわかんね、しらべる
        myClient = listener.EndAcceptTcpClient(ar);
        Debug.Log("connect: " + myClient.Client.RemoteEndPoint);

        //接続した人とのネットワークストリームを取得
        NetworkStream stream = myClient.GetStream();
//        StreamReader reader = new StreamReader(stream);

        var br = new BinaryReader(stream, System.Text.Encoding.UTF8);

        byte[] header = new byte[15];


        // 接続が切れるまで送受信を繰り返す
        while (myClient.Connected)
        {
            //ヘッダ情報取得
            br.Read(header, 0, 15);

            //本文の長さ
            int len = BitConverter.ToInt32(header, 11);

            //本文のバイトストリーム
            byte[] bs = new byte[len];

            br.Read(bs, 0, len);
            Debug.Log(System.Text.Encoding.UTF8.GetString(bs));


            //受信したバイト配列を棒読みちゃんへ送信する
            //ヘッダー＆本文の結合
            byte[] sendByte = new byte[header.Length + bs.Length];
            header.CopyTo(sendByte, 0);
            bs.CopyTo(sendByte, header.Length);
            //棒読みちゃんへ送信
            sendMessage(sendByte);

            //本文取得
            string message = System.Text.Encoding.UTF8.GetString(bs);
            //その他いろいろな処理

            // 金額からイベント起動
            PayedMoney money = supacha.getAmount(message);
            // 単語からイベント起動
            trigger.Trigger(message);


            // クライアントの接続が切れたら
            if (myClient.Client.Poll(1000, SelectMode.SelectRead) && (myClient.Client.Available == 0))
            {
                Debug.Log("disconnect: " + myClient.Client.RemoteEndPoint);
                myClient.Close();
                break;
                
            }
        }

        // 受付再開？
        myListener.BeginAcceptSocket(DoAcceptTcpClientCallback, myListener);

    }

    /// <summary>
    ///  終了処理
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        if (myListener != null) myListener.Stop();
        if (myClient != null) myClient.Close();
    }


    public void sendMessage(byte[] sendByte)
    {

        var sw = new System.Diagnostics.Stopwatch();
        sw.Start(); //計測開始
        try
        {
            //棒読みちゃんへ接続
            //memo:TcpClientの引数に"localhost"を渡したら2秒ぐらいかかったけど"127.0.0.1"を渡したら0.005秒とかになった。
            sendClient = new TcpClient(MY_IP_ADDRESS, sendPortNumber);
            NetworkStream stream = sendClient.GetStream();

            Debug.Log(sw.Elapsed); //経過時間
            sw.Stop(); //計測終了

            stream.Write(sendByte, 0, sendByte.Length);

            sendClient.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

}
