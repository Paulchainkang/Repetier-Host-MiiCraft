﻿namespace RepetierHost.view
{
    partial class JobStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobStatus));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.labelFinishTime = new System.Windows.Forms.Label();
            this.labelETA = new System.Windows.Forms.Label();
            this.labelTotalLines = new System.Windows.Forms.Label();
            this.labelLinesSend = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 250;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 41);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Start time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 65);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Finish time:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 90);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "ETA:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 114);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Total lines:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 139);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Lines send:";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(80, 174);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(100, 28);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(107, 16);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(13, 17);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "-";
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(107, 41);
            this.labelStartTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(13, 17);
            this.labelStartTime.TabIndex = 8;
            this.labelStartTime.Text = "-";
            // 
            // labelFinishTime
            // 
            this.labelFinishTime.AutoSize = true;
            this.labelFinishTime.Location = new System.Drawing.Point(107, 65);
            this.labelFinishTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFinishTime.Name = "labelFinishTime";
            this.labelFinishTime.Size = new System.Drawing.Size(13, 17);
            this.labelFinishTime.TabIndex = 8;
            this.labelFinishTime.Text = "-";
            // 
            // labelETA
            // 
            this.labelETA.AutoSize = true;
            this.labelETA.Location = new System.Drawing.Point(107, 90);
            this.labelETA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelETA.Name = "labelETA";
            this.labelETA.Size = new System.Drawing.Size(13, 17);
            this.labelETA.TabIndex = 8;
            this.labelETA.Text = "-";
            // 
            // labelTotalLines
            // 
            this.labelTotalLines.AutoSize = true;
            this.labelTotalLines.Location = new System.Drawing.Point(107, 114);
            this.labelTotalLines.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTotalLines.Name = "labelTotalLines";
            this.labelTotalLines.Size = new System.Drawing.Size(13, 17);
            this.labelTotalLines.TabIndex = 8;
            this.labelTotalLines.Text = "-";
            // 
            // labelLinesSend
            // 
            this.labelLinesSend.AutoSize = true;
            this.labelLinesSend.Location = new System.Drawing.Point(107, 139);
            this.labelLinesSend.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelLinesSend.Name = "labelLinesSend";
            this.labelLinesSend.Size = new System.Drawing.Size(13, 17);
            this.labelLinesSend.TabIndex = 8;
            this.labelLinesSend.Text = "-";
            // 
            // JobStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(259, 218);
            this.ControlBox = false;
            this.Controls.Add(this.labelLinesSend);
            this.Controls.Add(this.labelTotalLines);
            this.Controls.Add(this.labelETA);
            this.Controls.Add(this.labelFinishTime);
            this.Controls.Add(this.labelStartTime);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JobStatus";
            this.Text = "Job status";
            this.Shown += new System.EventHandler(this.JobStatus_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JobStatus_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelStartTime;
        private System.Windows.Forms.Label labelFinishTime;
        private System.Windows.Forms.Label labelETA;
        private System.Windows.Forms.Label labelTotalLines;
        private System.Windows.Forms.Label labelLinesSend;
    }
}