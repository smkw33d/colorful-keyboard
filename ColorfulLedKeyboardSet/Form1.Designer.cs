namespace ColorfulLedKeyboardSet
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
            if (disposing)
            {
                StopRgbLoop();
            }

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
            this.button1 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.speedBar = new System.Windows.Forms.TrackBar();
            this.CustomRGB_B = new System.Windows.Forms.Button();
            this.ColorTestLabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "RGB循环";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // speedBar
            // 
            this.speedBar.LargeChange = 1;
            this.speedBar.Location = new System.Drawing.Point(198, 5);
            this.speedBar.Maximum = 15;
            this.speedBar.Minimum = 1;
            this.speedBar.Name = "speedBar";
            this.speedBar.Size = new System.Drawing.Size(113, 56);
            this.speedBar.TabIndex = 1;
            this.speedBar.Value = 7;
            this.speedBar.ValueChanged += new System.EventHandler(this.speedBar_ValueChanged);
            // 
            // CustomRGB_B
            // 
            this.CustomRGB_B.Location = new System.Drawing.Point(12, 67);
            this.CustomRGB_B.Name = "CustomRGB_B";
            this.CustomRGB_B.Size = new System.Drawing.Size(97, 34);
            this.CustomRGB_B.TabIndex = 2;
            this.CustomRGB_B.Text = "自定义RGB";
            this.CustomRGB_B.UseVisualStyleBackColor = true;
            this.CustomRGB_B.Click += new System.EventHandler(this.CustomRGB_B_Click);
            // ColorTestLabel
            // 
            this.ColorTestLabel.AutoSize = true;
            this.ColorTestLabel.Location = new System.Drawing.Point(317, 22);
            this.ColorTestLabel.Name = "ColorTestLabel";
            this.ColorTestLabel.Size = new System.Drawing.Size(67, 15);
            this.ColorTestLabel.TabIndex = 5;
            this.ColorTestLabel.Text = "键盘颜色";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(110, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 34);
            this.button2.TabIndex = 6;
            this.button2.Text = "停止循环";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 114);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ColorTestLabel);
            this.Controls.Add(this.CustomRGB_B);
            this.Controls.Add(this.speedBar);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "colorful-keyboard";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TrackBar speedBar;
        private System.Windows.Forms.Button CustomRGB_B;
        private System.Windows.Forms.Label ColorTestLabel;
        private System.Windows.Forms.Button button2;
    }
}

