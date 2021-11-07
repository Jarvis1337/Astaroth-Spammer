using Leaf.xNet;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using Microsoft.VisualBasic;

public class DiscordClient
{
    public string token, language = "", cookie = "", superProperties = "", userAgent = "", userId = "";
    public WebSocket ws;
    public bool connected, phoneVerified;
    public string client_build_number;
    public List<string> queue = new List<string>(), idQueue = new List<string>();
    private string phoneNumber = "";
    private AutoReconnectData data = null;
    public bool connectedToVoice;
    public int payloads = 0;

    public DiscordClient(string token)
    {
        try
        {
            this.token = token;
            this.userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0";
        }
        catch
        {

        }
    }

    public string GetUserId()
    {
        try
        {
            if (userId != "")
            {
                return userId;
            }

            try
            {
                HttpRequest request = Utils.CreateCleanRequest();
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", GetLanguage());
                request.AddHeader("Alt-Used", "discord.com");
                request.AddHeader("Authorization", token);
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Cookie", GetCookie());
                request.AddHeader("DNT", "1");
                request.AddHeader("Host", "discord.com");
                request.AddHeader("Origin", "https://discord.com");
                request.AddHeader("Referer", "https://discord.com/channels/@me");
                request.AddHeader("TE", "Trailers");
                request.AddHeader("User-Agent", this.userAgent);
                request.AddHeader("X-Super-Properties", GetSuperProperties());
                dynamic jss = JObject.Parse(Utils.DecompressResponse(request.Get("https://discord.com/api/v9/users/@me")));
                string locale = (string)jss.id;
                userId = locale;
                return locale;
            }
            catch
            {
                return "";
            }
        }
        catch
        {
            return "";
        }
    }

