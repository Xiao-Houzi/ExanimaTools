using System;
using Avalonia.Data.Converters;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System.Globalization;
using ExanimaTools.Models;

namespace ExanimaTools.Controls
{
    public class EquipmentTypeToDrawingConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EquipmentType type)
            {
                if (type == EquipmentType.Weapon)
                {
                    // Sword Drawing
                    var canvas = new Canvas { Width = 128, Height = 256 };
                    canvas.Children.Add(new Line
                    {
                        StartPoint = new Avalonia.Point(64, 32),
                        EndPoint = new Avalonia.Point(64, 200),
                        Stroke = Brushes.White,
                        StrokeThickness = 6
                    });
                    canvas.Children.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = 56d,
                        [Canvas.TopProperty] = 200d,
                        Width = 16,
                        Height = 32,
                        Fill = new SolidColorBrush(Color.Parse("#FFD7B377")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    canvas.Children.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = 44d,
                        [Canvas.TopProperty] = 192d,
                        Width = 40,
                        Height = 12,
                        Fill = new SolidColorBrush(Color.Parse("#FF888888")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    canvas.Children.Add(new Ellipse
                    {
                        [Canvas.LeftProperty] = 58d,
                        [Canvas.TopProperty] = 16d,
                        Width = 12,
                        Height = 12,
                        Fill = new SolidColorBrush(Color.Parse("#FFCCCCCC")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    return canvas;
                }
                else if (type == EquipmentType.Armour)
                {
                    // Armour Drawing
                    var canvas = new Canvas { Width = 128, Height = 256 };
                    canvas.Children.Add(new Ellipse
                    {
                        [Canvas.LeftProperty] = 52d,
                        [Canvas.TopProperty] = 24d,
                        Width = 24,
                        Height = 24,
                        Fill = new SolidColorBrush(Color.Parse("#FFCCCCCC")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    canvas.Children.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = 44d,
                        [Canvas.TopProperty] = 48d,
                        Width = 40,
                        Height = 60,
                        Fill = new SolidColorBrush(Color.Parse("#FFB0B0B0")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    canvas.Children.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = 36d,
                        [Canvas.TopProperty] = 108d,
                        Width = 16,
                        Height = 56,
                        Fill = new SolidColorBrush(Color.Parse("#FFB0B0B0")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    canvas.Children.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = 76d,
                        [Canvas.TopProperty] = 108d,
                        Width = 16,
                        Height = 56,
                        Fill = new SolidColorBrush(Color.Parse("#FFB0B0B0")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    canvas.Children.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = 52d,
                        [Canvas.TopProperty] = 164d,
                        Width = 12,
                        Height = 48,
                        Fill = new SolidColorBrush(Color.Parse("#FF888888")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    canvas.Children.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = 64d,
                        [Canvas.TopProperty] = 164d,
                        Width = 12,
                        Height = 48,
                        Fill = new SolidColorBrush(Color.Parse("#FF888888")),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    return canvas;
                }
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
