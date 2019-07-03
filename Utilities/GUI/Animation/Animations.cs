using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Utilities.GUI.Animation
{
    public class Animations
    {
        public static SolidColorBrush GetAnimatedBrush(Color color_from, Color color_to)
        {
            SolidColorBrush border_brush = new SolidColorBrush(color_from);
            ColorAnimation ca = new ColorAnimation(color_from, color_to, new Duration(TimeSpan.FromMilliseconds(300)));
            border_brush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            return border_brush;
        }

        public static SolidColorBrush GetAnimatedBrush(Color color_from, Color color_to, int milliseconds)
        {
            SolidColorBrush border_brush = new SolidColorBrush(color_from);
            ColorAnimation ca = new ColorAnimation(color_from, color_to, new Duration(TimeSpan.FromMilliseconds(milliseconds)));
            border_brush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            return border_brush;            
        }

        public static void EnableHoverFade(UIElement fe)
        {
            double TOOLBAR_OPACITY = 0.2;

            fe.Opacity = TOOLBAR_OPACITY;
            fe.MouseLeave += (object sender, MouseEventArgs e) =>
            {
                DoubleAnimation animation = new DoubleAnimation(1, TOOLBAR_OPACITY, new Duration(TimeSpan.FromMilliseconds(500)));
                fe.BeginAnimation(UIElement.OpacityProperty, animation);
            };

            fe.MouseEnter += (object sender, MouseEventArgs e) =>
            {
                DoubleAnimation animation = new DoubleAnimation(TOOLBAR_OPACITY, 1, new Duration(TimeSpan.FromMilliseconds(500)));
                fe.BeginAnimation(UIElement.OpacityProperty, animation);
            };
        }

        class FadeManager
        {
            static Dictionary<FrameworkElement, FadeManager> responsible_managers = new Dictionary<FrameworkElement, FadeManager>();

            FrameworkElement fe;
            double from_opacity;
            double to_opacity;

            internal FadeManager(FrameworkElement fe, double from_opacity, double to_opacity)
            {
                this.fe = fe;
                this.from_opacity = from_opacity;
                this.to_opacity = to_opacity;

                // Check that we actually have something to do
                if (fe.Opacity == to_opacity)
                {
                    return;
                }

                // Do the before animation stuff
                fe.Opacity = from_opacity;
                fe.Visibility = Visibility.Visible;

                lock (responsible_managers)
                {
                    //Logging.Info("FadeManager: Current {0} animations running", responsible_managers.Count);
                    responsible_managers[fe] = this;

                    DoubleAnimation da = new DoubleAnimation(from_opacity, to_opacity, new Duration(TimeSpan.FromMilliseconds(250)));
                    da.Completed += da_Completed;
                    fe.BeginAnimation(FrameworkElement.OpacityProperty, da, HandoffBehavior.SnapshotAndReplace);
                }
            }

            void da_Completed(object sender, EventArgs e)
            {
                lock (responsible_managers)
                {
                    if (!responsible_managers.ContainsKey(this.fe) || this != responsible_managers[this.fe])
                    {                        
                    }
                    else
                    {                        
                        responsible_managers.Remove(this.fe);

                        fe.Opacity = to_opacity;
                        fe.BeginAnimation(FrameworkElement.OpacityProperty, null);
                        if (0 == to_opacity)
                        {
                            fe.Visibility = Visibility.Hidden;
                        }
                    }                    
                }
            }
        }

        public static void FadeIn(FrameworkElement fe)
        {
            new FadeManager(fe, 0, 1);
        }

        public static void FadeOut(FrameworkElement fe)
        {
            new FadeManager(fe, 1, 0);
        }


        public static void Fade(FrameworkElement fe, double from, double to)
        {
            new FadeManager(fe, from, to);
        }
    }
}
