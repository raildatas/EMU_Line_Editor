namespace ELE
{
    partial class SetDelayForm
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
            this.lbl_info = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tbx_delay = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbl_info
            // 
            this.lbl_info.Location = new System.Drawing.Point(13, 13);
            this.lbl_info.Multiline = true;
            this.lbl_info.Name = "lbl_info";
            this.lbl_info.ReadOnly = true;
            this.lbl_info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lbl_info.Size = new System.Drawing.Size(473, 138);
            this.lbl_info.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(270, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "分钟";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(381, 160);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 32);
            this.button1.TabIndex = 4;
            this.button1.Text = "确定(&Y)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbx_delay
            // 
            this.tbx_delay.Location = new System.Drawing.Point(164, 160);
            this.tbx_delay.Name = "tbx_delay";
            this.tbx_delay.Size = new System.Drawing.Size(100, 32);
            this.tbx_delay.TabIndex = 5;
            this.tbx_delay.Text = "0";
            // 
            // SetDelayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 206);
            this.Controls.Add(this.tbx_delay);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_info);
            this.Font = new System.Drawing.Font("微软雅黑", 14.1F);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetDelayForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "设置正晚点时间";
            this.Load += new System.EventHandler(this.SetDelayForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox lbl_info;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbx_delay;
    }
}