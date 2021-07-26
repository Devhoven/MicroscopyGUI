using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MicroscopeGUI
{
    // Stolen from https://stackoverflow.com/a/6782715
    // Modified it, so we can draw on it
    public class ImageViewer : Border
    {
        private Canvas _Child = null;
        private Point Origin;
        private Point Start;
        private bool ViewMode = true;

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
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this._Child = element as Canvas;
            if (_Child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                _Child.RenderTransform = group;
                _Child.RenderTransformOrigin = new Point(0.0, 0.0);

                this.MouseWheel += ChildMouseWheel;
                this.MouseLeftButtonDown += ChildMouseLeftButtonDown;
                this.MouseLeftButtonUp += ChildMouseLeftButtonUp;
                this.MouseMove += ChildMouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(
                  ChildPreviewMouseRightButtonDown);
            }
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

        #region Child Events

        private void ChildMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_Child != null)
            {
                var st = GetScaleTransform(_Child);
                var tt = GetTranslateTransform(_Child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(_Child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * st.ScaleX + tt.X;
                absoluteY = relative.Y * st.ScaleY + tt.Y;

                double zoomCorrected = zoom * st.ScaleX;
                st.ScaleX += zoomCorrected;
                st.ScaleY += zoomCorrected;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
            }
        }

        private void ChildMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_Child != null)
            {
                var tt = GetTranslateTransform(_Child);
                Start = e.GetPosition(this);
                Origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                _Child.CaptureMouse();
            }
        }

        private void ChildMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_Child != null)
            {
                _Child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void ChildPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewMode = !ViewMode;
            //this.Reset();
        }

        private void ChildMouseMove(object sender, MouseEventArgs e)
        {
            if (_Child != null)
            {
                if (_Child.IsMouseCaptured)
                {
                    if (ViewMode)
                    {
                        var tt = GetTranslateTransform(_Child);
                        Vector v = Start - e.GetPosition(this);
                        tt.X = Origin.X - v.X;
                        tt.Y = Origin.Y - v.Y;
                    }
                    else
                    {
                        var tt = GetTranslateTransform(_Child);
                        Point pos = e.GetPosition(this);
                        pos.X = 0;
                        pos.Y = 0;
                        Line l = new Line()
                        {
                            X1 = pos.X,
                            Y1 = pos.Y,

                            X2 = pos.X + 1,
                            Y2 = pos.Y + 1,

                            Stroke = Brushes.Red,
                            StrokeThickness = 20
                        };

                        _Child.Children.RemoveAt(1);
                        _Child.Children.Add(l);
                    }
                }
            }
        }

        #endregion
    }
}
