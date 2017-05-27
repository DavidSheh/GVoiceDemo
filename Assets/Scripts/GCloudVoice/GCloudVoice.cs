using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;



namespace gcloud_voice
{
public enum GCloudVoiceErr 
{    
    GCLOUD_VOICE_SUCC           = 0,
		
	//common base err
	GCLOUD_VOICE_PARAM_NULL = 0x1001,	//4097, some param is null
	GCLOUD_VOICE_NEED_SETAPPINFO = 0x1002,	//4098, you should call SetAppInfo first before call other api
	GCLOUD_VOICE_INIT_ERR = 0x1003,	//4099, Init Erro
	GCLOUD_VOICE_RECORDING_ERR = 0x1004,		//4100, now is recording, can't do other operator
	GCLOUD_VOICE_POLL_BUFF_ERR = 0x1005,	//4101, poll buffer is not enough or null 
	GCLOUD_VOICE_MODE_STATE_ERR = 0x1006,	//4102, call some api, but the mode is not correct, maybe you shoud call SetMode first and correct
	GCLOUD_VOICE_PARAM_INVALID = 0x1007,	//4103, some param is null or value is invalid for our request, used right param and make sure is value range is correct by our comment 
	GCLOUD_VOICE_OPENFILE_ERR = 0x1008, //4104, open a file err
	GCLOUD_VOICE_NEED_INIT = 0x1009, //4105, you should call Init before do this operator
	GCLOUD_VOICE_ENGINE_ERR = 0x100A, //4106, you have not get engine instance, this common in use c# api, but not get gcloudvoice instance first
	GCLOUD_VOICE_POLL_MSG_PARSE_ERR = 0x100B, //4107, this common in c# api, parse poll msg err
	GCLOUD_VOICE_POLL_MSG_NO = 0x100C, //4108, poll, no msg to update

	//realtime err
	GCLOUD_VOICE_REALTIME_STATE_ERR = 0x2001, //8193, call some realtime api, but state err, such as OpenMic but you have not Join Room first
	GCLOUD_VOICE_JOIN_ERR = 0x2002, //8194, join room failed
	GCLOUD_VOICE_QUIT_ROOMNAME_ERR = 0x2003,	//8195, quit room err, the quit roomname not equal join roomname
	GCLOUD_VOICE_OPENMIC_NOTANCHOR_ERR = 0x2004,//8196, open mic in bigroom,but not anchor role

	//message err
	GCLOUD_VOICE_AUTHKEY_ERR = 0x3001, //12289, apply authkey api error
	GCLOUD_VOICE_PATH_ACCESS_ERR = 0x3002, //12290, the path can not access ,may be path file not exists or deny to access
	GCLOUD_VOICE_PERMISSION_MIC_ERR = 0x3003,	//12291, you have not right to access micphone in android
	GCLOUD_VOICE_NEED_AUTHKEY = 0x3004,		//12292,you have not get authkey, call ApplyMessageKey first
	GCLOUD_VOICE_UPLOAD_ERR = 0x3005,   //12293, upload file err
    GCLOUD_VOICE_HTTP_BUSY = 0x3006,    //12294, http is busy,maybe the last upload/download not finish.
    GCLOUD_VOICE_DOWNLOAD_ERR = 0x3007, //12295, download file err
    GCLOUD_VOICE_SPEAKER_ERR = 0x3008, //12296, open or close speaker tve error
    GCLOUD_VOICE_TVE_PLAYSOUND_ERR = 0x3009, //12297, tve play file error
    GCLOUD_VOICE_AUTHING = 0x300a, // 12298, Already in applying auth key processing

    GCLOUD_VOICE_INTERNAL_TVE_ERR = 0x5001,		//20481, internal TVE err, our used
	GCLOUD_VOICE_INTERNAL_VISIT_ERR = 0x5002,	//20482, internal Not TVE err, out used
	GCLOUD_VOICE_INTERNAL_USED = 0x5003, //20483, internal used, you should not get this err num

            GCLOUD_VOICE_BADSERVER = 0x06001, // 24577, bad server address,should be "udp://capi.xxx.xxx.com"
        
        GCLOUD_VOICE_STTING =  0x07001, // 28673, Already in speach to text processing
}

public enum GCloudVoiceRole
{
    ANCHOR = 1, // member who can open microphone and say
    AUDIENCE,   // member who can only hear anchor's voice
}

public enum GCloudVoiceMode
{
	RealTime = 0, // realtime mode for TeamRoom or NationalRoom
	Messages,     // voice message mode
	Translation,  // speach to text mode
};

public abstract class IGCloudVoice
{
	public enum GCloudVoiceCompleteCode
	{
        GV_ON_JOINROOM_SUCC = 1,    //join room succ
        GV_ON_JOINROOM_TIMEOUT,  //join room timeout
        GV_ON_JOINROOM_SVR_ERR,  //communication with svr occur some err, such as err data recv from svr
        GV_ON_JOINROOM_UNKNOWN, //reserved, our internal unknow err

