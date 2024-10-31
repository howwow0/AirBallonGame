using System;
using System.Drawing;
using System.Windows.Forms;

namespace AirBallonGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool _isGameRunning = false;
        private readonly Random _random = new Random(DateTime.Now.Millisecond);
        private const string KeysFirstPlayer = "QWERTASDFZXCV";
        private const string KeysSecondPlayer = "YUIOPGHJKLBNM";
        private const string TextWinFirstPlayer = "Выйграл первый игрок.";
        private const string TextWinSecondPlayer = "Выйграл второй игрок.";
        private const string TextGameOver = "Игра окончена.";
        private const string TextStartGame = "Старт игры";
        private const string TextReadyToGame = "Вы готовы начать игру?";
        private const int MillisecondsForClick = 1500; // Время на нажатие кнопки
        private const int StepAirBallons = 15; // Количество пикселей на которое прыгает шар
        private const int StartHeight = 300; // Стартовая высота шаров

        private string _generatedKeyFirstPlayer = string.Empty;
        private string _generatedKeySecondPlayer = string.Empty;

        private void GenerateKey(bool isFirstPlayer)
        {
            string keys = isFirstPlayer ? KeysFirstPlayer : KeysSecondPlayer;
            string generatedKey = keys[_random.Next(keys.Length)].ToString();
            if (isFirstPlayer)
            {
                _generatedKeyFirstPlayer = generatedKey;
                label1.Text = generatedKey;
            }
            else
            {
                _generatedKeySecondPlayer = generatedKey;
                label2.Text = generatedKey;
            }
        }

        private void MoveBalloon(PictureBox pictureBox, int step)
        {
            var point = pictureBox.Location;
            point.Y += step;
            pictureBox.Location = point;
        }

        private void CheckBalloonPosition(PictureBox balloon, bool isFirstPlayer)
        {
            bool hasCollided = isFirstPlayer
                ? IsColliding(balloon, pictureBox1) || IsColliding(balloon, pictureBox5)
                : IsColliding(balloon, pictureBox2) || IsColliding(balloon, pictureBox6);

            bool winFirstPlayer = isFirstPlayer
                ? IsColliding(balloon, pictureBox5)
                : IsColliding(balloon, pictureBox2);

            if (hasCollided)
            {
                GameOver(winFirstPlayer);
            }
        }

        private void GameOver(bool winFirstPlayer)
        {
            _isGameRunning = false;
            timer1.Stop();
            timer2.Stop();
            string gameOverText =
                $"{TextGameOver} {(winFirstPlayer ? TextWinFirstPlayer : TextWinSecondPlayer)} {TextReadyToGame}";
            DialogResult result = MessageBox.Show(gameOverText, TextGameOver, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                StartGame();
            }
            else
            {
                Application.Exit();
            }
        }

        private void StartGame()
        {
            _isGameRunning = true;
            pictureBox3.Location = new Point(pictureBox3.Location.X, StartHeight);
            pictureBox3.BackColor = Color.Transparent;
            pictureBox4.Location = new Point(pictureBox4.Location.X, StartHeight);
            pictureBox4.BackColor = Color.Transparent;
            GenerateKey(true);
            GenerateKey(false);
            ResetTimer(timer1);
            ResetTimer(timer2);
        }

        private void ResetTimer(Timer timer)
        {
            timer.Interval = MillisecondsForClick;
            timer.Stop();
            timer.Start();
        }

        private bool IsColliding(PictureBox balloon, PictureBox boundary)
        {
            return balloon.Bounds.IntersectsWith(boundary.Bounds);
        }

        private void TimerTick(PictureBox pictureBox, bool isFirstPlayer)
        {
            MoveBalloon(pictureBox, StepAirBallons);
            CheckBalloonPosition(pictureBox, isFirstPlayer);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (MessageBox.Show(TextReadyToGame, TextStartGame, MessageBoxButtons.YesNo, MessageBoxIcon.Question) !=
                DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                StartGame();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isGameRunning) return;

            string keyString = e.KeyCode.ToString().ToUpper();
            if (keyString == _generatedKeyFirstPlayer)
            {
                MoveBalloon(pictureBox3, -StepAirBallons);
                CheckBalloonPosition(pictureBox3, true);
                GenerateKey(true);
                ResetTimer(timer1);
            }
            else if (keyString == _generatedKeySecondPlayer)
            {
                MoveBalloon(pictureBox4, -StepAirBallons);
                CheckBalloonPosition(pictureBox4, false);
                GenerateKey(false);
                ResetTimer(timer2);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimerTick(pictureBox3, true);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            TimerTick(pictureBox4, false);
        }
    }
}