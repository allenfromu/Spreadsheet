//Created by Jack Stafford for CS 3500, November 2014

namespace SS
{
    partial class SpreadsheetForm
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
            this.components = new System.ComponentModel.Container();
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.flowLayoutPanelBiggest = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.averageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.medianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.valueLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.cellSelectedDisplay = new System.Windows.Forms.Label();
            this.cellValueDisplay = new System.Windows.Forms.Label();
            this.cellContents = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanelBiggest.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetPanel1.Location = new System.Drawing.Point(0, 69);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(488, 351);
            this.spreadsheetPanel1.TabIndex = 0;
            // 
            // flowLayoutPanelBiggest
            // 
            this.flowLayoutPanelBiggest.AllowDrop = true;
            this.flowLayoutPanelBiggest.Controls.Add(this.menuStrip1);
            this.flowLayoutPanelBiggest.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanelBiggest.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanelBiggest.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelBiggest.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelBiggest.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelBiggest.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanelBiggest.Name = "flowLayoutPanelBiggest";
            this.flowLayoutPanelBiggest.Size = new System.Drawing.Size(488, 69);
            this.flowLayoutPanelBiggest.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mathToolStripMenuItem,
            this.undoMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(488, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectToolStripMenuItem.Text = "&Sign In";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Open a spreadsheet from a file";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.ToolTipText = "Close this window";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // mathToolStripMenuItem
            // 
            this.mathToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.averageToolStripMenuItem,
            this.medianToolStripMenuItem});
            this.mathToolStripMenuItem.Name = "mathToolStripMenuItem";
            this.mathToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.mathToolStripMenuItem.Text = "&Math";
            this.mathToolStripMenuItem.Click += new System.EventHandler(this.mathToolStripMenuItem_Click);
            // 
            // averageToolStripMenuItem
            // 
            this.averageToolStripMenuItem.Name = "averageToolStripMenuItem";
            this.averageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.averageToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.averageToolStripMenuItem.Text = "&Average";
            this.averageToolStripMenuItem.ToolTipText = "Compute average of selected column";
            this.averageToolStripMenuItem.Click += new System.EventHandler(this.averageToolStripMenuItem_Click);
            // 
            // medianToolStripMenuItem
            // 
            this.medianToolStripMenuItem.Name = "medianToolStripMenuItem";
            this.medianToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.medianToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.medianToolStripMenuItem.Text = "&Median";
            this.medianToolStripMenuItem.ToolTipText = "Compute median of selected column";
            this.medianToolStripMenuItem.Click += new System.EventHandler(this.medianToolStripMenuItem_Click);
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoMenuItem.Size = new System.Drawing.Size(48, 20);
            this.undoMenuItem.Text = "&Undo";
            this.undoMenuItem.Click += new System.EventHandler(this.undoClick);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.valueLabel);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(488, 17);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label1.Size = new System.Drawing.Size(49, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selected";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(90, 0);
            this.valueLabel.Margin = new System.Windows.Forms.Padding(35, 0, 3, 0);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.valueLabel.Size = new System.Drawing.Size(34, 16);
            this.valueLabel.TabIndex = 1;
            this.valueLabel.Text = "Value";
            this.valueLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.valueLabel, "Value is empty.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(187, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(60, 0, 3, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Contents";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.cellSelectedDisplay);
            this.flowLayoutPanel2.Controls.Add(this.cellValueDisplay);
            this.flowLayoutPanel2.Controls.Add(this.cellContents);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 41);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(488, 24);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // cellSelectedDisplay
            // 
            this.cellSelectedDisplay.BackColor = System.Drawing.Color.White;
            this.cellSelectedDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cellSelectedDisplay.Location = new System.Drawing.Point(12, 3);
            this.cellSelectedDisplay.Margin = new System.Windows.Forms.Padding(12, 3, 10, 3);
            this.cellSelectedDisplay.Name = "cellSelectedDisplay";
            this.cellSelectedDisplay.Size = new System.Drawing.Size(30, 20);
            this.cellSelectedDisplay.TabIndex = 4;
            this.cellSelectedDisplay.Text = "B3";
            this.cellSelectedDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cellValueDisplay
            // 
            this.cellValueDisplay.BackColor = System.Drawing.Color.White;
            this.cellValueDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cellValueDisplay.Location = new System.Drawing.Point(55, 3);
            this.cellValueDisplay.Margin = new System.Windows.Forms.Padding(3);
            this.cellValueDisplay.MaximumSize = new System.Drawing.Size(100, 20);
            this.cellValueDisplay.MinimumSize = new System.Drawing.Size(100, 20);
            this.cellValueDisplay.Name = "cellValueDisplay";
            this.cellValueDisplay.Size = new System.Drawing.Size(100, 20);
            this.cellValueDisplay.TabIndex = 5;
            this.cellValueDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.cellValueDisplay, "Value is empty.");
            // 
            // cellContents
            // 
            this.cellContents.AcceptsReturn = true;
            this.cellContents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cellContents.Location = new System.Drawing.Point(161, 3);
            this.cellContents.MaximumSize = new System.Drawing.Size(100, 20);
            this.cellContents.MinimumSize = new System.Drawing.Size(100, 20);
            this.cellContents.Name = "cellContents";
            this.cellContents.Size = new System.Drawing.Size(100, 20);
            this.cellContents.TabIndex = 6;
            this.cellContents.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cellContents_KeyPress);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 420);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.flowLayoutPanelBiggest);
            this.Name = "SpreadsheetForm";
            this.ShowIcon = false;
            this.flowLayoutPanelBiggest.ResumeLayout(false);
            this.flowLayoutPanelBiggest.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelBiggest;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label cellSelectedDisplay;
        private System.Windows.Forms.Label cellValueDisplay;
        private System.Windows.Forms.TextBox cellContents;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem medianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem averageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
    }
}

