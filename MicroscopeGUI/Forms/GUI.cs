using System;
using uEye;
using uEye.Defines;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;


namespace MicroscopeGUI
{
    public partial class GUI : Form
    {
        public static Camera Camera;
        public static PictureViewer Display;

        Thread WorkerThread;

        TableLayoutPanel[] Tools;
        int CurrentTool;
        
        public GUI()
        {
            InitializeComponent();

            // GUI Setup
            Display = new PictureViewer();
            MainLayout.Controls.Add(Display, 1, 0);

            // Binding the functionality to the menu bar buttons, since you can't do it in the GUI for some reason
            HistogramMenuItem.Click += new EventHandler(delegate (object o, EventArgs a)
            {
                new HistogramForm().Show();
            });
            ExitMenuItem.Click += new EventHandler(delegate (object o, EventArgs a)
            {
                Close();
            });

            // For debugging the camera
            Status StatusRet;

            // Camera initialization
            Camera = new Camera();
            StatusRet = Camera.Init();

            // Initializing the thread, which runs the image queue
            WorkerThread = new Thread(ImageQueue.Run);

            // Allocating image buffers for 1 second of an image queue
            Camera.Timing.Framerate.Get(out double Framerate);
            for (int i = 0; i < (int)Framerate; i++)
            {
                StatusRet = Camera.Memory.Allocate(out int MemID, false);
                if (StatusRet == Status.Success)
                    Camera.Memory.Sequence.Add(MemID);
            }
            StatusRet = Camera.Memory.Sequence.InitImageQueue();

            // Starting the live feed and the image queue thread
            StatusRet = Camera.Acquisition.Capture();
            WorkerThread.Start();
            
            Tools = new TableLayoutPanel[] { new ConfigStepCon(), new LocateStepCon(), new AnalysisStepCon() };
            CurrentToolCon.Controls.Add(Tools[0]);
        }

        // Highlighting the different labels 
        private void ConfigStepLabel_Click(object sender, EventArgs e) =>
            HighlightLabel(ConfigStepLabel, 0);
        private void LocateStepLabel_Click(object sender, EventArgs e) =>
            HighlightLabel(LocateStepLabel, 1);
        private void AnalysisStepLabel_Click(object sender, EventArgs e) =>
            HighlightLabel(AnalysisStepLabel, 2);

        void HighlightLabel(Label CurrentLabel, int NextTool)
        {
            ConfigStepLabel.TextAlign = ContentAlignment.BottomCenter;
            LocateStepLabel.TextAlign = ContentAlignment.BottomCenter;
            AnalysisStepLabel.TextAlign = ContentAlignment.BottomCenter;
            CurrentLabel.TextAlign = ContentAlignment.MiddleCenter;
            CurrentToolCon.Controls.Remove(Tools[CurrentTool]);
            CurrentTool = NextTool;
            CurrentToolCon.Controls.Add(Tools[CurrentTool]);
        }

        private void ChangeDirBtnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog BrowserDialog = new FolderBrowserDialog();
            if (BrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // The selected directory
                string SelectedDir = BrowserDialog.SelectedPath;
                // The file paths from the selected directory
                string[] FilePaths = System.IO.Directory.GetFiles(SelectedDir);
                // Filtering and loading the images from the paths
                LoadImagesFromFolder(FilePaths);
            }
        }

        private void InitializeImgGallery()
        {
            ImgListCon.Controls.Clear();
            ImgListCon.RowStyles.Clear();
            ImgListCon.AutoScroll = true;
        }

        private void LoadImagesFromFolder(string[] Paths)
        {
            InitializeImgGallery();

            int TablePos = 1;
            foreach(string Path in Paths)
            {
                if (Path.EndsWith(".jpg") | Path.EndsWith(".png"))
                {
                    // Creating a new PictureBox for each row of the tableLayoutPanel
                    Bitmap CurrentImage = new Bitmap(Path);
                    PictureBox NewImgCon = new PictureBox
                    {
                        Image = CurrentImage,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Anchor = AnchorStyles.Left | AnchorStyles.Top,
                        Name = Path.Substring(Path.LastIndexOf("\\")).Remove(0, 1),
                        Margin = new Padding(4, 0, 0, 4),
                        Size = new System.Drawing.Size(160, 160)
                    };
                    
                    // Binding the doubleclick event
                    NewImgCon.MouseDoubleClick += PicBoxClicked;

                    ImgListCon.Controls.Add(NewImgCon, 0, TablePos++);
                }
            }
        }

        private void PicBoxClicked(object sender, EventArgs e)
        {
            // TODO: actions when img is double clicked
        }

        // Closes the ImageQueue Thread and the camera correctly
        private void GUIClosing(object sender, FormClosingEventArgs e)
        {
            ImageQueue.StopRunning = true;
            WorkerThread.Join();
            Camera.Exit();
        }
    }
}
