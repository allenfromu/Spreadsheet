namespace SS
{
    partial class ConnectDialog
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
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.serverBox = new System.Windows.Forms.TextBox();
            this.spreadsheetLabel = new System.Windows.Forms.Label();
            this.spreadsheetBox = new System.Windows.Forms.TextBox();
            this.submitButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.portLabel = new System.Windows.Forms.Label();
            this.portBox = new System.Windows.Forms.TextBox();
            this.connectionFailed = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(35, 19);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(55, 13);
            this.usernameLabel.TabIndex = 0;
            this.usernameLabel.Text = "Username";
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(96, 12);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(100, 20);
            this.usernameBox.TabIndex = 1;
            this.usernameBox.Text = "Jack";
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(39, 45);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(51, 13);
            this.serverLabel.TabIndex = 2;
            this.serverLabel.Text = "Server IP";
            // 
            // serverBox
            // 
            this.serverBox.Location = new System.Drawing.Point(96, 38);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(100, 20);
            this.serverBox.TabIndex = 2;
            this.serverBox.Text = "lab1-37.eng.utah.edu";
            this.serverBox.TextChanged += new System.EventHandler(this.serverBox_TextChanged);
            // 
            // spreadsheetLabel
            // 
            this.spreadsheetLabel.AutoSize = true;
            this.spreadsheetLabel.Location = new System.Drawing.Point(23, 71);
            this.spreadsheetLabel.Name = "spreadsheetLabel";
            this.spreadsheetLabel.Size = new System.Drawing.Size(67, 13);
            this.spreadsheetLabel.TabIndex = 4;
            this.spreadsheetLabel.Text = "Spreadsheet";
            // 
            // spreadsheetBox
            // 
            this.spreadsheetBox.Location = new System.Drawing.Point(96, 64);
            this.spreadsheetBox.Name = "spreadsheetBox";
            this.spreadsheetBox.Size = new System.Drawing.Size(100, 20);
            this.spreadsheetBox.TabIndex = 3;
            this.spreadsheetBox.Text = "jackjack";
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(38, 147);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 5;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(120, 147);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(55, 97);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(26, 13);
            this.portLabel.TabIndex = 8;
            this.portLabel.Text = "Port";
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(96, 90);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(100, 20);
            this.portBox.TabIndex = 4;
            this.portBox.Text = "2115";
            // 
            // connectionFailed
            // 
            this.connectionFailed.AutoSize = true;
            this.connectionFailed.ForeColor = System.Drawing.Color.Red;
            this.connectionFailed.Location = new System.Drawing.Point(93, 122);
            this.connectionFailed.Name = "connectionFailed";
            this.connectionFailed.Size = new System.Drawing.Size(89, 13);
            this.connectionFailed.TabIndex = 9;
            this.connectionFailed.Text = "Connection failed";
            this.connectionFailed.Visible = false;
            // 
            // ConnectDialog
            // 
            this.AcceptButton = this.submitButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(235, 182);
            this.Controls.Add(this.connectionFailed);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.spreadsheetBox);
            this.Controls.Add(this.spreadsheetLabel);
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.serverLabel);
            this.Controls.Add(this.usernameBox);
            this.Controls.Add(this.usernameLabel);
            this.Name = "ConnectDialog";
            this.Text = "Connect to Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.TextBox serverBox;
        private System.Windows.Forms.Label spreadsheetLabel;
        private System.Windows.Forms.TextBox spreadsheetBox;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label connectionFailed;
    }
}