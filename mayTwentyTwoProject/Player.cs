using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
namespace DodgeGameAlonKolyakov
{
    [Serializable]
    public class Player : Character
    {
        public const int id = 1;
        public Player(int _width, int _height, int _x, int _y,int _speed) :
            base(_width, _height, _x, _y, _speed) 
        {
            this.isAlive = true;
            this.type = id;
            this.rect = NewRectangle();
        }
        protected override Rectangle NewRectangle()
        {
            Rectangle tempRect = new Rectangle() {
                RadiusX = _radius,
                RadiusY = _radius,
                Width = this._width,
                Height = this._height,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/angryBird.jpg")) }
            };
            Canvas.SetLeft(tempRect, _x);
            Canvas.SetTop(tempRect, _y);
            return tempRect;
        }
        public void Move(KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.Left:
                    Canvas.SetLeft(this.rect, this._x - _speed);
                    this._x = this._x - _speed;
                    if (this._x + this._width < 0)     // set the user from the other side
                        this._x = (int)(MainPage.designWidth * MainPage._scaleBoardWidth);
                    break;
                case VirtualKey.Right:
                    Canvas.SetLeft(this.rect, this._x + _speed);
                    this._x = this._x + _speed;
                    if (this._x > MainPage.designWidth * MainPage._scaleBoardWidth)
                        this._x = 0 - this._width;
                    break;
                case VirtualKey.Up:
                    Canvas.SetTop(this.rect, this._y - _speed);
                    this._y = this._y - _speed;
                    if (this._y + this._height < 0)            
                        this._y = (int)(MainPage.designHeight * MainPage._scaleBoardHeight);
                    break;
                case VirtualKey.Down:
                    Canvas.SetTop(this.rect, this._y + _speed);
                    this._y = this._y + _speed;
                    if (this._y > MainPage.designHeight * MainPage._scaleBoardHeight)
                        this._y = 0 - this._height;
                    break;
            }
            Debug.WriteLine(this._x + ", " + this._y, "my_coordinates");
        }       // movement of the player
    }
}