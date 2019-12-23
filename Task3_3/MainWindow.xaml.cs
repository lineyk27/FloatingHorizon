using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Globalization;

namespace Task3_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double LowBound = -6.28;
        double UpBound = 6.28;

        Color col = Colors.Green;
        bool Pointable = false;
        int CountHorizons = 10;
        int CountPoints = 10;
        double Thickness = 1.5;

        GeometryGroup group = new GeometryGroup();

        Line[,] Lines;
        double StepX
        {
            get
            {
                return (UpBound - LowBound) / CountPoints;
            }
        }
        double StepZ
        {
            get
            {
                return (UpBound - LowBound) / CountHorizons;
            }
        }
        int CenterX
        {
            get
            {
                return (int)Paint.Width / 2;
            }
        }
        int CenterY
        {
            get
            {
                return (int)Paint.Height / 2 - 70;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Lines = new Line[CountPoints, CountHorizons];
        }
        public Brush Stroke(Color col, int i = 4)
        {
            var stroke = new SolidColorBrush(col);
            stroke.Opacity = 10;
            return stroke;
        }
        public void GenerateLines()
        {
            int size = 48;
            int k = 0;
            int l = 0;

            double magic = 0.0;
            double m_step = 0;

            double added = (CountPoints % 2 == 0) ? StepX : 0;

            double startx, starty;
            for(double z = LowBound; z < UpBound; z += StepZ)
            {
                startx = LowBound;
                starty = func(LowBound, z);
                l = 0;
                for (double x = LowBound + StepX; x <= UpBound + StepX; x += StepX)
                {
                    var line = new Line();
                    line.StrokeThickness = Thickness;
                    line.X1 = startx * size + CenterX + magic;
                    line.Y1 = starty * size + CenterY + magic;

                    line.X2 = x * size + CenterX + magic;
                    line.Y2 = func(x,z) * size + CenterY + magic;

                    //Debug.WriteLine($"k = {k}, l = {l}");

                    Lines[l, k] = line;
                    //Paint.Children.Add(line);
                    startx = x ;
                    starty = func(x, z);
                    l += 1;
                }
                k += 1;
                magic += m_step;
            }
        }

        public void Draw()
        {
            GenerateLines();
            DrawLines();
        }

        public void DrawLines()
        {
            for(int i = 0; i < CountPoints; i++)
            {
                Line last = null;
                Line min = null;
                for(int j = 0; j < CountHorizons; j++)
                {
                    //Debug.WriteLine($"drawlines: i = {i}, j = {j}");
                    var current = Lines[i, j];
                    if(j == 0)
                    {
                        last = current;
                        min = last;
                    }
                    if(Middle(current) >= Middle(last))
                    {
                        current.Stroke = Stroke(col);
                        //Paint.Children.Add(current);
                        DrawUp(current, last);
                        last = current;
                    }
                    else if(Middle(current) <= Middle(last) && (Middle(current) <= Middle(min)))
                    {
                        current.Stroke = Stroke(col);
                        //Paint.Children.Add(current);
                        DrawDown(min, current);
                        min = current;
                    }
                    else if (CheckIntersect(current, last))
                    {
                        DrawUp(current, last);
                    }
                    else if (CheckIntersect(min, current))
                    {
                        DrawDown(min, current);
                    }
                }
            }
        }
        public bool CheckIntersect(Line p1, Line p2)
        {
            bool res = false;
            if(p1.Y1 > p2.Y1 && p1.Y2 < p2.Y2)
            {
                res = true;
            }
            else if(p1.Y1 < p2.Y1 && p1.Y2 > p2.Y2)
            {
                res = true;
            }
            return res;
        }
        public void DrawDown(Line up, Line down)
        {
            if (!CheckIntersect(up, down))
            {
                if (!Paint.Children.Contains(up))
                {
                    Paint.Children.Add(up);
                }
                if (!Paint.Children.Contains(down))
                {
                    Paint.Children.Add(down);
                }
            }
            else
            {
                var p = LinesIntersection(up, down);

                //DrawPoint(p, Colors.Blue);
                var line = new Line();
                if (up.Y1 > p.Y)
                {
                    line.X1 = down.X1;
                    line.Y1 = down.Y1;
                    DrawPoint(p, Colors.Aquamarine);
                    line.Stroke = Stroke(Colors.Red);

                    line.X2 = p.X;
                    line.Y2 = p.Y;
                }
                else
                {
                    line.X1 = down.X2;
                    line.Y1 = down.Y2;
                    DrawPoint(p, Colors.White);
                    line.Stroke = Stroke(Colors.White);

                    line.X2 = p.X;
                    line.Y2 = p.Y;
                }
                Paint.Children.Add(line);
            }
        }
        public void DrawUp(Line up, Line down)
        {

            if (!CheckIntersect(up, down))
            {
                if (!Paint.Children.Contains(up))
                {
                    Paint.Children.Add(up);
                }
                if (!Paint.Children.Contains(down))
                {
                    Paint.Children.Add(down);
                }
            }
            else
            {
                var p = LinesIntersection(up, down);

                //DrawPoint(p, Colors.Red);
                var line = new Line();
                if (up.Y1 > p.Y) {
                    line.X1 = up.X1;
                    line.Y1 = up.Y1;
                    DrawPoint(p, Colors.Cyan);
                    line.Stroke = Stroke(Colors.Yellow);

                    line.X2 = p.X;
                    line.Y2 = p.Y;
                }
                else
                {
                    line.X1 = up.X2;
                    line.Y1 = up.Y2;
                    DrawPoint(p, Colors.Brown);
                    line.Stroke = Stroke(Colors.Blue);

                    line.X2 = p.X;
                    line.Y2 = p.Y;
                }
                Paint.Children.Add(line);

            }
        }
        public void DrawPoint(Point p, Color color)
        {
            if(Double.IsNaN(p.X) || Double.IsInfinity(p.X)
                || Double.IsNaN(p.Y) || Double.IsInfinity(p.Y))
            {
                return;
            }
            var line = new Line();
            line.X1 = p.X;
            line.X2 = p.X + 2;
            line.Y1 = p.Y;
            line.Y2 = p.Y + 2;
            line.Stroke = new SolidColorBrush(color);
            line.StrokeThickness = 10;
            if (Pointable)
            {
                Paint.Children.Add(line);
            }
        }
        public Point LinesIntersection(Line first, Line second)
        {
            Point result = new Point();

            double xn = first.X1;
            double xnk = first.X2;

            double yn = first.Y1;
            double ynk = first.Y2;

            double ynp = second.Y1;
            double ync = first.Y1;

            double deltax = xnk - xn;
            double deltayc = first.Y2 - first.Y1;
            double deltayp = second.Y2 - second.Y1;
            double m = ((int)(ynk - yn)) / deltax;

            double x = xn - (deltax * (ynp - ync)) / (deltayp-deltayc);
            double y = m * (x - xn) + yn;
            result.Y = y;
            result.X = x;
            return result;
        }
        public double Middle(Line line)
        {
            return Math.Abs(line.Y2 + line.Y1) / 2;
        }
        public double R(double x, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));
        }
        public double func(double x, double z)
        {
            return 8 * Math.Cos(1.2 * R(x, z))/(R(x, z) + 1);
        }
        private void Paint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Draw();
        }
    }
}
