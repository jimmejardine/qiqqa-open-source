using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI
{
    /// <summary>
    /// Use this class to access "cached" values for some of the common dependency properties. 
    /// It is a lot faster setting htem using this if you very frequently set them to the same value (i.e. don't actually change them...)
    /// For more caching types see PropertyShadowForPanels...
    /// </summary>
    public class PropertyShadow
    {
        private FrameworkElement fe;

        public PropertyShadow(FrameworkElement fe)
        {
            this.fe = fe;

            // Initialise to match
            _shadow_Left = Canvas.GetLeft(fe);
            _shadow_Top = Canvas.GetTop(fe);
            _shadow_Width = fe.Width;
            _shadow_Height = fe.Height;
            _shadow_Opacity = fe.Opacity;
            _shadow_IsHitTestVisible = fe.IsHitTestVisible;
        }

        private double _shadow_Left = double.NaN;
        public bool SetShadowLeft(double value)
        {
            if (_shadow_Left != value)
            {
                Canvas.SetLeft(fe, _shadow_Left = value);
                return true;
            }
            else
            {
                return false;
            }
        }

        private double _shadow_Top = double.NaN;
        public bool SetShadowTop(double value)
        {
            if (_shadow_Top != value)
            {
                Canvas.SetTop(fe, _shadow_Top = value);
                return true;
            }
            else
            {
                return false;
            }
        }

        private double _shadow_Width = double.NaN;
        public bool SetShadowWidth(double value)
        {
            if (_shadow_Width != value)
            {
                fe.Width = _shadow_Width = value;
                return true;
            }
            else
            {
                return false;
            }
        }
        public double GetShadowWidth()
        {
            return _shadow_Width;
        }


        private double _shadow_Height = double.NaN;
        public bool SetShadowHeight(double value)
        {
            if (_shadow_Height != value)
            {
                fe.Height = _shadow_Height = value;
                return true;
            }
            else
            {
                return false;
            }
        }
        public double GetShadowHeight()
        {
            return _shadow_Height;
        }

        private double _shadow_Opacity = double.NaN;
        public double GetShadowOpacity()
        {
            return _shadow_Opacity;
        }
        public bool SetShadowOpacity(double value)
        {
            if (_shadow_Opacity != value)
            {
                fe.Opacity = _shadow_Opacity = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool _shadow_IsHitTestVisible = false;
        public bool SetShadowIsHitTestVisible(bool value)
        {
            if (_shadow_IsHitTestVisible != value)
            {
                fe.IsHitTestVisible = _shadow_IsHitTestVisible = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
