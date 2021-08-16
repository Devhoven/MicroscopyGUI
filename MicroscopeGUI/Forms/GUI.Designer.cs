
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
            this.ChangeDirBtn = new System.Windows.Forms.Button();
            this.ImgListCon = new System.Windows.Forms.TableLayoutPanel();
            this.CurrentToolCon = new System.Windows.Forms.TableLayoutPanel();
            this.StepCon = new System.Windows.Forms.TableLayoutPanel();
            this.LocateStepLabel = new System.Windows.Forms.Label();
            this.ConfigStepLabel = new System.Windows.Forms.Label();
            this.AnalysisStepLabel = new System.Windows.Forms.Label();
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.FilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HistogramMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.ImgGalleryCon.SuspendLayout();
            this.CurrentToolCon.SuspendLayout();
            this.StepCon.SuspendLayout();
            this.MenuBar.SuspendLayout();
            this.MainLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImgGalleryCon
            // 
            resources.ApplyResources(this.ImgGalleryCon, "ImgGalleryCon");
            this.ImgGalleryCon.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ImgGalleryCon.Controls.Add(this.ImgGalleryLabel, 0, 1);
            this.ImgGalleryCon.Controls.Add(this.ChangeDirBtn, 0, 2);
            this.ImgGalleryCon.Controls.Add(this.ImgListCon, 0, 3);
            this.ImgGalleryCon.Name = "ImgGalleryCon";
            // 
            // ImgGalleryLabel
            // 
            resources.ApplyResources(this.ImgGalleryLabel, "ImgGalleryLabel");
            this.ImgGalleryLabel.Name = "ImgGalleryLabel";
            // 
            // ChangeDirBtn
            // 
            resources.ApplyResources(this.ChangeDirBtn, "ChangeDirBtn");
            this.ChangeDirBtn.Name = "ChangeDirBtn";
            this.ChangeDirBtn.UseVisualStyleBackColor = true;
            this.ChangeDirBtn.Click += new System.EventHandler(this.ChangeDirBtnClick);
            // 
            // ImgListCon
            // 
            resources.ApplyResources(this.ImgListCon, "ImgListCon");
            this.ImgListCon.Name = "ImgListCon";
            // 
            // CurrentToolCon
            // 
            resources.ApplyResources(this.CurrentToolCon, "CurrentToolCon");
            this.CurrentToolCon.BackColor = System.Drawing.Color.OrangeRed;
            this.CurrentToolCon.Controls.Add(this.StepCon, 0, 0);
            this.CurrentToolCon.Name = "CurrentToolCon";
            // 
            // StepCon
            // 
            resources.ApplyResources(this.StepCon, "StepCon");
            this.StepCon.Controls.Add(this.LocateStepLabel, 1, 0);
            this.StepCon.Controls.Add(this.ConfigStepLabel, 0, 0);
            this.StepCon.Controls.Add(this.AnalysisStepLabel, 2, 0);
            this.StepCon.Name = "StepCon";
            // 
            // LocateStepLabel
            // 
            resources.ApplyResources(this.LocateStepLabel, "LocateStepLabel");
            this.LocateStepLabel.Name = "LocateStepLabel";
            this.LocateStepLabel.Click += new System.EventHandler(this.LocateStepLabel_Click);
            // 
            // ConfigStepLabel
            // 
            resources.ApplyResources(this.ConfigStepLabel, "ConfigStepLabel");
            this.ConfigStepLabel.Name = "ConfigStepLabel";
            this.ConfigStepLabel.Click += new System.EventHandler(this.ConfigStepLabel_Click);
            // 
            // AnalysisStepLabel
            // 
            resources.ApplyResources(this.AnalysisStepLabel, "AnalysisStepLabel");
            this.AnalysisStepLabel.Name = "AnalysisStepLabel";
            this.AnalysisStepLabel.Click += new System.EventHandler(this.AnalysisStepLabel_Click);
            // 
            // MenuBar
            // 
            this.MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FilesMenuItem});
            resources.ApplyResources(this.MenuBar, "MenuBar");
            this.MenuBar.Name = "MenuBar";
            // 
            // FilesMenuItem
            // 
            this.FilesMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HistogramMenuItem,
            this.ExitMenuItem});
            this.FilesMenuItem.Name = "FilesMenuItem";
            resources.ApplyResources(this.FilesMenuItem, "FilesMenuItem");
            // 
            // HistogramMenuItem
            // 
            this.HistogramMenuItem.Name = "HistogramMenuItem";
            resources.ApplyResources(this.HistogramMenuItem, "HistogramMenuItem");
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            resources.ApplyResources(this.ExitMenuItem, "ExitMenuItem");
            // 
            // MainLayout
            // 
            resources.ApplyResources(this.MainLayout, "MainLayout");
            this.MainLayout.Controls.Add(this.ImgGalleryCon, 2, 0);
            this.MainLayout.Controls.Add(this.CurrentToolCon, 0, 0);
            this.MainLayout.Name = "MainLayout";
            // 
            // GUI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.MainLayout);
            this.Controls.Add(this.MenuBar);
            this.MainMenuStrip = this.MenuBar;
            this.Name = "GUI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GUIClosing);
            this.ImgGalleryCon.ResumeLayout(false);
            this.ImgGalleryCon.PerformLayout();
            this.CurrentToolCon.ResumeLayout(false);
            this.StepCon.ResumeLayout(false);
            this.StepCon.PerformLayout();
            this.MenuBar.ResumeLayout(false);
            this.MenuBar.PerformLayout();
            this.MainLayout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel ImgGalleryCon;
        private System.Windows.Forms.Label ImgGalleryLabel;
        private System.Windows.Forms.TableLayoutPanel CurrentToolCon;
        private System.Windows.Forms.MenuStrip MenuBar;
        private System.Windows.Forms.ToolStripMenuItem FilesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.TableLayoutPanel MainLayout;
        private System.Windows.Forms.TableLayoutPanel StepCon;
        private System.Windows.Forms.Label LocateStepLabel;
        private System.Windows.Forms.Label ConfigStepLabel;
        private System.Windows.Forms.Label AnalysisStepLabel;
        private System.Windows.Forms.ToolStripMenuItem HistogramMenuItem;
        private System.Windows.Forms.Button ChangeDirBtn;
        private System.Windows.Forms.TableLayoutPanel ImgListCon;
    }
}

