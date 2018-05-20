using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace IGS1Consol
{

    class Program
    {

        class Game : GameWindow
        {
            bool state_start = true;//флаг для начал рисования
            bool state_end = false;//флаг для конца рисования
            private List<Point> points = new List<Point>();//список точек для отрисовки отрезков и др
            public Game()
                : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
            {
                VSync = VSyncMode.On;
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
                GL.Enable(EnableCap.DepthTest);
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);

                GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
                GL.MatrixMode(MatrixMode.Projection);
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                base.OnUpdateFrame(e);
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {

                base.OnRenderFrame(e);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.MatrixMode(MatrixMode.Modelview);

                    foreach (var p in points)//для каждой точки из списка отрисовываем их на экране и рисуем отрезки
                    {
                        GL.Begin(BeginMode.Points);
                        GL.Color3(1.0f, 1.0f, 0.0f);//выбираем цвет
                        GL.Vertex2(p.X, p.Y);//рисуем точку
                        GL.End();
                        DrawPolygon(points);//рисуем отрезок
                        if (points.Count % 4 == 0)//как только набралось 4 точки для анализа
                        {
                            bool d = checkIntersectionOfTwoLineSegments(points[points.Count - 4], points[points.Count - 3], points[points.Count - 2], points[points.Count - 1]);//проверяем два последних нарисованных отрезка на пересечение и если пересекаются  
                            if (d)
                            {
                                Point IX = Intersection(points[points.Count - 4], points[points.Count - 3], points[points.Count - 2], points[points.Count - 1]);//находим точку пересечения
                                Color4 yellow = new Color4(1f, 1f, 0f, 0f);
                                DrawCircle(IX.X, IX.Y, 0.1f, yellow);//если пересекаются отрисовываем круг желтый
                            }
                            else
                            {
                                Color4 red = new Color4(1.0f, 0f, 0f, 0f);
                                DrawCircle(Width/2 *0.0001f, Height / 2 * 0.0001f, 0.1f, red);//если не пересекаются отрисовываем круг красный
                        }
                        }
                    }

                SwapBuffers();
            }

            static public Point Intersection(Point A, Point B, Point C, Point D) //определяем точку пересечение для отрезков заданных точками ( только для возвращения точки песечения)
            {
                float xo = A.X, yo = A.Y;
                float p = B.X - A.X, q = B.Y - A.Y;

                float x1 = C.X, y1 = C.Y;
                float p1 = D.X - C.X, q1 = D.Y - C.Y;

                float x = (xo * q * p1 - x1 * q1 * p - yo * p * p1 + y1 * p * p1) /
                    (q * p1 - q1 * p);

                float y = (yo * p * q1 - y1 * p1 * q - xo * q * q1 + x1 * q * q1) /
                    (p * q1 - p1 * q);

               
                return new Point(x, y); //возвращаем точку пересечения
            }
         

            private bool checkIntersectionOfTwoLineSegments(Point p1, Point p2, Point p3, Point p4)//метод, проверяющий пересекаются ли 2 отрезка [p1, p2] и [p3, p4]
            {
                double Xa;
                double A1;
                double b1;
                double Ya;
                double A2;
                double b2;
                //сначала расставим точки по порядку, т.е. чтобы было p1.X <= p2.X

                if (p2.X < p1.X)
                {

                    Point tmp = p1;

                    p1 = p2;

                    p2 = tmp;

                }

                //и p3.X <= p4.X

                if (p4.X < p3.X)
                {

                    Point tmp = p3;

                    p3 = p4;

                    p4 = tmp;

                }

                //проверим существование потенциального интервала для точки пересечения отрезков

                if (p2.X < p3.X)
                {

                    return false; //ибо у отрезков нету взаимной абсциссы

                }

                //если оба отрезка вертикальные

                if ((p1.X - p2.X == 0) && (p3.X - p4.X == 0))
                {

                    //если они лежат на одном X

                    if (p1.X == p3.X)
                    {

                        //проверим пересекаются ли они, т.е. есть ли у них общий Y

                        //для этого возьмём отрицание от случая, когда они НЕ пересекаются

                        if (!((Math.Max(p1.Y, p2.Y) < Math.Min(p3.Y, p4.Y)) ||

                        (Math.Min(p1.Y, p2.Y) > Math.Max(p3.Y, p4.Y))))
                        {

                            return true;

                        }

                    }

                    return false;

                }

                //найдём коэффициенты уравнений, содержащих отрезки

                //f1(x) = A1*x + b1 = y

                //f2(x) = A2*x + b2 = y

                //если первый отрезок вертикальный

                if (p1.X - p2.X == 0)
                {

                    //найдём Xa, Ya - точки пересечения двух прямых

                     Xa = p1.X;

                     A2 = (p3.Y - p4.Y) / (p3.X - p4.X);

                     b2 = p3.Y - A2 * p3.X;

                     Ya = A2 * Xa + b2;

                    if (p3.X <= Xa && p4.X >= Xa && Math.Min(p1.Y, p2.Y) <= Ya &&

                    Math.Max(p1.Y, p2.Y) >= Ya)
                    {

                        return true;

                    }

                    return false;

                }

                //если второй отрезок вертикальный

                if (p3.X - p4.X == 0)
                {

                    //найдём Xa, Ya - точки пересечения двух прямых

                     Xa = p3.X;

                     A1 = (p1.Y - p2.Y) / (p1.X - p2.X);

                     b1 = p1.Y - A1 * p1.X;

                     Ya = A1 * Xa + b1;

                    if (p1.X <= Xa && p2.X >= Xa && Math.Min(p3.Y, p4.Y) <= Ya &&

                    Math.Max(p3.Y, p4.Y) >= Ya)
                    {

                        return true;

                    }

                    return false;

                }

                //оба отрезка невертикальные

                 A1 = (p1.Y - p2.Y) / (p1.X - p2.X);

                 A2 = (p3.Y - p4.Y) / (p3.X - p4.X);

                 b1 = p1.Y - A1 * p1.X;

                 b2 = p3.Y - A2 * p3.X;

                if (A1 == A2)
                {

                    return false; //отрезки параллельны

                }

                //Xa - абсцисса точки пересечения двух прямых

                 Xa = (b2 - b1) / (A1 - A2);

                if ((Xa < Math.Max(p1.X, p3.X)) || (Xa > Math.Min(p2.X, p4.X)))
                {

                    return false; //точка Xa находится вне пересечения проекций отрезков на ось X

                }

                else
                {

                    return true;

                }

            }


            protected override void OnMouseDown(MouseButtonEventArgs e) // по нажатию мышки 
            {

                base.OnMouseDown(e);

                if (e.Button == MouseButton.Left) //левая клавиша
                {
                    if (state_start == true)//начали рисовать 
                    {
                        float x = (e.X - Width / 2f) / (Width / 2f);
                        float y = -(e.Y - Height / 2f) / (Height / 2f);

                        // Store the coordinates to point list
                        points.Add(new Point(x, y));

                        Console.WriteLine("начальная точка " + x + " " + y);
                        state_end = true; //можно закончить
                        state_start = false;//нельзя заново начать
                    }

                }
                if (e.Button == MouseButton.Right)//правая клавиша
                {
                    if (state_end == true)//если можно закончить
                    {
                        float x = (e.X - Width / 2f) / (Width / 2f);
                        float y = -(e.Y - Height / 2f) / (Height / 2f);

                        // Store the coordinates to point list
                        points.Add(new Point(x, y));

                        Console.WriteLine("конечная точка " + x + " " + y);
                        state_start = true;//можно начать заново
                        state_end = false;//нельзя снова закончить
                    }
                }

            }



            public static void DrawCircle(float x, float y, float radius, Color4 c)//наррисовать круг
            {
           
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Color4(c);
                GL.Vertex2(x, y);
                for (int i = 0; i < 360; i++)
                {
                    GL.Color4(c);
                    GL.Vertex2(x + Math.Cos(i) * radius, y + Math.Sin(i) * radius);
                }
                GL.End();
           
            }

            public static void DrawPolygon(List<Point> points)
            {
                GL.Begin(PrimitiveType.Lines);
                int numberOfPoints = points.Count;
                for (int i = 0; i < numberOfPoints; i++)
                {
                    GL.Color3(0.2f, 0.9f, 1.0f);
                    GL.Vertex2(points[i].X, points[i].Y);

                }
                GL.End();
            }


            [STAThread]
            static void Main()
            {
                // The 'using' idiom guarantees proper resource cleanup.
                // We request 30 UpdateFrame events per second, and unlimited
                // RenderFrame events (as fast as the computer can handle).
                using (Game game = new Game())
                {
                    game.Run(30.0);
                }
            }
        }
    }
}
