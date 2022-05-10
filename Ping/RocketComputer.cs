using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace Ping
{
    //Класс нашей ракетки в итоге оказался посложнее класса компьютерной, но наследовать им друг у друга всё равно особо нечего, поэтому оставил так
    class RocketComputer : RocketPlayer
    {
        int speed;
        string d;

        //В зависимости от уровня сложности меняем длину и скорость ракетки
        public RocketComputer(string dif) : base()
        {
            if (dif == "easy")
            {
                this.speed = 12;
                rocket = new Rectangle(0, cons.HEIGHT / 2, this.width + 5, this.height);
            }
            else
            {
                this.speed = 20;
                rocket = new Rectangle(0, cons.HEIGHT / 2, this.width, this.height);
            }
            d = dif;
        }
        public void move(int x, int y, double angle)
        {
            //Если компьютер "простой", то дополнительно ослабляем его алгоритм небольшими шумами. Получилось хорошо, ведь иногда он ошибается как раз
            //на чуть-чуть, когда "сложный" бы точно отбил мячик
            if (d == "easy")
            {
                //Алгоритм всегда старается держаться на высоте центра мячика
                Random rnd = new Random();
                if (x >= cons.WIDTH / 2 || Math.Cos(angle) >= 0)
                {
                    return;
                }
                if (y < rocket.Y && y < rocket.Y + rocket.Height)
                {
                    rocket.Y -= Convert.ToInt32(speed*rnd.NextDouble() - speed*0.25);
                }
                else if (y > rocket.Y && y > rocket.Y + rocket.Height)
                {
                    rocket.Y += Convert.ToInt32(speed * rnd.NextDouble() - speed*0.2);
                }
            }
            //Умный компьютер
            else
            {
                //В отличии от лёгкого, он будет "отдыхать", когда не его очередь ловить мячик (смотрим на направление-косинус)
                if (x >= cons.WIDTH / 2 || Math.Cos(angle) >= 0)
                {
                    //Но заодно сместится поближе к центру поля
                    if (cons.HEIGHT/2  < rocket.Y + rocket.Height / 2)
                    {
                        rocket.Y -= 2;
                    }
                    else if (cons.HEIGHT / 2 > rocket.Y + rocket.Height/2)
                    {
                        rocket.Y += 2;
                    }
                    return;
                }
                //Тот же алгоритм без шумов
                if (y < rocket.Y && y < rocket.Y + rocket.Height)
                {
                    rocket.Y -= speed;
                }
                else if (y > rocket.Y && y > rocket.Y + rocket.Height)
                {
                    rocket.Y += speed;
                }
            }

        }


    }
}
