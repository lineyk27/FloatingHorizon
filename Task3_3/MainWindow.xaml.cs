using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

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
        //bool DebugOn = false;
        int CountHorizons = 6;
        int CountPoints = 14;
        double Thickness = 1.5;

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
        public Brush Stroke(Color col, int? horizon=null)
        {
            if (horizon != null)
            {
                int start = 50;
                byte g = (byte)(start + ((int)horizon * ((255-start)/CountHorizons)));
                byte r = 0;
                byte b = 0;
                Debug.WriteLine($"r-{r}, g-{g}, b-{b}, horizon-{horizon}");
                col = Color.FromRgb(r, g, b);
            }
            var stroke = new SolidColorBrush(col);
            stroke.Opacity = 10;
            return stroke;
        }
        public void GenerateLines()
        {
            int size = 53;
            int k = 0;
            int l;

            double magic = 0.0;
            double m_step = 0;

            double startx, starty;
            for(double z = LowBound; z <= UpBound + StepZ && k < CountHorizons; z += StepZ)
            {
                startx = LowBound;
                starty = func(LowBound, z);
                l = 0;
                for (double x = LowBound + StepX; x <= UpBound + StepX && l < CountPoints; x += StepX)
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
            Line[,] lines = MakeIntersected();
            //DrawLines();
            Draws(lines);
        }

        public Line[,] MakeIntersected()
        {
            Line[,] lines = new Line[CountPoints, CountHorizons];
            for (int i = 0; i < CountPoints; i++)
            {
                Line last = null;
                Line min = null;
                for (int j = 0; j < CountHorizons; j++)
                {
                    var current = Lines[i, j];
                    if (j == 0)
                    {
                        last = current;
                        min = last;
                    }
                    if (Middle(current) >= Middle(last) && !CheckIntersect(last, current))
                    {
                        last = current;
                        lines[i, j] = current;
                        lines[i, j].Stroke = Stroke(col);
                    }
                    else if ((Middle(current) < Middle(min)) && !CheckIntersect(min, current))
                    {
                        min = current;
                        lines[i, j] = current;
                        lines[i, j].Stroke = Stroke(col);
                    }
                    else if (CheckIntersect(last, current))
                    {
                        lines[i,j] = GetIntersection(last, current);
                        last = lines[i, j];
                        //lines[i, j].Stroke = Stroke(col);
                    }
                    else if (CheckIntersect(min, current))
                    {
                        lines[i, j] = GetIntersection(min, current, low:true);
                        min = lines[i, j];
                        //lines[i, j].Stroke = Stroke(col);
                    }
                    else 
                    {
                        lines[i, j] = null;
                    }
                }
            }
            return lines;
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
                    if(Middle(current) >= Middle(last) && !CheckIntersect(last, current))
                    {
                        current.Stroke = Stroke(col);
                        //Paint.Children.Add(current);
                        DrawUp(current, last);
                        last = current;
                    }
                    else if(/*Middle(current) <= Middle(last) &&*/(Middle(current) < Middle(min)) && !CheckIntersect(min, current))
                    {
                        current.Stroke = Stroke(col);
                        //Paint.Children.Add(current);
                        DrawDown(min, current);
                        min = current;
                    }
                    else if (CheckIntersect(last, current))
                    {
                        DrawIntersected(last, current);
                    }
                    else if (CheckIntersect(min, current))
                    {
                        DrawIntersected(min, current, low:true);
                    }
                }
            }
        }
        public void Draws(Line[,] lines)
        {
            for (int i = 0; i < CountPoints; i++)
            {
                for (int j = 0; j < CountHorizons; j++)
                {
                    if (lines[i, j] != null)
                    {
                        Paint.Children.Add(lines[i, j]);
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

        public Line GetIntersection(Line upper, Line lowwer, bool low = false)
        {
            var p = LinesIntersection(upper, lowwer);
            DrawPoint(p, Colors.White);

            var line = new Line();
            line.X1 = p.X;
            line.Y1 = p.Y;
            if (low)
            {
                if (p.Y > lowwer.Y2)
                {
                    line.X2 = lowwer.X2;
                    line.Y2 = lowwer.Y2;
                    line.Stroke = Stroke(Colors.White);
                }
                else
                {
                    line.X2 = lowwer.X1;
                    line.Y2 = lowwer.Y1;
                    line.Stroke = Stroke(Colors.Tomato);
                }
            }
            else
            {
                if (p.Y < lowwer.Y2)
                {
                    line.X2 = lowwer.X2;
                    line.Y2 = lowwer.Y2;
                    line.Stroke = Stroke(Colors.Pink);
                }
                else
                {
                    line.X2 = lowwer.X1;
                    line.Y2 = lowwer.Y1;
                    line.Stroke = Stroke(Colors.Orange);
                }
            }
            line.Stroke = Stroke(Colors.Green);
            return line;
        }

        public void DrawIntersected(Line upper, Line lowwer, bool low = false)
        {
            var p = LinesIntersection(upper, lowwer);

            DrawPoint(p, Colors.White);

            var line = new Line();
            line.X1 = p.X;
            line.Y1 = p.Y;
            if (low)
            {
                if(p.Y > lowwer.Y2)
                {
                    line.X2 = lowwer.X2;
                    line.Y2 = lowwer.Y2;
                    line.Stroke = Stroke(col);
                }
                else
                {
                    line.X2 = lowwer.X1;
                    line.Y2 = lowwer.Y1;
                    line.Stroke = Stroke(col);
                }
            }
            else
            {
                if (p.Y < lowwer.Y2)
                {
                    line.X2 = lowwer.X2;
                    line.Y2 = lowwer.Y2;
                    line.Stroke = Stroke(Colors.Red);
                }
                else
                {
                    line.X2 = lowwer.X1;
                    line.Y2 = lowwer.Y1;
                    line.Stroke = Stroke(Colors.Orange);
                }
            }
            line.Stroke = Stroke(col);
            line.StrokeThickness = 1;
            Paint.Children.Add(line);
        }
        public void DrawDown(Line upper, Line lowwer)
        {
            if (!Paint.Children.Contains(upper))
            {
                Paint.Children.Add(upper);
            }
            if (!Paint.Children.Contains(lowwer))
            {
                Paint.Children.Add(lowwer);
            }

        }
        public void DrawUp(Line up, Line down)
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
            //return line.Y1;
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
