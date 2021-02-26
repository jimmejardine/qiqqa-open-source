using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Utilities.GUI.Wizard
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl, ControlHostingWindow.WindowOpenedCapable, ControlHostingWindow.WindowClosedCapable
    {
        private PointOfInterestLocator poi_locator;
        private Route route;

        private DispatcherTimer dispatcher_timer;
        private DoubleAnimation opacity_animation;
        private List<PointOfInterestHighlighterWindow> highlighters = new List<PointOfInterestHighlighterWindow>();

        public Player(PointOfInterestLocator poi_locator, Route route)
        {
            Theme.Initialize();

            InitializeComponent();

            Background = ThemeColours.Background_Brush_Blue_VeryDark;

            this.poi_locator = poi_locator;
            this.route = route;

            CmdNext.Background = Brushes.LightGreen;
            CmdNext.Click += CmdNext_Click;

            dispatcher_timer = new DispatcherTimer();
            dispatcher_timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dispatcher_timer.Tick += dispatcher_timer_Tick;

            opacity_animation = new DoubleAnimation(0.5, 1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            opacity_animation.RepeatBehavior = RepeatBehavior.Forever;
            opacity_animation.AutoReverse = true;
            opacity_animation.AccelerationRatio = 0.1;

            ControlHostingWindow window = new ControlHostingWindow(route.Title, this);
            window.BorderThickness = new Thickness(0);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.Topmost = true;
            window.Show();
        }

        public void OnWindowOpened()
        {
            ApplyCurrentStep();
            dispatcher_timer.Start();
        }

        public void OnWindowClosed()
        {
            dispatcher_timer.Stop();

            foreach (var highlighter in highlighters)
            {
                highlighter.Close();
            }
        }

        private void dispatcher_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                RefreshVisualsForCurrentStep();
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Exception while RefreshVisualsForCurrentStep");
            }
        }

        private static Point ZERO_POINT = new Point(0, 0);

        private void RefreshVisualsForCurrentStep()
        {
            int current_highlighter = 0;

            if (null != route.CurrentStep)
            {
                poi_locator.Refresh();
                bool conditions_satisfied = route.CurrentStep.PostCondition(poi_locator);

                // Add a highlighter to each point of interest                
                {
                    string[] relevant_pois = conditions_satisfied ? route.CurrentStep.PostCondition_PointOfInterests : route.CurrentStep.PointOfInterests;
                    if (null != relevant_pois)
                    {
                        foreach (string poi in relevant_pois)
                        {
                            List<DependencyObject> poi_list = poi_locator.GetPOI(poi);
                            if (null != poi_list)
                            {
                                foreach (DependencyObject poi_do in poi_list)
                                {
                                    FrameworkElement fe = poi_do as FrameworkElement;
                                    if (null != fe && null != PresentationSource.FromVisual(fe))
                                    {
                                        try
                                        {
                                            Point fe_point = fe.PointToScreen(ZERO_POINT);

                                            if (highlighters.Count <= current_highlighter)
                                            {
                                                highlighters.Add(new PointOfInterestHighlighterWindow());
                                            }

                                            int MARGIN = 5;
                                            PointOfInterestHighlighterWindow highlighter = highlighters[current_highlighter];
                                            highlighter.Left = fe_point.X - MARGIN;
                                            highlighter.Top = fe_point.Y - MARGIN;
                                            highlighter.Width = fe.ActualWidth + 2 * MARGIN;
                                            highlighter.Height = fe.ActualHeight + 2 * MARGIN;
                                            highlighter.Show();
                                            ++current_highlighter;
                                        }
                                        catch (Exception ex)
                                        {
                                            Logging.Warn(ex, "Problem highlighting a control for a wizard.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (conditions_satisfied)
                {
                    TxtInstructions.Foreground = (route.CurrentStep.PostCondition_GreyInstructions) ? Brushes.LightGray : Brushes.Black;
                    if (true != CmdNext.IsEnabled)
                    {
                        CmdNext.IsEnabled = true;
                        CmdNext.BeginAnimation(OpacityProperty, opacity_animation);
                    }
                }
                else
                {
                    TxtInstructions.Foreground = Brushes.Black;
                    if (false != CmdNext.IsEnabled)
                    {
                        CmdNext.IsEnabled = false;
                        CmdNext.BeginAnimation(OpacityProperty, null);
                    }
                }
            }
            else
            {
                TxtInstructions.Foreground = Brushes.Black;
                if (true != CmdNext.IsEnabled)
                {
                    CmdNext.IsEnabled = true;
                    CmdNext.BeginAnimation(OpacityProperty, opacity_animation);
                }
            }

            // Kill any unused highlighters
            while (highlighters.Count > current_highlighter)
            {
                highlighters[current_highlighter].Close();
                highlighters.RemoveAt(current_highlighter);
            }
        }

        private void ApplyCurrentStep()
        {
            if (null != route.CurrentStep)
            {
                TxtInstructions.Text = route.CurrentStep.Instructions;
                TxtProgress.Text = String.Format("Step {0} of {1}.", route.CurrentStepIndex + 1, route.MaxStepIndex);
                CmdNext.Content = "Next";
            }
            else
            {
                TxtInstructions.Text = route.CompletionMessage;
                TxtProgress.Text = "";
                CmdNext.Content = "Close";
            }

            RefreshVisualsForCurrentStep();
        }

        private void CmdNext_Click(object sender, RoutedEventArgs e)
        {
            if (null == route.CurrentStep)
            {
                ControlHostingWindow.CloseHostedControl(this);
            }
            else
            {
                route.StepNext();
                ApplyCurrentStep();
            }
        }
    }
}
