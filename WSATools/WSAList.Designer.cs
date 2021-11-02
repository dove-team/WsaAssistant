
namespace WSATools
{
    partial class WSAList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WSAList));
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.panelLoading = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLoading.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBox
            // 
            this.checkedListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(447, 454);
            this.checkedListBox.TabIndex = 0;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(186, 472);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 28);
            this.buttonRefresh.TabIndex = 1;
            this.buttonRefresh.Text = "刷新";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(308, 472);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 28);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonInstall
            // 
            this.buttonInstall.Location = new System.Drawing.Point(64, 472);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(75, 28);
            this.buttonInstall.TabIndex = 3;
            this.buttonInstall.Text = "安装";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // panelLoading
            // 
            this.panelLoading.BackColor = System.Drawing.Color.Transparent;
            this.panelLoading.Controls.Add(this.label1);
            this.panelLoading.Location = new System.Drawing.Point(123, 156);
            this.panelLoading.Name = "panelLoading";
            this.panelLoading.Size = new System.Drawing.Size(200, 200);
            this.panelLoading.TabIndex = 4;
            this.panelLoading.Visible = false;
            // 
            // label1
            // 
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(77, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 46);
            this.label1.TabIndex = 0;
            // 
            // WSAList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 512);
            this.Controls.Add(this.panelLoading);
            this.Controls.Add(this.buttonInstall);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.checkedListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WSAList";
            this.ShowInTaskbar = false;
            this.Text = "WSA环境安装";
            this.Load += new System.EventHandler(this.WSAList_Load);
            this.panelLoading.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Panel panelLoading;
        private System.Windows.Forms.Label label1;
    }
}