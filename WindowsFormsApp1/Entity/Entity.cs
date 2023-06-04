using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Рыжик
{
    public class Entity
    {
        public PictureBox pictureBox;

        public bool Flip;
        public bool isMove;
        public bool Jump;
        public bool Boost;

        public int Width = 32;
        public int Height = 32;

        public int Dx;
        public int Dy;

        public int GravitySpeed;
        public int Force;
        public int JumpSpeed;
        public int currFrame;
        public Entity(PictureBox player, int width, int height)
        {
            this.pictureBox = player;
            Width = width;
            Height = height;
            GravitySpeed = 4;

            Dx = 2;
            Dy = 2;
        }

        public void Gravity()
        {
            pictureBox.Top += GravitySpeed;
        }

        public void Move()
        {
            pictureBox.Left += Dx;
            pictureBox.Top += Dy;
        }

        public void Position(PictureBox player, int Px, int Py)
        {
            player.Left = Px;
            player.Top = Py;
        }

        public void PlayAnimation(Image sprite, PictureBox entity, int Width, int Height, int currAnimation, int currLimit)
        {

            Image part = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(part);
            entity.BackColor = Color.Transparent;
            if (currFrame < currLimit - 1)
                currFrame++;
            else
                currFrame = 0;
            g.DrawImage(sprite, new Rectangle(new Point(0, 0), new Size(Width, Height)), 32 * currFrame, 32 * currAnimation, Width, Height, GraphicsUnit.Pixel);
            entity.Size = new Size(Width, Height);
            entity.Image = part;
            if (Flip)
                pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }


    }
}
