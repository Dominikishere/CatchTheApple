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

        private void KeyIsDown(Object sender, KeyEventArgs e)
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

            Canvas.SetLeft(worm, Canvas.GetLeft(worm) + SpeedX);
            Canvas.SetTop(worm, Canvas.GetTop(worm) - SpeedY);

            appleCatching();

            scoreTxt.Text = $"Score: {score}";
        }


        private void spawnApple()
        {
            ImageBrush appleBrush = new ImageBrush();
            var apple = new Image
            {
                Width = 50,
                Height = 50,
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/apple.png"))
            };

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

        private void appleCatching() // AI besegített a collisionnel
        {
            List<Image> applesToRemove = new List<Image>();

            foreach (var item in gameArea.Children)
            {
                if (item is Image apple)
                {
                    Rect wormHitBox = new Rect(
                        Canvas.GetLeft(worm),
                        Canvas.GetTop(worm),
                        worm.Width,
                        worm.Height);

                    Rect appleHitBox = new Rect(
                        Canvas.GetLeft(apple),
                        Canvas.GetTop(apple),
                        apple.Width,
                        apple.Height);

                    if (wormHitBox.IntersectsWith(appleHitBox))
                    {
                        applesToRemove.Add(apple);
                        score++;
                    }
                }
            }

            foreach (var apple in applesToRemove)
            {
                gameArea.Children.Remove(apple);
            }

        }
    }
}