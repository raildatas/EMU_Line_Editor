namespace ELE
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bll文件BToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.车内PIDS文件LToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.退出XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于动车线路编辑器AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.mtb_downtime = new System.Windows.Forms.MaskedTextBox();
            this.mtb_uptime = new System.Windows.Forms.MaskedTextBox();
            this.cbx_isAstar = new System.Windows.Forms.CheckBox();
            this.tbx_downtime = new System.Windows.Forms.TextBox();
            this.tbx_uptime = new System.Windows.Forms.TextBox();
            this.btn_down = new System.Windows.Forms.Button();
            this.btn_up = new System.Windows.Forms.Button();
            this.btn_downtim = new System.Windows.Forms.Label();
            this.btn_uptim = new System.Windows.Forms.Label();
            this.tbx_etim = new System.Windows.Forms.MaskedTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.rb_exp = new System.Windows.Forms.RadioButton();
            this.rb_max = new System.Windows.Forms.RadioButton();
            this.rb_atp = new System.Windows.Forms.RadioButton();
            this.tbx_afterstop = new System.Windows.Forms.MaskedTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbx_beforestop = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbx_afterteg = new System.Windows.Forms.CheckBox();
            this.cbx_beforeteg = new System.Windows.Forms.CheckBox();
            this.cbo_after = new System.Windows.Forms.ComboBox();
            this.cbo_line = new System.Windows.Forms.ComboBox();
            this.cbx_no350mode = new System.Windows.Forms.CheckBox();
            this.tbx_trainNum = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbo_rcp = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_submit = new System.Windows.Forms.Button();
            this.tbx_before = new System.Windows.Forms.TextBox();
            this.lbx_list = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbx_isTwoTrain = new System.Windows.Forms.CheckBox();
            this.cbo_trainSeat = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbo_trainType = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.lbl_event = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(26, 26);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem,
            this.关于AToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripMenuItem,
            this.打开OToolStripMenuItem,
            this.保存SToolStripMenuItem,
            this.导出EToolStripMenuItem,
            this.toolStripMenuItem1,
            this.退出XToolStripMenuItem,
            this.设置CToolStripMenuItem});
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            resources.ApplyResources(this.文件FToolStripMenuItem, "文件FToolStripMenuItem");
            // 
            // 新建NToolStripMenuItem
            // 
            this.新建NToolStripMenuItem.Name = "新建NToolStripMenuItem";
            resources.ApplyResources(this.新建NToolStripMenuItem, "新建NToolStripMenuItem");
            this.新建NToolStripMenuItem.Click += new System.EventHandler(this.新建NToolStripMenuItem_Click);
            // 
            // 打开OToolStripMenuItem
            // 
            this.打开OToolStripMenuItem.Name = "打开OToolStripMenuItem";
            resources.ApplyResources(this.打开OToolStripMenuItem, "打开OToolStripMenuItem");
            this.打开OToolStripMenuItem.Click += new System.EventHandler(this.打开OToolStripMenuItem_Click);
            // 
            // 保存SToolStripMenuItem
            // 
            this.保存SToolStripMenuItem.Name = "保存SToolStripMenuItem";
            resources.ApplyResources(this.保存SToolStripMenuItem, "保存SToolStripMenuItem");
            this.保存SToolStripMenuItem.Click += new System.EventHandler(this.保存SToolStripMenuItem_Click);
            // 
            // 导出EToolStripMenuItem
            // 
            this.导出EToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bll文件BToolStripMenuItem,
            this.车内PIDS文件LToolStripMenuItem});
            this.导出EToolStripMenuItem.Name = "导出EToolStripMenuItem";
            resources.ApplyResources(this.导出EToolStripMenuItem, "导出EToolStripMenuItem");
            // 
            // bll文件BToolStripMenuItem
            // 
            this.bll文件BToolStripMenuItem.Name = "bll文件BToolStripMenuItem";
            resources.ApplyResources(this.bll文件BToolStripMenuItem, "bll文件BToolStripMenuItem");
            this.bll文件BToolStripMenuItem.Click += new System.EventHandler(this.bll文件BToolStripMenuItem_Click);
            // 
            // 车内PIDS文件LToolStripMenuItem
            // 
            this.车内PIDS文件LToolStripMenuItem.Name = "车内PIDS文件LToolStripMenuItem";
            resources.ApplyResources(this.车内PIDS文件LToolStripMenuItem, "车内PIDS文件LToolStripMenuItem");
            this.车内PIDS文件LToolStripMenuItem.Click += new System.EventHandler(this.车内PIDS文件LToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // 退出XToolStripMenuItem
            // 
            this.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem";
            resources.ApplyResources(this.退出XToolStripMenuItem, "退出XToolStripMenuItem");
            this.退出XToolStripMenuItem.Click += new System.EventHandler(this.退出XToolStripMenuItem_Click);
            // 
            // 设置CToolStripMenuItem
            // 
            this.设置CToolStripMenuItem.Name = "设置CToolStripMenuItem";
            resources.ApplyResources(this.设置CToolStripMenuItem, "设置CToolStripMenuItem");
            this.设置CToolStripMenuItem.Click += new System.EventHandler(this.设置CToolStripMenuItem_Click);
            // 
            // 关于AToolStripMenuItem
            // 
            this.关于AToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.帮助HToolStripMenuItem,
            this.关于动车线路编辑器AToolStripMenuItem});
            this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
            resources.ApplyResources(this.关于AToolStripMenuItem, "关于AToolStripMenuItem");
            // 
            // 帮助HToolStripMenuItem
            // 
            this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
            resources.ApplyResources(this.帮助HToolStripMenuItem, "帮助HToolStripMenuItem");
            this.帮助HToolStripMenuItem.Click += new System.EventHandler(this.帮助HToolStripMenuItem_Click);
            // 
            // 关于动车线路编辑器AToolStripMenuItem
            // 
            this.关于动车线路编辑器AToolStripMenuItem.Name = "关于动车线路编辑器AToolStripMenuItem";
            resources.ApplyResources(this.关于动车线路编辑器AToolStripMenuItem, "关于动车线路编辑器AToolStripMenuItem");
            this.关于动车线路编辑器AToolStripMenuItem.Click += new System.EventHandler(this.关于动车线路编辑器AToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(26, 26);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除DToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // 删除DToolStripMenuItem
            // 
            this.删除DToolStripMenuItem.Name = "删除DToolStripMenuItem";
            resources.ApplyResources(this.删除DToolStripMenuItem, "删除DToolStripMenuItem");
            this.删除DToolStripMenuItem.Click += new System.EventHandler(this.删除DToolStripMenuItem_Click);
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.mtb_downtime);
            this.tabPage1.Controls.Add(this.mtb_uptime);
            this.tabPage1.Controls.Add(this.cbx_isAstar);
            this.tabPage1.Controls.Add(this.tbx_downtime);
            this.tabPage1.Controls.Add(this.tbx_uptime);
            this.tabPage1.Controls.Add(this.btn_down);
            this.tabPage1.Controls.Add(this.btn_up);
            this.tabPage1.Controls.Add(this.btn_downtim);
            this.tabPage1.Controls.Add(this.btn_uptim);
            this.tabPage1.Controls.Add(this.tbx_etim);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.rb_exp);
            this.tabPage1.Controls.Add(this.rb_max);
            this.tabPage1.Controls.Add(this.rb_atp);
            this.tabPage1.Controls.Add(this.tbx_afterstop);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.tbx_beforestop);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.cbx_afterteg);
            this.tabPage1.Controls.Add(this.cbx_beforeteg);
            this.tabPage1.Controls.Add(this.cbo_after);
            this.tabPage1.Controls.Add(this.cbo_line);
            this.tabPage1.Controls.Add(this.cbx_no350mode);
            this.tabPage1.Controls.Add(this.tbx_trainNum);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.cbo_rcp);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btn_submit);
            this.tabPage1.Controls.Add(this.tbx_before);
            this.tabPage1.Controls.Add(this.lbx_list);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.cbx_isTwoTrain);
            this.tabPage1.Controls.Add(this.cbo_trainSeat);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.cbo_trainType);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mtb_downtime
            // 
            resources.ApplyResources(this.mtb_downtime, "mtb_downtime");
            this.mtb_downtime.Name = "mtb_downtime";
            this.mtb_downtime.ValidatingType = typeof(System.DateTime);
            this.mtb_downtime.TextChanged += new System.EventHandler(this.mtb_downtime_TextChanged);
            // 
            // mtb_uptime
            // 
            resources.ApplyResources(this.mtb_uptime, "mtb_uptime");
            this.mtb_uptime.Name = "mtb_uptime";
            this.mtb_uptime.ValidatingType = typeof(System.DateTime);
            this.mtb_uptime.TextChanged += new System.EventHandler(this.mtb_uptime_TextChanged);
            // 
            // cbx_isAstar
            // 
            resources.ApplyResources(this.cbx_isAstar, "cbx_isAstar");
            this.cbx_isAstar.Name = "cbx_isAstar";
            this.cbx_isAstar.UseVisualStyleBackColor = true;
            this.cbx_isAstar.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tbx_downtime
            // 
            resources.ApplyResources(this.tbx_downtime, "tbx_downtime");
            this.tbx_downtime.Name = "tbx_downtime";
            this.tbx_downtime.ReadOnly = true;
            // 
            // tbx_uptime
            // 
            resources.ApplyResources(this.tbx_uptime, "tbx_uptime");
            this.tbx_uptime.Name = "tbx_uptime";
            this.tbx_uptime.ReadOnly = true;
            // 
            // btn_down
            // 
            resources.ApplyResources(this.btn_down, "btn_down");
            this.btn_down.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(8)))), ((int)(((byte)(178)))));
            this.btn_down.ForeColor = System.Drawing.Color.White;
            this.btn_down.Name = "btn_down";
            this.btn_down.UseVisualStyleBackColor = false;
            this.btn_down.Click += new System.EventHandler(this.btn_up_Click);
            // 
            // btn_up
            // 
            resources.ApplyResources(this.btn_up, "btn_up");
            this.btn_up.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(175)))), ((int)(((byte)(207)))));
            this.btn_up.ForeColor = System.Drawing.Color.White;
            this.btn_up.Name = "btn_up";
            this.btn_up.UseVisualStyleBackColor = false;
            this.btn_up.Click += new System.EventHandler(this.btn_up_Click);
            // 
            // btn_downtim
            // 
            resources.ApplyResources(this.btn_downtim, "btn_downtim");
            this.btn_downtim.Name = "btn_downtim";
            // 
            // btn_uptim
            // 
            resources.ApplyResources(this.btn_uptim, "btn_uptim");
            this.btn_uptim.Name = "btn_uptim";
            // 
            // tbx_etim
            // 
            resources.ApplyResources(this.tbx_etim, "tbx_etim");
            this.tbx_etim.Name = "tbx_etim";
            this.tbx_etim.ValidatingType = typeof(int);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // rb_exp
            // 
            resources.ApplyResources(this.rb_exp, "rb_exp");
            this.rb_exp.Checked = true;
            this.rb_exp.Name = "rb_exp";
            this.rb_exp.TabStop = true;
            this.rb_exp.UseVisualStyleBackColor = true;
            // 
            // rb_max
            // 
            resources.ApplyResources(this.rb_max, "rb_max");
            this.rb_max.Name = "rb_max";
            this.rb_max.UseVisualStyleBackColor = true;
            // 
            // rb_atp
            // 
            resources.ApplyResources(this.rb_atp, "rb_atp");
            this.rb_atp.Name = "rb_atp";
            this.rb_atp.UseVisualStyleBackColor = true;
            // 
            // tbx_afterstop
            // 
            resources.ApplyResources(this.tbx_afterstop, "tbx_afterstop");
            this.tbx_afterstop.Name = "tbx_afterstop";
            this.tbx_afterstop.ValidatingType = typeof(int);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // tbx_beforestop
            // 
            resources.ApplyResources(this.tbx_beforestop, "tbx_beforestop");
            this.tbx_beforestop.Name = "tbx_beforestop";
            this.tbx_beforestop.ValidatingType = typeof(int);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // cbx_afterteg
            // 
            resources.ApplyResources(this.cbx_afterteg, "cbx_afterteg");
            this.cbx_afterteg.Name = "cbx_afterteg";
            this.cbx_afterteg.UseVisualStyleBackColor = true;
            // 
            // cbx_beforeteg
            // 
            resources.ApplyResources(this.cbx_beforeteg, "cbx_beforeteg");
            this.cbx_beforeteg.Name = "cbx_beforeteg";
            this.cbx_beforeteg.UseVisualStyleBackColor = true;
            // 
            // cbo_after
            // 
            resources.ApplyResources(this.cbo_after, "cbo_after");
            this.cbo_after.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_after.FormattingEnabled = true;
            this.cbo_after.Name = "cbo_after";
            this.cbo_after.SelectedIndexChanged += new System.EventHandler(this.cbo_after_SelectedIndexChanged);
            this.cbo_after.Click += new System.EventHandler(this.cbo_after_Click);
            // 
            // cbo_line
            // 
            resources.ApplyResources(this.cbo_line, "cbo_line");
            this.cbo_line.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_line.FormattingEnabled = true;
            this.cbo_line.Name = "cbo_line";
            this.cbo_line.SelectedIndexChanged += new System.EventHandler(this.cbo_line_SelectedIndexChanged);
            this.cbo_line.Click += new System.EventHandler(this.cbo_line_Click);
            // 
            // cbx_no350mode
            // 
            resources.ApplyResources(this.cbx_no350mode, "cbx_no350mode");
            this.cbx_no350mode.Name = "cbx_no350mode";
            this.cbx_no350mode.UseVisualStyleBackColor = true;
            this.cbx_no350mode.CheckedChanged += new System.EventHandler(this.cbx_no350mode_CheckedChanged);
            // 
            // tbx_trainNum
            // 
            resources.ApplyResources(this.tbx_trainNum, "tbx_trainNum");
            this.tbx_trainNum.Name = "tbx_trainNum";
            this.tbx_trainNum.LostFocus += new System.EventHandler(this.textbox4_LostFocus);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // cbo_rcp
            // 
            resources.ApplyResources(this.cbo_rcp, "cbo_rcp");
            this.cbo_rcp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_rcp.FormattingEnabled = true;
            this.cbo_rcp.Name = "cbo_rcp";
            this.cbo_rcp.SelectedIndexChanged += new System.EventHandler(this.cbo_rcp_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btn_submit
            // 
            resources.ApplyResources(this.btn_submit, "btn_submit");
            this.btn_submit.Name = "btn_submit";
            this.btn_submit.UseVisualStyleBackColor = true;
            this.btn_submit.Click += new System.EventHandler(this.btn_submit_Click);
            // 
            // tbx_before
            // 
            resources.ApplyResources(this.tbx_before, "tbx_before");
            this.tbx_before.Name = "tbx_before";
            // 
            // lbx_list
            // 
            resources.ApplyResources(this.lbx_list, "lbx_list");
            this.lbx_list.ContextMenuStrip = this.contextMenuStrip1;
            this.lbx_list.FormattingEnabled = true;
            this.lbx_list.Name = "lbx_list";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cbx_isTwoTrain
            // 
            resources.ApplyResources(this.cbx_isTwoTrain, "cbx_isTwoTrain");
            this.cbx_isTwoTrain.Name = "cbx_isTwoTrain";
            this.cbx_isTwoTrain.UseVisualStyleBackColor = true;
            // 
            // cbo_trainSeat
            // 
            resources.ApplyResources(this.cbo_trainSeat, "cbo_trainSeat");
            this.cbo_trainSeat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_trainSeat.FormattingEnabled = true;
            this.cbo_trainSeat.Name = "cbo_trainSeat";
            this.cbo_trainSeat.SelectedIndexChanged += new System.EventHandler(this.cbo_trainSeat_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cbo_trainType
            // 
            resources.ApplyResources(this.cbo_trainType, "cbo_trainType");
            this.cbo_trainType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_trainType.FormattingEnabled = true;
            this.cbo_trainType.Name = "cbo_trainType";
            this.cbo_trainType.SelectedIndexChanged += new System.EventHandler(this.cbo_trainType_SelectedIndexChanged);
            this.cbo_trainType.Click += new System.EventHandler(this.cbo_trainType_Click);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // lbl_event
            // 
            resources.ApplyResources(this.lbl_event, "lbl_event");
            this.lbl_event.ForeColor = System.Drawing.Color.Red;
            this.lbl_event.Name = "lbl_event";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_event);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出EToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 退出XToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除DToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btn_submit;
        private System.Windows.Forms.TextBox tbx_before;
        private System.Windows.Forms.ListBox lbx_list;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbx_isTwoTrain;
        private System.Windows.Forms.ComboBox cbo_trainSeat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbo_trainType;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ComboBox cbo_rcp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbx_trainNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbx_no350mode;
        private System.Windows.Forms.ComboBox cbo_after;
        private System.Windows.Forms.ComboBox cbo_line;
        private System.Windows.Forms.Label lbl_event;
        private System.Windows.Forms.CheckBox cbx_afterteg;
        private System.Windows.Forms.CheckBox cbx_beforeteg;
        private System.Windows.Forms.MaskedTextBox tbx_beforestop;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MaskedTextBox tbx_afterstop;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton rb_exp;
        private System.Windows.Forms.RadioButton rb_max;
        private System.Windows.Forms.MaskedTextBox tbx_etim;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label btn_downtim;
        private System.Windows.Forms.Label btn_uptim;
        private System.Windows.Forms.Button btn_down;
        private System.Windows.Forms.Button btn_up;
        private System.Windows.Forms.TextBox tbx_downtime;
        private System.Windows.Forms.TextBox tbx_uptime;
        private System.Windows.Forms.ToolStripMenuItem bll文件BToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 车内PIDS文件LToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbx_isAstar;
        private System.Windows.Forms.ToolStripMenuItem 设置CToolStripMenuItem;
        private System.Windows.Forms.MaskedTextBox mtb_uptime;
        private System.Windows.Forms.MaskedTextBox mtb_downtime;
        private System.Windows.Forms.RadioButton rb_atp;
        private System.Windows.Forms.ToolStripMenuItem 关于AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助HToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于动车线路编辑器AToolStripMenuItem;
    }
}

