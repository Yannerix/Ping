using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Forms;

//Класс нашей ракетки

namespace Ping
{
    class RocketPlayer
    {
        //ракетка помнит своё направление двжиения и предыдущую позицию
        protected Rectangle rocket;
        Rectangle rocketLast;
        int direction;
        double angle;
        protected int width, height;
        public RocketPlayer()
        {
            this.width = 20;
            this.height = 100;
            rocket = new Rectangle(cons.WIDTH - this.width, cons.HEIGHT / 2, this.width, this.height);
            rocketLast = new Rectangle(cons.WIDTH - this.width, cons.HEIGHT / 2, this.width, this.height);
            direction = 1;
            angle = 0;
        }

        public Rectangle rocketLastP
        {
            get
            {
                return rocketLast;
            }

        }

        public Rectangle rocketP
        {
            get
            {
                return rocket;
            }
        }

        public int directionP
        {
            get
            {
                return direction;
            }
        }
        public double angleP
        {
            get
            {
                return angle;
            }
        }

        //Находим угол удара ракетки с помощью составления уравнения прямой по двум точкам, а затем и нахождением производной от неё
        public void setAbgle()
        {
            //Если не сдвинулись по иксу, то угол нулевой, а ошибки из-за деления на ноль не возникнет
            if (rocket.X == rocketLast.X)
            {
                angle = 0;
            }
            else
            {
                int x0 = rocket.X;
                int x1 = rocketLast.X;
                int y0 = rocket.Y;
                int y1 = rocketLast.Y;
                //Тангенс угла наклона касательной равен производной
                //Чтобы найти сам угол, немного приспособим его к нашим координатам
                angle = Math.PI*2 + Math.Atan((y0 - y1) / (x0 - x1));
            }
            if (rocket.X < rocketLast.X)
            {
                angle = angle + Math.PI;
            }
        }

        //Движение ракетки
        public void move(int x, int y)
        {
            //Текущее положение становится старым
            rocketLast = rocket;
            //а новое находим так, чтобы в нём был центр курсора
            if (x >= cons.WIDTH/2  && x <= cons.WIDTH - rocket.Width / 2)
            {
                rocket.X = x - rocket.Width / 2;
            }
            if (y >= rocket.Height/2 && y <= cons.HEIGHT - rocket.Height / 2)
            {
                rocket.Y = y - rocket.Height / 2;
            }
            //А ещё, если мы не двигались (то есть старая позиция равна новой), то изменять сведения об углах и направлении нам нельзя
            if ((rocketLast.X != rocket.X || rocketLast.Y != rocket.Y))
            { 
                if (rocketLast.X >= rocket.X)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
                setAbgle();

            }
        }


    }
}
