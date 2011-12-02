namespace NginxStarterGUI
{
    partial class NotifyIcon
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyIcon));
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemResume = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemOpration = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStripOpration = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemOStart = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOBrowse = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemOReload = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemORestart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemOQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOStop = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.contextMenuStripOpration.SuspendLayout();
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "Nginx Starter GUI";
			this.notifyIcon1.Visible = true;
			this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemResume,
            this.toolStripSeparator1,
            this.menuItemOpration,
            this.toolStripSeparator2,
            this.menuItemExit});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(101, 82);
			// 
			// menuItemResume
			// 
			this.menuItemResume.Name = "menuItemResume";
			this.menuItemResume.Size = new System.Drawing.Size(100, 22);
			this.menuItemResume.Text = "显示";
			this.menuItemResume.Click += new System.EventHandler(this.menuItemResume_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(97, 6);
			// 
			// menuItemOpration
			// 
			this.menuItemOpration.DropDown = this.contextMenuStripOpration;
			this.menuItemOpration.Name = "menuItemOpration";
			this.menuItemOpration.Size = new System.Drawing.Size(100, 22);
			this.menuItemOpration.Text = "操作";
			// 
			// contextMenuStripOpration
			// 
			this.contextMenuStripOpration.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemOStart,
            this.menuItemOBrowse,
            this.toolStripSeparator4,
            this.menuItemOReload,
            this.menuItemORestart,
            this.toolStripSeparator3,
            this.menuItemOQuit,
            this.menuItemOStop});
			this.contextMenuStripOpration.Name = "contextMenuStripOpration";
			this.contextMenuStripOpration.OwnerItem = this.menuItemOpration;
			this.contextMenuStripOpration.Size = new System.Drawing.Size(110, 148);
			// 
			// menuItemOStart
			// 
			this.menuItemOStart.Name = "menuItemOStart";
			this.menuItemOStart.Size = new System.Drawing.Size(109, 22);
			this.menuItemOStart.Text = "启动";
			this.menuItemOStart.Click += new System.EventHandler(this.menuItemOStart_Click);
			// 
			// menuItemOBrowse
			// 
			this.menuItemOBrowse.Name = "menuItemOBrowse";
			this.menuItemOBrowse.Size = new System.Drawing.Size(109, 22);
			this.menuItemOBrowse.Text = "浏览...";
			this.menuItemOBrowse.Click += new System.EventHandler(this.menuItemOBrowse_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(106, 6);
			// 
			// menuItemOReload
			// 
			this.menuItemOReload.Enabled = false;
			this.menuItemOReload.Name = "menuItemOReload";
			this.menuItemOReload.Size = new System.Drawing.Size(109, 22);
			this.menuItemOReload.Text = "重置";
			this.menuItemOReload.Click += new System.EventHandler(this.menuItemOReload_Click);
			// 
			// menuItemORestart
			// 
			this.menuItemORestart.Enabled = false;
			this.menuItemORestart.Name = "menuItemORestart";
			this.menuItemORestart.Size = new System.Drawing.Size(109, 22);
			this.menuItemORestart.Text = "重启";
			this.menuItemORestart.Click += new System.EventHandler(this.menuItemORestart_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(106, 6);
			// 
			// menuItemOQuit
			// 
			this.menuItemOQuit.Enabled = false;
			this.menuItemOQuit.Name = "menuItemOQuit";
			this.menuItemOQuit.Size = new System.Drawing.Size(109, 22);
			this.menuItemOQuit.Text = "关闭";
			this.menuItemOQuit.Click += new System.EventHandler(this.menuItemOQuit_Click);
			// 
			// menuItemOStop
			// 
			this.menuItemOStop.Name = "menuItemOStop";
			this.menuItemOStop.Size = new System.Drawing.Size(109, 22);
			this.menuItemOStop.Text = "强退";
			this.menuItemOStop.Click += new System.EventHandler(this.menuItemOStop_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(97, 6);
			// 
			// menuItemExit
			// 
			this.menuItemExit.Name = "menuItemExit";
			this.menuItemExit.Size = new System.Drawing.Size(100, 22);
			this.menuItemExit.Text = "退出";
			this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStripOpration.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuItemResume;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem menuItemOpration;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOpration;
        private System.Windows.Forms.ToolStripMenuItem menuItemOStart;
        private System.Windows.Forms.ToolStripMenuItem menuItemOBrowse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuItemOReload;
        private System.Windows.Forms.ToolStripMenuItem menuItemORestart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuItemOQuit;
        private System.Windows.Forms.ToolStripMenuItem menuItemOStop;
    }
}
