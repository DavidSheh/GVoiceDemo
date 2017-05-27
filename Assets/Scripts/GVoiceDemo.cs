using UnityEngine;
using gcloud_voice;
using UnityEngine.UI;
using System;

public class GVoiceDemo : MonoBehaviour
{
    // 用来显示调用API返回的结果
    public Text result;

    private IGCloudVoice m_voiceengine = null;

    // TODO: 这里的appId和appKey使用的是官方提供的测试值，正式项目中可使用申请的值
    private const string appId = "932849489";
    private const string appKey = "d94749efe9fce61333121de84123ef9b";

    // TODO: 这里使用的是测试账号，所以房间名使用默认的100，正式项目中可根据实际情况赋值
    private string roomName = "100";

    void Start()
    {
        if (m_voiceengine == null)
        {
            m_voiceengine = GCloudVoice.GetEngine();
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string strTime = System.Convert.ToInt64(ts.TotalSeconds).ToString();
            // TODO: 这里用时间模拟了一个openId，在正式项目中应该把这里的strTime换成用户唯一ID
            m_voiceengine.SetAppInfo(appId, appKey, strTime);
            m_voiceengine.Init();

            // 注册SDK常用回调监听
            m_voiceengine.OnJoinRoomComplete += OnJoinRoom;
            m_voiceengine.OnQuitRoomComplete += OnExitRoom;
            m_voiceengine.OnMemberVoice += OnMemberVoice;
        }
    }

    void Update()
    {
        if (m_voiceengine != null)
        {
            // 不断检测GVoice引擎回调
            m_voiceengine.Poll();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (m_voiceengine == null)
        {
            return;
        }

        // 应用暂停时GVoice引擎也暂停，应用重新开始时引擎继续
        if (pauseStatus)
        {
            m_voiceengine.Pause();
        }
        else
        {
            m_voiceengine.Resume();
        }
    }

    /// <summary>
    /// 加入房间，BtnJoin按钮点击调用
    /// </summary>
    public void JoinRoom()
    {
        m_voiceengine.SetMode(GCloudVoiceMode.RealTime);
        int ret = m_voiceengine.JoinTeamRoom(roomName, 15000);

        result.text += "\nJoinRoom:" + ret;
    }

    /// <summary>
    /// 退出房间，BtnExit按钮点击调用
    /// </summary>
    public void ExitRoom()
    {
        int ret = m_voiceengine.QuitRoom(roomName, 6000);
        result.text += "\nExitRoom:" + ret;
    }

    /// <summary>
    /// 打开麦克风，BtnOpenMic按钮点击调用
    /// </summary>
    public void OpenMic()
    {
        int ret = m_voiceengine.OpenMic();
        result.text += "\nOpenMic:" + ret;
    }

    /// <summary>
    /// 关闭麦克风，BtnCloseMic按钮点击调用
    /// </summary>
    public void CloseMic()
    {
        int ret = m_voiceengine.CloseMic();
        result.text += "\nCloseMic:" + ret;
    }

    /// <summary>
    /// 打开扬声器，BtnOpenSpeaker按钮点击调用
    /// </summary>
    public void OpenSpeaker()
    {
        int ret = m_voiceengine.OpenSpeaker();
        result.text += "\nOpenSpeaker:" + ret;
    }

    /// <summary>
    /// 关闭扬声器，BtnCloseSpeaker按钮点击调用
    /// </summary>
    public void CloseSpeaker()
    {
        int ret = m_voiceengine.CloseSpeaker();
        result.text += "\nCloseSpeaker:" + ret;
    }

    /// <summary>
    /// 加入房间回调
    /// </summary>
    /// <param name="code"></param>
    /// <param name="roomName"></param>
    /// <param name="memberID"></param>
    private void OnJoinRoom(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID)
    {
        result.text += string.Format("\nOnJoinRoom ---> code: {0}, roomName: {1}, memberID: {2}", code, roomName, memberID);
    }

    /// <summary>
    /// 退出房间回调
    /// </summary>
    /// <param name="code"></param>
    /// <param name="roomName"></param>
    /// <param name="memberID"></param>
    private void OnExitRoom(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID)
    {
        result.text += string.Format("\nOnExitRoom ---> code: {0}, roomName: {1}, memberID: {2}", code, roomName, memberID);

        m_voiceengine.OnJoinRoomComplete -= OnJoinRoom;
        m_voiceengine.OnQuitRoomComplete -= OnExitRoom;
        m_voiceengine.OnMemberVoice -= OnMemberVoice;
    }

    /// <summary>
    /// 有成员说话时回调
    /// </summary>
    /// <param name="members"></param>
    /// <param name="count"></param>
    private void OnMemberVoice(int[] members, int count)
    {
        result.text += string.Format("\nOnMemberVoice ---> count: {0}, roomName: {1}, memberID: {2}", count);
    }
}
