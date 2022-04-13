using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MicroscopeGUI
{
    // Base is stolen from https://stackoverflow.com/a/6782715
    // Modified it, so we can draw on it
    // Removed all of the if (_Child == null) querys, since it never will be null in this case
    public partial class ImageViewer : Border
    {
        Canvas _Child = null;
        // How many childs are in the canvas, except the image
        int ChildCount;

        bool AlternativeMeasureMode = false;
        MeasureMode CurrentMode = MeasureMode.Rectangle;
        public enum MeasureMode
        {
            Rectangle,
            Line,
            MeasureFactor
        }

        // Means unit per pixel
        double PixelPerMeasurement = 1;

        Point TransformOrigin;
        Point TransformStart;

        Point DrawStart;

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            if (element == null)
                throw new ArgumentException("The parent cannot be null [ImageViewer]");

            _Child = element as Canvas;

            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            _Child.RenderTransform = group;
            _Child.RenderTransformOrigin = new Point(0.0, 0.0);

            ChildCount = 0;

            InitializeContextMenu();

            MouseDown += ChildMouseDown;
            MouseMove += ChildMouseMove;
            MouseUp += ChildMouseUp;
            MouseWheel += ChildMouseWheel;

            (Application.Current.MainWindow as UI).KeyDown += ChildKeyDown;
            (Application.Current.MainWindow as UI).KeyUp += ChildKeyUp;
        }

        void InitializeContextMenu()
        {
            ContextMenu = new ContextMenu();

            MenuItem ResetImageItem = new MenuItem()
            {
                Header = "Reset Image Viewer",
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Restart.png"))
                }
            };
            MenuItem RectangleMeasureItem = new MenuItem()
            {
                Header = "Measure with a rectangle",
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Rectangle.png"))
                }
            };
            MenuItem LineMeasureItem = new MenuItem()
            {
                Header = "Measure with a line",
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Line.png"))
                }
            };
            MenuItem MeasureFactorItem = new MenuItem()
            {
                Header = "Measure the factor",
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Measure.png"))
                }
            };

            ResetImageItem.Click += (o, e) 
                => Reset();

            RectangleMeasureItem.Click += (o, e) =>
            {
                UserInfo.SetInfo("You can now measure with a rectangle");
                SetMeasureMode(MeasureMode.Rectangle);
            };

            LineMeasureItem.Click += (o, e) =>
            {
                UserInfo.SetInfo("You can now measure with a line");
                SetMeasureMode(MeasureMode.Line);
            };

            MeasureFactorItem.Click += (o, e) =>
            {
                UserInfo.SetInfo("Draw a line where you want to measure");
                SetMeasureMode(MeasureMode.MeasureFactor);
            };
            
            ContextMenu.Items.Add(ResetImageItem);
            ContextMenu.Items.Add(new Separator());
            ContextMenu.Items.Add(RectangleMeasureItem);
            ContextMenu.Items.Add(LineMeasureItem);
            ContextMenu.Items.Add(MeasureFactorItem);
        }

        public void Reset()
        {
            if (_Child != null)
            {
                // reset zoom
                var st = GetScaleTransform(_Child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(_Child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        public void SetMeasureMode(MeasureMode NewMode) 
            => CurrentMode = NewMode;
    }
}
