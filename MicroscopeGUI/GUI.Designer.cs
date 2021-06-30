
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
            this.CurrentToolCon = new System.Windows.Forms.TableLayoutPanel();
            this.NavigationBtnCon = new System.Windows.Forms.TableLayoutPanel();
            this.BackBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.StepLabelCon = new System.Windows.Forms.TableLayoutPanel();
            this.StepLabel = new System.Windows.Forms.Label();
            this.PreviousStepLabel = new System.Windows.Forms.Label();
            this.NextStepLabel = new System.Windows.Forms.Label();
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.FilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.ImgGalleryCon.SuspendLayout();
            this.CurrentToolCon.SuspendLayout();
            this.NavigationBtnCon.SuspendLayout();
            this.StepLabelCon.SuspendLayout();
            this.MenuBar.SuspendLayout();
            this.MainLayout.SuspendLayout();
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
            // CurrentToolCon
            // 
            resources.ApplyResources(this.CurrentToolCon, "CurrentToolCon");
            this.CurrentToolCon.BackColor = System.Drawing.Color.OrangeRed;
            this.CurrentToolCon.Controls.Add(this.NavigationBtnCon, 0, 1);
            this.CurrentToolCon.Controls.Add(this.StepLabelCon, 0, 0);
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
            // StepLabelCon
            // 
            resources.ApplyResources(this.StepLabelCon, "StepLabelCon");
            this.StepLabelCon.Controls.Add(this.StepLabel, 1, 0);
            this.StepLabelCon.Controls.Add(this.PreviousStepLabel, 0, 0);
            this.StepLabelCon.Controls.Add(this.NextStepLabel, 2, 0);
            this.StepLabelCon.Name = "StepLabelCon";
            // 
            // StepLabel
            // 
            resources.ApplyResources(this.StepLabel, "StepLabel");
            this.StepLabel.Name = "StepLabel";
            // 
            // PreviousStepLabel
            // 
            resources.ApplyResources(this.PreviousStepLabel, "PreviousStepLabel");
            this.PreviousStepLabel.Name = "PreviousStepLabel";
            // 
            // NextStepLabel
            // 
            resources.ApplyResources(this.NextStepLabel, "NextStepLabel");
            this.NextStepLabel.Name = "NextStepLabel";
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
            this.ExitMenuItem});
            this.FilesMenuItem.Name = "FilesMenuItem";
            resources.ApplyResources(this.FilesMenuItem, "FilesMenuItem");
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
            this.NavigationBtnCon.ResumeLayout(false);
            this.StepLabelCon.ResumeLayout(false);
            this.StepLabelCon.PerformLayout();
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
        private System.Windows.Forms.TableLayoutPanel NavigationBtnCon;
        private System.Windows.Forms.Button BackBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.TableLayoutPanel StepLabelCon;
        private System.Windows.Forms.Label StepLabel;
        private System.Windows.Forms.Label PreviousStepLabel;
        private System.Windows.Forms.Label NextStepLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.MenuStrip MenuBar;
        private System.Windows.Forms.ToolStripMenuItem FilesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.TableLayoutPanel MainLayout;
    }
}

