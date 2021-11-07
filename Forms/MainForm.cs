using System.Windows.Forms;
using System.Threading;
using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using MetroSuite;
using System.Collections.Generic;
using Leaf.xNet;
using System.Threading.Tasks;
using System.Linq;

public partial class MainForm : MetroForm
{
    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(IntPtr hwProc);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

    public List<DiscordClient> clients = new List<DiscordClient>();
    public List<string> invalidTokens = new List<string>();
    public int doneCheckingTokens = 0;

    public List<string> proxies = new List<string>();
    public List<string> invalidProxies = new List<string>();
    public int doneCheckingProxies = 0;
    public int proxyIndex = 0;

    public bool serverSpammer, dmSpammer, typingSpammer, webhookSpammer, massDmAdvertiser;
    public int multipleMessageIndex = 0, multipleDmMessageIndex = 0, multipleWebhookMessageIndex = 0, multipleDmAdvertiserMessageIndex = 0, tagAllIndex = 0, rolesTagAllIndex = 0;
    public int completedUsers = 0;

    private string[] mediaFormats = new string[] { "jpg", "png", "bmp", "jpeg", "jfif", "jpe", "rle", "dib", "svg", "svgz" };
    public bool tokensChanged;

    public List<DiscordClient> GetClients()
    {
        if (siticoneCheckBox37.Checked)
        {
            List<DiscordClient> theClients = new List<DiscordClient>();

            int limited = 1;
            string coso = gunaLineTextBox23.Text.Replace(" ", "").Replace('\t'.ToString(), "");

            if (coso.Length <= 6)
            {
                if (Microsoft.VisualBasic.Information.IsNumeric(coso))
                {
                    int temp = int.Parse(coso);

                    if (!(temp <= 0))
                    {
                        limited = temp;
                    }
                }
            }

            int i = 0;

            foreach (DiscordClient client in this.clients)
            {
                if (i == limited)
                {
                    break;
                }

                theClients.Add(clients[i]);
                i++;
            }

            return theClients;
        }

        return this.clients;
    }

