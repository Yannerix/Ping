using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


//Класс самой содержательной формы, в ней хранятся все объекты и релизуется отрисовка и обновления игрового процесса


namespace Ping
{
    public delegate void GameEventHandler(string a);
    public delegate void GameEventHandler2(int a, int b, double c, double d);
    public delegate void BallEventHandler(bool a);
    public partial class Form1 : Form
    {
        Game game;
        Ball ball;
        int mouseX, mouseY;
        RocketPlayer rocketPlayer;
        RocketComputer rocketComputer;
        Graphics g;    
        Bitmap buf;
        bool stop;
        //Сложность противника, передаётся из другой формы при инициализации этой
        string dif;
        public Form1(string a)
        {
            InitializeComponent();
            dif = a;
        }

        //Каждый тик таймера мы будем обновлять игру до тех пор, пока она нам не сообщит, что игра окончена
        private void timer1_Tick(object sender, EventArgs e)
        {
            stop = game.update(mouseX, mouseY);
            if (stop)
            {
                if (game.competerPointP == 10)
                {
                    Application.Exit();
                    DialogResult result = MessageBox.Show(
                    "Вы проиграли :(",
                    "PongGame",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    Application.Exit();
                    DialogResult result = MessageBox.Show(
                    "Вы победили! :)",
                    "PongGame",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            graphic();
        }

        //Отрисовка элементов игры, привязанная к тикам таймера.
        private void graphic()
        {
            buf = new Bitmap(field.Width, field.Height);
            g = Graphics.FromImage(buf);
            g.Clear(Color.Green);
            g.DrawLine(new Pen(Color.Black, 1), new Point(cons.WIDTH / 2, 0), new Point(cons.WIDTH / 2, cons.HEIGHT));

            //int coordx = Convert.ToInt32(Math.Cos(ball.angleP) * 100) + cons.WIDTH / 2;
            //int coordY = Convert.ToInt32(Math.Sin(ball.angleP) * 100) + cons.HEIGHT / 2;
            //g.DrawLine(new Pen(Color.Black, 1), new Point(cons.WIDTH / 2, cons.HEIGHT / 2), new Point(coordx, coordY));

            // coordx = Convert.ToInt32(Math.Cos(rocketPlayer.angleP) * 100) + cons.WIDTH / 2;
            //coordY = Convert.ToInt32(Math.Sin(rocketPlayer.angleP) * 100) + cons.HEIGHT / 2;
            //g.DrawLine(new Pen(Color.Red, 1), new Point(cons.WIDTH / 2, cons.HEIGHT / 2), new Point(coordx, coordY));
            //g.DrawString(Convert.ToString(rocketPlayer.angleP*180/Math.PI), new Font("Arial", 16), new SolidBrush(Color.White), new Point(400, 10));

            g.DrawString(Convert.ToString(game.competerPointP), new Font("Arial", 16), new SolidBrush(Color.White), new Point(10, 10));
            g.DrawString(Convert.ToString(game.playerPointP), new Font("Arial", 16), new SolidBrush(Color.White), new Point(770, 10));

            //int xMin = Math.Min(rocketPlayer.rocketP.X, rocketPlayer.rocketLastP.X);
            //int yMin = Math.Min(rocketPlayer.rocketP.Y, rocketPlayer.rocketLastP.Y);
            //int width = Math.Abs(rocketPlayer.rocketP.X  - rocketPlayer.rocketLastP.X );
            //int height = Math.Abs(rocketPlayer.rocketP.Y - rocketPlayer.rocketLastP.Y) + rocketPlayer.rocketP.Height;
            //g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(xMin, yMin, width, height));


            g.FillRectangle(new SolidBrush(Color.White), rocketPlayer.rocketP);
            g.FillRectangle(new SolidBrush(Color.White), rocketComputer.rocketP);
            //g.FillRectangle(new SolidBrush(Color.White), rocketPlayer.rocketLastP);

            g.FillEllipse(new SolidBrush(Color.White), ball.ballP);

            field.Image = buf;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rocketPlayer = new RocketPlayer();
            rocketComputer = new RocketComputer(dif);
            ball = new Ball();
            game = new Game(ball, rocketPlayer, rocketComputer);
            game.Collision += new GameEventHandler(ball.moveUpdate);
            ball.Goal += new BallEventHandler(game.AddPoint);
            game.Hit += new GameEventHandler2(ball.speedAppend);
            timer1.Start();
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!stop)
            {
                Application.Exit();
                DialogResult result = MessageBox.Show(
                "До свидания!",
                "PongGame",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            }
        }


        //В игре встречаются противные ситуации, когда шарик движется вертикально на стороне бота, добавлять рандомности не захотел, решил избавиться
        //От них радикально пересозданием мяча
        private void field_Click_1(object sender, EventArgs e)
        {
            ball.spawn(false);
        }

        //Игровой процесс должен знать о наших координатах курсора на каждой итерации 
        private void field_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
        }
    }
}
