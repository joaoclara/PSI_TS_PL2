
namespace ProjetoTS
{
    partial class Form1
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
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnLeaveSession = new System.Windows.Forms.Button();
            this.rtxtMessage = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(41, 152);
            this.btnSendMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(213, 29);
            this.btnSendMessage.TabIndex = 0;
            this.btnSendMessage.Text = "Send";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnLeaveSession
            // 
            this.btnLeaveSession.Location = new System.Drawing.Point(41, 213);
            this.btnLeaveSession.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLeaveSession.Name = "btnLeaveSession";
            this.btnLeaveSession.Size = new System.Drawing.Size(213, 29);
            this.btnLeaveSession.TabIndex = 1;
            this.btnLeaveSession.Text = "Leave";
            this.btnLeaveSession.UseVisualStyleBackColor = true;
            this.btnLeaveSession.Click += new System.EventHandler(this.btnLeaveSession_Click);
            // 
            // rtxtMessage
            // 
            this.rtxtMessage.Location = new System.Drawing.Point(41, 16);
            this.rtxtMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtxtMessage.Name = "rtxtMessage";
            this.rtxtMessage.Size = new System.Drawing.Size(212, 127);
            this.rtxtMessage.TabIndex = 2;
            this.rtxtMessage.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 272);
            this.Controls.Add(this.rtxtMessage);
            this.Controls.Add(this.btnLeaveSession);
            this.Controls.Add(this.btnSendMessage);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnLeaveSession;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox rtxtMessage;
    }
}

