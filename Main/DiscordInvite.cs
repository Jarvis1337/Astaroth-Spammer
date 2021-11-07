public class DiscordInvite
{
    public string inviteCode;
    public bool valid, isGroup;
    public ulong guildId, channelId, membersCount;
    public int channelType;

    public DiscordInvite(string inviteCode, bool valid, bool isGroup, ulong guildId, ulong channelId, ulong membersCount, int channelType)
    {
        this.inviteCode = inviteCode;
        this.valid = valid;
        this.isGroup = isGroup;
        this.guildId = guildId;
        this.channelId = channelId;
        this.membersCount = membersCount;
        this.channelType = channelType;
    }
}