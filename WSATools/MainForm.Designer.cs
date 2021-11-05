
namespace WSATools
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelLoading = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCondition = new System.Windows.Forms.TextBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonDowngrade = new System.Windows.Forms.Button();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.buttonUninstall = new System.Windows.Forms.Button();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonApkInstall = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonWSA = new System.Windows.Forms.Button();
            this.buttonVM = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelWSA = new System.Windows.Forms.Label();
            this.labelVM = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panelLoading.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLoading
            // 
            this.panelLoading.BackColor = System.Drawing.Color.Transparent;
            this.panelLoading.Controls.Add(this.label1);
            this.panelLoading.Location = new System.Drawing.Point(252, 134);
            this.panelLoading.Name = "panelLoading";
            this.panelLoading.Size = new System.Drawing.Size(200, 200);
            this.panelLoading.TabIndex = 5;
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
            // panel1
            // 
            this.panel1.Controls.Add(this.labelProgress);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.textBoxCondition);
            this.panel1.Controls.Add(this.buttonRefresh);
            this.panel1.Controls.Add(this.buttonDowngrade);
            this.panel1.Controls.Add(this.buttonQuery);
            this.panel1.Controls.Add(this.buttonUninstall);
            this.panel1.Controls.Add(this.buttonInstall);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.buttonApkInstall);
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Controls.Add(this.buttonRemove);
            this.panel1.Controls.Add(this.buttonWSA);
            this.panel1.Controls.Add(this.buttonVM);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.labelWSA);
            this.panel1.Controls.Add(this.labelVM);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(704, 468);
            this.panel1.TabIndex = 6;
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(490, 56);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(86, 17);
            this.labelProgress.TabIndex = 8;
            this.labelProgress.Text = "下载进度：0%";
            this.labelProgress.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(458, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 7;
            this.label7.Text = "搜索包：";
            // 
            // textBoxCondition
            // 
            this.textBoxCondition.Location = new System.Drawing.Point(458, 138);
            this.textBoxCondition.Name = "textBoxCondition";
            this.textBoxCondition.Size = new System.Drawing.Size(121, 23);
            this.textBoxCondition.TabIndex = 6;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(589, 244);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 30);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "刷新";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonDowngrade
            // 
            this.buttonDowngrade.Location = new System.Drawing.Point(458, 244);
            this.buttonDowngrade.Name = "buttonDowngrade";
            this.buttonDowngrade.Size = new System.Drawing.Size(75, 30);
            this.buttonDowngrade.TabIndex = 5;
            this.buttonDowngrade.Text = "降级";
            this.buttonDowngrade.UseVisualStyleBackColor = true;
            this.buttonDowngrade.Click += new System.EventHandler(this.buttonDowngrade_Click);
            // 
            // buttonQuery
            // 
            this.buttonQuery.Location = new System.Drawing.Point(602, 134);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(62, 30);
            this.buttonQuery.TabIndex = 5;
            this.buttonQuery.Text = "查询";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // buttonUninstall
            // 
            this.buttonUninstall.Location = new System.Drawing.Point(589, 200);
            this.buttonUninstall.Name = "buttonUninstall";
            this.buttonUninstall.Size = new System.Drawing.Size(75, 30);
            this.buttonUninstall.TabIndex = 5;
            this.buttonUninstall.Text = "卸载";
            this.buttonUninstall.UseVisualStyleBackColor = true;
            this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
            // 
            // buttonInstall
            // 
            this.buttonInstall.Location = new System.Drawing.Point(458, 200);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(75, 30);
            this.buttonInstall.TabIndex = 5;
            this.buttonInstall.Text = "安装";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.DarkRed;
            this.label6.Location = new System.Drawing.Point(484, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(202, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "可能由于Github源的原因初始化失败";
            // 
            // buttonApkInstall
            // 
            this.buttonApkInstall.Location = new System.Drawing.Point(369, 23);
            this.buttonApkInstall.Name = "buttonApkInstall";
            this.buttonApkInstall.Size = new System.Drawing.Size(109, 30);
            this.buttonApkInstall.TabIndex = 3;
            this.buttonApkInstall.Text = "安装ApkInstall";
            this.buttonApkInstall.UseVisualStyleBackColor = true;
            this.buttonApkInstall.Click += new System.EventHandler(this.buttonApkInstall_Click);
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(33, 119);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(395, 337);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(369, 71);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(109, 30);
            this.buttonRemove.TabIndex = 1;
            this.buttonRemove.Text = "卸载WSA环境";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonWSA
            // 
            this.buttonWSA.Enabled = false;
            this.buttonWSA.Location = new System.Drawing.Point(238, 72);
            this.buttonWSA.Name = "buttonWSA";
            this.buttonWSA.Size = new System.Drawing.Size(92, 30);
            this.buttonWSA.TabIndex = 1;
            this.buttonWSA.Text = "检测并安装";
            this.buttonWSA.UseVisualStyleBackColor = true;
            this.buttonWSA.Click += new System.EventHandler(this.buttonWSA_Click);
            // 
            // buttonVM
            // 
            this.buttonVM.Enabled = false;
            this.buttonVM.Location = new System.Drawing.Point(238, 24);
            this.buttonVM.Name = "buttonVM";
            this.buttonVM.Size = new System.Drawing.Size(92, 30);
            this.buttonVM.TabIndex = 1;
            this.buttonVM.Text = "检测并安装";
            this.buttonVM.UseVisualStyleBackColor = true;
            this.buttonVM.Click += new System.EventHandler(this.buttonVM_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "WSA状态：";
            // 
            // labelWSA
            // 
            this.labelWSA.AutoSize = true;
            this.labelWSA.ForeColor = System.Drawing.Color.DarkRed;
            this.labelWSA.Location = new System.Drawing.Point(157, 78);
            this.labelWSA.Name = "labelWSA";
            this.labelWSA.Size = new System.Drawing.Size(44, 17);
            this.labelWSA.TabIndex = 0;
            this.labelWSA.Text = "检测中";
            // 
            // labelVM
            // 
            this.labelVM.AutoSize = true;
            this.labelVM.ForeColor = System.Drawing.Color.DarkRed;
            this.labelVM.Location = new System.Drawing.Point(157, 30);
            this.labelVM.Name = "labelVM";
            this.labelVM.Size = new System.Drawing.Size(44, 17);
            this.labelVM.TabIndex = 0;
            this.labelVM.Text = "检测中";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "windows依赖状态：";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "APK文件|*.apk";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 468);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "WSA助手";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelLoading.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLoading;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelWSA;
        private System.Windows.Forms.Label labelVM;
        private System.Windows.Forms.Button buttonWSA;
        private System.Windows.Forms.Button buttonVM;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonApkInstall;
        private System.Windows.Forms.Button buttonDowngrade;
        private System.Windows.Forms.Button buttonUninstall;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.TextBox textBoxCondition;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Button buttonRemove;
    }
}

