namespace Qiqqa.Brainstorm.SceneManager
{
#if TEST
    public class TestHarness
    {
        public static void Test()
        {
            Library l = Library.GuestInstance;
            Thread.Sleep(1000);

            BrainstormControl bc = new BrainstormControl();
            //SampleSceneGenerator.CreateSampleScene_Coordinates(bc.ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl);
            //SampleSceneGenerator.CreateSampleScene_Spiral(bc.ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl);
            bc.SceneRenderingControl.OpenFromDisk(@"C:\temp\untitled.brain");
            ControlHostingWindow window = new ControlHostingWindow("SceneRenderer", bc);
            window.Show();
        }
    }
#endif
}