        GV_ON_NET_ERR,  //net err,may be can't connect to network

        GV_ON_QUITROOM_SUCC, //quitroom succ, if you have join room succ first, quit room will alway return succ

        GV_ON_MESSAGE_KEY_APPLIED_SUCC,  //apply message authkey succ
        GV_ON_MESSAGE_KEY_APPLIED_TIMEOUT,      //apply message authkey timeout
        GV_ON_MESSAGE_KEY_APPLIED_SVR_ERR,  //communication with svr occur some err, such as err data recv from svr
        GV_ON_MESSAGE_KEY_APPLIED_UNKNOWN,  //reserved,  our internal unknow err

        GV_ON_UPLOAD_RECORD_DONE,  //upload record file succ
        GV_ON_UPLOAD_RECORD_ERROR,  //upload record file occur error
        GV_ON_DOWNLOAD_RECORD_DONE, //download record file succ
        GV_ON_DOWNLOAD_RECORD_ERROR,    //download record file occur error

        GV_ON_STT_SUCC, // speech to text successful
        GV_ON_STT_TIMEOUT, // speech to text with timeout
        GV_ON_STT_APIERR, // server's error

        GV_ON_PLAYFILE_DONE,  //the record file played end

		GV_ON_ROOM_OFFLINE, // Dropped from the room
    };

    //realtime call back
    /// <summary>
    /// Callback when JoinXxxRoom successful or failed.
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="roomName">name of your joining</param>
    /// <param name="memberID"> if success, return the memberID</param>
    /// <returns></returns>
    public delegate void JoinRoomCompleteHandler(GCloudVoiceCompleteCode code, string roomName, int memberID) ;
    /// <summary>
    /// Callback when QuitRoom successful or failed.
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="roomName">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="memberID"> the quit room user's memberID</param>
    /// <returns></returns>
    public delegate void QuitRoomCompleteHandler(GCloudVoiceCompleteCode code, string roomName, int memberID) ;
    /// <summary>
    /// Callback when someone saied or silence in the same room.
    /// </summary>
    /// <param name="count">count of members who's status has changed.</param>
    /// <param name="members">a int array composed of [memberid_0, status,memberid_1, status ... memberid_2*count, status]
    /// here, status could be 0, 1, 2. 0 meets silence and 1/2 means saying</param>
    /// <returns></returns>
    public delegate void MemberVoiceHandler(int[] members, int count) ;

    //Voice Message Callback
    /// <summary>
    /// Callback when query message key successful or failed.
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <returns></returns>
    public delegate void ApplyMessageKeyCompleteHandler(GCloudVoiceCompleteCode code);
    /// <summary>
    /// Callback when upload voice file successful or failed.
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="filepath">file to upload</param>
    /// <param name="fileid"> if success ,get back the id for the file.</param>
    /// <returns></returns>
    public delegate void UploadReccordFileCompleteHandler(GCloudVoiceCompleteCode code, string filepath, string fileid) ;
    /// <summary>
    /// Callback when download voice file successful or failed.
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="filepath">file to download to .</param>
    /// <param name="fileid"> if success ,get back the id for the file.</param>
    /// <returns></returns>
    public delegate void DownloadRecordFileCompleteHandler(GCloudVoiceCompleteCode code, string filepath, string fileid) ;
    /// <summary>
    /// Callback when finish a voice file play end.
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="filepath">file had been plaied.</param>
    /// <returns></returns>
    public delegate void PlayRecordFilCompleteHandler(GCloudVoiceCompleteCode code, string filepath) ;

    /// <summary>
    /// Callback when translate voice file to text successful or failed.
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="fileID">fileID to translate.</param>
    /// <param name="result"> if success ,get back the text of voice file.</param>
    /// <returns></returns>
    public delegate void SpeechToTextHandler(GCloudVoiceCompleteCode code, string fileID, string result) ;

	/// <summary>
	/// Callback when dropped from the room.
	/// </summary>
	/// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
	/// <param name="roomName">name of your joining, should be 0-9A-Za-Z._- and less than 127 bytes.</param>
	/// <param name="memberID"> if success, return the memberID.</param>
	/// <returns></returns>
	public delegate void StatusUpdateHandler(GCloudVoiceCompleteCode code, string roomName, int memberID) ;
	
