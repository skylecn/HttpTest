using Best.HTTP;
using System;
using ProtoMsg;
using Google.Protobuf;
using System.IO;
using UnityEngine;

public class NetManager
{
    // 服务器地址
    public string ServerAddress = "http://127.0.0.1/";

    // 超时
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
    /// 登录
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void Login(string username, string password)
    {
        var request = HTTPRequest.CreatePost(string.Format("{0}/Login", ServerAddress), LoginCallBack);

        // 设置超时
        request.TimeoutSettings.Timeout = TimeOut;
        request.TimeoutSettings.ConnectTimeout = TimeOut;

        // 填充消息内容
        request.SetHeader("content-type", "application/octet-stream");
        var msgLogin = new CsLogin() { Id = username, Psw = password };
        request.UploadSettings.UploadStream = new MemoryStream(msgLogin.ToByteArray());

        request.Send();
    }

    /// <summary>
    /// 登录回调
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
                            // 成功登录
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
                // todo: 提示用户无法连接到服务器
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                // 输出日志
                UnityEngine.Debug.LogError("ConnectionTimedOut");
                // todo: 提示用户连接超时
                break;
            case HTTPRequestStates.TimedOut:
                // 输出日志
                UnityEngine.Debug.LogError("TimedOut");
                // todo: 提示用户连接超时
                break;
            default:
                {
                    UnityEngine.Debug.LogError($"Request finished with error! Request state: {req.State}");
                }
                break;
        }
    }

    /// <summary>
    /// 显示逻辑错误Tip
    /// </summary>
    /// <param name="errorCode"></param>
    void ShowErrorTip(int errorCode)
    {

    }


}
