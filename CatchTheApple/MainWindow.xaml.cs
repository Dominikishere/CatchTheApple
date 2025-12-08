using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CatchTheApple
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private Random random = new Random();
        int score = 0;
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

            Canvas.SetLeft(apple, random.Next(0, 750));
            Canvas.SetTop(apple, random.Next(0, 350));

            gameArea.Children.Add(apple);
        }
    }
}