using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CatchTheApple
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer appleRemover = new DispatcherTimer();

        private Random random = new Random();
        private Random appleRandom = new Random();

        private int score = 0;
        private int health = 3;

        private bool KeyW, KeyA, KeyS, KeyD;
        private float SpeedY, SpeedX, Friction = 0.88f, Speed = 2;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += gameLoop;
            timer.Start();
            gameArea.Focus();

            ImageBrush playerBrush = new ImageBrush();
            playerBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/worm.png"));
            worm.Fill = playerBrush;
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) KeyW = false;
            if (e.Key == Key.A) KeyA = false;
            if (e.Key == Key.S) KeyS = false;
            if (e.Key == Key.D) KeyD = false;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) KeyW = true;
            if (e.Key == Key.A) KeyA = true;
            if (e.Key == Key.S) KeyS = true;
            if (e.Key == Key.D) KeyD = true;
        }
        private void gameLoop(object sender, EventArgs e)
        {
            if (random.NextDouble() < 0.05)
            {
                spawnApple();
            }

            if (KeyW)
            {
                SpeedY += Speed;
            }
            if (KeyA)
            {
                SpeedX -= Speed;
            }
            if (KeyS)
            {
                SpeedY -= Speed;
            }
            if (KeyD)
            {
                SpeedX += Speed;
            }

            SpeedY *= Friction;
            SpeedX *= Friction;

            double newX = Canvas.GetLeft(worm) + SpeedX;
            double newY = Canvas.GetTop(worm) - SpeedY;

            // Clamp worm inside canvas (0 to canvas width/height minus worm size)
            newX = Math.Max(0, Math.Min(newX, gameArea.ActualWidth - worm.Width));
            newY = Math.Max(0, Math.Min(newY, gameArea.ActualHeight - worm.Height));

            Canvas.SetLeft(worm, newX);
            Canvas.SetTop(worm, newY);

            appleCatching();

            scoreTxt.Text = $"Score: {score}";
            healthTxt.Text = $"health: {health}";

            gameOver();

        }


        private void spawnApple()
        {
            ImageBrush appleBrush = new ImageBrush();
            ImageBrush rottenAppleBrush = new ImageBrush();

            var apple = new Image
            {
                Width = 50,
                Height = 50,
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/apple.png"))
            };

            var rottenApple = new Image
            {
                Width= 50,
                Height= 50,
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/rottenApple.png"))
            };

            apple.Tag = "good";
            rottenApple.Tag = "bad";

            if (appleRandom.Next(0,4) == 0)
            {
                var wormX = Canvas.GetLeft(worm);
                var wormY = Canvas.GetTop(worm);

                double appleX = 0; //  AI segítség, hogy ne spawnoljon alma a worm hitboxába, valamint köré se annyira
                double appleY = 0;
                double minDistance = 120;

                double distance = 0;

                while (distance < minDistance)
                {
                    appleX = random.Next(0, 750);
                    appleY = random.Next(0, 350);

                    double dx = appleX - wormX;
                    double dy = appleY - wormY;
                    distance = Math.Sqrt(dx * dx + dy * dy);
                }

                Canvas.SetLeft(rottenApple, appleX);
                Canvas.SetTop(rottenApple, appleY);

                gameArea.Children.Add(rottenApple);

                DispatcherTimer appleTimer = new DispatcherTimer();
                appleTimer.Interval = TimeSpan.FromSeconds(3);
                appleTimer.Tick += (s, e) =>
                {
                    appleTimer.Stop();
                    gameArea.Children.Remove(rottenApple);
                };

                appleTimer.Start();

            } else
            {
                var wormX = Canvas.GetLeft(worm);
                var wormY = Canvas.GetTop(worm);

                double appleX = 0;
                double appleY = 0;
                double minDistance = 120;

                double distance = 0;

                while (distance < minDistance)
                {
                    appleX = random.Next(0, 750);
                    appleY = random.Next(0, 350);

                    double dx = appleX - wormX;
                    double dy = appleY - wormY;
                    distance = Math.Sqrt(dx * dx + dy * dy);
                }

                Canvas.SetLeft(apple, appleX);
                Canvas.SetTop(apple, appleY);

                gameArea.Children.Add(apple);

                DispatcherTimer appleTimer = new DispatcherTimer();
                appleTimer.Interval = TimeSpan.FromSeconds(3);
                appleTimer.Tick += (s, e) =>
                {
                    appleTimer.Stop();
                    gameArea.Children.Remove(apple);
                };

                appleTimer.Start();
            }
        }

        private void appleCatching() // AI besegített a collisionnel
        {
            List<Image> toRemove = new List<Image>();

            Rect wormHitBox = new Rect(
                        Canvas.GetLeft(worm),
                        Canvas.GetTop(worm),
                        worm.Width,
                        worm.Height);


            foreach (var item in gameArea.Children)
            {
                if (item is Image img)
                {
                    string? type = img.Tag as string;

                    Rect hitBox = new Rect(
                        Canvas.GetLeft(img),
                        Canvas.GetTop(img),
                        img.Width,
                        img.Height);

                    if (wormHitBox.IntersectsWith(hitBox))
                    {
                        if (type == "good")
                        {
                            score++;
                        }
                        else if (type == "bad")
                        {
                            health--;
                        }

                        toRemove.Add(img);
                    }
                }
            }
            foreach (var img in toRemove)
            {
                gameArea.Children.Remove(img);
            }
        }

        private void gameOver()
        {

            if (score == 30)
            {
                Canvas.SetLeft(worm, 350);
                Canvas.SetTop(worm, 180);

                health = 3;
                score = 0;

                SpeedX = 0;
                SpeedY = 0;
                KeyW = KeyA = KeyS = KeyD = false;

                MessageBox.Show("You Won!", "Game Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (health == 0)
            {
                Canvas.SetLeft(worm, 350);
                Canvas.SetTop(worm, 180);
                
                health = 3;
                score = 0;

                SpeedX = 0;
                SpeedY = 0;
                KeyW = KeyA = KeyS = KeyD = false;

                MessageBox.Show("Game Over!", "Game Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}