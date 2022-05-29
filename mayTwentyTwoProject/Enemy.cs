using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
namespace DodgeGameAlonKolyakov
{
    //[Serializable]
    public class Enemy : Character
    {
        public const int id = 2;
        public Enemy(int _width, int _height, int _x, int _y, int _speed) :
            base(_width, _height, _x , _y, _speed)
        {
            this.rect = NewRectangle();
            this.type = id;
        }
        protected override Rectangle NewRectangle()
        {
            Rectangle tempRect = new Rectangle() {
                RadiusX = _radius * 2,
                RadiusY = _radius * 2,
                Width = this._width,
                Height = this._height,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
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
            Canvas.SetLeft(tempRect, _x - _radius); Canvas.SetTop(tempRect, _y - _radius);
            return tempRect;
        }
        public void Move(Player user)
        {
            if (this._x + this._radius < user._x + user._radius)
                this._x += _speed;
            else
                this._x -= _speed;

            if (this._y + this._radius < user._y + user._radius)
                this._y += _speed;
            else
                this._y -= _speed;
            Canvas.SetTop(this.rect, this._y);
            Canvas.SetLeft(this.rect, this._x);
        }
    }
}