	public abstract event JoinRoomCompleteHandler OnJoinRoomComplete;
	public abstract event QuitRoomCompleteHandler OnQuitRoomComplete;
	public abstract event MemberVoiceHandler      OnMemberVoice;
	public abstract event ApplyMessageKeyCompleteHandler OnApplyMessageKeyComplete;
	public abstract event UploadReccordFileCompleteHandler OnUploadReccordFileComplete;
	public abstract event DownloadRecordFileCompleteHandler OnDownloadRecordFileComplete;
	public abstract event PlayRecordFilCompleteHandler OnPlayRecordFilComplete;
    public abstract event SpeechToTextHandler OnSpeechToText;
	public abstract event StatusUpdateHandler OnStatusUpdate;


    /// <summary>
    /// Set your app's info such as appID/appKey.
    /// </summary>
    /// <param name="appID">your game ID from gcloud.qq.com</param>
    /// <param name="appKey">your game key from gcloud.qq.com</param>
    /// <param name="openID">player's openID from QQ or Wechat. or a unit role ID.</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int SetAppInfo(string appID, string appKey, string openID);

    /// <summary>
    /// Set your Server's Infomation.
    /// </summary>
    /// <param name="URL">Server's URL</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int SetServerInfo(string URL);


    /// <summary>
    /// Init the voice engine.
    /// </summary>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int Init();

    /// <summary>
    /// Set the mode for engine.
    /// </summary>
    /// <param name="mode">mode to set
    /// RealTime:    realtime mode for TeamRoom or NationalRoom
    /// Messages:    voice message mode
    /// Translation: speach to text mode</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int SetMode(GCloudVoiceMode mode);

    /// <summary>
    /// Trigger engine's callback.
    /// You should invoke poll on your loop. such as Update() in Unity
    /// </summary>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract  int Poll();

    /// <summary>
    /// The Application's Pause.
    /// When your app pause such as goto backend you should invoke this
    /// </summary>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int Pause();

    /// <summary>
    /// The Application's Resume.
    /// When your app reuse such as come back from  backend you should invoke this
    /// </summary>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int Resume();

    /// <summary>
    /// Join in team room.
    /// </summary>
    /// <param name="roomName">the room to join, should be less than 127byte, composed by alpha.</param>
    /// <param name="msTimeout">time for join, it is micro second. value range[5000, 60000]</param>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int JoinTeamRoom(string roomName, int msTimeout);

    /// <summary>
    /// Join in a national room.
    /// </summary>
    /// <param name="roomName">the room to join, should be less than 127byte, composed by alpha.</param>
    /// <param name="role"> GCloudVoiceRole value illustrate wheather can send voice data.</param>
    /// <param name="msTimeout">time for join, it is micro second. value range[5000, 60000]</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int JoinNationalRoom(string roomName, GCloudVoiceRole role, int msTimeout);

    /// <summary>
    /// Join in a FM room.
    /// </summary>
    /// <param name="roomName">the room to join, should be less than 127byte, composed by alpha.</param>
    /// <param name="msTimeout">time for join, it is micro second. value range[5000, 60000]</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int JoinFMRoom(string roomName, int msTimeout);

    /// <summary>
    /// Quit the voice room.
    /// </summary>
    /// <param name="roomName">the room to join, should be less than 127byte, composed by alpha.</param>
    /// <param name="msTimeout">time for quit, it is micro second. value range[5000, 60000]</param>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int QuitRoom(string roomName, int msTimeout);

    /// <summary>
    ///  Open player's micro phone  and begin to send player's voice data.
    /// </summary>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int OpenMic();

    /// <summary>
    /// Close players's micro phone and stop to send player's voice data.
    /// </summary>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int CloseMic();

    /// <summary>
    ///  Open player's speaker and begin recvie voice data from the net .
    /// </summary>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int OpenSpeaker();

    /// <summary>
    ///  Close player's speaker and stop to recive voice data from the net.
    /// </summary>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int CloseSpeaker();

    /// <summary>
    /// Apply the key for voice message.
    /// </summary>
    /// <param name="msTimeout">time for apply, it is micro second.value range[5000, 60000]</param>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int ApplyMessageKey(int msTimeout);

    /// <summary>
    /// Limit the max voice message's last time.
    /// </summary>
    /// <param name="msTime">message's largest time in micro second.value range[1000, 2*60*1000]</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int SetMaxMessageLength(int msTime);

    /// <summary>
    /// Open player's microphone and record player's voice.
    /// </summary>
    /// <param name="filePath">voice data to store. file path should be such as: "your_dir/your_file_name" be sure to use "/" not "\"</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int StartRecording(string filePath);

