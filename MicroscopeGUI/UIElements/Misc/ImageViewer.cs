using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MicroscopeGUI
{
    // Base is stolen from https://stackoverflow.com/a/6782715
    // Modified it, so we can draw on it
    // Removed all of the if (_Child == null) querys, since it never will be null in this case
    public class ImageViewer : Border
    {
        Canvas _Child = null;
        // How many childs are in the canvas, except the image
        int ChildCount;

        MeasureMode CurrentMode = MeasureMode.Rectangle;
        enum MeasureMode
        {
            Rectangle,
            MeasureFactor
        }

        // Means unit per pixel
        double PixelPerMeasurement = 2;

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
            _Child = element as Canvas;

            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            _Child.RenderTransform = group;
            _Child.RenderTransformOrigin = new Point(0.0, 0.0);

            ChildCount = 0;

            MouseDown += ChildMouseDown;
            MouseUp += ChildMouseUp;
            MouseWheel += ChildMouseWheel;
            MouseMove += ChildMouseMove;
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

        public void ToggleMode()
        {
            if (CurrentMode == MeasureMode.Rectangle)
                CurrentMode = MeasureMode.MeasureFactor;
            else
                CurrentMode = MeasureMode.Rectangle;
        }

        #region Child Events
        private void ChildMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var tt = GetTranslateTransform(_Child);
                TransformStart = e.GetPosition(this);
                TransformOrigin = new Point(tt.X, tt.Y);
                Cursor = Cursors.Hand;
                _Child.CaptureMouse();
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                var tt = GetTranslateTransform(_Child);
                DrawStart = e.GetPosition(_Child);
                Cursor = Cursors.Hand;
                _Child.CaptureMouse();
            }
            else if (e.ChangedButton == MouseButton.Right)
                Reset();
        }

        private void ChildMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _Child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                _Child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;

                if (CurrentMode == MeasureMode.MeasureFactor)
                {
                    FactorInputBox InputDialog = new FactorInputBox();
                    InputDialog.ShowDialog();

                    // If the bool is set to true, the action was aborted
                    if (!InputDialog.Aborted)
                    {
                        double Measurement = double.Parse(InputDialog.InputBox.Text);

                        // Returns the position of the mouse relative to the image
                        Point CurrentMousePos = e.GetPosition(_Child);

                        double SizeFactor = GetScreenToPixelFactor();
                        int PixelLength = (int)Math.Round((CurrentMousePos - DrawStart).Length * SizeFactor);

                        PixelPerMeasurement = PixelLength / Measurement;
                    }

                    // Removing the old elements, except the image, since we don't need the line anymore
                    RemoveChilds();
                }
            }
        }

        private void ChildMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Returns the position of the mouse relative to the image
                Point CurrentMousePos = e.GetPosition(_Child);

                RemoveChilds();

                if (CurrentMode == MeasureMode.Rectangle)
                {
                    // Returns the parameters of a rectangle with a positive size (is required for a rectangle)
                    (double X, double Y, double Width, double Height) = GetRectangle(DrawStart, CurrentMousePos);
                    RenderRectangle(X, Y, Width, Height);
                }
                else if(CurrentMode == MeasureMode.MeasureFactor)
                {
                    RenderLine(DrawStart, CurrentMousePos);
                }
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                var tt = GetTranslateTransform(_Child);
                Vector v = TransformStart - e.GetPosition(this);
                tt.X = TransformOrigin.X - v.X;
                tt.Y = TransformOrigin.Y - v.Y;
            }
        }

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
        #endregion

        void RenderRectangle(double X, double Y, double Width, double Height)
        {
            // If the width and height are zero => The user only clicked on the canvas
            // No new rectangle / text is added, thus "removing" the old rectangle
            if (Width == 0 && Height == 0)
                return;

            double SizeFactor = GetScreenToPixelFactor();

            // The values that are actually shown on the screen
            int PixelWidth = (int)Math.Round(Width * SizeFactor);
            int PixelHeight = (int)Math.Round(Height * SizeFactor);

            Rectangle Rect = new Rectangle()
            {
                Width = Width,
                Height = Height,
                Stroke = Settings.LineColor,
                StrokeThickness = Settings.LineThickness
            };
            // Disables aliasing on the rectangle :)
            RenderOptions.SetEdgeMode(Rect, EdgeMode.Aliased);
            TextBlock WidthDisplay = new TextBlock()
            {
                Text = Math.Round(PixelWidth * PixelPerMeasurement, 2).ToString(),
                FontSize = 20,
                Foreground = Settings.LineTextColor
            };
            TextBlock HeightDisplay = new TextBlock()
            {
                Text = Math.Round(PixelHeight * PixelPerMeasurement, 2).ToString(),
                FontSize = 20,
                Foreground = Settings.LineTextColor
            };
            // Rotates the height text 90 degrees
            HeightDisplay.LayoutTransform = new RotateTransform(90);

            // Adding the newly created shapes to the canvas
            _Child.Children.Add(Rect);
            _Child.Children.Add(WidthDisplay);
            _Child.Children.Add(HeightDisplay);

            // Sets the render location of the new elements on the canvas
            Canvas.SetLeft(Rect, X);
            Canvas.SetTop(Rect, Y);

            // Sets the position of the text blocks
            Size RenderedSize = WidthDisplay.GetElementPixelSize();
            Canvas.SetLeft(WidthDisplay, X + Width / 2 - RenderedSize.Width / 2);
            Canvas.SetTop(WidthDisplay, Y - WidthDisplay.FontSize - Rect.StrokeThickness - 3);

            RenderedSize = HeightDisplay.GetElementPixelSize();
            Canvas.SetLeft(HeightDisplay, X + Width);
            Canvas.SetTop(HeightDisplay, Y + Height / 2 - RenderedSize.Width / 2);

            ChildCount = 3;
        }

        void RenderLine(Point p1, Point p2)
        {
            if (p1.Equals(p2))
                return;

            Line Line = new Line()
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = Settings.LineColor,
                StrokeThickness = Settings.LineThickness
            };

            _Child.Children.Add(Line);

            ChildCount = 1;
        }

        // Removing the old elements, except the image 
        void RemoveChilds()
        {
            _Child.Children.RemoveRange(1, ChildCount);
            ChildCount = 0;
        }

        // Constructs a rectangle out of two points
        (double, double, double, double) GetRectangle(Point p1, Point p2)
        {
            double Width = p2.X - p1.X;
            double Height = p2.Y - p1.Y;
            if (Width < 0)
                p1.X = p2.X;
            if (Height < 0)
                p1.Y = p2.Y;

            return (p1.X, p1.Y, Math.Abs(Width), Math.Abs(Height));
        }

        double GetScreenToPixelFactor()
        {
            // Calculates the factor which converts the "screen" lengths to actual pixel lengths
            double ActualWidth = (double)_Child.Children[0].GetValue(Image.ActualWidthProperty);
            return ImageQueue.Width / ActualWidth;
        }
    }
}
