namespace Jasen.Framework.CodeGenerator
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTool = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tsBtnConnect = new System.Windows.Forms.ToolStripButton();
            this.tsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnLookUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnSystemSeeting = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tvwMetaData = new System.Windows.Forms.TreeView();
            this.rtxtCode = new System.Windows.Forms.RichTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnGenerateFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnGenerateAllFile = new System.Windows.Forms.ToolStripButton();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiTool});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(724, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.tsmiFile.Size = new System.Drawing.Size(59, 20);
            this.tsmiFile.Text = "File(&F)";
            // 
            // tsmiTool
            // 
            this.tsmiTool.Name = "tsmiTool";
            this.tsmiTool.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.tsmiTool.Size = new System.Drawing.Size(65, 20);
            this.tsmiTool.Text = "Tools(&T)";
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnConnect,
            this.tsSeparator,
            this.tsBtnLookUp,
            this.toolStripSeparator2,
            this.tsBtnSystemSeeting,
            this.toolStripSeparator1,
            this.tsBtnGenerateFile,
            this.toolStripSeparator3,
            this.tsBtnGenerateAllFile});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(724, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // tsBtnConnect
            // 
            this.tsBtnConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnConnect.Name = "tsBtnConnect";
            this.tsBtnConnect.Size = new System.Drawing.Size(69, 22);
            this.tsBtnConnect.Text = "连接数据库";
            // 
            // tsSeparator
            // 
            this.tsSeparator.Name = "tsSeparator";
            this.tsSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtnLookUp
            // 
            this.tsBtnLookUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnLookUp.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnLookUp.Image")));
            this.tsBtnLookUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnLookUp.Name = "tsBtnLookUp";
            this.tsBtnLookUp.Size = new System.Drawing.Size(33, 22);
            this.tsBtnLookUp.Text = "查看";
            this.tsBtnLookUp.Click += new System.EventHandler(this.tsBtnLookUp_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtnSystemSeeting
            // 
            this.tsBtnSystemSeeting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnSystemSeeting.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnSystemSeeting.Image")));
            this.tsBtnSystemSeeting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnSystemSeeting.Name = "tsBtnSystemSeeting";
            this.tsBtnSystemSeeting.Size = new System.Drawing.Size(57, 22);
            this.tsBtnSystemSeeting.Text = "系统设置";
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 415);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(724, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 49);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tvwMetaData);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rtxtCode);
            this.splitContainer.Size = new System.Drawing.Size(724, 366);
            this.splitContainer.SplitterDistance = 170;
            this.splitContainer.TabIndex = 3;
            // 
            // tvwMetaData
            // 
            this.tvwMetaData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.tvwMetaData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwMetaData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwMetaData.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tvwMetaData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.tvwMetaData.FullRowSelect = true;
            this.tvwMetaData.Location = new System.Drawing.Point(0, 0);
            this.tvwMetaData.Name = "tvwMetaData";
            this.tvwMetaData.Size = new System.Drawing.Size(170, 366);
            this.tvwMetaData.TabIndex = 0;
            // 
            // rtxtCode
            // 
            this.rtxtCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtCode.Location = new System.Drawing.Point(0, 0);
            this.rtxtCode.Name = "rtxtCode";
            this.rtxtCode.Size = new System.Drawing.Size(550, 366);
            this.rtxtCode.TabIndex = 0;
            this.rtxtCode.Text = "";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtnGenerateFile
            // 
            this.tsBtnGenerateFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnGenerateFile.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnGenerateFile.Image")));
            this.tsBtnGenerateFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnGenerateFile.Name = "tsBtnGenerateFile";
            this.tsBtnGenerateFile.Size = new System.Drawing.Size(57, 22);
            this.tsBtnGenerateFile.Text = "生成文件";
            this.tsBtnGenerateFile.Click += new System.EventHandler(this.tsBtnGenerateFile_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtnGenerateAllFile
            // 
            this.tsBtnGenerateAllFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnGenerateAllFile.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnGenerateAllFile.Image")));
            this.tsBtnGenerateAllFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnGenerateAllFile.Name = "tsBtnGenerateAllFile";
            this.tsBtnGenerateAllFile.Size = new System.Drawing.Size(57, 22);
            this.tsBtnGenerateAllFile.Text = "全部生成";
            this.tsBtnGenerateAllFile.Click += new System.EventHandler(this.tsBtnGenerateAllFile_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 437);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Code Generator";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView tvwMetaData;
        private System.Windows.Forms.RichTextBox rtxtCode;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiTool;
        private System.Windows.Forms.ToolStripButton tsBtnConnect;
        private System.Windows.Forms.ToolStripSeparator tsSeparator;
        private System.Windows.Forms.ToolStripButton tsBtnLookUp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsBtnSystemSeeting;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsBtnGenerateFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsBtnGenerateAllFile;
    }
}

