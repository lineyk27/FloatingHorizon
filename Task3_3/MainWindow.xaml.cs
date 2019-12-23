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

        int CountHorizons = 10;
        int CountPoints = 50;
        double Thickness = 2.5;

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
                return (int)Paint.Height / 2;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Lines = new Line[CountPoints, CountHorizons];

        }
        public Brush Stroke(int i = 0)
        {
            byte r = 0;
            byte g = (byte)(100 + (10 * i));
            byte b = 0;
            var col = Color.FromRgb(r, g, b);

            var stroke = new SolidColorBrush(col);
            stroke.Opacity = 20;
            return stroke;
        }
        public void GenerateLines()
        {
            int size = 36;
            int k = 0;
            int l = 0;

            double magic = 0.1;
            double m_step = 3;

            double startx, starty;
            for(double z = LowBound; z <= UpBound; z += StepZ)
            {
                startx = LowBound;
                starty = func(LowBound, z);
                l = 0;
                for(double x = LowBound + StepX; x <= UpBound; x += StepX)
                {
                    var line = new Line();
                    line.StrokeThickness = Thickness;
                    line.X1 = startx * size + CenterX + magic;
                    line.Y1 = starty * size + CenterY + magic;

                    line.X2 = x * size + CenterX + magic;
                    line.Y2 = func(x,z) * size + CenterY + magic;

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
                    var current = Lines[i, j];
                    if(j == 0)
                    {
                        last = current;
                        min = last;
                    }
                    if(Middle(current) >= Middle(last))
                    {
                        current.Stroke = Stroke(j);
                        Paint.Children.Add(current);
                        last = current;
                    }
                    else if(Middle(current) <= Middle(last) && (Middle(current) <= Middle(min)))
                    {
                        current.Stroke = Stroke(j);
                        Paint.Children.Add(current);
                        min = current;
                    }
                    /*
                    else if(Middle(current) > Middle(last))
                    {
                        current.Stroke = Stroke(j);
                        Paint.Children.Add(current);
                        last = current;
                    }*/
                    else
                    {
                        //current.Stroke = Stroke(CountHorizons - j);
                        //Paint.Children.Add(current);
                    }
                }
            }


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
