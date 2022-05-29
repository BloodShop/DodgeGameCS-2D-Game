using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
namespace DodgeGameAlonKolyakov
{
    public class Coin : Character
    {
        public const int id = 3;
        public Coin(int _width, int _height, int _x, int _y) :
           base(_width, _height, _x, _y, 0)
        {
            this.rect = NewRectangle();
            this.type = id;
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
                Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/btc.png")) }
            };
            Canvas.SetLeft(tempRect, _x);
            Canvas.SetTop(tempRect, _y);
            return tempRect;
        }
    }
}