    public bool IsPhoneVerified()
    {
        try
        {
            if (phoneNumber != "")
            {
                return phoneVerified;
            }

            try
            {
                HttpRequest request = Utils.CreateCleanRequest();
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", GetLanguage());
                request.AddHeader("Alt-Used", "discord.com");
                request.AddHeader("Authorization", token);
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Cookie", GetCookie());
                request.AddHeader("DNT", "1");
                request.AddHeader("Host", "discord.com");
                request.AddHeader("Origin", "https://discord.com");
                request.AddHeader("Referer", "https://discord.com/channels/@me");
                request.AddHeader("TE", "Trailers");
                request.AddHeader("User-Agent", this.userAgent);
                request.AddHeader("X-Super-Properties", GetSuperProperties());
                string lol = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/users/@me"));
                dynamic jss = JObject.Parse(Utils.DecompressResponse(request.Get("https://discord.com/api/v9/users/@me")));
                string locale = (string)jss.phone;

                try
                {
                    if (locale == null || locale == "null")
                    {
                        phoneNumber = "n";
                        phoneVerified = false;
                    }
                }
                catch
                {
                    phoneNumber = (string)jss.phone;
                    phoneVerified = true;
                }

                return phoneVerified;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public List<string> GetGuildChannels(string guildId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;
            List<string> channels = new List<string>();

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string response = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/guilds/" + guildId + "/channels"));
            dynamic dynJson = JsonConvert.DeserializeObject(response);

            foreach (var item in dynJson)
            {
                try
                {
                    if ((string) item.type == "0")
                    {
                        channels.Add((string)item.id);
                    }
                }
                catch
                {

                }
            }

            return channels;
        }
        catch
        {

        }

        return new List<string>();
    }

    public List<string> GetGuildRoles(string guildId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;
            List<string> roles = new List<string>();

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string response = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/guilds/" + guildId + "/roles"));
            dynamic dynJson = JsonConvert.DeserializeObject(response);

            foreach (var item in dynJson)
            {
                try
                {
                    if ((string) item.name != "@everyone")
                    {
                        roles.Add((string)item.id);
                    }
                }
                catch
                {

                }
            }

            return roles;
        }
        catch
        {

        }

        return new List<string>();
    }

    public List<string> GetGroupRecipients(string channelId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;
            List<string> recipients = new List<string>();

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string response = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/channels/" + channelId));
            dynamic dynJson = JsonConvert.DeserializeObject(response);

            foreach (var item in dynJson)
            {
                try
                {
                    foreach (var another in item)
                    {
                        try
                        {
                            foreach (var anotherino in another)
                            {
                                try
                                {
                                    recipients.Add((string)anotherino.id);
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return recipients;
        }
        catch
        {

        }

        return new List<string>();
    }

    public string GetLanguage()
    {
        try
        {
            if (language != "")
            {
                return language;
            }

            HttpRequest request = new HttpRequest();

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "it");
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie("it"));
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string response = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/users/@me"));
            dynamic jss = JObject.Parse(response);

            language = (string)jss.locale;
            userId = (string)jss.id;

            try
            {
                if ((string) jss.phone != null && (string) jss.phone != "null")
                {
                    phoneVerified = true;
                    phoneNumber = (string)jss.phone;
                }
                else
                {
                    phoneVerified = false;
                    phoneNumber = "n";
                }
            }
            catch
            {
                phoneVerified = false;
                phoneNumber = "n";
            }

            return language;
        }
        catch
        {
            return "";
        }
    }

    public string GetCookie(string language = "")
    {
        try
        {
            if (cookie != "")
            {
                return cookie + "; OptanonConsent=" + Utils.GetTest() + "; __cfruid=" + Utils.GetUniqueKey1(40) + "-1625931828";
            }
            
            if (language == "")
            {
                cookie = Utils.GetRandomCookie(GetLanguage());
            }
            else
            {
                cookie = Utils.GetRandomCookie(language);
            }

            return cookie + "; OptanonConsent=" + Utils.GetTest();
        }
        catch
        {
            return "";
        }
    }

    public string GetSuperProperties()
    {
        try
        {
            if (superProperties != "")
            {
                return superProperties;
            }

            client_build_number = Utils.GetUniqueInt(5).ToString();
            superProperties = Utils.Base64Encode("{\"os\":\"Windows\",\"browser\":\"Firefox\",\"device\":\"\",\"system_locale\":\"it-IT\",\"browser_user_agent\":\"" + userAgent + "\",\"browser_version\":\"89.0\",\"os_version\":\"10\",\"referrer\":\"\",\"referring_domain\":\"\",\"referrer_current\":\"\",\"referring_domain_current\":\"\",\"release_channel\":\"stable\",\"client_build_number\":" + client_build_number + ",\"client_event_source\":null}");
            return superProperties;
        }
        catch
        {
            return "";
        }
    }

    public bool FirstlyFetchInvite(DiscordInvite invite, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            try
            {
                return request.Get("https://discord.com/api/v9/invites/" + invite.inviteCode + "?inputValue=" + invite.inviteCode + "&with_counts=true&with_expiration=true").IsOK;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public bool FirstlyFetchGroupInvite(DiscordInvite invite, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            try
            {
                return request.Get("https://discord.com/api/v9/invites/" + invite.inviteCode + "?with_counts=true&with_expiration=true").IsOK;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public void JoinGuild(DiscordInvite invite, string contextProperties, string captchaBotID, string captchaBotChannelID, bool communityRules, bool reactionVerification, bool captchaBot, bool groupMode, HttpProxyClient proxyClient, string captchaKey)
    {
        try
        {
            if (!groupMode)
            {
                if (!FirstlyFetchInvite(invite, proxyClient))
                {
                    return;
                }

                HttpRequest request = Utils.CreateCleanRequest();
                request.Proxy = proxyClient;

                request.AddHeader("Accept", "*/*");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", GetLanguage());
                request.AddHeader("Alt-Used", "discord.com");
                request.AddHeader("Authorization", token);
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "0");
                request.AddHeader("Cookie", GetCookie());
                request.AddHeader("DNT", "1");
                request.AddHeader("Host", "discord.com");
                request.AddHeader("Origin", "https://discord.com");
                request.AddHeader("Referer", "https://discord.com/channels/@me");
                request.AddHeader("TE", "Trailers");
                request.AddHeader("User-Agent", this.userAgent);
                request.AddHeader("X-Context-Properties", contextProperties);
                request.AddHeader("X-Super-Properties", GetSuperProperties());

                try
                {
                    request.Post("https://discord.com/api/v9/invites/" + invite.inviteCode);
                }
                catch
                {

                }

                try
                {
                    if (request.Response.IsOK && request.Response.StatusCode == Leaf.xNet.HttpStatusCode.OK)
                    {
                        try
                        {
                            if (communityRules)
                            {
                                string response = Utils.DecompressResponse(request.Response);
                                dynamic jss = JObject.Parse(response);

                                if ((bool)jss.show_verification_form)
                                {
                                    Thread.Sleep(350);

                                    string rules = GetRules(invite, proxyClient);
                                    BypassRules(invite, rules, proxyClient);
                                }
                            }
                        }
                        catch
                        {

                        }

                        try
                        {
                            Thread.Sleep(1000);

                            if (reactionVerification)
                            {
                                BypassReactionVerification(invite, true, proxyClient);
                            }
                            else
                            {
                                BypassReactionVerification(invite, false, proxyClient);
                            }

                            Thread.Sleep(1250);
                        }
                        catch
                        {

                        }

                        try
                        {
                            if (captchaBot)
                            {
                                Thread.Sleep(1500);
                                BypassCaptchaBot(captchaBotID, captchaBotChannelID, proxyClient, captchaKey);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                if (!FirstlyFetchGroupInvite(invite, proxyClient))
                {
                    return;
                }

                HttpRequest request = Utils.CreateCleanRequest();
                request.Proxy = proxyClient;

                request.AddHeader("Accept", "*/*");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", GetLanguage());
                request.AddHeader("Alt-Used", "discord.com");
                request.AddHeader("Authorization", token);
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "0");
                request.AddHeader("Cookie", GetCookie());
                request.AddHeader("DNT", "1");
                request.AddHeader("Host", "discord.com");
                request.AddHeader("Origin", "https://discord.com");
                request.AddHeader("Referer", "https://discord.com/channels/@me");
                request.AddHeader("TE", "Trailers");
                request.AddHeader("User-Agent", this.userAgent);
                request.AddHeader("X-Context-Properties", contextProperties);
                request.AddHeader("X-Super-Properties", GetSuperProperties());

                try
                {
                    request.Post("https://discord.com/api/v9/invites/" + invite.inviteCode);
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    public async void BypassCaptchaBot(string captchaBotID, string captchaBotChannelID, HttpProxyClient proxyClient, string captchaKey)
    {
        try
        {
            if (Utils.IsIDValid(captchaBotID))
            {
                string embedUrl = "", channelId = "";

                if (Utils.IsIDValid(captchaBotChannelID))
                {
                    channelId = captchaBotChannelID;
                }
                else
                {
                    channelId = GetDMChannel(captchaBotID, proxyClient);
                }

                embedUrl = GetEmbedURL(channelId, proxyClient);
                HttpRequest request = Utils.CreateCleanRequest();
                request.Proxy = proxyClient;
                string captchaBase64 = Convert.ToBase64String(request.Get(embedUrl).ToBytes());
                TwoCaptcha.TwoCaptcha solver = new TwoCaptcha.TwoCaptcha(captchaKey);
                TwoCaptcha.Captcha.Normal captcha = new TwoCaptcha.Captcha.Normal();
                captcha.SetBase64(captchaBase64);
                captcha.SetCaseSensitive(true);
                await solver.Solve(captcha);
                string solved = captcha.Code;

                AnswerToCaptcha(channelId, solved, proxyClient);
            }
        }
        catch
        {

        }
    }

    public void AnswerToCaptcha(string channelId, string solved, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;
            string data = "{\"content\":\"" + solved + "\",\"nonce\":\"" + Utils.GetUniqueLong(18).ToString() + "\",\"tts\":false}";

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Post("https://discord.com/api/v9/channels/" + channelId + "/messages", data, "application/json");
        }
        catch
        {

        }
    }

    public void BypassReactionVerification(DiscordInvite invite, bool doThat, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string messages = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/channels/" + invite.channelId + "/messages?limit=50"));

            if (doThat)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(messages);

                foreach (var item in dynJson)
                {
                    try
                    {
                        foreach (var item1 in item.reactions)
                        {
                            try
                            {
                                string reaction = "", id = "";
                                id = item1.emoji.id;
                                reaction = item1.emoji.name;

                                if (id != null && id != "")
                                {
                                    reaction += ":" + id;
                                }

                                Thread.Sleep(1500);
                                AddReaction(reaction, invite.channelId.ToString(), (string)item.id, proxyClient);
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch
                    {

                    }
                }

                try
                {
                    string channelDone = invite.channelId.ToString();

                    foreach (string channelId in GetGuildChannels(invite.guildId.ToString(), proxyClient))
                    {
                        if (channelId == channelDone)
                        {
                            continue;
                        }

                        try
                        {
                            HttpRequest request1 = Utils.CreateCleanRequest();
                            request1.Proxy = proxyClient;

                            request1.AddHeader("Accept", "*/*");
                            request1.AddHeader("Accept-Encoding", "gzip, deflate, br");
                            request1.AddHeader("Accept-Language", GetLanguage());
                            request1.AddHeader("Alt-Used", "discord.com");
                            request1.AddHeader("Authorization", token);
                            request1.AddHeader("Connection", "keep-alive");
                            request1.AddHeader("Cookie", GetCookie());
                            request1.AddHeader("Host", "discord.com");
                            request1.AddHeader("Referer", "https://discord.com/channels/@me");
                            request1.AddHeader("TE", "Trailers");
                            request1.AddHeader("User-Agent", this.userAgent);
                            request1.AddHeader("X-Super-Properties", GetSuperProperties());

                            string messages1 = Utils.DecompressResponse(request1.Get("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50"));
                            dynamic dynJson1 = JsonConvert.DeserializeObject(messages1);

                            foreach (var item in dynJson1)
                            {
                                try
                                {
                                    foreach (var item1 in item.reactions)
                                    {
                                        try
                                        {
                                            string reaction = "", id = "";
                                            id = item1.emoji.id;
                                            reaction = item1.emoji.name;

                                            if (id != null && id != "")
                                            {
                                                reaction += ":" + id;
                                            }

                                            Thread.Sleep(1500);
                                            AddReaction(reaction, channelId, (string)item.id, proxyClient);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    public void AddReaction(string reaction, string channelId, string messageId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", "0");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/" + channelId + "/" + messageId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Put("https://discord.com/api/v9/channels/" + channelId + "/messages/" + messageId + "/reactions/" + reaction + "/@me");
        }
        catch
        {

        }
    }

    public string GetRules(DiscordInvite invite, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/" + invite.guildId + "/" + invite.channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string data = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/guilds/" + invite.guildId + "/member-verification?with_guild=false&invite_code=" + invite.inviteCode)), toSend = "";

            if (data.Contains("\"form_fields\": []") || data.Contains("\"form_fields\":[]"))
            {
                string lol1 = Microsoft.VisualBasic.Strings.Split(data, "{\"version\": \"")[1];
                string lol2 = Microsoft.VisualBasic.Strings.Split(lol1, "\"")[0];

                toSend = "{\"version\": \"" + lol2 + "\",\"form_fields\": []}";
            }
            else
            {
                string lol1 = Microsoft.VisualBasic.Strings.Split(data, "}], \"description\":")[0];

                toSend = lol1 + ",\"response\":true}]}";
            }

            return toSend;
        }
        catch
        {
            return "";
        }
    }

    public void BypassRules(DiscordInvite invite, string data, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/" + invite.guildId + "/" + invite.channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Put("https://discord.com/api/v9/guilds/" + invite.guildId + "/requests/@me", data, "application/json");
        }
        catch
        {

        }
    }

    public void LeaveGuild(DiscordInvite invite, bool groupMode, HttpProxyClient proxyClient)
    {
        try
        {
            if (groupMode)
            {
                LeaveGuild(invite.channelId.ToString(), groupMode, proxyClient);
            }
            else
            {
                LeaveGuild(invite.guildId.ToString(), groupMode, proxyClient);
            }
        }
        catch
        {

        }
    }

    public void LeaveGuild(string guildId, bool groupMode, HttpProxyClient proxyClient)
    {
        try
        {
            if (!groupMode)
            {
                HttpRequest request = Utils.CreateCleanRequest();
                request.Proxy = proxyClient;

                request.AddHeader("Accept", "*/*");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", GetLanguage());
                request.AddHeader("Alt-Used", "discord.com");
                request.AddHeader("Authorization", token);
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "17");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", GetCookie());
                request.AddHeader("DNT", "1");
                request.AddHeader("Host", "discord.com");
                request.AddHeader("Origin", "https://discord.com");
                request.AddHeader("Referer", "https://discord.com/channels/@me");
                request.AddHeader("TE", "Trailers");
                request.AddHeader("User-Agent", this.userAgent);
                request.AddHeader("X-Super-Properties", GetSuperProperties());

                request.Delete("https://discord.com/api/v9/users/@me/guilds/" + guildId, "{\"lurking\":false}", "application/json");
            }
            else
            {
                HttpRequest request = Utils.CreateCleanRequest();
                request.Proxy = proxyClient;

                request.AddHeader("Accept", "*/*");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", GetLanguage());
                request.AddHeader("Alt-Used", "discord.com");
                request.AddHeader("Authorization", token);
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "17");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", GetCookie());
                request.AddHeader("DNT", "1");
                request.AddHeader("Host", "discord.com");
                request.AddHeader("Origin", "https://discord.com");
                request.AddHeader("Referer", "https://discord.com/channels/@me");
                request.AddHeader("TE", "Trailers");
                request.AddHeader("User-Agent", this.userAgent);
                request.AddHeader("X-Super-Properties", GetSuperProperties());

                request.Delete("https://discord.com/api/v9/channels/" + guildId, "{\"lurking\":false}", "application/json");
            }
        }
        catch
        {

        }
    }

    public void ReadChannel(string channelId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Get("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50");
        }
        catch
        {

        }
    }

    public string GetEmbedURL(string channelId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string messages = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50"));
            dynamic dynJson = JsonConvert.DeserializeObject(messages);

            foreach (var item in dynJson)
            {
                try
                {
                    foreach (var item1 in item.embeds)
                    {
                        try
                        {
                            return (string)item1.image.url;
                        }
                        catch
                        {

                        }
                    }

                    break;
                }
                catch
                {

                }
            }
        }
        catch
        {

        }

        return "";
    }

    public string GetDMChannel(string userId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string dms = Utils.DecompressResponse(request.Get("https://discord.com/api/v9/users/@me/channels"));
            dynamic jss = JsonConvert.DeserializeObject(dms);

            foreach (var item in jss)
            {
                try
                {
                    foreach (var another in item.recipients)
                    {
                        try
                        {
                            if ((string) another.id == userId)
                            {
                                return (string)item.id;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return CreateDM(userId, proxyClient);
        }
        catch
        {

        }

        return "";
    }

    public string CreateDM(string userId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;
            string data = "{\"recipients\":[\"" + userId + "\"]}";

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Context-Properties", "e30=");
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            string result = Utils.DecompressResponse(request.Post("https://discord.com/api/v9/users/@me/channels", data, "application/json"));
            dynamic jss = JObject.Parse(result);
            string theId = (string)jss.id;

            return theId;
        }
        catch
        {

        }

        return "";
    }

    public void ConnectToWebSocket()
    {
        try
        {
            if (!connected)
            {
                ws = new WebSocket("wss://gateway.discord.gg/?encoding=json&v=9");
                ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                ws.Origin = "https://discord.com";
                ws.EnableRedirection = false;
                ws.EmitOnPing = false;
                ws.CustomHeaders = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Encoding", "gzip, deflate, br" },
                { "Accept-Language", "it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3" },
                { "Cache-Control", "no-cache" },
                { "Connection", "keep-alive, Upgrade" },
                { "DNT", "1" },
                { "Host", "gateway.discord.gg" },
                { "Origin", "https://discord.com" },
                { "Pragma", "no-cache" },
                { "Sec-WebSocket-Extensions", "permessage-deflate" },
                { "Sec-WebSocket-Version", "13" },
                { "Upgrade", "WebSocket" },
                { "User-Agent", this.userAgent }
            };
                GetSuperProperties();
                new Thread(fetchQueue).Start();
                ws.OnMessage += Ws_OnMessage;
                ws.Connect();
                ws.Send("{\"op\":2,\"d\":{\"token\":\"" + token + "\",\"capabilities\":125,\"properties\":{\"os\":\"Windows\",\"browser\":\"Firefox\",\"device\":\"\",\"system_locale\":\"it-IT\",\"browser_user_agent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0\",\"browser_version\":\"89.0\",\"os_version\":\"10\",\"referrer\":\"\",\"referring_domain\":\"\",\"referrer_current\":\"\",\"referring_domain_current\":\"\",\"release_channel\":\"stable\",\"client_build_number\":" + client_build_number.ToString() + ",\"client_event_source\":null},\"presence\":{\"status\":\"online\",\"since\":0,\"activities\":[],\"afk\":false},\"compress\":false,\"client_state\":{\"guild_hashes\":{},\"highest_last_message_id\":\"0\",\"read_state_version\":0,\"user_guild_settings_version\":-1}}}");
                connected = true;
            }
        }
        catch
        {

        }
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        try
        {
            string data = System.Text.Encoding.UTF8.GetString(e.RawData);
            queue.Add(data);
            dynamic jss = JObject.Parse(data);

            if (jss.op == 10)
            {
                int heartbeat_interval = jss.d.heartbeat_interval;
                new Thread(() => doHeartbeat(heartbeat_interval)).Start();
            }

            if (jss.t == "GUILD_MEMBER_LIST_UPDATE")
            {
                idQueue.Add(data);
                payloads++;
            }
            else if ((string)jss.t == "VOICE_STATE_UPDATE")
            {
                try
                {
                    if ((string)jss.d.member.user.id == GetUserId())
                    {
                        if ((string)jss.d.channel_id == null || (string)jss.d.channel_id == "null")
                        {
                            connectedToVoice = false;

                            if (this.data != null)
                            {
                                if (Utils.globalAutoReconnect)
                                {
                                    JoinVoice(this.data.guildId, this.data.channelId, this.data.userIdGoLive, this.data.microphoneMuted, this.data.headphonesMuted, this.data.videoEnabled, this.data.goLive, this.data.joinGoLive, this.data.speakInStage);
                                }
                            }
                        }
                        else
                        {
                            connectedToVoice = true;
                        }
                    }
                }
                catch
                {
                    connectedToVoice = false;
                }
            }
        }
        catch
        {

        }
    }

    public void doHeartbeat(int heartbeat_interval)
    {
        try
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(heartbeat_interval);
                    ws.Send("{\"op\":1,\"d\":null}");
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    public void fetchQueue()
    {
        while (true)
        {
            Thread.Sleep(250);

            try
            {
                if (!(queue.Count <= 0))
                {
                    string data = queue[0];
                    queue.RemoveAt(0);
                }
            }
            catch
            {

            }

            try
            {
                if (!(idQueue.Count <= 0))
                {
                    string data = idQueue[0];
                    idQueue.RemoveAt(0);

                    string[] splitted = Strings.Split(data, "\"id\":\"");

                    for (int i = 1; i < splitted.Length; i++)
                    {
                        try
                        {
                            string another = splitted[i];

                            string[] anotherSplit = Strings.Split(another, "\"");
                            string finalId = anotherSplit[0];

                            if (Information.IsNumeric(finalId) && finalId.Length == 18 && data.Contains(finalId + '"' + "," + '"' + "discriminator") && !Utils.users.Contains(finalId))
                            {
                                Utils.users.Add(finalId);
                            }
                        }
                        catch
                        {

                        }
                    }

                    for (int i = 0; i < Utils.users.Count; i++)
                    {
                        try
                        {
                            for (int j = 0; j < Utils.users.Count; j++)
                            {
                                try
                                {
                                    if (i != j)
                                    {
                                        if (Utils.users[i] == Utils.users[j])
                                        {
                                            Utils.users.RemoveAt(i);
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }
        }
    }

    public void ParseGroup(DiscordInvite invite, HttpProxyClient proxyClient)
    {
        try
        {
            Utils.users.Clear();
            Utils.users.AddRange(GetGroupRecipients(invite.channelId.ToString(), proxyClient));
        }
        catch
        {

        }
    }

    public void ParseGuild(DiscordInvite invite, HttpProxyClient proxyClient, string channelId)
    {
        try
        {
            if (Utils.lastChannelId == channelId)
            {
                payloads = 0;
                Utils.users.Clear();

                try
                {
                    //Thread.Sleep(1000);
                    int first = 0, second = 99;

                    if (Utils.lastChannelId == channelId)
                    {
                        try
                        {
                            ws.Send("{\"op\":14,\"d\":{\"guild_id\":\"" + invite.guildId.ToString() + "\",\"typing\":true,\"activities\":true,\"threads\":true,\"channels\":{\"" + channelId.ToString() + "\":[[" + first.ToString() + "," + second.ToString() + "]]}}}");
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        return;
                    }

                    ulong members = invite.membersCount;
                    //Thread.Sleep(1000);

                    while (members > 100 && Utils.lastChannelId == channelId)
                    {
                        try
                        {
                            //Thread.Sleep(1000);

                            if (payloads >= 2)
                            {
                                payloads = 0;
                                first += 100;
                                second += 100;
                                members -= 100;

                                if (Utils.lastChannelId != channelId)
                                {
                                    return;
                                }

                                try
                                {
                                    ws.Send("{\"op\":14,\"d\":{\"guild_id\":\"" + invite.guildId.ToString() + "\",\"typing\":true,\"activities\":true,\"threads\":true,\"channels\":{\"" + channelId.ToString() + "\":[[" + first.ToString() + "," + second.ToString() + "]]}}}");
                                }
                                catch
                                {

                                }

                                if (Utils.lastChannelId != channelId)
                                {
                                    return;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    public void SendMessage(string channelId, string message, string reference, HttpProxyClient proxyClient, bool delete = false)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            string data = "";

            if (reference == "")
            {
                data = "{\"content\":\"" + message + "\",\"nonce\":\"" + Utils.GetUniqueLong(18).ToString() + "\",\"tts\":false}";
            }
            else
            {
                data = "{\"content\":\"" + message + "\",\"nonce\":\"" + Utils.GetUniqueLong(18).ToString() + "\",\"tts\":false,\"message_reference\":{\"channel_id\":\"" + channelId + "\",\"message_id\":\"" + reference + "\"}}";
            }

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            dynamic jss = JObject.Parse(Utils.DecompressResponse(request.Post("https://discord.com/api/v9/channels/" + channelId + "/messages", data, "application/json")));
            string theId = (string) jss.id;

            if (delete)
            {
                DeleteMessage(channelId, theId, proxyClient);
            }
        }
        catch
        {

        }
    }

    public void SendToWS(string data)
    {
        try
        {
            if (!connected)
            {
                ConnectToWebSocket();
            }

            ws.Send(data);
        }
        catch
        {

        }
    }

    public void SetStatus(UserStatus status, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            string theStatus = "online";

            if (status.Equals(UserStatus.DoNotDisturb))
            {
                theStatus = "dnd";
            }
            else if (status.Equals(UserStatus.Idle))
            {
                theStatus = "idle";
            }
            else if (status.Equals(UserStatus.Invisible))
            {
                theStatus = "invisible";
            }

            string data = "{\"status\":\"" + theStatus + "\"}";

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Patch("https://discord.com/api/v9/users/@me/settings", data, "application/json");
        }
        catch
        {

        }
    }

    public void SetNickname(string guildId, string nickname, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            string data = "{\"nick\":\"" + nickname + "\"}";

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Patch("https://discord.com/api/v9/guilds/" + guildId + "/members/@me", data, "application/json");
        }
        catch
        {

        }
    }

    public void SetHypeSquad(HypeSquad house, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            int hypesquad = 1;

            if (house.Equals(HypeSquad.Balance))
            {
                hypesquad = 3;
            }
            else if (house.Equals(HypeSquad.Brilliance))
            {
                hypesquad = 2;
            }

            string data = "{\"house_id\":" + hypesquad.ToString() + "}";

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Post("https://discord.com/api/v9/hypesquad/online", data, "application/json");
        }
        catch
        {

        }
    }

    public void TypeInChannel(string channelId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", "0");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Post("https://discord.com/api/v9/channels/" + channelId + "/typing");
        }
        catch
        {

        }
    }

    public void AddFriend(string friend, HttpProxyClient proxyClient)
    {
        try
        {
            if (Utils.IsIDValid(friend))
            {
                AddFriendByID(friend, proxyClient);
            }
            else if (Utils.IsTagValid(friend))
            {
                AddFriendByTag(friend, proxyClient);
            }
        }
        catch
        {

        }
    }

    public void AddFriendByID(string userId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", "2");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Context-Properties", "eyJsb2NhdGlvbiI6IkNvbnRleHRNZW51In0=");
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Put("https://discord.com/api/v9/users/@me/relationships/" + userId, "{}", "application/json");
        }
        catch
        {

        }
    }

    public void AddFriendByTag(string tag, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            string[] splitted = Strings.Split(tag, "#");
            string data = "{\"username\":\"" + splitted[0] + "\",\"discriminator\":" + splitted[1] + "}";

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Context-Properties", "eyJsb2NhdGlvbiI6IkFkZCBGcmllbmQifQ==");
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Post("https://discord.com/api/v9/users/@me/relationships", data, "application/json");
        }
        catch
        {

        }
    }

    public void AddFriendByTag(string username, string discriminator, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            string data = "{\"username\":\"" + username + "\",\"discriminator\":" + discriminator + "}";

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", data.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Context-Properties", "eyJsb2NhdGlvbiI6IkFkZCBGcmllbmQifQ==");
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Post("https://discord.com/api/v9/users/@me/relationships", data, "application/json");
        }
        catch
        {

        }
    }

    public void RemoveFriend(string userId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Context-Properties", "eyJsb2NhdGlvbiI6IkZyaWVuZHMifQ==");
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Delete("https://discord.com/api/v9/users/@me/relationships/" + userId);
        }
        catch
        {
            
        }
    }

    public string FetchEmote(string channelId, string messageId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            dynamic dynJson = JsonConvert.DeserializeObject(Utils.DecompressResponse(request.Get("https://discord.com/api/v9/channels/" + channelId + "/messages?limit=50")));

            foreach (var item in dynJson)
            {
                try
                {
                    if ((string)item.id == messageId)
                    {
                        try
                        {
                            foreach (var item1 in item.reactions)
                            {
                                try
                                {
                                    string reaction = "", id = "";
                                    id = item1.emoji.id;
                                    reaction = item1.emoji.name;

                                    if (id != null && id != "")
                                    {
                                        reaction += ":" + id;
                                    }

                                    return reaction;
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return "";
        }
        catch
        {
            return "";
        }
    }

    public void RemoveReaction(string reaction, string channelId, string messageId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/" + channelId + "/" + messageId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Delete("https://discord.com/api/v9/channels/" + channelId + "/messages/" + messageId + "/reactions/" + reaction + "/@me");
        }
        catch
        {

        }
    }

    public void JoinVoice(string guildId, string channelId, string userIdGoLive, bool microphoneMuted, bool headphonesMuted, bool videoEnabled, bool goLive, bool joinGoLive, bool speakInStage)
    {
        try
        {
            if (!connectedToVoice)
            {
                connectedToVoice = true;
                this.data = new AutoReconnectData(guildId, channelId, userIdGoLive, microphoneMuted, headphonesMuted, videoEnabled, goLive, joinGoLive, speakInStage);

                SendToWS("{\"op\":4,\"d\":{\"guild_id\":\"" + guildId + "\",\"channel_id\":\"" + channelId + "\",\"self_mute\":" + microphoneMuted.ToString().ToLower() + ",\"self_deaf\":" + headphonesMuted.ToString().ToLower() + ",\"self_video\":" + videoEnabled.ToString().ToLower() + ",\"preferred_region\":null}}");

                if (speakInStage)
                {
                    SendSpeakRequestToStageChannel(guildId, channelId);
                }
                else
                {
                    if (goLive)
                    {
                        GoLive(guildId, channelId);
                    }

                    if (joinGoLive && Utils.IsIDValid(userIdGoLive))
                    {
                        JoinGoLive(guildId, channelId, userIdGoLive);
                    }
                }
            }
        }
        catch
        {
            connectedToVoice = false;
        }
    }

    public void LeaveVoice()
    {
        try
        {
            if (connectedToVoice)
            {
                connectedToVoice = false;
                SendToWS("{\"op\":4,\"d\":{\"guild_id\":null,\"channel_id\":null,\"self_mute\":false,\"self_deaf\":false,\"self_video\":false}}");
            }
        }
        catch
        {
            connectedToVoice = true;
        }
    }

    public void SendSpeakRequestToStageChannel(string guildId, string channelId)
    {
        try
        {
            string timestamp = "";

            string year = "", month = "", day = "", hour = "", minute = "", second = "";

            year = DateTime.Now.Year.ToString();
            month = DateTime.Now.Month.ToString();

            if (month.Length == 1)
            {
                month = "0" + month;
            }

            day = DateTime.Now.Day.ToString();

            if (day.Length == 1)
            {
                day = "0" + day;
            }

            hour = DateTime.Now.Hour.ToString();

            if (hour.Length == 1)
            {
                hour = "0" + hour;
            }

            minute = DateTime.Now.Minute.ToString();

            if (minute.Length == 1)
            {
                minute = "0" + minute;
            }

            second = DateTime.Now.Minute.ToString();

            if (second.Length == 1)
            {
                second = "0" + second;
            }

            timestamp = year + "-" + month + "-" + day + "T" + hour + ":" + minute + ":" + second + "." + DateTime.Now.Millisecond.ToString() + "Z";

            string messageJson = "{\"request_to_speak_timestamp\":\"" + timestamp + "\",\"channel_id\":\"" + channelId + "\"}";

            HttpRequest request = Utils.CreateCleanRequest();

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", messageJson.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/" + guildId + "/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Patch("https://discord.com/api/v9/guilds/" + guildId + "/voice-states/@me", messageJson, "application/json");
        }
        catch
        {

        }
    }

    public void GoLive(string guildId, string channelId)
    {
        try
        {
            SendToWS("{\"op\":18,\"d\":{\"type\":\"guild\",\"guild_id\":\"" + guildId + "\",\"channel_id\":\"" + channelId + "\",\"preferred_region\":null}}");
            SendToWS("{\"op\":22,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + GetUserId() + "\",\"paused\":false}}");
        }
        catch
        {

        }
    }

    public void StopGoLive(string guildId, string channelId)
    {
        try
        {
            SendToWS("{\"op\":19,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + GetUserId() + "\"}}");
        }
        catch
        {

        }
    }

    public void JoinGoLive(string guildId, string channelId, string theUser)
    {
        try
        {
            SendToWS("{\"op\":20,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + theUser + "\"}}");
        }
        catch
        {

        }
    }

    public void LeaveGoLive(string guildId, string channelId, string userId)
    {
        try
        {
            SendToWS("{\"op\":19,\"d\":{\"stream_key\":\"guild:" + guildId + ":" + channelId + ":" + userId + "\"}}");
        }
        catch
        {

        }
    }

    public void PhoneLock()
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();

            request.AddHeader("Authorization", token);

            try
            {
                request.Post("https://discord.com/api/v9/invites/NXk4rE5jFA");
            }
            catch
            {

            }
        }
        catch
        {

        }
    }

    public void DeleteMessage(string channelId, string messageId, HttpProxyClient proxyClient)
    {
        try
        {
            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("DNT", "1");
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me/" + channelId);
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Delete("https://discord.com/api/v9/channels/" + channelId + "/messages/" + messageId);
        }
        catch
        {

        }
    }

    public void SetAvatar(System.Drawing.Image image, HttpProxyClient proxyClient)
    {
        try
        {
            var ms = new System.IO.MemoryStream();
            image.Save(ms, image.RawFormat);
            SetAvatar(Convert.ToBase64String(ms.ToArray()), proxyClient);
        }
        catch
        {

        }
    }

    public void SetAvatar(string base64, HttpProxyClient proxyClient)
    {
        try
        {
            string content = "{\"avatar\":\"data:image/png;base64," + base64 + "\"}";

            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", content.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Patch("https://discord.com/api/v9/users/@me", content, "application/json");
        }
        catch
        {

        }
    }

    public void ResetAvatar(HttpProxyClient proxyClient)
    {
        try
        {
            string content = "{\"avatar\":null}";

            HttpRequest request = Utils.CreateCleanRequest();
            request.Proxy = proxyClient;

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", GetLanguage());
            request.AddHeader("Alt-Used", "discord.com");
            request.AddHeader("Authorization", token);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", content.Length.ToString());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", GetCookie());
            request.AddHeader("Host", "discord.com");
            request.AddHeader("Origin", "https://discord.com");
            request.AddHeader("Referer", "https://discord.com/channels/@me");
            request.AddHeader("TE", "Trailers");
            request.AddHeader("User-Agent", this.userAgent);
            request.AddHeader("X-Super-Properties", GetSuperProperties());

            request.Patch("https://discord.com/api/v9/users/@me", content, "application/json");
        }
        catch
        {

        }
    }
}