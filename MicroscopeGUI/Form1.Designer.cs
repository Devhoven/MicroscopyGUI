
namespace MicroscopeGUI
{
    partial class GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.ImgGalleryCon = new System.Windows.Forms.TableLayoutPanel();
            this.ImgGalleryLabel = new System.Windows.Forms.Label();
            this.LiveImgCon = new System.Windows.Forms.PictureBox();
            this.CurrentToolCon = new System.Windows.Forms.TableLayoutPanel();
            this.NavigationBtnCon = new System.Windows.Forms.TableLayoutPanel();
            this.BackBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.StepLabel = new System.Windows.Forms.Label();
            this.ImgGalleryCon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LiveImgCon)).BeginInit();
            this.CurrentToolCon.SuspendLayout();
            this.NavigationBtnCon.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImgGalleryCon
            // 
            resources.ApplyResources(this.ImgGalleryCon, "ImgGalleryCon");
            this.ImgGalleryCon.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ImgGalleryCon.Controls.Add(this.ImgGalleryLabel, 0, 0);
            this.ImgGalleryCon.Name = "ImgGalleryCon";
            // 
            // ImgGalleryLabel
            // 
            resources.ApplyResources(this.ImgGalleryLabel, "ImgGalleryLabel");
            this.ImgGalleryLabel.Name = "ImgGalleryLabel";
            // 
            // LiveImgCon
            // 
            resources.ApplyResources(this.LiveImgCon, "LiveImgCon");
            this.LiveImgCon.Name = "LiveImgCon";
            this.LiveImgCon.TabStop = false;
            // 
            // CurrentToolCon
            // 
            resources.ApplyResources(this.CurrentToolCon, "CurrentToolCon");
            this.CurrentToolCon.BackColor = System.Drawing.Color.OrangeRed;
            this.CurrentToolCon.Controls.Add(this.NavigationBtnCon, 0, 1);
            this.CurrentToolCon.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.CurrentToolCon.Name = "CurrentToolCon";
            // 
            // NavigationBtnCon
            // 
            resources.ApplyResources(this.NavigationBtnCon, "NavigationBtnCon");
            this.NavigationBtnCon.Controls.Add(this.BackBtn, 0, 0);
            this.NavigationBtnCon.Controls.Add(this.NextBtn, 1, 0);
            this.NavigationBtnCon.Name = "NavigationBtnCon";
            // 
            // BackBtn
            // 
            resources.ApplyResources(this.BackBtn, "BackBtn");
            this.BackBtn.Name = "BackBtn";
            this.BackBtn.UseVisualStyleBackColor = true;
            this.BackBtn.Click += new System.EventHandler(this.BackBtnClick);
            // 
            // NextBtn
            // 
            resources.ApplyResources(this.NextBtn, "NextBtn");
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtnClick);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.StepLabel, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // StepLabel
            // 
            resources.ApplyResources(this.StepLabel, "StepLabel");
            this.StepLabel.Name = "StepLabel";
            // 
            // GUI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.CurrentToolCon);
            this.Controls.Add(this.LiveImgCon);
            this.Controls.Add(this.ImgGalleryCon);
            this.Name = "GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GUIClosing);
            this.ImgGalleryCon.ResumeLayout(false);
            this.ImgGalleryCon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LiveImgCon)).EndInit();
            this.CurrentToolCon.ResumeLayout(false);
            this.NavigationBtnCon.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel ImgGalleryCon;
        private System.Windows.Forms.Label ImgGalleryLabel;
        private System.Windows.Forms.PictureBox LiveImgCon;
        private System.Windows.Forms.TableLayoutPanel CurrentToolCon;
        private System.Windows.Forms.TableLayoutPanel NavigationBtnCon;
        private System.Windows.Forms.Button BackBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label StepLabel;
    }
}

