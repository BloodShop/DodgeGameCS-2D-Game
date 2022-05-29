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
        //[JsonConstructor]
        //public Enemy(int _x, int _y, bool isAlive, int _width, int _height, int _radius, int _speed) :
        //    base(_x, _y, isAlive, _width, _height, _radius, _speed)
        //{
        //    this.rect = NewRectangle();
        //}
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

            #region Tring to catch the next step of the User
            //if (!NextStepCollision(user/*, out int xNew, out int yNew)*/)) // if there isnt collision in the next step
            //{
            //    tb = null;
            //}
            //else // there is collision next step that we checked
            //{
            //    //Canvas.SetTop(enemyEllipse, this._y);
            //    //Canvas.SetLeft(enemyEllipse, this._x);
            //    tb = new TextBlock()
            //    {
            //        Text = "YOU LOST!",
            //        TextAlignment = Windows.UI.Xaml.TextAlignment.Center,
            //        FontFamily = new Windows.UI.Xaml.Media.FontFamily("Ariel"),
            //        FontStyle = Windows.UI.Text.FontStyle.Italic,
            //        Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
            //    };
            //    if (this._x + this._radius < user._x + user._radius / 2)
            //        this._x += _speed / 2;
            //    else
            //        this._x -= _speed / 2;

            //    if (this._y + this._radius / 2 < user._y + user._radius / 2)
            //        this._y += _speed / 2;
            //    else
            //        this._y -= _speed / 2;

            //    Canvas.SetTop(this.ellipse, this._y);
            //    Canvas.SetLeft(this.ellipse, this._x);
            //    //Canvas.SetTop(this.ellipse, yNew);
            //    //Canvas.SetLeft(this.ellipse, xNew);
            //}
            #endregion
        }
        #region NEXT STEP COLLISION
        //public bool NextStepCollision(Character character/*, out int xNew, out int yNew*/)
        //{
        //    int xSum = (this._x - character._x);
        //    int ySum = (this._y - character._y);
        //    double differenceDistance = Math.Sqrt(Math.Pow(xSum, 2) + Math.Pow(ySum, 2));

        //    if (differenceDistance <= this._radius + character._radius)
        //    {
        //        if (character is Player)
        //        {

        //        }
        //        //double distanceRadiusRr = Math.Sqrt(Math.Pow(user._radius, 2) + Math.Pow(this._radius, 2));
        //        //differenceDistance = this._radius + user._radius;
        //        //xNew = this._x + (int)Math.Cos(distanceRadiusRr - differenceDistance);
        //        //yNew = this._y + (int)Math.Sin(distanceRadiusRr - differenceDistance);
        //        return true;
        //    }
        //    //xNew = -1;
        //    //yNew = -1;
        //    return false;
        //}
        #endregion
    }
}
