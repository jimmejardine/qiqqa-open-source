using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utilities.GUI.Animation;

namespace Utilities.GUI
{
    public class BackgroundFader
    {
        UserControl control;
        Color color_focussed;
        Color color_unfocussed;

        public BackgroundFader(UserControl control) :
            this(control, Colors.SteelBlue, Colors.LightSteelBlue)
        {
        }

        public BackgroundFader(UserControl control, Color color_focussed, Color color_unfocussed)
        {
            this.control = control;
            this.color_focussed = color_focussed;
            this.color_unfocussed = color_unfocussed;

            this.control.Background = new SolidColorBrush(color_unfocussed);

            this.control.MouseEnter += DocumentNodeContentControl_MouseEnter;
            this.control.MouseLeave += DocumentNodeContentControl_MouseLeave;
        }

        void DocumentNodeContentControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.control.Background = Animations.GetAnimatedBrush(color_unfocussed, color_focussed, 300);
        }

        void DocumentNodeContentControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.control.Background = Animations.GetAnimatedBrush(color_focussed, color_unfocussed, 1000);
        }
    }
}
