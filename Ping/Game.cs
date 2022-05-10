using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


//Класс, отвечающий за игровой процесс
namespace Ping
{

    class Game
    {
        int playerPoints;
        int computerPoints;
        Ball ball;
        RocketComputer computer;
        RocketPlayer player;
        //Событие, вызываемое при столкновении мячика с чем-нибудь
        public event GameEventHandler Collision;
        //Событие, вызываемое при инертном ударе ракеткой об мячик
        public event GameEventHandler2 Hit;
        public Game(Ball ball, RocketPlayer rocPl, RocketComputer rocCom)
        {
            this.playerPoints = 0;
            this.computerPoints = 0;
            this.ball = ball;
            this.player = rocPl;
            this.computer = rocCom;
        }


        //В будущем нужно будет вывести информацию о том, кто победил в игре
        public int competerPointP
        {
            get { return computerPoints; }
        }
        public int playerPointP
        {
            get { return playerPoints; }
        }

        //Добавляет очки игроку, который смог забить гол
        public void AddPoint(bool player)
        {
            if (player)
            {
                playerPoints++;
                ball.spawn(true);
            }
            else 
            {
                computerPoints++;
                ball.spawn(false);
            }
        }

        //Функция, отвечающая за колиизии ракеток с мячиком. 
        public void checkCollision()
        {
            /*
            *Так как наша ракетка двигается по дмумерной плоскости, а частота обновления
            *таймера меньше частоты событий об изменениях координат курсора, не всегда
            *удаётся "схватить" тот факт, что ракетка задела мячик, поэтому позволим ракетке
            *помнить о своей позиции на предыдущей итерации (свойство player.rocketLastP). 
            *
            *С помощью этой информации создаём большой прямоугольник, описывающий приблизительную траекторию взмаха ракетки
            */
            int xMin = Math.Min(player.rocketP.X, player.rocketLastP.X);
            int yMin = Math.Min(player.rocketP.Y, player.rocketLastP.Y);
            int width = Math.Abs(player.rocketP.X - player.rocketLastP.X) + player.rocketP.Width;
            int height = Math.Abs(player.rocketP.Y - player.rocketLastP.Y) + player.rocketP.Height;
            double len = Math.Sqrt((player.rocketP.X - player.rocketLastP.X) * (player.rocketP.X - player.rocketLastP.X) + (player.rocketP.Y - player.rocketLastP.Y) * (player.rocketP.Y - player.rocketLastP.Y));
            //У взмаха даже есть своя скорость, можно будет разнообразить игру 
            double speed = len/10;
            
            //Теперь определим направления двжиений взамаха ракетки и шарика
            string directionBall = "none", directionRocket = "none";
            if (Math.Cos(ball.angleP) <= 0)
            {
                directionBall = "left";
            }
            else
            {
                directionBall = "right";
            }

            if (player.directionP == -1)
            {
                directionRocket = "left";
            }
            else 
            {
                directionRocket = "right";
            }


            //Проверяем, пересекался ли наш взмах с мячиком
            if (ball.ballP.IntersectsWith(new Rectangle(xMin - ball.speedXP/2, yMin, width + ball.speedXP, height)))
            {
                //Возможна ситуация, когда ракетка не двигалась, тогда её предыдущее и настоящее положения не отличаются => 
                //Имеем привычную нам ракетку с известной шириной и высотой
                if (width == player.rocketP.Width && height == player.rocketP.Height)
                {
                    //Дополнительным условием большинства коллизий будет выступать проверка, не пришлось ли пересечение на рёбра ракетки
                    if (ball.ballP.Y <= yMin + height - 5 && ball.ballP.Y + ball.ballP.Height >= yMin + 5)
                    {
                        //Если нет, то всё хорошо и просто сообщаем шарику, что он упёрся в вертикальную преграду
                        Collision("rocket");
                    }
                    else
                    {
                        //мячик будто бы отбился от пола 
                        Collision("floor");
                    }
                }
                //Итак, вхмах был, соответсвенно он может отличаться по направлению от шарика или же совпадать с ним
                else if (directionBall != directionRocket)
                {
                    //Если отличается, то достаточно просто отразить его от ракетки по закону угла падения-угла отражения
                    if (ball.ballP.Y <= yMin + height - 5 && ball.ballP.Y + ball.ballP.Height >= yMin + 5)
                    {
                        Collision("rocket");
                    }
                    else
                    {
                        Collision("floor");
                    }
                    //Однако для интереса добавим в мячик информацию о силе(скорости) удара и его угле
                    Hit(xMin, player.directionP, speed, player.angleP);
                }
                //Если же направления совпадают
                else 
                {
                    if (ball.ballP.X + ball.ballP.Width + ball.speedXP/2 <= player.rocketP.X + width || ball.ballP.X + ball.speedXP / 2 >= player.rocketP.X - width)
                    {
                        //Ситуацию, когда ракетка движется вправо, чтобы поймать летящий вправо мячик, пришлось прописывать отдельно, не работала почему-то.
                        if (directionRocket == "right" && ball.ballP.X + width + ball.speedXP / 2 <= xMin + width )
                        {
                            //В ней достаточно просто отразить направления мячика
                            Collision("rocket");
                        }
                        else
                        {  //А вот ситуации, когда ракетка подгоняет мячик, чтобы его ускорить, опять же регистрируют лёгкий удар
                            Hit(player.rocketP.X, player.directionP, speed, player.angleP);
                        }
                        /*Ещё должен быть случай, когда ракетка движется влево, чтобы поймать влево летящий мячик (и, вероятно, получить авторог).
                         * Но, логично, он будет встречаться довольно редко. При попытках тестирования особо его не встречал, вероятно, такая обособенность
                         * связана со скоростями (так как при движении от нашей стенки она просто не может оказаться большой, ведь чаще всего мы уже получили гол)
                         */

                    }
                }
            }

            //А это коллизии с ракеткой-компьютером. ТАк же обрабатываются рёбра ракетки.
            if (ball.ballP.IntersectsWith(computer.rocketP))
            {
                if (ball.ballP.Y <= computer.rocketP.Y + computer.rocketP.Height - 5 && ball.ballP.Y + ball.ballP.Height >= computer.rocketP.Y + 5)
                {
                    Collision("rocket");
                }
                else
                {
                    Collision("floor");
                }
            }
            
            //И на всякий случай идёт проверка того, что на следующей итерации коллизии так же не будет, потому что иногда мячик просто впечатывался
            //В ракетку. Лучше предостеречься
            if (ball.ballP.IntersectsWith(new Rectangle(computer.rocketP.X - ball.speedXP, computer.rocketP.Y, computer.rocketP.Width - 5, computer.rocketP.Height)))
            {
                Collision("rocket");
            }
 
        }


        //Просто обновляем всё, желательно в этом же порядке. 
        public bool update(int x, int y)
        {
            player.move(x, y);
            computer.move(ball.ballP.X, ball.ballP.Y, ball.angleP);
            checkCollision();
            ball.move();
            //Когда-нибудь мы вернём информацию о том, что игра закончилась
            return (computerPoints == 10 || playerPoints == 10);
        }


    }
}
