using System;
using System.Windows;
using System.Windows.Input;

namespace MicroscopeGUI
{
    public partial class ImageViewer
    {
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
                DrawStart = e.GetPosition(_Child);
                Cursor = Cursors.Hand;
                _Child.CaptureMouse();
            }
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
                // Returns the position of the mouse relative to the image
                // Has to be saved here, otherwise it will take the position of after the input box xD
                Point CurrentMousePos = e.GetPosition(_Child);

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

                        double SizeFactor = GetScreenToPixelFactor();
                        int PixelLength = (int)Math.Round((CurrentMousePos - DrawStart).Length * SizeFactor);

                        PixelPerMeasurement = Measurement / PixelLength;

                        CurrentMode = MeasureMode.Rectangle;

                        UserInfo.SetInfo("You can now measure again");
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
                else if (CurrentMode == MeasureMode.Line)
                {
                    RenderLineWithLength(DrawStart, CurrentMousePos);
                }
                else if (CurrentMode == MeasureMode.MeasureFactor)
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

        private void ChildKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
                Reset();
            else if (e.Key == Key.LeftShift)
                AlternativeMeasureMode = true;
        }

        private void ChildKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                AlternativeMeasureMode = false;
        }
    }
}
