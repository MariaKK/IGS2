using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGS1Consol
{
    class Point
    {
        public float X { get; set; }//координата х
        public float Y { get; set; }//координата у
        public Point(float x, float y) //конструктор с установкой
        {
            X = x;
            Y = y;

        }
    }
}
