namespace QiqqaToolBench
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.taskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkLibraryIntegrityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewWebPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grabSomeBibTexOffGoogleScholarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPDFAndMarkATitleTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowserPane = new System.Windows.Forms.WebBrowser();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.taskToolStripMenuItem,
            this.experimentsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1254, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // taskToolStripMenuItem
            // 
            this.taskToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectLibraryToolStripMenuItem,
            this.checkLibraryIntegrityToolStripMenuItem});
            this.taskToolStripMenuItem.Name = "taskToolStripMenuItem";
            this.taskToolStripMenuItem.Size = new System.Drawing.Size(50, 24);
            this.taskToolStripMenuItem.Text = "Task";
            // 
            // selectLibraryToolStripMenuItem
            // 
            this.selectLibraryToolStripMenuItem.Name = "selectLibraryToolStripMenuItem";
            this.selectLibraryToolStripMenuItem.Size = new System.Drawing.Size(239, 26);
            this.selectLibraryToolStripMenuItem.Text = "Select Library";
            this.selectLibraryToolStripMenuItem.Click += new System.EventHandler(this.SelectLibraryToolStripMenuItem_Click);
            // 
            // checkLibraryIntegrityToolStripMenuItem
            // 
            this.checkLibraryIntegrityToolStripMenuItem.Name = "checkLibraryIntegrityToolStripMenuItem";
            this.checkLibraryIntegrityToolStripMenuItem.Size = new System.Drawing.Size(239, 26);
            this.checkLibraryIntegrityToolStripMenuItem.Text = "Check Library Integrity";
            this.checkLibraryIntegrityToolStripMenuItem.Click += new System.EventHandler(this.CheckLibraryIntegrityToolStripMenuItem_Click);
            // 
            // experimentsToolStripMenuItem
            // 
            this.experimentsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewPDFToolStripMenuItem,
            this.viewWebPageToolStripMenuItem,
            this.grabSomeBibTexOffGoogleScholarToolStripMenuItem,
            this.viewPDFAndMarkATitleTextToolStripMenuItem});
            this.experimentsToolStripMenuItem.Name = "experimentsToolStripMenuItem";
            this.experimentsToolStripMenuItem.Size = new System.Drawing.Size(104, 24);
            this.experimentsToolStripMenuItem.Text = "Experiments";
            // 
            // viewPDFToolStripMenuItem
            // 
            this.viewPDFToolStripMenuItem.Name = "viewPDFToolStripMenuItem";
            this.viewPDFToolStripMenuItem.Size = new System.Drawing.Size(343, 26);
            this.viewPDFToolStripMenuItem.Text = "View PDF";
            this.viewPDFToolStripMenuItem.Click += new System.EventHandler(this.ViewPDFToolStripMenuItem_Click);
            // 
            // viewWebPageToolStripMenuItem
            // 
            this.viewWebPageToolStripMenuItem.Name = "viewWebPageToolStripMenuItem";
            this.viewWebPageToolStripMenuItem.Size = new System.Drawing.Size(343, 26);
            this.viewWebPageToolStripMenuItem.Text = "View Web Page";
            this.viewWebPageToolStripMenuItem.Click += new System.EventHandler(this.ViewWebPageToolStripMenuItem_Click);
            // 
            // grabSomeBibTexOffGoogleScholarToolStripMenuItem
            // 
            this.grabSomeBibTexOffGoogleScholarToolStripMenuItem.Name = "grabSomeBibTexOffGoogleScholarToolStripMenuItem";
            this.grabSomeBibTexOffGoogleScholarToolStripMenuItem.Size = new System.Drawing.Size(343, 26);
            this.grabSomeBibTexOffGoogleScholarToolStripMenuItem.Text = "Grab Some BibTex off Google Scholar";
            this.grabSomeBibTexOffGoogleScholarToolStripMenuItem.Click += new System.EventHandler(this.GrabSomeBibTexOffGoogleScholarToolStripMenuItem_Click);
            // 
            // viewPDFAndMarkATitleTextToolStripMenuItem
            // 
            this.viewPDFAndMarkATitleTextToolStripMenuItem.Name = "viewPDFAndMarkATitleTextToolStripMenuItem";
            this.viewPDFAndMarkATitleTextToolStripMenuItem.Size = new System.Drawing.Size(343, 26);
            this.viewPDFAndMarkATitleTextToolStripMenuItem.Text = "View PDF and mark a Title Text";
            this.viewPDFAndMarkATitleTextToolStripMenuItem.Click += new System.EventHandler(this.ViewPDFAndMarkATitleTextToolStripMenuItem_Click);
            // 
            // webBrowserPane
            // 
            this.webBrowserPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowserPane.Location = new System.Drawing.Point(0, 97);
            this.webBrowserPane.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserPane.Name = "webBrowserPane";
            this.webBrowserPane.Size = new System.Drawing.Size(1254, 355);
            this.webBrowserPane.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripSeparator1,
            this.toolStripButton7,
            this.toolStripButton8});
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1254, 27);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "MainToolStrip";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1254, 26);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(97, 20);
            this.toolStripStatusLabel1.Text = "VersionStatus";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton5.Text = "toolStripButton5";
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton6.Text = "toolStripButton6";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton7.Text = "toolStripButton7";
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton8.Text = "toolStripButton8";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1254, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.webBrowserPane);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "MainForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem taskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectLibraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkLibraryIntegrityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem experimentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewWebPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grabSomeBibTexOffGoogleScholarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPDFAndMarkATitleTextToolStripMenuItem;
        private System.Windows.Forms.WebBrowser webBrowserPane;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
    }
}

