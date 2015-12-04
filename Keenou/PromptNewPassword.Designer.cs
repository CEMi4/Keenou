namespace Keenou
{
    partial class PromptNewPassword
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
            this.t_cloudPW = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.t_cloudPWConf = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.b_submitOK = new System.Windows.Forms.Button();
            this.b_submitCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // t_cloudPW
            // 
            this.t_cloudPW.BackColor = System.Drawing.Color.AliceBlue;
            this.t_cloudPW.Location = new System.Drawing.Point(118, 13);
            this.t_cloudPW.Name = "t_cloudPW";
            this.t_cloudPW.Size = new System.Drawing.Size(183, 20);
            this.t_cloudPW.TabIndex = 0;
            this.t_cloudPW.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Password:";
            // 
            // t_cloudPWConf
            // 
            this.t_cloudPWConf.BackColor = System.Drawing.Color.AliceBlue;
            this.t_cloudPWConf.Location = new System.Drawing.Point(118, 39);
            this.t_cloudPWConf.Name = "t_cloudPWConf";
            this.t_cloudPWConf.Size = new System.Drawing.Size(183, 20);
            this.t_cloudPWConf.TabIndex = 1;
            this.t_cloudPWConf.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Password (confirm):";
            // 
            // b_submitOK
            // 
            this.b_submitOK.BackColor = System.Drawing.Color.SteelBlue;
            this.b_submitOK.FlatAppearance.BorderColor = System.Drawing.Color.RoyalBlue;
            this.b_submitOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.b_submitOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SteelBlue;
            this.b_submitOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_submitOK.ForeColor = System.Drawing.Color.White;
            this.b_submitOK.Location = new System.Drawing.Point(25, 68);
            this.b_submitOK.Name = "b_submitOK";
            this.b_submitOK.Size = new System.Drawing.Size(123, 23);
            this.b_submitOK.TabIndex = 2;
            this.b_submitOK.Text = "OK";
            this.b_submitOK.UseVisualStyleBackColor = false;
            this.b_submitOK.Click += new System.EventHandler(this.b_encryptCloud_Click);
            // 
            // b_submitCancel
            // 
            this.b_submitCancel.BackColor = System.Drawing.Color.SteelBlue;
            this.b_submitCancel.FlatAppearance.BorderColor = System.Drawing.Color.RoyalBlue;
            this.b_submitCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.b_submitCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SteelBlue;
            this.b_submitCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_submitCancel.ForeColor = System.Drawing.Color.White;
            this.b_submitCancel.Location = new System.Drawing.Point(165, 68);
            this.b_submitCancel.Name = "b_submitCancel";
            this.b_submitCancel.Size = new System.Drawing.Size(123, 23);
            this.b_submitCancel.TabIndex = 3;
            this.b_submitCancel.Text = "Cancel";
            this.b_submitCancel.UseVisualStyleBackColor = false;
            this.b_submitCancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // PromptNewPassword
            // 
            this.AcceptButton = this.b_submitOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 104);
            this.Controls.Add(this.b_submitCancel);
            this.Controls.Add(this.t_cloudPW);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.t_cloudPWConf);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.b_submitOK);
            this.Name = "PromptNewPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please choose a password";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.TextBox t_cloudPW;
        private System.Windows.Forms.Label label3;
        protected System.Windows.Forms.TextBox t_cloudPWConf;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button b_submitOK;
        private System.Windows.Forms.Button b_submitCancel;
    }
}