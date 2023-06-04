using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Рыжик.Inf;

namespace Рыжик
{
    public partial class Level1 : Form
    {
        public bool IsGameEnd = true;
        ActiveStatus active;
        Image spriteMouse = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\Fox.png"));
        Image spriteDog = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\Cat-Sheet.png"));
        Image spriteDoor = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\Door.png"));
        System.Media.SoundPlayer music = new System.Media.SoundPlayer();

        public Entity cat;
        public Entity dog;
        public Entity dog2;
        public Level1()
        {
            InitializeComponent();
            EnableDoubleBuffering();
            MaximizeBox = false;
            timer1.Tick += new EventHandler(MoveEnemies);
            timer1.Tick += new EventHandler(Gravity);
            timer1.Tick += new EventHandler(UpdateAnimation);
            timer1.Tick += new EventHandler(ObjectInCollusion);
            KeyDown += new KeyEventHandler(KeyIsPress);
            KeyUp += new KeyEventHandler(KeyIsUp);
            timer1.Interval = 30;
            PlayMusic();
            Init();
        }


        public void Init()
        {

            cat = new Entity(player, 32, 32);
            dog = new Entity(enemy, 32, 32);
            dog2 = new Entity(enemy2, 32, 32);

            cat.Position(player, 12, 373);
            door.BackColor = Color.Transparent;
            door.Image = spriteDoor;
            timer1.Start();
        }

        public void PlayMusic()
        {
            music.SoundLocation = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Music\\stray_11-Secret-Lab.wav");
            music.PlayLooping();
        }

        public void EnableDoubleBuffering()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
               ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint,
               true);
            this.UpdateStyles();
        }


        public void UpdateAnimation(object sender, EventArgs e)
        {
            if (cat.isMove)
            {
                switch (active)
                {
                    case ActiveStatus.Run:
                        cat.Move();
                        cat.PlayAnimation(spriteDog, player, cat.Width, cat.Width, Inf.CatRunSetAnimation, Inf.CatRunFrames);
                        break;
                    case ActiveStatus.Boost:
                        cat.Move();
                        cat.PlayAnimation(spriteDog, player, cat.Width, cat.Width, Inf.CatShiftSetAnimation, Inf.CatShiftFrames);
                        break;
                    case ActiveStatus.Jump:
                        cat.Move();
                        break;
                }
            }
            else
                cat.PlayAnimation(spriteDog, player, cat.Width, cat.Width, Inf.CatIdleSetAnimation, Inf.CatIdleFrames);
        }

        public void MoveEnemies(object sender, EventArgs e)
        {
            dog.PlayAnimation(spriteMouse, enemy, cat.Width, cat.Width, Inf.DogRunSetAnimation, Inf.DogRunFrames);
            dog2.PlayAnimation(spriteMouse, enemy2, cat.Width, cat.Width, Inf.DogRunSetAnimation, Inf.DogRunFrames);
            dog.Move();
            dog2.Move();
        }

        public void KeyIsPress(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.A:
                    active = ActiveStatus.Run;
                    cat.isMove = true;
                    cat.Flip = true;
                    cat.Dx = -2;
                    break;
                case Keys.D:
                    active = ActiveStatus.Run;
                    cat.isMove = true;
                    cat.Flip = false;
                    cat.Dx = 2;
                    break;
                case Keys.Space:
                    active = ActiveStatus.Jump;
                    cat.Jump = true;
                    break;
            }

            if (e.KeyCode == Keys.ControlKey && cat.Boost == false)
            {
                active = ActiveStatus.Boost;
                cat.Boost = true;
                cat.isMove = true;
                cat.Dx *= 3;
            }

            if (cat.Jump == true && cat.Force < 0)
                cat.Jump = false;


            if (cat.Jump == true)
            {
                cat.GravitySpeed = -8;
                cat.Force -= 1;
            }

            else
                cat.GravitySpeed = 10;

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            cat.Dx = 0;
            cat.Dy = 0;
            cat.isMove = false;
            cat.Boost = false;
            cat.Jump = false;
            cat.GravitySpeed = 8;
        }

        private void ObjectInCollusion(object sender, EventArgs e)
        {
            foreach (Control x in this.Controls)
            {
                if (player.Bounds.IntersectsWith(x.Bounds))
                {
                    switch ((string)x.Tag)
                    {
                        case "platform":
                            player.Top = x.Top - player.Height;
                            cat.Force = 8;
                            break;
                        case "enemy":
                            RestartGame();
                            break;
                    }
                }
                if (player.Bounds.IntersectsWith(door.Bounds) && IsGameEnd)
                {
                    EndGame();
                }



                if (enemy.Bounds.IntersectsWith(x.Bounds) && (string)x.Tag == "platform")
                    enemy.Top = x.Top - enemy.Height;

                if (enemy2.Bounds.IntersectsWith(x.Bounds) && (string)x.Tag == "platform")
                    enemy2.Top = x.Top - enemy.Height;

                if (player.Top + player.Height > this.ClientSize.Height + 50)
                    RestartGame();

                if (enemy.Left + enemy.Width > pictureBox4.Left + pictureBox4.Width)
                {
                    dog.Flip = true;
                    dog.Dx = -2;
                }

                if (enemy.Left < pictureBox4.Left)
                {
                    dog.Dx = 2;
                    dog.Flip = false;
                }

                if (enemy2.Left + enemy2.Width > pictureBox2.Left + pictureBox2.Width)
                {
                    dog2.Flip = true;
                    dog2.Dx = -2;
                }

                if (enemy2.Left < pictureBox2.Left)
                {
                    dog2.Dx = 2;
                    dog2.Flip = false;
                }
            }
        }

        public void Gravity(object sender, EventArgs e)
        {
            cat.Gravity();
            dog.Gravity();
            dog2.Gravity();
        }

        public void RestartGame()
        {
            cat.Position(player, 12, 373);
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false)
                    x.Visible = true;
            }
            timer1.Start();
        }

        public void EndGame()
        {
            Level2 level2 = new Level2();   
            this.Hide();
            timer1.Stop();
            level2.Show();
            IsGameEnd = false;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
