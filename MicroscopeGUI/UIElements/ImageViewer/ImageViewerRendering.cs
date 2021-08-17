using System;
using System.IO;
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
    public partial class ImageViewer
    {
        // Renders a rectangle out of a point and two positive lengths
        void RenderRectangle(double X, double Y, double Width, double Height)
        {
            // If the width and height are zero => The user only clicked on the canvas
            // No new rectangle / text is added, thus "removing" the old rectangle
            if (Width == 0 && Height == 0)
                return;

            double SizeFactor = GetScreenToPixelFactor();

            if (AlternativeMeasureMode)
            {
                if (Width < Height)
                    Height = Width;
                else
                    Width = Height;
            }

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
                Text = Math.Round(PixelWidth * PixelPerMeasurement, 2).ToString() + " mm",
                FontSize = 20,
                Foreground = Settings.LineTextColor
            };
            TextBlock HeightDisplay = new TextBlock()
            {
                Text = Math.Round(PixelHeight * PixelPerMeasurement, 2).ToString() + " mm",
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

        // Renders a line between two points and shows the length
        void RenderLineWithLength(Point p1, Point p2)
        {
            RenderLine(p1, p2);

            double SizeFactor = GetScreenToPixelFactor();
            TextBlock LengthDisplay = new TextBlock()
            {

                Text = Math.Round((p2 - p1).Length * SizeFactor * PixelPerMeasurement, 2).ToString() + " mm",
                FontSize = 20,
                Foreground = Settings.LineTextColor
            };

            Size RenderedSize = LengthDisplay.GetElementPixelSize();
            Canvas.SetLeft(LengthDisplay, p1.X - RenderedSize.Width / 2);
            if (p1.Y < p2.Y)
                Canvas.SetTop(LengthDisplay, p1.Y - LengthDisplay.FontSize - 10);
            else
                Canvas.SetTop(LengthDisplay, p1.Y);

            _Child.Children.Add(LengthDisplay);

            ChildCount++;
        }

        // Renders a line between two points
        void RenderLine(Point p1, Point p2)
        {
            if (p1.Equals(p2))
                return;

            if (AlternativeMeasureMode)
            {
                // Just checks in what region the mouse is relative to the old point and straightens it out
                Vector Between = p2 - p1;
                Between.Normalize();
                double Angle = Math.Abs(Vector.AngleBetween(Between, new Vector(1, 0)));
                if (Angle < 45 || Angle > 135)
                    p2.Y = p1.Y;
                else
                    p2.X = p1.X;
            }

            Line Line = new Line()
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = Settings.LineColor,
                StrokeThickness = Settings.LineThickness
            };
            // Disables aliasing on the line 
            RenderOptions.SetEdgeMode(Line, EdgeMode.Aliased);

            _Child.Children.Add(Line);

            ChildCount = 1;
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

        // Calculates the factor which converts the "screen" lengths to actual pixel lengths
        double GetScreenToPixelFactor()
        {
            double ActualWidth = (double)_Child.Children[0].GetValue(Image.ActualWidthProperty);
            return ImageQueue.Width / ActualWidth;
        }

        // Removing the old elements, except the image 
        void RemoveChilds()
        {
            _Child.Children.RemoveRange(1, ChildCount);
            ChildCount = 0;
        }
    }
}
