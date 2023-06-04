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
    public partial class Level4 : Form
    {
        public bool Key;
        public bool SecondKey;
        ActiveStatus active;
        Image spriteMouse = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\Fox.png"));
        Image spriteCat = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\Cat-Sheet.png"));
        Image spriteDoor = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\Door.png"));
        Image spriteKey = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\key_big.png"));
        System.Media.SoundPlayer music = new System.Media.SoundPlayer();

        public Entity cat1;
        public Entity dog;
        public Entity dog2;
        public Level4()
        {
            InitializeComponent();
            EnableDoubleBuffering();
            MaximizeBox = false;
            Level4Timer.Tick += new EventHandler(MoveEnemies);
            Level4Timer.Tick += new EventHandler(Gravity);
            Level4Timer.Tick += new EventHandler(UpdateAnimation);
            Level4Timer.Tick += new EventHandler(ObjectInCollusion);
            KeyDown += new KeyEventHandler(KeyIsPress);
            KeyUp += new KeyEventHandler(KeyIsUp);
            Level4Timer.Interval = 30;
            PlayMusic();
            Init();
        }


        public void Init()
        {

            cat1 = new Entity(player, 32, 32);
            dog = new Entity(enemy, 32, 32);
            dog2 = new Entity(enemy2, 32, 32);

            cat1.Position(player, 12, 373);
            door.BackColor = Color.Transparent;
            door.Image = spriteDoor;
            KeyBox.Image = spriteKey;
            secondBoxKey.Image = spriteKey;
            Level4Timer.Start();
        }

        public void PlayMusic()
        {
            music.SoundLocation = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Music\\stray_26-Last-Night.wav");
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
            if (cat1.isMove)
            {
                switch (active)
                {
                    case ActiveStatus.Run:
                        cat1.Move();
                        cat1.PlayAnimation(spriteCat, player, cat1.Width, cat1.Width, Inf.CatRunSetAnimation, Inf.CatRunFrames);
                        break;
                    case ActiveStatus.Boost:
                        cat1.Move();
                        cat1.PlayAnimation(spriteCat, player, cat1.Width, cat1.Width, Inf.CatShiftSetAnimation, Inf.CatShiftFrames);
                        break;
                    case ActiveStatus.Jump:
                        cat1.Move();
                        break;
                }
            }
            else
                cat1.PlayAnimation(spriteCat, player, cat1.Width, cat1.Width, Inf.CatIdleSetAnimation, Inf.CatIdleFrames);
        }

        public void MoveEnemies(object sender, EventArgs e)
        {
            dog.PlayAnimation(spriteMouse, enemy, cat1.Width, cat1.Width, Inf.DogRunSetAnimation, Inf.DogRunFrames);
            dog2.PlayAnimation(spriteMouse, enemy2, cat1.Width, cat1.Width, Inf.DogRunSetAnimation, Inf.DogRunFrames);
            dog.Move();
            dog2.Move();
        }

        public void KeyIsPress(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.A:
                    active = ActiveStatus.Run;
                    cat1.isMove = true;
                    cat1.Flip = true;
                    cat1.Dx = -2;
                    break;
                case Keys.D:
                    active = ActiveStatus.Run;
                    cat1.isMove = true;
                    cat1.Flip = false;
                    cat1.Dx = 2;
                    break;
                case Keys.Space:
                    active = ActiveStatus.Jump;
                    cat1.Jump = true;
                    break;
            }

            if (e.KeyCode == Keys.ControlKey && cat1.Boost == false)
            {
                active = ActiveStatus.Boost;
                cat1.Boost = true;
                cat1.isMove = true;
                cat1.Dx *= 3;
            }

            if (cat1.Jump == true && cat1.Force < 0)
                cat1.Jump = false;


            if (cat1.Jump == true)
            {
                cat1.GravitySpeed = -8;
                cat1.Force -= 1;
            }

            else
                cat1.GravitySpeed = 10;

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            cat1.Dx = 0;
            cat1.Dy = 0;
            cat1.isMove = false;
            cat1.Boost = false;
            cat1.Jump = false;
            cat1.GravitySpeed = 8;
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
                            cat1.Force = 8;
                            break;
                        case "enemy":
                            RestartGame();
                            break;
                    }
                }
                if (player.Bounds.IntersectsWith(door.Bounds) && Key && SecondKey)
                    EndGame();

                if (player.Bounds.IntersectsWith(KeyBox.Bounds))
                {
                    Key = true;
                    KeyBox.Visible = false;
                }

                if (player.Bounds.IntersectsWith(secondBoxKey.Bounds))
                {
                    SecondKey = true;
                    secondBoxKey.Visible = false;
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
            cat1.Gravity();
            dog.Gravity();
            dog2.Gravity();
        }

        public void RestartGame()
        {
            cat1.Position(player, 12, 373);
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false)
                    x.Visible = true;
            }
            timer1.Start();
        }

        public void EndGame()
        {
            Level5 level5 = new Level5();
            this.Hide();
            timer1.Stop();
            level5.Show();
            Key = false;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
