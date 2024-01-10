namespace eothello
{
    partial class ONielo
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ONielo));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.GameSettingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.NewGameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveGameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadGameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SpeakMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.VirtualMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.InformationPanelMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BackGroundBanner = new System.Windows.Forms.PictureBox();
            this.WhitePicture = new System.Windows.Forms.PictureBox();
            this.BlackPicture = new System.Windows.Forms.PictureBox();
            this.HighlightIndicator1 = new System.Windows.Forms.PictureBox();
            this.HighLightIndicator2 = new System.Windows.Forms.PictureBox();
            this.CounterBlack = new System.Windows.Forms.Label();
            this.CounterWhite = new System.Windows.Forms.Label();
            this.PlayerNameLabel = new System.Windows.Forms.TextBox();
            this.PlayerNameLabel2 = new System.Windows.Forms.TextBox();
            this.saveGameToolStripMenuItem = new System.Windows.Forms.ToolStripDropDownMenu();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BackGroundBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WhitePicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlackPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighlightIndicator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighLightIndicator2)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GameSettingsMenu,
            this.SettingsMenu,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // GameSettingsMenu
            // 
            this.GameSettingsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewGameMenu,
            this.SaveGameMenu,
            this.LoadGameMenu,
            this.ExitMenu});
            this.GameSettingsMenu.Name = "GameSettingsMenu";
            this.GameSettingsMenu.Size = new System.Drawing.Size(50, 20);
            this.GameSettingsMenu.Text = "Game";
            // 
            // NewGameMenu
            // 
            this.NewGameMenu.Name = "NewGameMenu";
            this.NewGameMenu.Size = new System.Drawing.Size(134, 22);
            this.NewGameMenu.Text = "New Game";
            this.NewGameMenu.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click_1);
            // 
            // SaveGameMenu
            // 
            this.SaveGameMenu.Name = "SaveGameMenu";
            this.SaveGameMenu.Size = new System.Drawing.Size(134, 22);
            this.SaveGameMenu.Text = "Save Game";
            this.SaveGameMenu.Click += new System.EventHandler(this.saveGameToolStripMenuItem1_Click);
            // 
            // LoadGameMenu
            // 
            this.LoadGameMenu.Name = "LoadGameMenu";
            this.LoadGameMenu.Size = new System.Drawing.Size(134, 22);
            this.LoadGameMenu.Text = "Load Game";
            // 
            // ExitMenu
            // 
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.Size = new System.Drawing.Size(134, 22);
            this.ExitMenu.Text = "Exit";
            this.ExitMenu.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // SettingsMenu
            // 
            this.SettingsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SpeakMenu,
            this.VirtualMenu,
            this.InformationPanelMenu});
            this.SettingsMenu.Name = "SettingsMenu";
            this.SettingsMenu.Size = new System.Drawing.Size(61, 20);
            this.SettingsMenu.Text = "Settings";
            // 
            // SpeakMenu
            // 
            this.SpeakMenu.Name = "SpeakMenu";
            this.SpeakMenu.Size = new System.Drawing.Size(169, 22);
            this.SpeakMenu.Text = "Speak";
            // 
            // VirtualMenu
            // 
            this.VirtualMenu.Name = "VirtualMenu";
            this.VirtualMenu.Size = new System.Drawing.Size(169, 22);
            this.VirtualMenu.Text = "Virtual Player";
            this.VirtualMenu.Click += new System.EventHandler(this.virtualPlayerToolStripMenuItem_Click);
            // 
            // InformationPanelMenu
            // 
            this.InformationPanelMenu.Checked = true;
            this.InformationPanelMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.InformationPanelMenu.Name = "InformationPanelMenu";
            this.InformationPanelMenu.Size = new System.Drawing.Size(169, 22);
            this.InformationPanelMenu.Text = "Information Panel";
            this.InformationPanelMenu.Click += new System.EventHandler(this.informationPanelToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMeToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutMeToolStripMenuItem
            // 
            this.aboutMeToolStripMenuItem.Name = "aboutMeToolStripMenuItem";
            this.aboutMeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutMeToolStripMenuItem.Text = "About me";
            this.aboutMeToolStripMenuItem.Click += new System.EventHandler(this.aboutMeToolStripMenuItem_Click);
            // 
            // BackGroundBanner
            // 
            this.BackGroundBanner.BackColor = System.Drawing.Color.Green;
            this.BackGroundBanner.Location = new System.Drawing.Point(-186, 665);
            this.BackGroundBanner.Name = "BackGroundBanner";
            this.BackGroundBanner.Size = new System.Drawing.Size(1337, 75);
            this.BackGroundBanner.TabIndex = 1;
            this.BackGroundBanner.TabStop = false;
            // 
            // WhitePicture
            // 
            this.WhitePicture.BackColor = System.Drawing.Color.White;
            this.WhitePicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WhitePicture.Location = new System.Drawing.Point(506, 678);
            this.WhitePicture.Name = "WhitePicture";
            this.WhitePicture.Padding = new System.Windows.Forms.Padding(30);
            this.WhitePicture.Size = new System.Drawing.Size(50, 50);
            this.WhitePicture.TabIndex = 2;
            this.WhitePicture.TabStop = false;
            // 
            // BlackPicture
            // 
            this.BlackPicture.BackColor = System.Drawing.Color.Black;
            this.BlackPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BlackPicture.Location = new System.Drawing.Point(128, 678);
            this.BlackPicture.Name = "BlackPicture";
            this.BlackPicture.Size = new System.Drawing.Size(50, 50);
            this.BlackPicture.TabIndex = 3;
            this.BlackPicture.TabStop = false;
            // 
            // HighlightIndicator1
            // 
            this.HighlightIndicator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.HighlightIndicator1.Location = new System.Drawing.Point(489, 671);
            this.HighlightIndicator1.Name = "HighlightIndicator1";
            this.HighlightIndicator1.Size = new System.Drawing.Size(271, 61);
            this.HighlightIndicator1.TabIndex = 4;
            this.HighlightIndicator1.TabStop = false;
            // 
            // HighLightIndicator2
            // 
            this.HighLightIndicator2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.HighLightIndicator2.Location = new System.Drawing.Point(108, 671);
            this.HighLightIndicator2.Name = "HighLightIndicator2";
            this.HighLightIndicator2.Size = new System.Drawing.Size(283, 61);
            this.HighLightIndicator2.TabIndex = 5;
            this.HighLightIndicator2.TabStop = false;
            // 
            // CounterBlack
            // 
            this.CounterBlack.AutoSize = true;
            this.CounterBlack.BackColor = System.Drawing.Color.Green;
            this.CounterBlack.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CounterBlack.Location = new System.Drawing.Point(24, 674);
            this.CounterBlack.Name = "CounterBlack";
            this.CounterBlack.Size = new System.Drawing.Size(45, 54);
            this.CounterBlack.TabIndex = 6;
            this.CounterBlack.Text = "0";
            // 
            // CounterWhite
            // 
            this.CounterWhite.AutoSize = true;
            this.CounterWhite.BackColor = System.Drawing.Color.Green;
            this.CounterWhite.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CounterWhite.Location = new System.Drawing.Point(412, 674);
            this.CounterWhite.Name = "CounterWhite";
            this.CounterWhite.Size = new System.Drawing.Size(45, 54);
            this.CounterWhite.TabIndex = 7;
            this.CounterWhite.Text = "0";
            // 
            // PlayerNameLabel
            // 
            this.PlayerNameLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PlayerNameLabel.Location = new System.Drawing.Point(184, 678);
            this.PlayerNameLabel.Name = "PlayerNameLabel";
            this.PlayerNameLabel.Size = new System.Drawing.Size(187, 39);
            this.PlayerNameLabel.TabIndex = 8;
            // 
            // PlayerNameLabel2
            // 
            this.PlayerNameLabel2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PlayerNameLabel2.Location = new System.Drawing.Point(562, 678);
            this.PlayerNameLabel2.Name = "PlayerNameLabel2";
            this.PlayerNameLabel2.Size = new System.Drawing.Size(179, 39);
            this.PlayerNameLabel2.TabIndex = 9;
            // 
            // saveGameToolStripMenuItem
            // 
            this.saveGameToolStripMenuItem.AllowDrop = true;
            this.saveGameToolStripMenuItem.AutoClose = false;
            this.saveGameToolStripMenuItem.Name = "saveGameToolStripMenuItem";
            this.saveGameToolStripMenuItem.ShowItemToolTips = false;
            this.saveGameToolStripMenuItem.Size = new System.Drawing.Size(61, 4);
            // 
            // ONielo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(784, 761);
            this.Controls.Add(this.PlayerNameLabel2);
            this.Controls.Add(this.PlayerNameLabel);
            this.Controls.Add(this.BlackPicture);
            this.Controls.Add(this.HighLightIndicator2);
            this.Controls.Add(this.CounterWhite);
            this.Controls.Add(this.CounterBlack);
            this.Controls.Add(this.WhitePicture);
            this.Controls.Add(this.HighlightIndicator1);
            this.Controls.Add(this.BackGroundBanner);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 800);
            this.Name = "ONielo";
            this.Text = "O\'Nielo";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BackGroundBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WhitePicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlackPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighlightIndicator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighLightIndicator2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem GameSettingsMenu;
        private PictureBox BackGroundBanner;
        private PictureBox WhitePicture;
        private PictureBox BlackPicture;
        private PictureBox HighlightIndicator1;
        private PictureBox HighLightIndicator2;
        private Label CounterBlack;
        private Label CounterWhite;
#pragma warning disable CS0169 // The field 'ONielo.label3' is never used
        private Label label3;
#pragma warning restore CS0169 // The field 'ONielo.label3' is never used
#pragma warning disable CS0169 // The field 'ONielo.label4' is never used
        private Label label4;
#pragma warning restore CS0169 // The field 'ONielo.label4' is never used
        private ToolStripMenuItem NewGameMenu;
        private ToolStripMenuItem LoadGameMenu;
        private TextBox PlayerNameLabel;
        private TextBox PlayerNameLabel2;
        private ToolStripDropDownMenu saveGameToolStripMenuItem;
        private ToolStripMenuItem SaveGameMenu;
        private ToolStripMenuItem SettingsMenu;
        private ToolStripMenuItem SpeakMenu;
        private ToolStripMenuItem VirtualMenu;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutMeToolStripMenuItem;
        private ToolStripMenuItem InformationPanelMenu;
        private ToolStripMenuItem ExitMenu;
    }
}