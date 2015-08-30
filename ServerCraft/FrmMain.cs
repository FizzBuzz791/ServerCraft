using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace MC_Server_Config
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

            BringToFront();
        }

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
        /// </summary>
        private void ConfigureSettings()
        {
            string flight = _serverSettings.Find(s => s.StartsWith("allow-flight"));
            if (!string.IsNullOrEmpty(flight))
            {
                flight = flight.Substring(flight.IndexOf("=", StringComparison.Ordinal) + 1);
                chkFlight.Checked = Convert.ToBoolean(flight);
            }

            // TODO: All the other settings
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

        #region Event Handlers
        private void btnSave_Click(object sender, EventArgs e)
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

        private void chkFlight_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("allow-flight", chkFlight);
        }

        private void chkNether_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("allow-nether", chkNether);
        }

        private void chkAnnounce_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("announce-player-achievments", chkAnnounce);
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

        private void chkQuery_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("enable-query", chkQuery);
            spQueryPort.Enabled = chkQuery.Checked;
        }

        private void chkRemote_CheckedChanged(object sender, EventArgs e)
        {
            ChangeBoolean("enable-rcon", chkRemote);
            txtRemotePassword.Enabled = chkRemote.Checked;
            spRemotePort.Enabled = chkRemote.Checked;
        }

        private void btnDefault_ItemClick(object sender, ItemClickEventArgs e)
        {
            chkFlight.Checked = false;
            chkNether.Checked = true;
            chkAnnounce.Checked = true;
            cbDifficulty.SelectedIndex = 1;
            chkQuery.Checked = false;
            chkRemote.Checked = false;
            chkCommandBlocks.Checked = false;
            chkGameMode.Checked = false;
            cbGameMode.SelectedIndex = 0;
            chkStructures.Checked = true;
            chkHardcore.Checked = false;
            txtLevelName.Text = @"world";
            txtLevelSeed.Text = string.Empty;
            txtLevelType.Text = @"DEFAULT";
            spMaxBuildHeight.Value = 256;
            spMaxPlayers.Value = 20;
            spMaxTickTime.Value = 60000;
            spMaxWorldSize.Value = 29999984;
            txtMessage.Text = @"A Minecraft Server";
            spNetwork.Value = 256;
            chkOnline.Checked = true;
            spPermission.Value = 4;
            spIdleTimeout.Value = 0;
            chkPvP.Checked = true;
            spQueryPort.Value = 25565;
            txtRemotePassword.Text = string.Empty;
            spRemotePort.Value = 25575;
            txtResource.Text = string.Empty;
            txtResourceHash.Text = string.Empty;
            txtServerIP.Text = string.Empty;
            spServerPort.Value = 25565;
            chkSnooper.Checked = true;
            chkAnimals.Checked = true;
            chkMonsters.Checked = true;
            chkNPCs.Checked = true;
            spProtection.Value = 16;
            chkNative.Checked = true;
            spView.Value = 10;
            chkWhiteList.Checked = true;
        }
        #endregion Event Handlers
    }
}