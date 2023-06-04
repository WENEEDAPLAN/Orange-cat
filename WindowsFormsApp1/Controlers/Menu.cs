using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Рыжик
{
    public partial class Menu : Form
    {
        System.Media.SoundPlayer music = new System.Media.SoundPlayer();

        public Menu()
        {
            InitializeComponent();
            timerMenu.Start();
            timerMenu.Interval = 30;
            PlayMusic();
        }

        private void PlayMusic()
        {
            music.SoundLocation = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Music\\menuMusic.wav");
            music.PlayLooping();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Level1 level1 = new Level1();
            this.Hide();
            level1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            music.Stop();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

       
    }
}