    public HttpProxyClient GetProxy()
    {
        try
        {
            if (siticoneCheckBox23.Checked)
            {
                try
                {
                    if (proxyIndex >= proxies.Count)
                    {
                        proxyIndex = -1;
                    }

                    proxyIndex++;

                    if (proxyIndex >= proxies.Count)
                    {
                        proxyIndex = 0;
                    }

                    return Utils.ParseProxy(proxies[proxyIndex]);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public MainForm()
    {
        try
        {
            InitializeComponent();

            tokensChanged = true;

            try
            {
                if (!System.IO.File.Exists("tokens.txt"))
                {
                    System.IO.File.WriteAllText("tokens.txt", "");
                }
                else
                {
                    LoadTokens("tokens.txt");
                }
            }
            catch
            {

            }

            try
            {
                if (!System.IO.File.Exists("proxies.txt"))
                {
                    System.IO.File.WriteAllText("proxies.txt", "");
                }
                else
                {
                    LoadProxies("proxies.txt");
                }
            }
            catch
            {

            }

            CheckForIllegalCrossThreadCalls = false;

            siticoneComboBox2.SelectedIndex = 0;
            siticoneComboBox1.SelectedIndex = 0;

            Thread updateAllThread = new Thread(updateAll);
            updateAllThread.Start();

            openFileDialog5.Filter = "All files (*.*)|*.*";

            foreach (string format in mediaFormats)
            {
                openFileDialog5.Filter += "|" + format.ToUpper() + " Image (*." + format + ")|*." + format;
            }
        }
        catch
        {

        }
    }

    public void save()
    {
        try
        {
            string allProxies = "", allTokens = "";

            try
            {
                foreach (string proxy in proxies)
                {
                    try
                    {
                        if (allProxies == "")
                        {
                            allProxies = proxy;
                        }
                        else
                        {
                            allProxies += Environment.NewLine + proxy;
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

            try
            {
                System.IO.File.WriteAllText("proxies.txt", allProxies);
            }
            catch
            {

            }

            try
            {
                foreach (DiscordClient client in clients)
                {
                    try
                    {
                        if (allTokens == "")
                        {
                            allTokens = client.token;
                        }
                        else
                        {
                            allTokens += Environment.NewLine + client.token;
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

            try
            {
                System.IO.File.WriteAllText("tokens.txt", allTokens);
            }
            catch
            {

            }
        }
        catch
        {

        }
    }

    public void updateAll()
    {
        while (true)
        {
            try
            {
                Thread.Sleep(1000);

                try
                {
                    if (metroLabel14.Text == "0")
                    {
                        invalidTokens.Clear();
                        doneCheckingTokens = 0;
                        siticoneButton2.Enabled = true;
                        siticoneButton2.Text = "Remove dead tokens";
                        metroLabel14.Text = clients.Count.ToString();
                        siticoneButton1.Enabled = true;
                    }

                    if (clients.Count == 0)
                    {
                        metroLabel14.Text = "0";
                    }

                    if (metroLabel15.Text == "0")
                    {
                        invalidProxies.Clear();
                        doneCheckingProxies = 0;
                        siticoneButton3.Enabled = true;
                        siticoneButton3.Text = "Remove dead proxies";
                        metroLabel15.Text = proxies.Count.ToString();
                        siticoneButton4.Enabled = true;
                    }

                    if (proxies.Count == 0)
                    {
                        metroLabel15.Text = "0";
                    }

                    if (Utils.users.Count == 0)
                    {
                        siticoneCheckBox7.Text = "Mass Mention";
                    }
                    else
                    {
                        siticoneCheckBox7.Text = "Mass Mention (" + Utils.users.Count.ToString() + ")";
                    }

                    if (gunaLineTextBox4.Text.Contains(","))
                    {
                        siticoneCheckBox8.Text = "Multiple Channels (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox4.Text, ",").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox8.Text = "Multiple Channels";
                    }

                    if (gunaTextBox1.Text.Contains("|"))
                    {
                        siticoneCheckBox9.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox9.Text = "Multiple Messages";
                    }

                    if (gunaLineTextBox6.Text.Contains(","))
                    {
                        siticoneCheckBox11.Text = "Multiple Users (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox6.Text, ",").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox11.Text = "Multiple Users";
                    }

                    if (gunaTextBox2.Text.Contains("|"))
                    {
                        siticoneCheckBox12.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox12.Text = "Multiple Messages";
                    }

                    if (gunaLineTextBox11.Text.Contains(","))
                    {
                        siticoneCheckBox5.Text = "Multiple Friends (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox11.Text, ",").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox5.Text = "Multiple Friends";
                    }

                    if (gunaLineTextBox12.Text.Contains(","))
                    {
                        siticoneCheckBox13.Text = "Multiple Channels (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox12.Text, ",").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox13.Text = "Multiple Channels";
                    }

                    if (gunaLineTextBox17.Text.Contains(","))
                    {
                        siticoneCheckBox22.Text = "Multiple Webhooks (" + Microsoft.VisualBasic.Strings.Split(gunaLineTextBox17.Text, ",").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox22.Text = "Multiple Webhooks";
                    }

                    if (gunaTextBox3.Text.Contains("|"))
                    {
                        siticoneCheckBox27.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox27.Text = "Multiple Messages";
                    }

                    metroLabel8.Text = "Parsed users: " + Utils.users.Count.ToString() + ", completed users: " + completedUsers.ToString();

                    if (gunaTextBox4.Text.Contains("|"))
                    {
                        siticoneCheckBox28.Text = "Multiple Messages (" + Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|").Length.ToString() + ")";
                    }
                    else
                    {
                        siticoneCheckBox28.Text = "Multiple Messages";
                    }

                    if (completedUsers == Utils.users.Count)
                    {
                        gunaButton20.Enabled = false;
                        new Thread(reEnableDMAdvertiser).Start();
                    }

                    if (Utils.roles.Count == 0)
                    {
                        siticoneCheckBox29.Text = "Roles Mention";
                    }
                    else
                    {
                        siticoneCheckBox29.Text = "Roles Mention (" + Utils.roles.Count.ToString() + ")";
                    }

                    metroLabel32.Text = Utils.users.Count.ToString();
                    metroLabel33.Text = Utils.roles.Count.ToString();

                    if (pictureBox2.BackgroundImage == null)
                    {
                        siticoneButton22.Enabled = false;
                    }
                    else
                    {
                        siticoneButton22.Enabled = true;
                    }

                    if (tokensChanged)
                    {
                        tokensChanged = false;
                        save();
                    }
                }
                catch
                {

                }
            }
            catch
            {

            }
        }
    }

    public string getTagAllUser()
    {
        try
        {
            if (tagAllIndex >= Utils.users.Count)
            {
                tagAllIndex = -1;
            }

            tagAllIndex++;

            if (tagAllIndex >= Utils.users.Count)
            {
                tagAllIndex = 0;
            }

            return Utils.users[tagAllIndex];
        }
        catch
        {

        }

        return "";
    }

    public string getTagAllRole()
    {
        try
        {
            if (rolesTagAllIndex >= Utils.roles.Count)
            {
                rolesTagAllIndex = -1;
            }

            rolesTagAllIndex++;

            if (rolesTagAllIndex >= Utils.roles.Count)
            {
                rolesTagAllIndex = 0;
            }

            return Utils.roles[rolesTagAllIndex];
        }
        catch
        {

        }

        return "";
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        try
        {
            Process.GetCurrentProcess().Kill();
        }
        catch
        {

        }
    }

    private void bunifuHSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel17.Text = "Delay: " + bunifuHSlider1.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider2_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel18.Text = "Delay: " + bunifuHSlider2.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider3_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel19.Text = "Delay: " + bunifuHSlider3.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider4_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel20.Text = "Delay: " + bunifuHSlider4.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider5_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel21.Text = "Delay: " + bunifuHSlider5.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider6_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel22.Text = "Delay: " + bunifuHSlider6.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider9_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel25.Text = "Delay: " + bunifuHSlider9.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider10_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel26.Text = "Auto end: " + bunifuHSlider10.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider11_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel27.Text = "Delay: " + bunifuHSlider11.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void bunifuHSlider12_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
    {
        try
        {
            metroLabel28.Text = "Delay: " + bunifuHSlider12.Value.ToString() + "ms";
        }
        catch
        {

        }
    }

    private void gunaLinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(gunaLinkLabel1.Text);
        }
        catch
        {

        }
    }

    private void siticoneButton1_Click(object sender, EventArgs e)
    {
        // Homepage - Reset Tokens
        try
        {
            clients.Clear();
            metroLabel14.Text = "0";

            invalidTokens.Clear();
            doneCheckingTokens = 0;
            siticoneButton2.Enabled = true;
            siticoneButton2.Text = "Remove dead tokens";
            tokensChanged = true;
        }
        catch
        {

        }
    }

    private void siticoneButton2_Click(object sender, EventArgs e)
    {
        // Homepage - Remove dead tokens
        try
        {
            invalidTokens.Clear();
            doneCheckingTokens = 0;
            siticoneButton2.Enabled = false;
            siticoneButton1.Enabled = false;
            siticoneButton2.Text = "Removing dead tokens...";
            Thread thread = new Thread(CheckTokens);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void CheckTokens()
    {
        try
        {
            try
            {
                foreach (DiscordClient client in clients)
                {
                    try
                    {
                        if (!siticoneButton2.Enabled)
                        {
                            Thread.Sleep(45);

                            Thread thread = new Thread(() => CheckToken(client));
                            
                            thread.Start();
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

            try
            {
                while (doneCheckingTokens != clients.Count)
                {

                }
            }
            catch
            {

            }

            metroLabel14.Text = (clients.Count).ToString();

            foreach (string token in invalidTokens)
            {
                try
                {
                    foreach (DiscordClient client in clients)
                    {
                        try
                        {
                            if (client.token == token)
                            {
                                clients.Remove(client);
                                metroLabel14.Text = (clients.Count).ToString();

                                break;
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

            invalidTokens.Clear();
            doneCheckingTokens = 0;
            siticoneButton2.Enabled = true;
            siticoneButton2.Text = "Remove dead tokens";
            metroLabel14.Text = clients.Count.ToString();
            siticoneButton1.Enabled = true;
            tokensChanged = true;
        }
        catch
        {

        }
    }

    public void CheckToken(DiscordClient client)
    {
        try
        {
            if (!siticoneButton2.Enabled)
            {
                if (!Utils.IsTokenValid(client.token))
                {
                    invalidTokens.Add(client.token);
                    metroLabel14.Text = (clients.Count - invalidTokens.Count).ToString();
                }

                doneCheckingTokens++;
            }
        }
        catch
        {

        }
    }

    private void siticoneButton5_Click(object sender, EventArgs e)
    {
        // Homepage - Load Tokens
        try
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string fileName in openFileDialog1.FileNames)
                    {
                        try
                        {
                            LoadTokens(fileName);
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

    public void LoadTokens(string fileName)
    {
        foreach (string token in Utils.SplitToLines(System.IO.File.ReadAllText(fileName)))
        {
            try
            {
                string tok = Utils.GetCleanToken(token);
                bool inserted = false;

                if (!Utils.IsTokenFormatValid(tok))
                {
                    continue;
                }

                try
                {
                    foreach (DiscordClient client in clients)
                    {
                        try
                        {
                            if (client.token == tok)
                            {
                                inserted = true;
                                break;
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

                if (!inserted)
                {
                    clients.Add(new DiscordClient(tok));
                    metroLabel14.Text = clients.Count.ToString();
                }
            }
            catch
            {

            }
        }

        tokensChanged = true;
    }

    private void siticoneButton4_Click(object sender, EventArgs e)
    {
        // Homepage - Reset Proxies
        try
        {
            proxies.Clear();
            metroLabel15.Text = "0";

            invalidProxies.Clear();
            doneCheckingProxies = 0;
            siticoneButton3.Enabled = true;
            siticoneButton3.Text = "Remove dead proxies";
            tokensChanged = true;
        }
        catch
        {

        }
    }

    private void siticoneButton3_Click(object sender, EventArgs e)
    {
        // Homepage - Remove dead proxies
        try
        {
            invalidProxies.Clear();
            doneCheckingProxies = 0;
            siticoneButton3.Enabled = false;
            siticoneButton4.Enabled = false;
            siticoneButton3.Text = "Removing dead proxies...";
            Thread thread = new Thread(CheckProxies);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void CheckProxies()
    {
        try
        {
            foreach (string proxy in proxies)
            {
                try
                {
                    if (!siticoneButton3.Enabled)
                    {
                        Thread.Sleep(45);

                        Thread thread = new Thread(() => CheckProxy(proxy));
                        
                        thread.Start();
                    }
                }
                catch
                {

                }
            }

            try
            {
                while (doneCheckingProxies != proxies.Count)
                {

                }
            }
            catch
            {

            }

            metroLabel15.Text = (proxies.Count).ToString();

            try
            {
                foreach (string proxy in invalidProxies)
                {
                    try
                    {
                        foreach (string anotherProxy in proxies)
                        {
                            try
                            {
                                if (proxy == anotherProxy)
                                {
                                    proxies.Remove(proxy);
                                    metroLabel15.Text = (proxies.Count).ToString();

                                    break;
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

            invalidProxies.Clear();
            doneCheckingProxies = 0;
            siticoneButton3.Enabled = true;
            siticoneButton3.Text = "Remove dead proxies";
            metroLabel15.Text = proxies.Count.ToString();
            siticoneButton4.Enabled = true;
            tokensChanged = true;
        }
        catch
        {

        }
    }

    public void CheckProxy(string proxy)
    {
        try
        {
            if (!siticoneButton3.Enabled)
            {
                if (!Utils.IsProxyValid(proxy))
                {
                    invalidProxies.Add(proxy);
                    metroLabel15.Text = (proxies.Count - invalidProxies.Count).ToString();
                }

                doneCheckingProxies++;
            }
        }
        catch
        {

        }
    }

    private void siticoneButton6_Click(object sender, EventArgs e)
    {
        // Homepage - Load Proxies
        try
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string fileName in openFileDialog2.FileNames)
                    {
                        try
                        {
                            LoadProxies(fileName);
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

        tokensChanged = true;
    }

    public void LoadProxies(string fileName)
    {
        try
        {
            foreach (string proxy in Utils.SplitToLines(System.IO.File.ReadAllText(fileName)))
            {
                try
                {
                    string prxy = Utils.GetCleanToken(proxy);
                    bool inserted = false;

                    if (!Utils.IsProxyFormatValid(prxy))
                    {
                        continue;
                    }

                    try
                    {
                        foreach (string theProxy in proxies)
                        {
                            try
                            {
                                if (theProxy == prxy)
                                {
                                    inserted = true;
                                    break;
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

                    if (!inserted)
                    {
                        proxies.Add(prxy);
                        metroLabel15.Text = proxies.Count.ToString();
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

    private void gunaButton1_Click(object sender, EventArgs e)
    {
        // Guild Manager - Join guild
        try
        {
            if (siticoneCheckBox3.Checked)
            {
                if (!Utils.IsCaptchaKeyValid(gunaLineTextBox20.Text))
                {
                    MessageBox.Show("The 2Captcha key is not valid! Go to insert a new valid one on the section 'Settings and Utils'.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Thread thread = new Thread(() => JoinGuild(gunaLineTextBox1.Text, gunaLineTextBox2.Text, gunaLineTextBox3.Text, siticoneCheckBox1.Checked, siticoneCheckBox2.Checked, siticoneCheckBox3.Checked, siticoneCheckBox4.Checked));
            thread.Start();
        }
        catch
        {

        }
    }

    public void JoinGuild(string theInvite, string captchaBotID, string captchaBotChannelID, bool communityRules, bool reactionVerification, bool captchaBot, bool groupMode)
    {
        try
        {
            DiscordInvite invite = Utils.GetInviteInformations(theInvite, groupMode);

            if (!invite.valid)
            {
                MessageBox.Show("The inserted invite link / code is not valid! Check if the invite is valid and check if you got rate limited from the Discord API.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (siticoneCheckBox33.Checked)
                {
                    gunaButton1.Enabled = false;
                    gunaButton2.Enabled = true;
                }

                string contextProperties = "";

                if (!groupMode)
                {
                    contextProperties = Utils.GetXCP(invite);
                }
                else
                {
                    contextProperties = Utils.GetGroupXCP(invite);
                }

                foreach (DiscordClient client in this.GetClients())
                {
                    try
                    {
                        Thread.Sleep(5);

                        if (siticoneOSToggleSwith1.Checked)
                        {
                            Thread.Sleep(bunifuHSlider1.Value);
                        }

                        Thread thread = new Thread(() => ClientJoin(client, invite, contextProperties, captchaBotID, captchaBotChannelID, communityRules, reactionVerification, captchaBot, groupMode, GetProxy(), gunaLineTextBox20.Text));                 
                        thread.Start();
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

    public void ClientJoin(DiscordClient client, DiscordInvite invite, string contextProperties, string captchaBotID, string captchaBotChannelID, bool communityRules, bool reactionVerification, bool captchaBot, bool groupMode, HttpProxyClient proxyClient, string captchaKey)
    {
        if (siticoneCheckBox33.Checked)
        {
            while (siticoneCheckBox33.Checked && gunaButton2.Enabled)
            {
                Thread.Sleep(5);

                if (siticoneOSToggleSwith1.Checked)
                {
                    Thread.Sleep(bunifuHSlider1.Value);
                }

                client.JoinGuild(invite, contextProperties, captchaBotID, captchaBotChannelID, communityRules, reactionVerification, captchaBot, groupMode, proxyClient, captchaKey);

                Thread.Sleep(5);

                if (siticoneOSToggleSwith1.Checked)
                {
                    Thread.Sleep(bunifuHSlider1.Value);
                }

                client.LeaveGuild(invite, groupMode, proxyClient);
            }
        }
        else
        {
            client.JoinGuild(invite, contextProperties, captchaBotID, captchaBotChannelID, communityRules, reactionVerification, captchaBot, groupMode, proxyClient, captchaKey);
        }
    }


    private void gunaButton2_Click(object sender, EventArgs e)
    {
        // Guild Manager - Leave guild

        try
        {
            if (siticoneCheckBox33.Checked)
            {
                gunaButton2.Enabled = false;
                gunaButton1.Enabled = true;
                return;
            }

            Thread thread = new Thread(() => LeaveGuild(gunaLineTextBox1.Text, siticoneCheckBox4.Checked));
            thread.Start();
        }
        catch
        {

        }
    }

    public void LeaveGuild(string guildInviteID, bool groupMode)
    {
        try
        {
            if (!Utils.IsIDValid(guildInviteID))
            {
                DiscordInvite invite = Utils.GetInviteInformations(guildInviteID, groupMode);

                if (!invite.valid)
                {
                    MessageBox.Show("The inserted invite link / code / ID is not valid! Check if the invite is valid and check if you got rate limited from the Discord API, try also to use the right guild ID.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    foreach (DiscordClient client in this.GetClients())
                    {
                        try
                        {
                            Thread.Sleep(5);

                            if (siticoneOSToggleSwith1.Checked)
                            {
                                Thread.Sleep(bunifuHSlider1.Value);
                            }

                            Thread thread = new Thread(() => client.LeaveGuild(invite, groupMode, GetProxy()));                     
                            thread.Start();
                        }
                        catch
                        {

                        }
                    }
                }
            }
            else
            {
                foreach (DiscordClient client in this.GetClients())
                {
                    try
                    {
                        Thread.Sleep(5);

                        if (siticoneOSToggleSwith1.Checked)
                        {
                            Thread.Sleep(bunifuHSlider1.Value);
                        }

                        Thread thread = new Thread(() => client.LeaveGuild(guildInviteID, groupMode, GetProxy()));               
                        thread.Start();
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

    public int GetServerSpam()
    {
        try
        {
            if (siticoneRadioButton3.Checked)
            {
                return 0;
            }
            else if (siticoneRadioButton4.Checked)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        catch
        {
            return 0;
        }
    }

    public void SetServerSpam(int status)
    {
        try
        {
            if (status == 0)
            {
                siticoneRadioButton3.Checked = true;
            }
            else if (status == 1)
            {
                siticoneRadioButton4.Checked = true;
            }
            else
            {
                siticoneRadioButton5.Checked = true;
            }
        }
        catch
        {

        }
    }

    private void gunaButton4_Click(object sender, EventArgs e)
    {
        // Server Spammer - Start Spamming
        try
        {
            rolesTagAllIndex = 0;
            tagAllIndex = 0;
            int actual = GetServerSpam();
            gunaButton4.Enabled = false;
            gunaButton3.Enabled = true;
            SetServerSpam(actual);

            if (siticoneCheckBox8.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox4.Text))
                {
                    MessageBox.Show("The IDs of the channels are not valid!", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton3.Enabled = false;
                    gunaButton4.Enabled = true;

                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox4.Text))
                {
                    MessageBox.Show("The ID of the channel is not valid!", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton3.Enabled = false;
                    gunaButton4.Enabled = true;

                    return;
                }
            }

            Thread thread = new Thread(DoServerSpammer);
            
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton3_Click(object sender, EventArgs e)
    {
        // Server Spammer - Stop Spamming
        try
        {
            serverSpammer = false;
            new Thread(reEnableServerSpammer).Start();
        }
        catch
        {

        }
    }

    public void reEnableServerSpammer()
    {
        try
        {
            if (siticoneOSToggleSwith2.Checked)
            {
                Thread.Sleep(bunifuHSlider2.Value);
            }

            rolesTagAllIndex = 0;
            tagAllIndex = 0;
            gunaButton3.Enabled = false;
            gunaButton4.Enabled = true;
        }
        catch
        {

        }
    }

    public void DoServerSpammer()
    {
        try
        {
            serverSpammer = true;

            foreach (DiscordClient client in this.GetClients())
            {
                Thread.Sleep(5);

                try
                {
                    int times = 1;

                    if (siticoneRadioButton4.Checked)
                    {
                        times = 2;
                    }
                    else if (siticoneRadioButton5.Checked)
                    {
                        times = 10;
                    }

                    HttpProxyClient proxyClient = GetProxy();

                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            if (siticoneCheckBox8.Checked)
                            {
                                foreach (string id in Utils.GetIDs(gunaLineTextBox4.Text))
                                {
                                    Thread.Sleep(5);

                                    try
                                    {
                                        Thread.Sleep(5);

                                        if (siticoneOSToggleSwith2.Checked)
                                        {
                                            Thread.Sleep(bunifuHSlider2.Value);
                                        }

                                        Thread thread = new Thread(() => SpamServer(client, id, proxyClient));
                                        
                                        thread.Start();
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    Thread.Sleep(5);

                                    if (siticoneOSToggleSwith2.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider2.Value);
                                    }

                                    Thread thread = new Thread(() => SpamServer(client, gunaLineTextBox4.Text, proxyClient));
                                    
                                    thread.Start();
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

    public string getServerSpamMessage()
    {
        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox9.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox1.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox1.Text;
                    }
                }
                else
                {
                    if (multipleMessageIndex < 0)
                    {
                        multipleMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox1.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleMessageIndex > count)
                    {
                        multipleMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox1.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox1.Text;
                        }

                        multipleMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox1.Text, "|");
                        string definitive = splitted[multipleMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleMessageIndex == count)
                        {
                            multipleMessageIndex = 0;
                        }
                        else
                        {
                            multipleMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            if (Utils.users.Count == 0)
            {
                msg = msg.Replace("[mtag]", "");
                msg = msg.Replace("[all]", "");
            }

            if (Utils.roles.Count == 0)
            {
                msg = msg.Replace("[rtag]", "");
                msg = msg.Replace("[rall]", "");
            }

            if (siticoneCheckBox7.Checked)
            {
                try
                {
                    while (msg.Contains("[mtag]"))
                    {
                        try
                        {
                            string tag = getTagAllUser();

                            if (tag != "")
                            {
                                msg = Utils.ReplaceFirst(msg, "[mtag]", " <@" + tag + "> ");
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

                try
                {
                    string allUsers = "";

                    foreach (string user in Utils.users)
                    {
                        allUsers += "<@" + user + "> ";
                    }

                    allUsers = allUsers.Substring(0, allUsers.Length - 1);
                    msg = msg.Replace("[all]", allUsers);
                }
                catch
                {

                }
            }
            else
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");
            }

            if (siticoneCheckBox29.Checked)
            {
                try
                {
                    while (msg.Contains("[rtag]"))
                    {
                        try
                        {
                            string tag = getTagAllRole();

                            if (tag != "")
                            {
                                msg = Utils.ReplaceFirst(msg, "[rtag]", " <@&" + tag + "> ");
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

                try
                {
                    string allRoles = "";

                    foreach (string role in Utils.roles)
                    {
                        allRoles += "<@&" + role + "> ";
                    }

                    allRoles = allRoles.Substring(0, allRoles.Length - 1);
                    msg = msg.Replace("[rall]", allRoles);
                }
                catch
                {

                }
            }
            else
            {
                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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
        catch
        {

        }

        return msg;
    }

    public void SpamServer(DiscordClient client, string channelId, HttpProxyClient proxyClient)
    {
        Thread.Sleep(5);

        try
        {
            client.ReadChannel(channelId, proxyClient);
        }
        catch
        {

        }

        while (serverSpammer)
        {
            Thread.Sleep(5);

            try
            {
                if (siticoneOSToggleSwith2.Checked)
                {
                    Thread.Sleep(bunifuHSlider2.Value);
                }

                client.SendMessage(channelId, getServerSpamMessage(), Utils.IsIDValid(gunaLineTextBox5.Text) ? gunaLineTextBox5.Text : "", proxyClient, siticoneCheckBox34.Checked);
            }
            catch
            {

            }
        }
    }

    public int GetDMSpam()
    {
        try
        {
            if (siticoneRadioButton8.Checked)
            {
                return 0;
            }
            else if (siticoneRadioButton7.Checked)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        catch
        {
            return 0;
        }
    }

    public void SetDMSpam(int status)
    {
        try
        {
            if (status == 0)
            {
                siticoneRadioButton8.Checked = true;
            }
            else if (status == 1)
            {
                siticoneRadioButton7.Checked = true;
            }
            else
            {
                siticoneRadioButton6.Checked = true;
            }
        }
        catch
        {
            
        }
    }

    private void gunaButton5_Click(object sender, EventArgs e)
    {
        // DM Spammer - Start Spamming
        try
        {
            int actual = GetDMSpam();
            gunaButton5.Enabled = false;
            gunaButton6.Enabled = true;
            SetDMSpam(actual);

            if (siticoneCheckBox11.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox6.Text))
                {
                    MessageBox.Show("The IDs of the users are not valid!", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton6.Enabled = false;
                    gunaButton5.Enabled = true;

                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox6.Text))
                {
                    MessageBox.Show("The ID of the user is not valid!", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton6.Enabled = false;
                    gunaButton5.Enabled = true;

                    return;
                }
            }

            Thread thread = new Thread(DoDMSpammer);
            
            thread.Start();
        }
        catch
        {
            
        }
    }

    private void gunaButton6_Click(object sender, EventArgs e)
    {
        // DM Spammer - Stop Spamming
        try
        {
            dmSpammer = false;
            new Thread(reEnableDMSpammer).Start();
        }
        catch
        {

        }
    }

    public void DoDMSpammer()
    {
        try
        {
            dmSpammer = true;

            foreach (DiscordClient client in this.GetClients())
            {
                Thread.Sleep(5);

                try
                {
                    int times = 1;

                    if (siticoneRadioButton7.Checked)
                    {
                        times = 2;
                    }
                    else if (siticoneRadioButton6.Checked)
                    {
                        times = 10;
                    }

                    List<string> ids = new List<string>();
                    HttpProxyClient proxyClient = GetProxy();

                    try
                    {
                        if (siticoneCheckBox12.Checked)
                        {
                            try
                            {
                                foreach (string id in Utils.GetIDs(gunaLineTextBox6.Text))
                                {
                                    try
                                    {
                                        ids.Add(client.GetDMChannel(id, proxyClient));
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
                            ids.Add(client.GetDMChannel(gunaLineTextBox6.Text, proxyClient));
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        foreach (string id in ids)
                        {
                            try
                            {
                                if (!Utils.IsIDValid(id))
                                {
                                    continue;
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

                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            foreach (string id in ids)
                            {
                                Thread.Sleep(5);

                                try
                                {
                                    if (siticoneOSToggleSwith3.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider3.Value);
                                    }

                                    Thread thread = new Thread(() => SpamDM(client, id, GetProxy()));
                                    
                                    thread.Start();
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

    public string getDMSpamMessage()
    {
        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox12.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox2.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox2.Text;
                    }
                }
                else
                {
                    if (multipleDmMessageIndex < 0)
                    {
                        multipleDmMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox2.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleDmMessageIndex > count)
                    {
                        multipleDmMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox2.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox2.Text;
                        }

                        multipleDmMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleDmMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox2.Text, "|");
                        string definitive = splitted[multipleDmMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleDmMessageIndex == count)
                        {
                            multipleDmMessageIndex = 0;
                        }
                        else
                        {
                            multipleDmMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");

                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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
        catch
        {

        }

        return msg;
    }

    public void reEnableDMSpammer()
    {
        try
        {
            if (siticoneOSToggleSwith3.Checked)
            {
                Thread.Sleep(bunifuHSlider3.Value);
            }

            gunaButton6.Enabled = false;
            gunaButton5.Enabled = true;
        }
        catch
        {

        }
    }

    public void SpamDM(DiscordClient client, string channelId, HttpProxyClient proxyClient)
    {
        Thread.Sleep(5);

        try
        {
            client.ReadChannel(channelId, proxyClient);
        }
        catch
        {

        }

        try
        {
            while (dmSpammer)
            {
                try
                {
                    Thread.Sleep(5);

                    if (siticoneOSToggleSwith3.Checked)
                    {
                        Thread.Sleep(bunifuHSlider3.Value);
                    }

                    client.SendMessage(channelId, getDMSpamMessage(), Utils.IsIDValid(gunaLineTextBox7.Text) ? gunaLineTextBox7.Text : "", proxyClient);
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

    private void gunaButton7_Click(object sender, EventArgs e)
    {
        // Reaction Spammer - Add reaction

        try
        {
            if (siticoneRadioButton1.Checked)
            {
                if (!Utils.IsEmojiValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emoji is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!Utils.IsEmoteValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emote is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!Utils.IsIDValid(gunaLineTextBox9.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox10.Text))
            {
                MessageBox.Show("The ID of the message is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread thread = new Thread(() => DoReactionAdder(gunaLineTextBox8.Text, gunaLineTextBox9.Text, gunaLineTextBox10.Text));
            
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton8_Click(object sender, EventArgs e)
    {
        // Reaction Spammer - Remove reaction

        try
        {
            if (siticoneRadioButton1.Checked)
            {
                if (!Utils.IsEmojiValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emoji is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!Utils.IsEmoteValid(gunaLineTextBox8.Text))
                {
                    MessageBox.Show("The emote is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!Utils.IsIDValid(gunaLineTextBox9.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox10.Text))
            {
                MessageBox.Show("The ID of the message is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread thread = new Thread(() => DoReactionRemover(gunaLineTextBox8.Text, gunaLineTextBox9.Text, gunaLineTextBox10.Text));
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoReactionAdder(string reaction, string channelId, string messageId)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(5);

                    if (siticoneOSToggleSwith4.Checked)
                    {
                        Thread.Sleep(bunifuHSlider4.Value);
                    }

                    Thread thread = new Thread(() => AddReaction(client, reaction, channelId, messageId, GetProxy()));
                    
                    thread.Start();
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

    public void DoReactionRemover(string reaction, string channelId, string messageId)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(5);

                    if (siticoneOSToggleSwith4.Checked)
                    {
                        Thread.Sleep(bunifuHSlider4.Value);
                    }

                    Thread thread = new Thread(() => RemoveReaction(client, reaction, channelId, messageId, GetProxy()));
                    
                    thread.Start();
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

    public void AddReaction(DiscordClient client, string reaction, string channelId, string messageId, HttpProxyClient proxyClient)
    {
        try
        {
            client.AddReaction(reaction, channelId, messageId, proxyClient);
        }
        catch
        {

        }
    }

    public void RemoveReaction(DiscordClient client, string reaction, string channelId, string messageId, HttpProxyClient proxyClient)
    {
        try
        {
            client.RemoveReaction(reaction, channelId, messageId, proxyClient);
        }
        catch
        {

        }
    }

    private void gunaButton10_Click(object sender, EventArgs e)
    {
        // Friend Spammer - Add friend

        try
        {
            if (siticoneCheckBox35.Checked)
            {
                if (siticoneCheckBox5.Checked)
                {
                    if (!Utils.AreIDsValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The IDs of the users are not valid! Please, insert new valids.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (!Utils.IsIDValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The ID of the user is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                gunaButton10.Enabled = false;
                gunaButton9.Enabled = true;
            }
            else
            {
                if (siticoneCheckBox5.Checked)
                {
                    if (!Utils.AreFriendsValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The IDs and / or tags of the users are not valid! Please, insert new valids.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (!Utils.IsFriendValid(gunaLineTextBox11.Text))
                    {
                        MessageBox.Show("The ID / tag of the user is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            Thread thread = new Thread(() => DoFriendAdder(gunaLineTextBox11.Text, siticoneCheckBox5.Checked));
            
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton9_Click(object sender, EventArgs e)
    {
        // Friend Spammer - Remove friend

        try
        {
            if (siticoneCheckBox5.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox11.Text))
                {
                    MessageBox.Show("The IDs of the users are not valid! Please, insert new valids.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox11.Text))
                {
                    MessageBox.Show("The ID of the user is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (siticoneCheckBox35.Checked)
            {
                gunaButton9.Enabled = false;
                gunaButton10.Enabled = true;
            }

            Thread thread = new Thread(() => DoFriendRemover(gunaLineTextBox11.Text, siticoneCheckBox5.Checked));
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoFriendAdder(string str, bool multiple)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                Thread.Sleep(5);

                try
                {
                    if (multiple)
                    {
                        HttpProxyClient proxyClient = GetProxy();

                        try
                        {
                            foreach (string user in Utils.GetFriends(str))
                            {
                                try
                                {
                                    Thread.Sleep(5);

                                    if (siticoneOSToggleSwith5.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider5.Value);
                                    }

                                    Thread thread = new Thread(() => AddFriend(client, user, proxyClient));
                                    
                                    thread.Start();
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
                        try
                        {
                            if (siticoneOSToggleSwith5.Checked)
                            {
                                Thread.Sleep(bunifuHSlider5.Value);
                            }

                            Thread thread = new Thread(() => AddFriend(client, str, GetProxy()));
                            
                            thread.Start();
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

    public void DoFriendRemover(string str, bool multiple)
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                Thread.Sleep(5);

                try
                {
                    if (multiple)
                    {
                        HttpProxyClient proxyClient = GetProxy();

                        try
                        {
                            foreach (string user in Utils.GetIDs(str))
                            {
                                Thread.Sleep(5);

                                if (siticoneOSToggleSwith5.Checked)
                                {
                                    Thread.Sleep(bunifuHSlider5.Value);
                                }

                                try
                                {
                                    Thread thread = new Thread(() => RemoveFriend(client, user, proxyClient));
                                    
                                    thread.Start();
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
                        try
                        {
                            if (siticoneOSToggleSwith5.Checked)
                            {
                                Thread.Sleep(bunifuHSlider5.Value);
                            }

                            Thread thread = new Thread(() => RemoveFriend(client, str, GetProxy()));
                            
                            thread.Start();
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

    public void AddFriend(DiscordClient client, string friend, HttpProxyClient proxyClient)
    {
        try
        {
            client.AddFriend(friend, proxyClient);

            if (siticoneCheckBox35.Checked && gunaButton9.Enabled)
            {
                if (siticoneOSToggleSwith5.Checked)
                {
                    Thread.Sleep(bunifuHSlider5.Value);
                }

                RemoveFriend(client, friend, proxyClient);
            }
        }
        catch
        {

        }
    }

    public void RemoveFriend(DiscordClient client, string userId, HttpProxyClient proxyClient)
    {
        try
        {
            client.RemoveFriend(userId, proxyClient);

            if (siticoneCheckBox35.Checked && gunaButton9.Enabled)
            {
                if (siticoneOSToggleSwith5.Checked)
                {
                    Thread.Sleep(bunifuHSlider5.Value);
                }

                AddFriend(client, userId, proxyClient);
            }
        }
        catch
        {

        }
    }

    private void gunaButton11_Click(object sender, EventArgs e)
    {
        // Typing Spammer - Start Spamming
        try
        {
            gunaButton11.Enabled = false;
            gunaButton12.Enabled = true;

            if (siticoneCheckBox13.Checked)
            {
                if (!Utils.AreIDsValid(gunaLineTextBox12.Text))
                {
                    MessageBox.Show("The IDs of the channels are not valid!", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton12.Enabled = false;
                    gunaButton11.Enabled = true;

                    return;
                }
            }
            else
            {
                if (!Utils.IsIDValid(gunaLineTextBox12.Text))
                {
                    MessageBox.Show("The ID of the channel is not valid!", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    gunaButton12.Enabled = false;
                    gunaButton11.Enabled = true;

                    return;
                }
            }

            typingSpammer = true;
            Thread thread = new Thread(DoTypingSpammer);
            
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton12_Click(object sender, EventArgs e)
    {
        // Typing Spammer - Stop Spamming
        try
        {
            typingSpammer = false;
            gunaButton12.Enabled = false;
            gunaButton11.Enabled = true;
        }
        catch
        {

        }
    }

    public void DoTypingSpammer()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(5);

                    if (siticoneOSToggleSwith6.Checked)
                    {
                        Thread.Sleep(bunifuHSlider6.Value);
                    }

                    if (siticoneCheckBox13.Checked)
                    {
                        HttpProxyClient proxyClient = GetProxy();

                        try
                        {
                            foreach (string id in Utils.GetIDs(gunaLineTextBox12.Text))
                            {
                                try
                                {
                                    if (siticoneOSToggleSwith6.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider6.Value);
                                    }

                                    Thread thread = new Thread(() => TypingSpam(client, id, proxyClient));
                                    
                                    thread.Start();
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
                        try
                        {
                            Thread thread = new Thread(() => TypingSpam(client, gunaLineTextBox12.Text, GetProxy()));
                            
                            thread.Start();
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

    public void TypingSpam(DiscordClient client, string channelId, HttpProxyClient proxyClient)
    {
        try
        {
            while (typingSpammer)
            {
                try
                {
                    Thread.Sleep(5);

                    client.TypeInChannel(channelId, proxyClient);
                    Thread.Sleep(8000);
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

    private void gunaButton15_Click(object sender, EventArgs e)
    {
        // Voice Spammer - Join voice
        try
        {
            if (!Utils.IsIDValid(gunaLineTextBox14.Text))
            {
                MessageBox.Show("The ID of the guild is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox15.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread thread = new Thread(DoJoinVoice);
            
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton16_Click(object sender, EventArgs e)
    {
        // Voice Spammer - Leave voice
        try
        {
            Thread thread = new Thread(DoLeaveVoice);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoJoinVoice()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(5);

                    if (siticoneOSToggleSwith9.Checked)
                    {
                        Thread.Sleep(bunifuHSlider9.Value);
                    }

                    Thread thread = new Thread(() => JoinVoice(client));
                    
                    thread.Start();
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

    public void DoLeaveVoice()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread.Sleep(5);

                    if (siticoneOSToggleSwith9.Checked)
                    {
                        Thread.Sleep(bunifuHSlider9.Value);
                    }

                    Thread thread = new Thread(() => LeaveVoice(client));
                    
                    thread.Start();
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

    public void JoinVoice(DiscordClient client)
    {
        // Join voice there.

        try
        {
            client.JoinVoice(gunaLineTextBox14.Text, gunaLineTextBox15.Text, gunaLineTextBox16.Text, siticoneCheckBox14.Checked, siticoneCheckBox15.Checked, siticoneCheckBox16.Checked, siticoneCheckBox17.Checked, siticoneCheckBox18.Checked, siticoneCheckBox20.Checked);

            if (siticoneCheckBox21.Checked)
            {
                if (siticoneOSToggleSwith10.Checked)
                {
                    Thread.Sleep(bunifuHSlider10.Value);
                }

                LeaveVoice(client);
            }
        }
        catch
        {

        }
    }

    public void LeaveVoice(DiscordClient client)
    {
        // Leave voice there.

        try
        {
            client.LeaveVoice();
        }
        catch
        {

        }
    }

    private void gunaButton17_Click(object sender, EventArgs e)
    {
        // Webhook Spammer - Start Spamming
        try
        {
            gunaButton17.Enabled = false;
            webhookSpammer = true;
            Thread thread = new Thread(DoWebhookSpammer);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoWebhookSpammer()
    {
        try
        {
            int times = 1;

            if (siticoneRadioButton10.Checked)
            {
                times = 2;
            }
            else if (siticoneRadioButton9.Checked)
            {
                times = 10;
            }

            if (siticoneCheckBox22.Checked)
            {
                try
                {
                    foreach (string webhook in Utils.GetIDs(gunaLineTextBox17.Text))
                    {
                        Thread.Sleep(5);

                        try
                        {
                            if (!Utils.IsWebhookValid(webhook))
                            {
                                MessageBox.Show("The webhooks are not valid ('" + webhook + "'). Please, remove the invalid webhooks and try again.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                gunaButton17.Enabled = true;
                                return;
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

                gunaButton18.Enabled = true;

                try
                {
                    for (int i = 0; i < times; i++)
                    {
                        try
                        {
                            Thread.Sleep(5);

                            foreach (string webhook in Utils.GetIDs(gunaLineTextBox17.Text))
                            {
                                Thread.Sleep(5);

                                try
                                {
                                    Thread thread = new Thread(() => SpamWebhook(webhook));
                                    
                                    thread.Start();
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
            else
            {
                try
                {
                    if (!Utils.IsWebhookValid(gunaLineTextBox17.Text))
                    {
                        MessageBox.Show("The webhook is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        gunaButton17.Enabled = true;
                        return;
                    }

                    gunaButton18.Enabled = true;

                    try
                    {
                        for (int i = 0; i < times; i++)
                        {
                            try
                            {
                                Thread thread = new Thread(() => SpamWebhook(gunaLineTextBox17.Text));
                                
                                thread.Start();
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
                catch
                {

                }
            }
        }
        catch
        {

        }
    }

    private void gunaButton18_Click(object sender, EventArgs e)
    {
        // Webhook Spammer - Stop Spamming
        try
        {
            webhookSpammer = false;
            gunaButton18.Enabled = false;
            gunaButton17.Enabled = true;
        }
        catch
        {

        }
    }

    public string GetWebhookSpammerMessage()
    {
        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox27.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox3.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox3.Text;
                    }
                }
                else
                {
                    if (multipleWebhookMessageIndex < 0)
                    {
                        multipleWebhookMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox3.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleWebhookMessageIndex > count)
                    {
                        multipleWebhookMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox3.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox3.Text;
                        }

                        multipleWebhookMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleWebhookMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox3.Text, "|");
                        string definitive = splitted[multipleWebhookMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleWebhookMessageIndex == count)
                        {
                            multipleWebhookMessageIndex = 0;
                        }
                        else
                        {
                            multipleWebhookMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");

                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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
        catch
        {

        }

        return msg;
    }

    public void SpamWebhook(string url)
    {
        try
        {
            while (webhookSpammer)
            {
                try
                {
                    if (siticoneOSToggleSwith11.Checked)
                    {
                        Thread.Sleep(bunifuHSlider11.Value);
                    }

                    Utils.SendMessageToWebhook(url, gunaLineTextBox18.Text, gunaLineTextBox19.Text, GetWebhookSpammerMessage());
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

    private void gunaButton19_Click(object sender, EventArgs e)
    {
        // Mass DM Advertiser - Start Advertising
        try
        {
            if (Utils.users.Count == 0)
            {
                MessageBox.Show("There are no parsed users! Please, join in a guild and let the Spammer parse the users.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Utils.GetCleanToken(gunaTextBox4.Text) == "")
            {
                MessageBox.Show("Please, insert a valid message for advertising!", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            gunaButton19.Enabled = false;
            gunaButton20.Enabled = true;
            completedUsers = 0;
            massDmAdvertiser = true;

            Thread thread = new Thread(DoMassDMAdvertiser);
            
            thread.Start();
        }
        catch
        {

        }
    }

    private void gunaButton20_Click(object sender, EventArgs e)
    {
        // Mass DM Advertiser - Stop Advertising
        try
        {
            massDmAdvertiser = false;
            new Thread(reEnableDMAdvertiser).Start();
        }
        catch
        {

        }
    }

    public void reEnableDMAdvertiser()
    {
        try
        {
            if (siticoneOSToggleSwith12.Checked)
            {
                Thread.Sleep(bunifuHSlider12.Value);
            }

            gunaButton20.Enabled = false;
            gunaButton19.Enabled = true;
        }
        catch
        {

        }
    }

    public void DoMassDMAdvertiser()
    {
        try
        {
            int actualIndex = 0;

            foreach (DiscordClient client in this.GetClients())
            {
                if (siticoneOSToggleSwith12.Checked)
                {
                    Thread.Sleep(bunifuHSlider12.Value);
                }

                Thread.Sleep(5);

                try
                {
                    if (massDmAdvertiser)
                    {
                        if (siticoneOSToggleSwith12.Checked)
                        {
                            Thread.Sleep(bunifuHSlider12.Value);
                        }

                        try
                        {
                            List<string> preparedUsers = new List<string>();
                            int dms = 6;

                            try
                            {
                                if (client.IsPhoneVerified())
                                {
                                    dms = 22;
                                }
                            }
                            catch
                            {

                            }

                            try
                            {
                                for (int i = actualIndex; i < Utils.users.Count; i++)
                                {
                                    try
                                    {
                                        if (dms > 0)
                                        {
                                            dms--;
                                            actualIndex++;
                                            preparedUsers.Add(Utils.users[i]);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }
                            }
                            catch
                            {

                            }

                            try
                            {
                                if (siticoneOSToggleSwith12.Checked)
                                {
                                    Thread.Sleep(bunifuHSlider12.Value);
                                }

                                Thread thread = new Thread(() => Advertise(client, preparedUsers, GetProxy()));
                                
                                thread.Start();
                            }
                            catch
                            {

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

    public void Advertise(DiscordClient client, List<string> users, HttpProxyClient proxyClient)
    {
        try
        {
            if (massDmAdvertiser)
            {
                try
                {
                    foreach (string user in users)
                    {
                        Thread.Sleep(5);

                        try
                        {
                            if (massDmAdvertiser)
                            {
                                try
                                {
                                    if (siticoneOSToggleSwith12.Checked)
                                    {
                                        Thread.Sleep(bunifuHSlider12.Value);
                                    }

                                    if (massDmAdvertiser)
                                    {
                                        client.SendMessage(client.GetDMChannel(user, proxyClient), GetMassDMAdvertiserMessage(), "", proxyClient);
                                        completedUsers++;
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

    public string GetMassDMAdvertiserMessage()
    {
        string msg = "";

        try
        {
            try
            {
                if (!siticoneCheckBox28.Checked)
                {
                    List<string> lines = new List<string>();

                    foreach (string line in Utils.SplitToLines(gunaTextBox4.Text))
                    {
                        lines.Add(line);
                    }

                    if (lines.Count != 1)
                    {
                        foreach (string line in lines)
                        {
                            msg = msg + " \\u000d" + line;
                        }
                    }
                    else
                    {
                        msg = gunaTextBox4.Text;
                    }
                }
                else
                {
                    if (multipleDmAdvertiserMessageIndex < 0)
                    {
                        multipleDmAdvertiserMessageIndex = 0;
                    }

                    int count = 0;

                    foreach (char c in gunaTextBox4.Text.ToCharArray())
                    {
                        if (c == '|')
                        {
                            count++;
                        }
                    }

                    if (multipleDmAdvertiserMessageIndex > count)
                    {
                        multipleDmAdvertiserMessageIndex = 0;
                    }

                    if (count == 0)
                    {
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(gunaTextBox4.Text))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = gunaTextBox4.Text;
                        }

                        multipleDmAdvertiserMessageIndex++;
                    }
                    else if (count == 1 && Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|")[1].Replace(" ", "").Replace('\t'.ToString(), "").Trim() == "")
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|");
                        string definitive = splitted[0];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        multipleDmAdvertiserMessageIndex++;
                    }
                    else
                    {
                        string[] splitted = Microsoft.VisualBasic.Strings.Split(gunaTextBox4.Text, "|");
                        string definitive = splitted[multipleDmAdvertiserMessageIndex];
                        List<string> lines = new List<string>();

                        foreach (string line in Utils.SplitToLines(definitive))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count != 1)
                        {
                            foreach (string line in lines)
                            {
                                msg = msg + " \\u000d" + line;
                            }
                        }
                        else
                        {
                            msg = definitive;
                        }

                        if (multipleDmAdvertiserMessageIndex == count)
                        {
                            multipleDmAdvertiserMessageIndex = 0;
                        }
                        else
                        {
                            multipleDmAdvertiserMessageIndex++;
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                msg = msg.Replace(" [mtag] ", "");
                msg = msg.Replace(" [mtag]", "");
                msg = msg.Replace("[mtag]", "");

                msg = msg.Replace(" [all] ", "");
                msg = msg.Replace(" [all]", "");
                msg = msg.Replace("[all]", "");

                msg = msg.Replace(" [rtag] ", "");
                msg = msg.Replace(" [rtag]", "");
                msg = msg.Replace("[rtag]", "");

                msg = msg.Replace(" [rall] ", "");
                msg = msg.Replace(" [rall]", "");
                msg = msg.Replace("[rall]", "");
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[lag]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[lag]", Utils.GetLagMessage());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            try
            {
                while (msg.Contains("[rndnum]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndnum]", Utils.GetUniqueInt(4).ToString());
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {
                
            }

            try
            {
                while (msg.Contains("[rndstr]"))
                {
                    try
                    {
                        msg = Utils.ReplaceFirst(msg, "[rndstr]", Utils.GetUniqueKey(16));
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
        catch
        {

        }

        return msg;
    }

    private void siticoneButton8_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Set new nickname for all tokens
        try
        {
            if (!Utils.IsIDValid(gunaLineTextBox21.Text))
            {
                MessageBox.Show("The ID of the guild is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Thread thread = new Thread(DoSetNickName);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoSetNickName()
    {
        try
        {
            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread thread = new Thread(() => SetNickname(client, gunaLineTextBox21.Text, gunaLineTextBox22.Text, GetProxy()));
                    
                    thread.Start();
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

    public void SetNickname(DiscordClient client, string guildId, string nickname, HttpProxyClient proxyClient)
    {
        try
        {
            client.SetNickname(guildId, nickname, proxyClient);
        }
        catch
        {

        }
    }

    private void siticoneButton11_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Set new online status for all tokens
        try
        {
            Thread thread = new Thread(DoSetStatus);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoSetStatus()
    {
        try
        {
            UserStatus status = UserStatus.Online;

            if (siticoneComboBox2.SelectedIndex == 1)
            {
                status = UserStatus.Idle;
            }
            else if (siticoneComboBox2.SelectedIndex == 2)
            {
                status = UserStatus.DoNotDisturb;
            }
            else if (siticoneComboBox2.SelectedIndex == 3)
            {
                status = UserStatus.Invisible;
            }

            foreach (DiscordClient client in this.GetClients())
            {
                try
                {
                    Thread thread = new Thread(() => SetStatus(client, status, GetProxy()));
                    
                    thread.Start();
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

    public void SetStatus(DiscordClient client, UserStatus status, HttpProxyClient proxyClient)
    {
        try
        {
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

            try
            {
                client.ConnectToWebSocket();
            }
            catch
            {

            }

            try
            {
                client.SetStatus(status, proxyClient);
            }
            catch
            {

            }

            try
            {
                client.SendToWS("{\"op\":3,\"d\":{\"status\":\"" + theStatus + "\",\"since\":0,\"activities\":[],\"afk\":false}}");
            }
            catch
            {

            }
        }
        catch
        {

        }
    }

    private void siticoneButton7_Click(object sender, EventArgs e)
    {
        // Reaction Spammer - Fetch from message
        try
        {
            if (!Utils.IsIDValid(gunaLineTextBox9.Text))
            {
                MessageBox.Show("The ID of the channel is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsIDValid(gunaLineTextBox10.Text))
            {
                MessageBox.Show("The ID of the message is not valid! Please, insert a new valid one.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            siticoneButton7.Enabled = false;
            siticoneButton7.Text = "Fetching";
            Thread thread = new Thread(FetchEmote);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void FetchEmote()
    {
        try
        {
            try
            {
                gunaLineTextBox8.Text = clients[0].FetchEmote(gunaLineTextBox9.Text, gunaLineTextBox10.Text, GetProxy());
            }
            catch
            {

            }

            siticoneButton7.Enabled = true;
            siticoneButton7.Text = "Fetch from message";
        }
        catch
        {
            
        }
    }

    private void siticoneButton12_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Generate Text
        try
        {
            Thread thread = new Thread(GenerateText);
            thread.SetApartmentState(ApartmentState.STA);
            
            thread.Start();
        }
        catch
        {

        }
    }

    private void siticoneButton10_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(() => ParseUsers(gunaLineTextBox25.Text, gunaLineTextBox24.Text));
        thread.Start();
    }

    public void ParseUsers(string invite, string channelId)
    {
        try
        {
            if (!Utils.IsIDValid(channelId))
            {
                MessageBox.Show("The ID of the channel that you inserted is not valid! Please, ensure to insert the ID of the channel of this guild that most of the users are visible to you.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DiscordInvite theInvite = Utils.GetInviteInformations(invite, false);

            if (!theInvite.valid)
            {
                MessageBox.Show("The guild invite link / code that you have inserted is not valid! Please, ensure that the invite you inserted is the real invite you want to use and check if you are not rate-limited from the Discord API.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsTokenValid(clients[0].token))
            {
                MessageBox.Show("Failed to parse users! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clients[0].ConnectToWebSocket();
            Utils.lastChannelId = channelId;
            clients[0].ParseGuild(theInvite, null, channelId);
        }
        catch
        {
            MessageBox.Show("Failed to parse users! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void siticoneButton13_Click(object sender, EventArgs e)
    {
        if (!(Utils.users.Count > 0))
        {
            return;
        }

        if (saveFileDialog1.ShowDialog().Equals(DialogResult.OK))
        {
            string preparedList = "";

            foreach (string user in Utils.users)
            {
                if (preparedList == "")
                {
                    preparedList = user;
                }
                else
                {
                    preparedList += Environment.NewLine + user;
                }
            }

            System.IO.File.WriteAllText(saveFileDialog1.FileName, preparedList);
        }
    }

    private void siticoneButton14_Click(object sender, EventArgs e)
    {
        if (openFileDialog3.ShowDialog().Equals(DialogResult.OK))
        {
            try
            {
                string realLine = "";

                foreach (string line in Utils.SplitToLines(System.IO.File.ReadAllText(openFileDialog3.FileName)))
                {
                    realLine = line.Replace(" ", "").Trim().Replace('\t'.ToString(), "");

                    if (Utils.IsIDValid(realLine))
                    {
                        Utils.users.Add(realLine);
                    }
                }
            }
            catch
            {

            }
        }
    }

    private void siticoneButton18_Click(object sender, EventArgs e)
    {
        // Miscellaneous - Phone Lock all loaded tokens
        Thread thread = new Thread(PhoneLockAll);
        thread.Start();
    }

    public void PhoneLockAll()
    {
        foreach (DiscordClient client in clients)
        {
            Thread.Sleep(5);
            Thread thread = new Thread(() => PhoneLock(client));
            thread.Start();
        }
    }

    public void PhoneLock(DiscordClient client)
    {
        client.PhoneLock();
    }

    private void siticoneButton19_Click(object sender, EventArgs e)
    {

    }

    private void siticoneButton20_Click(object sender, EventArgs e)
    {

    }

    private void siticoneButton17_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(() => ParseRoles(gunaLineTextBox27.Text));
        thread.Start();
    }

    public void ParseRoles(string guildId)
    {
        try
        {
            if (!Utils.IsIDValid(guildId))
            {
                MessageBox.Show("The ID of the guild that you have inserted is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsTokenValid(clients[0].token))
            {
                MessageBox.Show("Failed to parse users! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Utils.roles = clients[0].GetGuildRoles(guildId, null);
        }
        catch
        {
            MessageBox.Show("Failed to parse roles! Please, ensure that your tokens are all valid or if your tokens list is empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void siticoneButton16_Click(object sender, EventArgs e)
    {
        if (!(Utils.roles.Count > 0))
        {
            return;
        }

        if (saveFileDialog2.ShowDialog().Equals(DialogResult.OK))
        {
            string preparedList = "";

            foreach (string role in Utils.roles)
            {
                if (preparedList == "")
                {
                    preparedList = role;
                }
                else
                {
                    preparedList += Environment.NewLine + role;
                }
            }

            System.IO.File.WriteAllText(saveFileDialog2.FileName, preparedList);
        }
    }

    private void siticoneButton15_Click(object sender, EventArgs e)
    {
        if (openFileDialog4.ShowDialog().Equals(DialogResult.OK))
        {
            try
            {
                string realLine = "";

                foreach (string line in Utils.SplitToLines(System.IO.File.ReadAllText(openFileDialog4.FileName)))
                {
                    realLine = line.Replace(" ", "").Trim().Replace('\t'.ToString(), "");

                    if (Utils.IsIDValid(realLine))
                    {
                        Utils.roles.Add(realLine);
                    }
                }
            }
            catch
            {

            }
        }
    }

    private void siticoneButton21_Click(object sender, EventArgs e)
    {
        if (openFileDialog5.ShowDialog().Equals(DialogResult.OK))
        {
            pictureBox2.BackgroundImage = System.Drawing.Image.FromFile(openFileDialog5.FileName);
        }
    }

    private void siticoneButton22_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(SetAvatarAll);
        thread.Start();
    }

    public void SetAvatarAll()
    {
        foreach (DiscordClient client in clients)
        {
            try
            {
                Thread.Sleep(5);
                Thread thread = new Thread(() => SetAvatar(client, GetProxy()));
                thread.Start();
            }
            catch
            {

            }
        }
    }

    public void SetAvatar(DiscordClient client, HttpProxyClient proxyClient)
    {
        try
        {
            client.SetAvatar(pictureBox2.BackgroundImage, proxyClient);
        }
        catch
        {

        }
    }

    private void siticoneCheckBox35_CheckedChanged(object sender, EventArgs e)
    {
        if (siticoneCheckBox35.Checked)
        {
            gunaButton10.Enabled = true;
            gunaButton9.Enabled = false;
            gunaButton10.Text = "Start Spamming";
            gunaButton9.Text = "Stop Spamming";
        }
        else
        {
            gunaButton10.Enabled = true;
            gunaButton9.Enabled = true;
            gunaButton10.Text = "Add friend";
            gunaButton9.Text = "Remove friend";
        }
    }

    private void siticoneCheckBox33_CheckedChanged(object sender, EventArgs e)
    {
        if (siticoneCheckBox33.Checked)
        {
            gunaButton1.Text = "Start Spamming";
            gunaButton2.Text = "Stop Spamming";
            gunaButton1.Enabled = true;
            gunaButton2.Enabled = false;
        }
        else
        {
            gunaButton1.Text = "Join guild";
            gunaButton2.Text = "Leave guild";
            gunaButton1.Enabled = true;
            gunaButton2.Enabled = true;
        }
    }

    private void siticoneButton23_Click(object sender, EventArgs e)
    {
        Thread thread = new Thread(ResetAvatarAll);
        thread.Start();
    }

    public void ResetAvatarAll()
    {
        foreach (DiscordClient client in clients)
        {
            Thread.Sleep(5);
            Thread thread = new Thread(() => ResetAvatar(client, GetProxy()));
            thread.Start();
        }
    }

    public void ResetAvatar(DiscordClient client, HttpProxyClient proxyClient)
    {
        client.ResetAvatar(proxyClient);
    }

    public void GenerateText()
    {
        try
        {
            gunaTextBox5.Text = "";
            int placeholders = 0;

            if (gunaLineTextBox13.Text.Length > 8 || !Microsoft.VisualBasic.Information.IsNumeric(gunaLineTextBox13.Text))
            {
                MessageBox.Show("The number of placeholders is not valid! Please, insert a valid number of placeholders to generate.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            placeholders = int.Parse(gunaLineTextBox13.Text);

            if (placeholders <= 0)
            {
                MessageBox.Show("The number of placeholders is not valid! Please, insert a valid number of placeholders to generate.", "AstarothSpammer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string result = "";

            for (int i = 0; i < placeholders; i++)
            {
                if (siticoneCheckBox6.Checked)
                {
                    result += "[rndnum] ";
                }

                if (siticoneCheckBox10.Checked)
                {
                    result += "[rndstr] ";
                }

                if (siticoneCheckBox24.Checked)
                {
                    result += "[mtag] ";
                }

                if (siticoneCheckBox25.Checked)
                {
                    result += "[lag] ";
                }

                if (siticoneCheckBox30.Checked)
                {
                    result += "[all] ";
                }

                if (siticoneCheckBox31.Checked)
                {
                    result += "[rtag] ";
                }

                if (siticoneCheckBox32.Checked)
                {
                    result += "[rall] ";
                }
            }

            gunaTextBox5.Text = result.Substring(0, result.Length - 1);

            try
            {
                if (siticoneCheckBox26.Checked)
                {
                    Clipboard.SetText(gunaTextBox5.Text);
                }
            }
            catch
            {

            }
        }
        catch
        {

        }
    }

    private void siticoneCheckBox4_CheckedChanged(object sender, EventArgs e)
    {
        // Guild Manager - Group Mode
        /*try
        {
            if (siticoneCheckBox4.Checked)
            {
                gunaButton1.Text = "Join group";
                gunaButton2.Text = "Leave group";
                siticoneCheckBox1.Checked = false;
                siticoneCheckBox2.Checked = false;
                siticoneCheckBox3.Checked = false;
            }
            else
            {
                gunaButton1.Text = "Join guild";
                gunaButton2.Text = "Leave guild";
            }
        }
        catch
        {

        }*/
    }

    private void siticoneCheckBox19_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            Utils.globalAutoReconnect = siticoneCheckBox19.Checked;
        }
        catch
        {

        }
    }

    private void siticoneButton9_Click(object sender, EventArgs e)
    {
        // Settings and Utils - Set new HypeSquad for all tokens
        try
        {
            Thread thread = new Thread(DoHypeSquadSetter);
            
            thread.Start();
        }
        catch
        {

        }
    }

    public void DoHypeSquadSetter()
    {
        try
        {
            HypeSquad hypeSquad = HypeSquad.Balance;

            if (siticoneComboBox1.SelectedIndex == 1)
            {
                hypeSquad = HypeSquad.Bravery;
            }
            else if (siticoneComboBox1.SelectedIndex == 2)
            {
                hypeSquad = HypeSquad.Brilliance;
            }

            try
            {
                foreach (DiscordClient client in this.GetClients())
                {
                    Thread.Sleep(5);

                    try
                    {
                        Thread thread = new Thread(() => SetHypeSquad(client, hypeSquad, GetProxy()));
                        
                        thread.Start();
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
        catch
        {

        }
    }

    public void SetHypeSquad(DiscordClient client, HypeSquad hypeSquad, HttpProxyClient proxyClient)
    {
        try
        {
            client.SetHypeSquad(hypeSquad, proxyClient);
        }
        catch
        {

        }
    }

    private void siticoneCheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (siticoneCheckBox4.Checked)
            {
                siticoneCheckBox1.Checked = false;
            }
        }
        catch
        {

        }
    }

    private void siticoneCheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (siticoneCheckBox4.Checked)
            {
                siticoneCheckBox2.Checked = false;
            }
        }
        catch
        {

        }
    }

    private void siticoneCheckBox3_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (siticoneCheckBox4.Checked)
            {
                siticoneCheckBox3.Checked = false;
            }
        }
        catch
        {

        }
    }
}