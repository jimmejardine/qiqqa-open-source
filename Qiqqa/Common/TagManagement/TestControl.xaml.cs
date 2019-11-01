namespace Qiqqa.Common.TagManagement
{
#if TEST
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl : UserControl
    {
        public TestControl()
        {
            InitializeComponent();
        }

        class TestData
        {
            public string Tags { set; get; }
        }

        public static void Test()
        {
            TestData td = new TestData();
            AugmentedBindable<TestData> bindable = new AugmentedBindable<TestData>(td);

            TestControl tc = new TestControl();
            tc.DataContext = bindable;
            ControlHostingWindow w = new ControlHostingWindow("Tag editor", tc);
            w.Show();
        }
    }
#endif
}
