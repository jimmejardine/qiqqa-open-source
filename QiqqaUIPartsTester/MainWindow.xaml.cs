using System;
using System.Threading.Tasks;
using System.Windows;
using Utilities.GUI;
using Utilities.Misc;

namespace QiqqaUIPartsTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Unhandled_Exception(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_BibTeX_Editor(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_BibTex_Editor_Dual_Control(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Embedded_Browser(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Embedded_Browser_With_Interaction(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Background_Task_Keeps_MessagePump_Going_On_Exit(object sender, RoutedEventArgs e)
        {

        }

        private int restartable_task_state = 0;

        private void Button_Click_Restartable_Background_Task(object sender, RoutedEventArgs e)
        {
            restartable_task_state += 10;

            Func<Task> cb = null;
            cb = async () =>
            {
                await Task.Delay(50);

                restartable_task_state += 10;

                WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    LogProgress.Value = restartable_task_state % 1000;
                    LogText.Text = $"state #: { restartable_task_state }";
                });

                if (restartable_task_state < 500 && cb != null)
                {
                    SafeThreadPool.QueueAsyncUserWorkItem(cb);
                }
            };

            SafeThreadPool.QueueAsyncUserWorkItem(cb);
        }
    }
}
