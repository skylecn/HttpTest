using Best.HTTP;
using System;
using ProtoMsg;
using Google.Protobuf;
using System.IO;
using UnityEngine;

public class NetManager
{
    // ��������ַ
    public string ServerAddress = "http://127.0.0.1/";

    // ��ʱ
    public TimeSpan TimeOut = TimeSpan.FromSeconds(20);

    #region Singleton
    private class SingletonNested
    {
        static SingletonNested()
        {
        }
        internal static readonly NetManager instance = new NetManager();
    }

    public static NetManager instance { get { return SingletonNested.instance; } }
    #endregion

    /// <summary>
    /// ��¼
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void Login(string username, string password)
    {
        var request = HTTPRequest.CreatePost(string.Format("{0}/Login", ServerAddress), LoginCallBack);

        // ���ó�ʱ
        request.TimeoutSettings.Timeout = TimeOut;
        request.TimeoutSettings.ConnectTimeout = TimeOut;

        // �����Ϣ����
        request.SetHeader("content-type", "application/octet-stream");
        var msgLogin = new CsLogin() { Id = username, Psw = password };
        request.UploadSettings.UploadStream = new MemoryStream(msgLogin.ToByteArray());

        request.Send();
    }

    /// <summary>
    /// ��¼�ص�
    /// </summary>
    /// <param name="req"></param>
    /// <param name="resp"></param>
    void LoginCallBack(HTTPRequest req, HTTPResponse resp)
    {
        switch(req.State)
        {
            case HTTPRequestStates.Finished:
                {
                    if (resp.IsSuccess)
                    {
                        var retLogin = ScLogin.Parser.ParseFrom(resp.Data);
                        if(retLogin.ErrCode == 0)
                        {
                            // �ɹ���¼
                        }
                        else
                        {
                            ShowErrorTip(retLogin.ErrCode);
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"Server sent an error: {resp.StatusCode}-{resp.Message}");
                    }
                }
                break;
            case HTTPRequestStates.Error:
                UnityEngine.Debug.LogError($"Request finished with error! Request state: {req.State}-{req.Exception}");
                // todo: ��ʾ�û��޷����ӵ�������
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                // �����־
                UnityEngine.Debug.LogError("ConnectionTimedOut");
                // todo: ��ʾ�û����ӳ�ʱ
                break;
            case HTTPRequestStates.TimedOut:
                // �����־
                UnityEngine.Debug.LogError("TimedOut");
                // todo: ��ʾ�û����ӳ�ʱ
                break;
            default:
                {
                    UnityEngine.Debug.LogError($"Request finished with error! Request state: {req.State}");
                }
                break;
        }
    }

    /// <summary>
    /// ��ʾ�߼�����Tip
    /// </summary>
    /// <param name="errorCode"></param>
    void ShowErrorTip(int errorCode)
    {

    }


}
