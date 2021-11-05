using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoVid_Killer
{
    public partial class Form1 : Form
    {

        bool goUp, goDown, goLeft, goRight, gameOver;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int sanatizer = 10;
        int virusSpeed = 3;
        int score;
        Random randNum = new Random();
        
        List<PictureBox> virusList = new List<PictureBox>();


        public Form1()
        {
            InitializeComponent();
            Begin();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                GameTimer.Stop();

                GameOver();

            }

            lblSan.Text = "Sanatizer: " + sanatizer;
            lblScore.Text = "Score: " + score;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }

            if (goRight == true && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }

            if (goUp == true && player.Top > 40)
            {
                player.Top -= speed;
            }

            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        sanatizer += 10;
                    }
                }
                

                if (x is PictureBox && (string)x.Tag == "virus")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                        
                    }

                    if (x.Left > player.Left)
                    {
                        x.Left -= virusSpeed;
                        ((PictureBox)x).Image = Properties.Resources.vleft;
                    }

                    if (x.Left < player.Left)
                    {
                        x.Left += virusSpeed;
                        ((PictureBox)x).Image = Properties.Resources.vright;
                    }

                    if (x.Top > player.Top)
                    {
                        x.Top -= virusSpeed;
                        ((PictureBox)x).Image = Properties.Resources.vup;
                    }

                    if (x.Top < player.Top)
                    {
                        x.Top += virusSpeed;
                        ((PictureBox)x).Image = Properties.Resources.vdown;
                    }
                }

                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "virus")
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();

                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();

                            virusList.Remove((PictureBox)x);
                            SpawnVirus();
                        }
                    }
                }
            }
        }

        private void KeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Space && sanatizer > 0 && gameOver == false )
            {
                sanatizer--;
                ShootBullet(facing);

                if(sanatizer == 3 )
                {
                    DropSanatizer();
                }
            }
            
        }

        private void KeyDownEvent(object sender, KeyEventArgs e)
        {
            if (gameOver == true)
            {
                return;
            }

            if (e.KeyCode==Keys.Left)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }
        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        private void SpawnVirus()
        {
            PictureBox virus = new PictureBox();
            virus.Tag = "virus";
            virus.Image = Properties.Resources.vdown;
            virus.Left = randNum.Next(0, 900);
            virus.Top = randNum.Next(0, 800);
            virus.SizeMode = PictureBoxSizeMode.AutoSize;
            virusList.Add(virus);
            this.Controls.Add(virus);
            player.BringToFront();
        }

        private void DropSanatizer()
        {
            PictureBox sanatizerPB = new PictureBox();
            sanatizerPB.Image = Properties.Resources.san;
            sanatizerPB.SizeMode = PictureBoxSizeMode.AutoSize;
            sanatizerPB.Left = randNum.Next(10, this.ClientSize.Width - sanatizerPB.Width);
            sanatizerPB.Top = randNum.Next(50, this.ClientSize.Height - sanatizerPB.Height);
            sanatizerPB.Tag = "ammo";
            this.Controls.Add(sanatizerPB);
            sanatizerPB.BringToFront();
            player.BringToFront();
            
        }

        private void GameOver()
        {
            DialogResult dialog = MessageBox.Show("Your Score: " + score + "\nDo You Want to Try Again ?", "Game Over!", MessageBoxButtons.RetryCancel);
            if (dialog == DialogResult.Retry)
            {
                RestartGame();
            }
            else if (dialog == DialogResult.Cancel)
            {
                DialogResult dialog2 = MessageBox.Show("Wash Your Hands, Wear a Mask, and Use Sanatizer!\n\nLets Remove Covid-19 From Earth", "Message From Developers...!", MessageBoxButtons.OK);
                Environment.Exit(0);
            }
        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.up;

            foreach (PictureBox i in virusList)
            {
                this.Controls.Remove(i);
            }

            virusList.Clear();

            for (int i = 0; i < 3; i++)
            {
                SpawnVirus();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            sanatizer = 10;
            
            GameTimer.Start();
        }

        private void Begin()
        {
            DialogResult dialog = MessageBox.Show("Welcome to The Covid Killer Game\n\nKill The Corona Virus Using Sanatizer and Save The World From CoVid-19.", "COVID KILLER", MessageBoxButtons.OKCancel);
            if (dialog == DialogResult.OK)
            {
                RestartGame();
            }
            else if (dialog == DialogResult.Cancel)
            {
                DialogResult dialog2 = MessageBox.Show("Wash Your Hands, Wear a Mask, and Use Sanatizer!\n\nLets Remove Covid-19 From Earth", "Message From Developers...!", MessageBoxButtons.OK);
                Environment.Exit(0);
                
            }
        }
    }
}