    /// <summary>
    /// Stop player's microphone and stop record player's voice.
    /// </summary>
    ///<returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int StopRecording();

    /// <summary>
    /// Upload player's voice message file to the net.
    /// </summary>
    /// <param name="filePath">voice data to store. file path should be such as: "your_dir/your_file_name" be sure to use "/" not "\"</param>
    /// <param name="msTimeout">time for upload, it is micro second. value range[5000, 60000]</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int UploadRecordedFile(string filePath, int msTimeout);

    /// <summary>
    /// Download other players' voice message.
    /// </summary>
    /// <param name="fileID">file to be download</param>
    /// <param name="downloadFilePath">voice data to store. file path should be such as: "your_dir/your_file_name" be sure to use "/" not "\"</param>
    /// <param name="msTimeout">time for download, it is micro second. value range[5000, 60000]</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout);

    /// <summary>
    /// Play local voice message file.
    /// </summary>
    /// <param name="downloadFilePath">voice data to store. file path should be such as: "your_dir/your_file_name" be sure to use "/" not "\"</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int PlayRecordedFile (string downloadFilePath);

    /// <summary>
    /// Stop play the file.
    /// </summary>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int StopPlayFile();

    /// <summary>
    /// Translate voice data to text.
    /// </summary>
    /// <param name="fileID">file to be translate</param>
    /// <param name="language">a GCloudLanguage indicate which language to be translate</param>
    /// <param name="msTimeout">timeout for stt</param>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int SpeechToText(string fileID, int language = 0, int msTimeout = 6000);


    //exten interface
    /// <summary>
    /// Don't play voice of the member.
    /// </summary>
    /// <param name="member">member to forbid</param>
    /// <param name="bEnable">do forbid if it is true</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int ForbidMemberVoice(int member, bool bEnable);

    /// <summary>
    /// Open Voice Engine's logcat
    /// </summary>
    /// <param name="enable"> open logcat if it is true</param>
    /// <returns>return GCLOUD_VOICE_SUCC</returns>
    public abstract int EnableLog(bool enable);

    /// <summary>
    /// Get micphone's volume
    /// </summary>
    /// <returns>micphone's volume, if return value>0, means you have said something capture by micphone</returns>
    public abstract int GetMicLevel();

    /// <summary>
    /// Get speaker's volume
    /// </summary>
    /// <returns>speaker's volume, value is equal you Call SetSpeakerVolume param value</returns>
    public abstract int GetSpeakerLevel();

    /// <summary>
    /// set sepaker's volume
    /// </summary>
    /// <param name="vol">setspeakervolume, 
    /// Android & IOS, value range is 0-800, 100 means original voice volume, 50 means only 1/2 original voice volume, 200 means double original voice volume
    /// Windows value range is 0x0-0xFFFF, suggested value bigger than 0xff00, then you can hear you speaker sound</param>
    /// <returns>if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int SetSpeakerVolume(int vol) ;

    /// <summary>
    /// Test wheather microphone is available
    /// </summary>
    /// <returns> if success return GCLOUD_VOICE_SUCC, means have detect micphone device,failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int TestMic() ;

    /// <summary>
    /// Get voice message's info
    /// </summary>
    /// <param name="filepath">file path should be such as:"your_dir/your_file_name" be sure to use "/" not "\"</param>
    /// <param name="bytes">on return for file's size</param>
    /// <param name="seconds">on return for voice's length</param>
    /// <returns> if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno</returns>
    public abstract int GetFileParam(string filepath, int [] bytes, float [] seconds);

    /// <summary>
    /// YOU SHOULD NOT INVOKE THIS ONE.
    /// </summary>
    /// <param name="nCmd"></param>
    /// <param name="nParam1"></param>
    /// <param name="nParam2"></param>
    /// <param name="pOutput"></param>
    /// <returns></returns>
    public abstract int invoke( uint nCmd, uint nParam1, uint nParam2, uint [] pOutput );



	public abstract int JoinTeamRoom (string roomName, string token, int timestamp, int msTimeout);
	public abstract int JoinNationalRoom (string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout);
	public abstract int ApplyMessageKey (string token, int timestamp, int msTimeout);
	public abstract int SpeechToText (string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000);

	
}



//class CGCloudVoiceSys
class GCloudVoice
{
    /// <summary>
    /// Get the voice engine instance
    /// </summary>
    /// <returns> the voice instance on success, or null on failed.</returns>
    public static IGCloudVoice GetEngine()
	{
		if (instance == null)
		{
			instance = new GCloudVoiceEngine();
		}
		return instance;
	
	}
	private static IGCloudVoice instance = null;
}
}//end namespace

