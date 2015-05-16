namespace CameraCapture
{
    partial class CameraCapture
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
            this.rawImageBox = new Emgu.CV.UI.ImageBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.processedImageBox = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.rawImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // rawImageBox
            // 
            this.rawImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rawImageBox.Location = new System.Drawing.Point(12, 12);
            this.rawImageBox.Name = "rawImageBox";
            this.rawImageBox.Size = new System.Drawing.Size(640, 480);
            this.rawImageBox.TabIndex = 2;
            this.rawImageBox.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(627, 498);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.TabStop = false;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // processedImageBox
            // 
            this.processedImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.processedImageBox.Location = new System.Drawing.Point(669, 12);
            this.processedImageBox.Name = "processedImageBox";
            this.processedImageBox.Size = new System.Drawing.Size(640, 480);
            this.processedImageBox.TabIndex = 4;
            this.processedImageBox.TabStop = false;
            // 
            // CameraCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1319, 528);
            this.Controls.Add(this.processedImageBox);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.rawImageBox);
            this.Name = "CameraCapture";
            this.Text = "Camera Output";
            ((System.ComponentModel.ISupportInitialize)(this.rawImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox rawImageBox;
        private System.Windows.Forms.Button btnStart;
        private Emgu.CV.UI.ImageBox processedImageBox;
    }
}

