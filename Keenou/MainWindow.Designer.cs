namespace Keenou
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.g_homeDirectory = new System.Windows.Forms.GroupBox();
            this.b_setVolumeSize = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.l_volumeSize = new System.Windows.Forms.Label();
            this.t_volumeSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.b_encrypt = new System.Windows.Forms.Button();
            this.g_advanced = new System.Windows.Forms.GroupBox();
            this.l_volumeLocWarn = new System.Windows.Forms.Label();
            this.b_volumeLoc = new System.Windows.Forms.Button();
            this.l_volumeLoc = new System.Windows.Forms.Label();
            this.t_volumeLoc = new System.Windows.Forms.TextBox();
            this.l_hash = new System.Windows.Forms.Label();
            this.c_cipher = new System.Windows.Forms.ComboBox();
            this.l_cipher = new System.Windows.Forms.Label();
            this.c_hash = new System.Windows.Forms.ComboBox();
            this.l_sid = new System.Windows.Forms.Label();
            this.l_userName = new System.Windows.Forms.Label();
            this.t_sid = new System.Windows.Forms.TextBox();
            this.t_userName = new System.Windows.Forms.TextBox();
            this.l_passwordConf = new System.Windows.Forms.Label();
            this.t_passwordConf = new System.Windows.Forms.TextBox();
            this.l_password = new System.Windows.Forms.Label();
            this.t_password = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.l_statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.s_progress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.l_homeAlreadyEncrypted = new System.Windows.Forms.Label();
            this.g_homeDirectory.SuspendLayout();
            this.g_advanced.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // g_homeDirectory
            // 
            this.g_homeDirectory.Controls.Add(this.b_setVolumeSize);
            this.g_homeDirectory.Controls.Add(this.label2);
            this.g_homeDirectory.Controls.Add(this.l_volumeSize);
            this.g_homeDirectory.Controls.Add(this.t_volumeSize);
            this.g_homeDirectory.Controls.Add(this.label1);
            this.g_homeDirectory.Controls.Add(this.b_encrypt);
            this.g_homeDirectory.Controls.Add(this.g_advanced);
            this.g_homeDirectory.Controls.Add(this.l_sid);
            this.g_homeDirectory.Controls.Add(this.l_userName);
            this.g_homeDirectory.Controls.Add(this.t_sid);
            this.g_homeDirectory.Controls.Add(this.t_userName);
            this.g_homeDirectory.Controls.Add(this.l_passwordConf);
            this.g_homeDirectory.Controls.Add(this.t_passwordConf);
            this.g_homeDirectory.Controls.Add(this.l_password);
            this.g_homeDirectory.Controls.Add(this.t_password);
            this.g_homeDirectory.Location = new System.Drawing.Point(23, 25);
            this.g_homeDirectory.Name = "g_homeDirectory";
            this.g_homeDirectory.Size = new System.Drawing.Size(497, 353);
            this.g_homeDirectory.TabIndex = 0;
            this.g_homeDirectory.TabStop = false;
            this.g_homeDirectory.Text = "Home Directory";
            // 
            // b_setVolumeSize
            // 
            this.b_setVolumeSize.Location = new System.Drawing.Point(403, 172);
            this.b_setVolumeSize.Name = "b_setVolumeSize";
            this.b_setVolumeSize.Size = new System.Drawing.Size(75, 23);
            this.b_setVolumeSize.TabIndex = 14;
            this.b_setVolumeSize.Text = "Estimate";
            this.b_setVolumeSize.UseVisualStyleBackColor = true;
            this.b_setVolumeSize.Click += new System.EventHandler(this.b_setVolumeSize_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Help;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(40, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(438, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Warning: Ensure the volume size is big enough to fit your home folder now and in " +
    "the future!";
            // 
            // l_volumeSize
            // 
            this.l_volumeSize.AutoSize = true;
            this.l_volumeSize.Location = new System.Drawing.Point(197, 177);
            this.l_volumeSize.Name = "l_volumeSize";
            this.l_volumeSize.Size = new System.Drawing.Size(93, 13);
            this.l_volumeSize.TabIndex = 12;
            this.l_volumeSize.Text = "Volume Size (MB):";
            // 
            // t_volumeSize
            // 
            this.t_volumeSize.Location = new System.Drawing.Point(295, 174);
            this.t_volumeSize.Name = "t_volumeSize";
            this.t_volumeSize.Size = new System.Drawing.Size(100, 20);
            this.t_volumeSize.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Help;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(62, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(399, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Warning: This password MUST be the same as your local Windows user password!";
            // 
            // b_encrypt
            // 
            this.b_encrypt.Location = new System.Drawing.Point(416, 324);
            this.b_encrypt.Name = "b_encrypt";
            this.b_encrypt.Size = new System.Drawing.Size(75, 23);
            this.b_encrypt.TabIndex = 10;
            this.b_encrypt.Text = "Encrypt";
            this.b_encrypt.UseVisualStyleBackColor = true;
            this.b_encrypt.Click += new System.EventHandler(this.b_encrypt_Click);
            // 
            // g_advanced
            // 
            this.g_advanced.Controls.Add(this.l_volumeLocWarn);
            this.g_advanced.Controls.Add(this.b_volumeLoc);
            this.g_advanced.Controls.Add(this.l_volumeLoc);
            this.g_advanced.Controls.Add(this.t_volumeLoc);
            this.g_advanced.Controls.Add(this.l_hash);
            this.g_advanced.Controls.Add(this.c_cipher);
            this.g_advanced.Controls.Add(this.l_cipher);
            this.g_advanced.Controls.Add(this.c_hash);
            this.g_advanced.Location = new System.Drawing.Point(13, 200);
            this.g_advanced.Name = "g_advanced";
            this.g_advanced.Size = new System.Drawing.Size(465, 109);
            this.g_advanced.TabIndex = 9;
            this.g_advanced.TabStop = false;
            this.g_advanced.Text = "Advanced Options";
            // 
            // l_volumeLocWarn
            // 
            this.l_volumeLocWarn.AutoSize = true;
            this.l_volumeLocWarn.Cursor = System.Windows.Forms.Cursors.Help;
            this.l_volumeLocWarn.ForeColor = System.Drawing.Color.Red;
            this.l_volumeLocWarn.Location = new System.Drawing.Point(6, 22);
            this.l_volumeLocWarn.Name = "l_volumeLocWarn";
            this.l_volumeLocWarn.Size = new System.Drawing.Size(450, 13);
            this.l_volumeLocWarn.TabIndex = 7;
            this.l_volumeLocWarn.Text = "Warning: The volume location cannot be located in an encrypted partition or your " +
    "home folder!";
            // 
            // b_volumeLoc
            // 
            this.b_volumeLoc.Location = new System.Drawing.Point(373, 43);
            this.b_volumeLoc.Name = "b_volumeLoc";
            this.b_volumeLoc.Size = new System.Drawing.Size(75, 23);
            this.b_volumeLoc.TabIndex = 6;
            this.b_volumeLoc.Text = "Choose";
            this.b_volumeLoc.UseVisualStyleBackColor = true;
            this.b_volumeLoc.Click += new System.EventHandler(this.b_volumeLoc_Click);
            // 
            // l_volumeLoc
            // 
            this.l_volumeLoc.AutoSize = true;
            this.l_volumeLoc.Location = new System.Drawing.Point(8, 48);
            this.l_volumeLoc.Name = "l_volumeLoc";
            this.l_volumeLoc.Size = new System.Drawing.Size(85, 13);
            this.l_volumeLoc.TabIndex = 5;
            this.l_volumeLoc.Text = "Volume location:";
            // 
            // t_volumeLoc
            // 
            this.t_volumeLoc.Location = new System.Drawing.Point(99, 45);
            this.t_volumeLoc.Name = "t_volumeLoc";
            this.t_volumeLoc.Size = new System.Drawing.Size(268, 20);
            this.t_volumeLoc.TabIndex = 4;
            // 
            // l_hash
            // 
            this.l_hash.AutoSize = true;
            this.l_hash.Location = new System.Drawing.Point(58, 78);
            this.l_hash.Name = "l_hash";
            this.l_hash.Size = new System.Drawing.Size(35, 13);
            this.l_hash.TabIndex = 3;
            this.l_hash.Text = "Hash:";
            // 
            // c_cipher
            // 
            this.c_cipher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_cipher.Items.AddRange(new object[] {
            "AES",
            "Serpent",
            "Twofish",
            "AES(Twofish) ",
            "AES(Twofish(Serpent)) ",
            "Serpent(AES) ",
            "Serpent(Twofish(AES)) ",
            "Twofish(Serpent) "});
            this.c_cipher.Location = new System.Drawing.Point(293, 75);
            this.c_cipher.Name = "c_cipher";
            this.c_cipher.Size = new System.Drawing.Size(121, 21);
            this.c_cipher.TabIndex = 0;
            // 
            // l_cipher
            // 
            this.l_cipher.AutoSize = true;
            this.l_cipher.Location = new System.Drawing.Point(247, 78);
            this.l_cipher.Name = "l_cipher";
            this.l_cipher.Size = new System.Drawing.Size(40, 13);
            this.l_cipher.TabIndex = 1;
            this.l_cipher.Text = "Cipher:";
            // 
            // c_hash
            // 
            this.c_hash.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_hash.Items.AddRange(new object[] {
            "sha256",
            "sha512",
            "whirlpool",
            "ripemd160"});
            this.c_hash.Location = new System.Drawing.Point(99, 75);
            this.c_hash.Name = "c_hash";
            this.c_hash.Size = new System.Drawing.Size(121, 21);
            this.c_hash.TabIndex = 2;
            // 
            // l_sid
            // 
            this.l_sid.AutoSize = true;
            this.l_sid.Location = new System.Drawing.Point(171, 36);
            this.l_sid.Name = "l_sid";
            this.l_sid.Size = new System.Drawing.Size(28, 13);
            this.l_sid.TabIndex = 7;
            this.l_sid.Text = "SID:";
            // 
            // l_userName
            // 
            this.l_userName.AutoSize = true;
            this.l_userName.Location = new System.Drawing.Point(10, 36);
            this.l_userName.Name = "l_userName";
            this.l_userName.Size = new System.Drawing.Size(58, 13);
            this.l_userName.TabIndex = 6;
            this.l_userName.Text = "Username:";
            // 
            // t_sid
            // 
            this.t_sid.Location = new System.Drawing.Point(205, 33);
            this.t_sid.Name = "t_sid";
            this.t_sid.ReadOnly = true;
            this.t_sid.Size = new System.Drawing.Size(273, 20);
            this.t_sid.TabIndex = 5;
            // 
            // t_userName
            // 
            this.t_userName.Location = new System.Drawing.Point(74, 33);
            this.t_userName.Name = "t_userName";
            this.t_userName.ReadOnly = true;
            this.t_userName.Size = new System.Drawing.Size(91, 20);
            this.t_userName.TabIndex = 4;
            // 
            // l_passwordConf
            // 
            this.l_passwordConf.AutoSize = true;
            this.l_passwordConf.Location = new System.Drawing.Point(190, 120);
            this.l_passwordConf.Name = "l_passwordConf";
            this.l_passwordConf.Size = new System.Drawing.Size(99, 13);
            this.l_passwordConf.TabIndex = 3;
            this.l_passwordConf.Text = "Password (confirm):";
            // 
            // t_passwordConf
            // 
            this.t_passwordConf.Location = new System.Drawing.Point(295, 117);
            this.t_passwordConf.Name = "t_passwordConf";
            this.t_passwordConf.Size = new System.Drawing.Size(183, 20);
            this.t_passwordConf.TabIndex = 2;
            this.t_passwordConf.UseSystemPasswordChar = true;
            // 
            // l_password
            // 
            this.l_password.AutoSize = true;
            this.l_password.Location = new System.Drawing.Point(233, 94);
            this.l_password.Name = "l_password";
            this.l_password.Size = new System.Drawing.Size(56, 13);
            this.l_password.TabIndex = 1;
            this.l_password.Text = "Password:";
            // 
            // t_password
            // 
            this.t_password.Location = new System.Drawing.Point(295, 91);
            this.t_password.Name = "t_password";
            this.t_password.Size = new System.Drawing.Size(183, 20);
            this.t_password.TabIndex = 0;
            this.t_password.UseSystemPasswordChar = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.l_statusLabel,
            this.toolStripStatusLabel1,
            this.s_progress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 402);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(542, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // l_statusLabel
            // 
            this.l_statusLabel.Name = "l_statusLabel";
            this.l_statusLabel.Size = new System.Drawing.Size(39, 17);
            this.l_statusLabel.Text = "Status";
            // 
            // s_progress
            // 
            this.s_progress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.s_progress.Name = "s_progress";
            this.s_progress.Size = new System.Drawing.Size(100, 16);
            this.s_progress.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(488, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // l_homeAlreadyEncrypted
            // 
            this.l_homeAlreadyEncrypted.AutoSize = true;
            this.l_homeAlreadyEncrypted.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.l_homeAlreadyEncrypted.Location = new System.Drawing.Point(311, 38);
            this.l_homeAlreadyEncrypted.Name = "l_homeAlreadyEncrypted";
            this.l_homeAlreadyEncrypted.Size = new System.Drawing.Size(201, 13);
            this.l_homeAlreadyEncrypted.TabIndex = 15;
            this.l_homeAlreadyEncrypted.Text = "Your home directory is already encrypted!";
            this.l_homeAlreadyEncrypted.Visible = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(542, 424);
            this.Controls.Add(this.l_homeAlreadyEncrypted);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.g_homeDirectory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainWindow";
            this.Text = "Keenou";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.g_homeDirectory.ResumeLayout(false);
            this.g_homeDirectory.PerformLayout();
            this.g_advanced.ResumeLayout(false);
            this.g_advanced.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label l_passwordConf;
        private System.Windows.Forms.Label l_password;
        private System.Windows.Forms.Label l_sid;
        private System.Windows.Forms.Label l_userName;
        protected System.Windows.Forms.TextBox t_password;
        protected System.Windows.Forms.TextBox t_passwordConf;
        public System.Windows.Forms.TextBox t_userName;
        public System.Windows.Forms.TextBox t_sid;
        private System.Windows.Forms.Label l_cipher;
        public System.Windows.Forms.ComboBox c_cipher;
        private System.Windows.Forms.Label l_hash;
        public System.Windows.Forms.ComboBox c_hash;
        public System.Windows.Forms.GroupBox g_advanced;
        private System.Windows.Forms.Button b_encrypt;
        public System.Windows.Forms.Button b_volumeLoc;
        private System.Windows.Forms.Label l_volumeLoc;
        public System.Windows.Forms.TextBox t_volumeLoc;
        private System.Windows.Forms.Label l_volumeLocWarn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label l_volumeSize;
        public System.Windows.Forms.TextBox t_volumeSize;
        private System.Windows.Forms.Button b_setVolumeSize;
        public System.Windows.Forms.GroupBox g_homeDirectory;
        public System.Windows.Forms.ToolStripStatusLabel l_statusLabel;
        public System.Windows.Forms.ToolStripProgressBar s_progress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label l_homeAlreadyEncrypted;
    }
}

