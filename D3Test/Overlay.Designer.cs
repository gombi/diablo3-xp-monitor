namespace D3Test
{
    partial class Overlay
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblXPHour = new System.Windows.Forms.Label();
            this.lblTimeToLvl = new System.Windows.Forms.Label();
            this.lblRun = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(185, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "XP/Hour:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(16, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Run time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(13, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "TimeToLvl:";
            // 
            // lblXPHour
            // 
            this.lblXPHour.BackColor = System.Drawing.Color.Transparent;
            this.lblXPHour.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXPHour.ForeColor = System.Drawing.Color.White;
            this.lblXPHour.Location = new System.Drawing.Point(183, 48);
            this.lblXPHour.Name = "lblXPHour";
            this.lblXPHour.Size = new System.Drawing.Size(173, 18);
            this.lblXPHour.TabIndex = 3;
            this.lblXPHour.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimeToLvl
            // 
            this.lblTimeToLvl.AutoSize = true;
            this.lblTimeToLvl.BackColor = System.Drawing.Color.Transparent;
            this.lblTimeToLvl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeToLvl.ForeColor = System.Drawing.Color.White;
            this.lblTimeToLvl.Location = new System.Drawing.Point(120, 21);
            this.lblTimeToLvl.Name = "lblTimeToLvl";
            this.lblTimeToLvl.Size = new System.Drawing.Size(65, 14);
            this.lblTimeToLvl.TabIndex = 2;
            this.lblTimeToLvl.Text = "00:00:00";
            // 
            // lblRun
            // 
            this.lblRun.AutoSize = true;
            this.lblRun.BackColor = System.Drawing.Color.Transparent;
            this.lblRun.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRun.ForeColor = System.Drawing.Color.White;
            this.lblRun.Location = new System.Drawing.Point(120, 51);
            this.lblRun.Name = "lblRun";
            this.lblRun.Size = new System.Drawing.Size(65, 14);
            this.lblRun.TabIndex = 1;
            this.lblRun.Text = "00:00:00";
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.BackColor = System.Drawing.Color.Transparent;
            this.lblWarning.ForeColor = System.Drawing.Color.Yellow;
            this.lblWarning.Location = new System.Drawing.Point(317, 21);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(52, 13);
            this.lblWarning.TabIndex = 4;
            this.lblWarning.Text = "Bad input";
            this.lblWarning.Visible = false;
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::D3Test.Properties.Resources.back;
            this.ClientSize = new System.Drawing.Size(376, 91);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.lblXPHour);
            this.Controls.Add(this.lblTimeToLvl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblRun);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Overlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Diablo 3 XP Monitor by Gombi - http://gombi.dk";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblXPHour;
        private System.Windows.Forms.Label lblTimeToLvl;
        private System.Windows.Forms.Label lblRun;
        private System.Windows.Forms.Label lblWarning;
    }
}