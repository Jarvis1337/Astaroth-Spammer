public class AutoReconnectData
{
    public string guildId;
    public string channelId;
    public string userIdGoLive;
    public bool microphoneMuted;
    public bool headphonesMuted;
    public bool videoEnabled;
    public bool goLive;
    public bool joinGoLive;
    public bool speakInStage;

    public AutoReconnectData(string guildId, string channelId, string userIdGoLive, bool microphoneMuted, bool headphonesMuted, bool videoEnabled, bool goLive, bool joinGoLive, bool speakInStage)
    {
        this.guildId = guildId;
        this.channelId = channelId;
        this.userIdGoLive = userIdGoLive;
        this.microphoneMuted = microphoneMuted;
        this.headphonesMuted = headphonesMuted;
        this.videoEnabled = videoEnabled;
        this.goLive = goLive;
        this.joinGoLive = joinGoLive;
        this.speakInStage = speakInStage;
    }
}