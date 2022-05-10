using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

//Наш маленький мячик
namespace Ping
{
    class Ball
    {
        //У шарика много переменных, отвечающих за его поведение на поле
        int radius;
        Rectangle ball;
        int  speedX, speedY;
        double speed, usk, speedAdd;
        double angle, rocketAngle;
        public event BallEventHandler Goal;
        public Ball()
        {
            radius = 12;
            ball = new Rectangle(cons.WIDTH / 2 - radius, cons.HEIGHT/2 + radius, radius*2, radius*2);
            spawn(false);
        }

        public Rectangle ballP
        {
            get
            {
                return ball;
            }
        }

        public int speedXP
        {
            get
            {
                return speedX;
            }
        }

        //Пересоздаём шарик в +- приятных углах для игры по центру поля. Изначально направляем его в строну компьютера, дабы успеть сориентироваться
        public void spawn(bool player)
        {
            Random rnd = new Random();
            ball.Y = rnd.Next(2 * radius + 10, cons.HEIGHT - 2 * radius - 2);
            speed = 8;
            speedAdd = 0;
            rocketAngle = 0;
            usk = -0.3;
            ball.X = cons.WIDTH/2 + radius;
            //Выбор угла
            angle = rnd.NextDouble() * Math.PI * 3 / 4 - Math.PI / 2;
            if (!player)
            {
                //Если мяч брошен не игроку, то значит брошен на 180 градусов в другую сторону
                angle = Math.PI + angle;
            }
            //Сразу же изменим скорость
            speedRemove();
        }

        public double angleP
        {
            get { return angle; }
        }
        //Заставляем шарик летать по полю, изменяя его координаты
        public void move()
        {
            ball.X += speedX;
            ball.Y += speedY;
            //Попутно проверим, не стукнулись мы в стенки. Мне кажется, что с точки зрения ООП красивее было бы так же проверять 
            //столкновения в классе Game, но здесь оно выглядит как-то поприятнее
            if (ball.X >= cons.WIDTH - radius - 10)
            {
                //speedX = 0;
                //speedY = 0;
                Goal(false);
                //moveUpdate("rocket");
            }
            if (ball.X <= 0)
            {
                //speedX = 0;
                //speedY = 0;
                Goal(true);
                //moveUpdate("rocket");
            }
            if (ball.Y >= cons.HEIGHT - radius*2)
            {
                ball.Y = cons.HEIGHT - radius * 2;
                moveUpdate("floor");
            }
            if (ball.Y <= 0)
            {
                moveUpdate("floor");
                ball.Y = 0;
            }
            speedRemove();
        }

        //Изменяем значения скоростей по оси x и y, учитывая добавочную скорость speedAdd ракетки 
        void speedRemove()
        {

            speedX = Convert.ToInt32(Math.Cos(angle) * (speed + speedAdd));
            speedY = Convert.ToInt32(Math.Sin(angle) * (speed + speedAdd)); 
            //Чтобы мяч не становился сверхзвуковым, добавчную скорость нужно постепенно обнулять
            if (speedAdd > 0)
            {
                speedAdd += usk;
            }
            else
            {
                speedAdd = 0;
            }

        }

        //Обновляем угол движения шарика в зависимости от того, во что мы стукнулись
        public void moveUpdate(string type)
        {
            if (type == "floor")
            {
                angle = -angle;
            }
            if (type == "rocket")
            {
                angle = Math.PI - angle;
 
            }
            speedRemove();
            //ball.X += 2*speedX;
            //ball.Y += 2*speedY;
        }

        //Функция, меняющая параметры шарика в зависимости от удара ракеткой
        public void speedAppend(int x, int w, double sped, double ang)
        {
            speedAdd += sped;
            //Где-то вычитал, что при изменении угла поверхности на а, угол отражения изменяется на 2а. Выглядит похоже, но вертикальные полёты
            //Мяча всё равно сложно контролироваться
            angle += 2*ang;
            speedRemove();
            //"Прилепляем мячик к ракетке, ведь удар мог и не закончиться, а нам нужно начало пути"
            ball.X = x + (ball.Width + 1)*w;
            //ball.Y = y + 2*speedY;
        }


    }
}
