using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI.Wizard
{
    public class Test
    {
        private DockPanel dock_panel;
        private CheckBox check_box;
        private TextBlock text_block;

        public Test()
        {
            dock_panel = new DockPanel();

            check_box = new CheckBox();
            check_box.Checked += check_box_Checked;
            check_box.Unchecked += check_box_Unchecked;
            DockPanel.SetDock(check_box, Dock.Top);
            dock_panel.Children.Add(check_box);

            Button button = new Button();
            button.Content = "YOOHOOO";
            button.Click += button_Click;
            DockPanel.SetDock(check_box, Dock.Bottom);
            dock_panel.Children.Add(button);

            text_block = new TextBlock();
            text_block.Text = "HELLOOOOOOO";
            dock_panel.Children.Add(text_block);


            WizardDPs.SetPointOfInterest(text_block, "TB");
            WizardDPs.SetPointOfInterest(check_box, "Wizard:1:CheckBox");
            WizardDPs.SetPointOfInterest(button, "Wizard:1:Button");

            ControlHostingWindow chw = new ControlHostingWindow("Test", dock_panel);

            PointOfInterestLocator poi_locator = new PointOfInterestLocator();

            Player player = new Player(poi_locator, TestWizard.GetRoute());

            chw.Show();


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            TextBlock tb = new TextBlock();
            tb.Text = "Super";
            ControlHostingWindow window = new ControlHostingWindow("New window", tb);
            WizardDPs.SetPointOfInterest(window, "Wizard:1:TargetWindow");
            window.Show();
        }

        public static void Go()
        {
            new Test();
        }

        private void check_box_Unchecked(object sender, RoutedEventArgs e)
        {
            text_block.Visibility = Visibility.Collapsed;
        }

        private void check_box_Checked(object sender, RoutedEventArgs e)
        {
            text_block.Visibility = Visibility.Visible;

        }
    }
}
