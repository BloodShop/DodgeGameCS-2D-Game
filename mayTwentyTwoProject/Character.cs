using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
namespace DodgeGameAlonKolyakov
{
    public class Character
    {
        protected Random r = new Random();
        // Common fields
        public int _x { get; set; }
        public int _y { get; set; }
        public bool isAlive { get; set; }
        public Rectangle rect { get; set; }
        public int _width { get; set; }
        public int _height { get; set; }
        public int _radius { get; set; }
        public int _speed { get; set; }
        public int type { get; set; }

        [JsonConstructor]
        public Character(int _x, int _y, bool isAlive, int _width, int _height, int _radius, int _speed, int type)
        {
            this._x = _x;
            this._y = _y;
            this.isAlive = isAlive;
            this._width = _width;
            this._height = _height;
            this._radius = _radius;
            this._speed = _speed;
            this.rect = LoadRectangle(type);
        }
        public Character(int characterWidth, int characterHeight, int xLocation, int yLocation, int speedCharacter)
        {
            this._width = characterWidth;
            this._height = characterHeight;
            this._radius = characterWidth / 2;
            this._x = xLocation;
            this._y = yLocation;
            this._speed = speedCharacter;
            this.isAlive = false;
        }
        public Character() { }
        protected virtual Rectangle NewRectangle()
        {
            Rectangle tempRect = new Rectangle();
            return tempRect;
        }
        protected Rectangle LoadRectangle(int type)
        {
            Rectangle tempRect = new Rectangle()
            #region
            {
                RadiusX = _radius,
                RadiusY = _radius,
                Width = this._width,
                Height = this._height,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            if (type == 1)
                tempRect.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/angryBird.jpg")) };
            else if (type == 2)
            {
                switch (r.Next(1, 4))
                {
                    case 1:
                        tempRect.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/enemy1.jpg")) };
                        break;
                    case 2:
                        tempRect.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/enemy2.jpg")) };
                        break;
                    case 3:
                        tempRect.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/enemy3.jpg")) };
                        break;
                }
            }
            else if (type == 3)
                tempRect.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/btc.png")) };

            Canvas.SetLeft(tempRect, _x);
            Canvas.SetTop(tempRect, _y);
            #endregion
            return tempRect;
        }
    }
}