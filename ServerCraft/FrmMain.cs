using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace ServerCraft
{
    public partial class FrmMain : Form
    {
        private string _file;
        private readonly List<string> _serverSettings = new List<string>();

        /// <summary>
        /// Default constructor, just initialise the form, nothing fancy.
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (ReadFile())
                ConfigureSettings();
        }

        /// <summary>
        /// Open a user specified *.properties file and read settings into memory.
        /// </summary>
        /// <returns></returns>
        private bool ReadFile()
        {
            bool result = false;

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = @"Server File (.properties)|*.properties"
            };

            if (ofd.ShowDialog().Equals(DialogResult.OK))
            {
                _file = ofd.FileName;
                lblSource.Caption = string.Format("File: {0}", _file);
                _serverSettings.AddRange(File.ReadAllLines(_file));
                _serverSettings.RemoveAt(1); // Get rid of the Timestamp. The whole thing needs to be changed when saving anyway.
                result = true;
            }
            else
            {
                MessageBox.Show(@"Unable to open file.");
            }

            return result;
        }

        /// <summary>
        /// Grab any settings present in the current file and reflect them in the form.
        /// If setting is not present, use the default value as per http://minecraft.gamepedia.com/Server.properties
        /// </summary>
        private void ConfigureSettings()
        {
            chkFlight.Checked = GetBooleanValue("allow-flight", false);
            chkAnnounce.Checked = GetBooleanValue("announce-player-achievements", true);
            cbDifficulty.SelectedIndex = GetIntValue("difficulty", 1);
            chkQuery.Checked = GetBooleanValue("enable-query", false);
            chkRemote.Checked = GetBooleanValue("enable-rcon", false);
            chkCommand.Checked = GetBooleanValue("enable-command-block", false);
            chkForceGameMode.Checked = GetBooleanValue("force-gamemode", false);
            cbGameMode.SelectedIndex = GetIntValue("gamemode", 0);
            chkStructures.Checked = GetBooleanValue("generate-structures", true);
            chkHardcore.Checked = GetBooleanValue("hardcore", false);
            txtName.Text = GetStringValue("level-name", "world");
            txtSeed.Text = GetStringValue("level-seed", string.Empty);
            txtType.Text = GetStringValue("level-type", "DEFAULT");
            spinBuildHeight.Value = GetIntValue("max-build-height", 256);
            spinPlayers.Value = GetIntValue("max-players", 20);
            spinTickTime.Value = GetIntValue("max-tick-time", 60000);
            spinWorldSize.Value = GetIntValue("max-world-size", 29999984);
            txtMessage.Text = GetStringValue("motd", "A Minecraft Server");
            spinNetwork.Value = GetIntValue("network-compression-threshold", 256);
            chkOnline.Checked = GetBooleanValue("online-mode", true);
            spinPermission.Value = GetIntValue("op-permission-level", 4);
            spinTimeout.Value = GetIntValue("player-idle-timeout", 0);
            chkPvP.Checked = GetBooleanValue("pvp", true);
            txtResource.Text = GetStringValue("resource-pack", string.Empty);
            txtResourceHash.Text = GetStringValue("resource-pack-hash", string.Empty);
            txtServerIP.Text = GetStringValue("server-ip", string.Empty);
            spinServerPort.Value = GetIntValue("server-port", 25565);
            chkSnooper.Checked = GetBooleanValue("snooper-enabled", true);
            chkAnimals.Checked = GetBooleanValue("spawn-animals", true);
            chkMonsters.Checked = GetBooleanValue("spawn-monsters", true);
            chkNPCs.Checked = GetBooleanValue("spawn-npcs", true);
            spinView.Value = GetIntValue("view-distance", 10);
            chkWhitelist.Checked = GetBooleanValue("white-list", false);
            chkNether.Checked = GetBooleanValue("allow-nether", true);
            spinQueryPort.Value = GetIntValue("query.port", 25565);
            txtRemotePassword.Text = GetStringValue("rcon.password", string.Empty);
            spinRemotePort.Value = GetIntValue("rcon.port", 25575);
            spinSpawnProtection.Value = GetIntValue("spawn-protection", 16);
            chkNativeTransport.Checked = GetBooleanValue("use-native-transport", true);
        }

        private bool GetBooleanValue(string key, bool defaultValue)
        {
            bool result = defaultValue;

            string value = _serverSettings.Find(s => s.StartsWith(key));
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Substring(value.IndexOf("=", StringComparison.Ordinal) + 1);
                result = Convert.ToBoolean(value);
            }
            return result;
        }

        private int GetIntValue(string key, int defaultValue)
        {
            int result = defaultValue;

            string value = _serverSettings.Find(s => s.StartsWith(key));
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Substring(value.IndexOf("=", StringComparison.Ordinal) + 1);
                result = Convert.ToInt32(value);
            }
            return result;
        }

        private string GetStringValue(string key, string defaultValue)
        {
            string result = defaultValue;

            string value = _serverSettings.Find(s => s.StartsWith(key));
            if (!string.IsNullOrEmpty(value))
                result = value.Substring(value.IndexOf("=", StringComparison.Ordinal) + 1);
            return result;
        }

        /// <summary>
        /// Change a boolean in the server settings.
        /// </summary>
        /// <param name="matchString"></param>
        /// <param name="checkBox"></param>
        private void ChangeBoolean(string matchString, CheckEdit checkBox)
        {
            // Doesn't matter if there's any, just get rid of them.
            _serverSettings.RemoveAll(s => s.StartsWith(matchString));

            // It's important to use .ToLower() or the boolean won't be recognised.
            _serverSettings.Add(string.Format("{0}={1}", matchString, checkBox.Checked.ToString().ToLower()));
        }

        /// <summary>
        /// Change a string in the server settings.
        /// </summary>
        /// <param name="matchString"></param>
        /// <param name="textBox"></param>
        private void ChangeString(string matchString, Control textBox)
        {
            // Get rid of any matches. There's probably more elegant ways to do this, but simple is good.
            _serverSettings.RemoveAll(s => s.StartsWith(matchString));
            _serverSettings.Add(string.Format("{0}={1}", matchString, textBox.Text));
        }

        /// <summary>
        /// Change a number in the server settings.
        /// </summary>
        /// <param name="matchString"></param>
        /// <param name="spin"></param>
        private void ChangeInt(string matchString, SpinEdit spin)
        {
            _serverSettings.RemoveAll(s => s.StartsWith(matchString));
            _serverSettings.Add(string.Format("{0}={1}", matchString, spin.Value));
        }

        #region Event Handlers
        private void chkFlight_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("allow-flight", chkFlight);
        }

        private void chkNether_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("allow-nether", chkNether);
        }

        private void cbDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            _serverSettings.RemoveAll(s => s.StartsWith("difficulty"));
            string difficulty;
            switch (cbDifficulty.SelectedText)
            {
                case "Peaceful":
                    difficulty = "0";
                    break;
                case "Easy":
                    difficulty = "1";
                    break;
                case "Normal":
                    difficulty = "2";
                    break;
                case "Hard":
                    difficulty = "3";
                    break;
                default:
                    difficulty = "1";
                    break;
            }
            _serverSettings.Add(string.Format("difficulty={0}", difficulty));
        }

        private void btnDefault_ItemClick(object sender, ItemClickEventArgs e)
        {
            chkFlight.Checked = false;
            chkAnnounce.Checked = true;
            cbDifficulty.SelectedIndex = 1;
            chkQuery.Checked = false;
            chkRemote.Checked = false;
            chkCommand.Checked = false;
            chkForceGameMode.Checked = false;
            cbGameMode.SelectedIndex = 0;
            chkStructures.Checked = true;
            chkHardcore.Checked = false;
            txtName.Text = @"world";
            txtSeed.Text = string.Empty;
            txtType.Text = @"DEFAULT";
            spinBuildHeight.Value = 256;
            spinPlayers.Value = 20;
            spinTickTime.Value = 60000;
            spinWorldSize.Value = 29999984;
            txtMessage.Text = @"A Minecraft Server";
            spinNetwork.Value = 256;
            chkOnline.Checked = true;
            spinPermission.Value = 4;
            spinTimeout.Value = 0;
            chkPvP.Checked = true;
            txtResource.Text = string.Empty;
            txtResourceHash.Text = string.Empty;
            txtServerIP.Text = string.Empty;
            spinServerPort.Value = 25565;
            chkSnooper.Checked = true;
            chkAnimals.Checked = true;
            chkMonsters.Checked = true;
            chkNPCs.Checked = true;
            spinView.Value = 10;
            chkWhitelist.Checked = false;
            chkNether.Checked = true;
            spinQueryPort.Value = 25565;
            txtRemotePassword.Text = string.Empty;
            spinRemotePort.Value = 25575;
            spinSpawnProtection.Value = 16;
            chkNativeTransport.Checked = true;
        }

        private void btnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            // The timezone part isn't perfect, but as far as I know, this is just a comment and doesn't really mean much.
            _serverSettings.Insert(1,
                string.Format("#{0} {1} {2}", DateTime.Now.ToString("ddd MMM dd HH:mm:ss"), TimeZoneInfo.Local.Id,
                    DateTime.Now.Year));

            if (File.Exists(_file)) // Just delete it and re-create it. Easier than overwriting.
                File.Delete(_file);

            File.WriteAllLines(_file, _serverSettings);
            MessageBox.Show(@"Complete!");
        }

        private void chkAnnounce_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("announce-player-achievements", chkAnnounce);
        }

        private void chkCommand_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("enable-command-block", chkCommand);
        }

        private void chkQuery_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("enable-query", chkQuery);
            spinQueryPort.Enabled = chkQuery.Checked;
        }

        private void chkRemote_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("enable-rcon", chkRemote);
            txtRemotePassword.Enabled = chkRemote.Checked;
            spinRemotePort.Enabled = chkRemote.Checked;
        }

        private void chkForceGameMode_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("force-gamemode", chkForceGameMode);
        }

        private void cbGameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            _serverSettings.RemoveAll(s => s.StartsWith("gamemode"));
            string gamemode;
            switch (cbGameMode.SelectedText)
            {
                case "Survival":
                    gamemode = "0";
                    break;
                case "Creative":
                    gamemode = "1";
                    break;
                case "Adventure":
                    gamemode = "2";
                    break;
                case "Spectator":
                    gamemode = "3";
                    break;
                default:
                    gamemode = "0";
                    break;
            }
            _serverSettings.Add(string.Format("gamemode={0}", gamemode));
        }

        private void chkStructures_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("generate-structures", chkStructures);
        }

        private void chkHardcore_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("hardcore", chkHardcore);
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            ChangeString("level-name", txtName);
        }

        private void txtSeed_TextChanged(object sender, EventArgs e)
        {
            ChangeString("level-seed", txtSeed);
        }

        private void txtType_TextChanged(object sender, EventArgs e)
        {
            ChangeString("level-type", txtType);
        }

        private void spinBuildHeight_ValueChanged(object sender, EventArgs e)
        {
            ChangeInt("max-build-height", spinBuildHeight);
        }

        private void spinPlayers_ValueChanged(object sender, EventArgs e)
        {
            ChangeInt("max-players", spinPlayers);
        }

        private void spinTickTime_ValueChanged(object sender, EventArgs e)
        {
            ChangeInt("max-tick-time", spinTickTime);
        }

        private void spinQueryPort_ValueChanged(object sender, EventArgs e)
        {
            ChangeInt("query.port", spinQueryPort);
        }

        private void txtRemotePassword_TextChanged(object sender, EventArgs e)
        {
            ChangeString("rcon.password", txtRemotePassword);
        }

        private void spinRemotePort_ValueChanged(object sender, EventArgs e)
        {
            ChangeInt("rcon.port", spinRemotePort);
        }

        private void spinSpawnProtection_ValueChanged(object sender, EventArgs e)
        {
            ChangeInt("spawn-protection", spinSpawnProtection);
        }

        private void chkNativeTransport_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("user-native-transport", chkNativeTransport);
        }
        #endregion Event Handlers
    }
}