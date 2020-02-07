/*
 * Description: A basic PONG simulator
 * Author:  Deo Narayan         
 * Date: February 6, 2020    
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
        SolidBrush purpleBrush = new SolidBrush(Color.Purple);
        Font drawFont = new Font("Courier New", 10);
        
        //random number generator
        Random randGen = new Random();

        //decider of ball direction
        int ballDirection = 0;
        
        //decider of power up
        int powerUpChooser;

        //determines colour of paddles
        Boolean p1Visible = true;
        Boolean p2Visible = true;

        //determine direction of paddles
        Boolean p1Inverse = false;
        Boolean p2Inverse = false;

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int BALL_SPEED = 2;
        Rectangle ball;

        //power up directions, speed, and rectangle
        Rectangle powerUp;
        Boolean powerUpMoveRight = false;
        int POWER_UP_SPEED = 3;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 4;
        Rectangle p1, p2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 3;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        private void endGameButton_Click(object sender, EventArgs e)
        {
           //closes game when end game button is pressed
            this.Close();
        }

        private void playAgainButton_Click(object sender, EventArgs e)
        {
            SetParameters();
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk == true)
            {
                //resets parameters
                BALL_SPEED = 2;
                player1Score = player2Score = 0;
                scoreLabel.Visible = false;
                endGameButton.Visible = false;
                playAgainButton.Visible = false;
                endGameButton.Enabled= false;
                playAgainButton.Enabled = false;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            p1.Width = p2.Width = 10;    //height for both paddles set the same
            p1.Height = p2.Height = 40;  //width for both paddles set the same

            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            //  set Width and Height of ball
            ball.Width = 15;
            ball.Height = 15;

            // set starting X and Y position for ball to middle of screen, (use this.Width and ball.Width)
            ball.X = this.Width / 2;
            ball.Y = this.Height / 2;

            //set Width and Height of power up
            powerUp.Width = 15;
            powerUp.Height = 15;

            // set starting X and Y position for power up to middle of screen, (use this.Width and ball.Width)
            powerUp.X = this.Width / 2;
            powerUp.Y = this.Height / 2;
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            //  create code to move ball either left or right and up or down based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight == true)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            if (ballMoveRight == false)
            {
                ball.X = ball.X - BALL_SPEED;
            }
            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + BALL_SPEED;
            }
            if (ballMoveDown == false)
            {
                ball.Y = ball.Y - BALL_SPEED;
            }

            //code to move power up either left or right based on powerUpMoveRight and using POWER_UP_SPEED
            if (powerUpMoveRight == true)
            {
                powerUp.X = powerUp.X + POWER_UP_SPEED;
            }
            else
            {
                powerUp.X = powerUp.X - POWER_UP_SPEED;
            }

            #endregion

            #region update paddle positions

            if (aKeyDown == true)
            {
                //  create code to move player 1 paddle up using p1.Y and PADDLE_SPEED
                if (p1Inverse == false && p1.Y > 0)
                {
                    //normal parameters
                    p1.Y = p1.Y - PADDLE_SPEED;
                }
                else if (p1.Y < this.Height - p1.Height)
                {
                    //with power up (inverse)
                    p1.Y = p1.Y + PADDLE_SPEED;
                }
                
            }
            if (zKeyDown == true )
            {

                if (p1Inverse == false && p1.Y < this.Height - p1.Height)
                {
                    //normal parameters
                    p1.Y = p1.Y + PADDLE_SPEED;
                }
                else if (p1.Y > 0)
                {
                    //with power up (inverse)
                    p1.Y = p1.Y - PADDLE_SPEED;
                }
            }
            if (jKeyDown == true)
            {
                //  create code to move player 1 paddle up using p1.Y and PADDLE_SPEED
                if (p2Inverse == false && p2.Y > 0)
                {
                    p2.Y = p2.Y - PADDLE_SPEED;
                }
                else if  (p2.Y < this.Height - p2.Height)
                {
                    p2.Y = p2.Y + PADDLE_SPEED;
                }
            }
            if (mKeyDown == true)
            {

                if (p2Inverse == false && p2.Y < this.Height - p2.Height)
                {
                    p2.Y = p2.Y + PADDLE_SPEED;
                }
                else if (p2.Y > 0)
                {
                    p2.Y = p2.Y - PADDLE_SPEED;
                }
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                // use ballMoveDown boolean to change direction
                ballMoveDown = true;
                //play a collision sound
                collisionSound.Play();
              
            }
            else if (ball.Y > this.Height - ball.Height)
            {
                ballMoveDown = false;
                collisionSound.Play();
            }
            
            #endregion
            //ball collides with paddle
            if (ball.IntersectsWith(p2) || ball.IntersectsWith(p1))
            { 
                //inverse ball direction
                ballMoveRight = !ballMoveRight;
                collisionSound.Play(); 
                //increase speed
                BALL_SPEED++;
            }
           
            #region ball collision with paddles

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                //reset parameters
                p1Inverse = p2Inverse = false;
                p1Visible = p2Visible = true;
                p1.Width = p2.Width = 10;
               
                //score sound
                scoreSound.Play();

                //increase player score
                player2Score++;
                scoreLabel.Text = player1Score + " - " + player2Score;
                scoreLabel.Visible = true;

                //pause 1sec
                this.Refresh();
                Thread.Sleep(1000);
                scoreLabel.Visible = false;

                //reset ball position and speed
                ball.Y = this.Height / 2;
                ball.X = this.Width / 2;
                BALL_SPEED = 3;

                //if score results in game over
                if (player2Score == gameWinScore)
                {
                    GameOver("Player 2");
                }

                //randomizes beginning ball direction
                ballDirection = randGen.Next(1, 3);
                if (ballDirection == 1)
                {
                    ballMoveDown = true;
                }
                else
                {
                    ballMoveDown = false;
                }

                ballDirection = randGen.Next(1, 3);
                if (ballDirection == 1)
                {
                    ballMoveRight = true;
                }
                else
                {
                    ballMoveRight = false;
                }
            }

            if (ball.X > this.Width - ball.Width)
            {
                p1Inverse = p2Inverse = false;
                p1Visible = p2Visible = true;
                p1.Width = p2.Width = 10;

                scoreSound.Play();

                player1Score++;
                scoreLabel.Text = player1Score + " - " + player2Score;
                scoreLabel.Visible = true;

                this.Refresh();
                Thread.Sleep(1000);

                scoreLabel.Visible = false;

                ball.Y = this.Height / 2;
                ball.X = this.Width / 2;
                BALL_SPEED = 3;

                if (player1Score == gameWinScore)
                {
                    GameOver("Player 1");
                }

                ballDirection = randGen.Next(1, 3);
                if (ballDirection == 1)
                {
                    ballMoveDown = true;
                }
                else
                {
                    ballMoveDown = false;
                }
                
                ballDirection = randGen.Next(1, 3);
                if (ballDirection == 1)
                {
                    ballMoveRight = true;
                }
                else
                {
                    ballMoveRight = false;
                }
                
               
            }
            
            //power up collides with wall
            if (powerUp.X < 0)
            {
                //return to middle and switch direction
                powerUp.X = this.Width / 2;
                //change power up direction
                powerUpMoveRight = !powerUpMoveRight;
            }

            //power up collides with other wall
            if (powerUp.X > this.Width - powerUp.Width)
            {
                powerUp.X = this.Width / 2;
                powerUpMoveRight = !powerUpMoveRight;
            }

            //power up collides with p1 paddle
            if (powerUp.IntersectsWith(p1))
            {
                //return to middle and switch direction
                powerUp.X = this.Width / 2;
                powerUpMoveRight = !powerUpMoveRight;

                //if black make other paddle invisible
                if (powerUpChooser == 1)
                {
                    p2Visible = false;
                }

                //if green double paddle height
                if (powerUpChooser == 2)
                {
                    p1.Height = p1.Height * 2;
                }

                //if purple inverse other paddle controls
                if (powerUpChooser == 3)
                {
                    p2Inverse = true;
                }
            }

            //power up collides with p1 paddle
            if (powerUp.IntersectsWith(p2))
            {
                powerUp.X = this.Width / 2;
                powerUpMoveRight = !powerUpMoveRight;

                if (powerUpChooser == 1)
                {
                    p1Visible = false;
                }
                if (powerUpChooser == 2)
                {
                    p2.Height = p2.Height * 2;
                }
                if (powerUpChooser == 3)
                {
                    p1Inverse = true;
                }
            }

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            //tell program old game has ended
            newGameOk = true;

            //pause game
            gameUpdateLoop.Stop();

            //say who is winner
            scoreLabel.Text = winner + " is the winner!";
            scoreLabel.Visible = true;

            //pause 2 sec
            Refresh();
            Thread.Sleep(2000);

            //end game options
            endGameButton.Visible = true;
            playAgainButton.Visible = true;
            endGameButton.Enabled = true;
            playAgainButton.Enabled = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //fill in ball in white
            e.Graphics.FillRectangle(whiteBrush, ball);
            
            //if paddle is to be visible
            if (p1Visible == true)
            {
                //fill in paddle in white
                e.Graphics.FillRectangle(whiteBrush, p1);
            }
            //if paddle is to be invisible
            else
            {
                //fill in paddle in black
                e.Graphics.FillRectangle(blackBrush, p1);
            }

            if (p2Visible == true)
            {
                e.Graphics.FillRectangle(whiteBrush, p2);
            }
            else
            {
                e.Graphics.FillRectangle(blackBrush, p2);
            }
            
            //randomly decides power up
            powerUpChooser = randGen.Next(1, 4);

            if (powerUpChooser == 1)
            {
                //black power up 
                e.Graphics.FillRectangle(blackBrush, powerUp);
            }
            
            else if (powerUpChooser == 2)
            {
                //green power up
                e.Graphics.FillRectangle(greenBrush, powerUp);
            }
            else
            {
                //purple power up
                e.Graphics.FillRectangle(purpleBrush, powerUp);
            }
        }
    }
}
