using System;
using Utilities.GUI.Brainstorm.Connectors;
using Utilities.GUI.Brainstorm.Nodes;
using Utilities.GUI.Brainstorm.Nodes.SimpleNodes;
using Utilities.Random;

namespace Utilities.GUI.Brainstorm.SceneManager
{
    class SampleSceneGenerator
    {
        private SampleSceneGenerator() { }

        public static void CreateSampleScene_Coordinates(SceneRenderingControl scene_rendering_control)
        {
            int EXTENT = 30;
            int SCALE = 100;

            NodeControl nx = scene_rendering_control.AddNewNodeControl(new StringNodeContent() { Text = "nx" }, -2 * SCALE * EXTENT, 0, 100, 100);
            NodeControl px = scene_rendering_control.AddNewNodeControl(new StringNodeContent() { Text = "px" }, +2 * SCALE * EXTENT, 0, 100, 100);
            NodeControl ny = scene_rendering_control.AddNewNodeControl(new StringNodeContent() { Text = "ny" }, 0, -2 * SCALE * EXTENT, 100, 100);
            NodeControl py = scene_rendering_control.AddNewNodeControl(new StringNodeContent() { Text = "py" }, 0, +2 * SCALE * EXTENT, 100, 100);            
            
            for (int x = -EXTENT; x <= EXTENT; ++x)
            {
                for (int y = -EXTENT; y <= EXTENT; ++y)
                {
                    double width = 100 / (1 + Math.Abs(x) + Math.Abs(y));
                    double height = 100 / (1 + Math.Abs(x) + Math.Abs(y));

                    double xpos = x * SCALE;
                    double ypos = y * SCALE;

                    StringNodeContent snc = new StringNodeContent();
                    snc.Text = String.Format("{0} {1}", xpos, ypos);

                    NodeControl node_control = scene_rendering_control.AddNewNodeControl(snc, xpos, ypos, width, height);

                    if (x == -EXTENT)
                    {
                        ConnectorControl cc = new ConnectorControl(scene_rendering_control);
                        cc.SetNodes(nx, node_control);
                        scene_rendering_control.AddNewConnectorControl(cc);
                    }
                    if (x == +EXTENT)
                    {
                        ConnectorControl cc = new ConnectorControl(scene_rendering_control);
                        cc.SetNodes(px, node_control);
                        scene_rendering_control.AddNewConnectorControl(cc);
                    }
                    if (y == -EXTENT)
                    {
                        ConnectorControl cc = new ConnectorControl(scene_rendering_control);
                        cc.SetNodes(ny, node_control);
                        scene_rendering_control.AddNewConnectorControl(cc);
                    }
                    if (y == +EXTENT)
                    {
                        ConnectorControl cc = new ConnectorControl(scene_rendering_control);
                        cc.SetNodes(py, node_control);
                        scene_rendering_control.AddNewConnectorControl(cc);
                    }
                }
            }
        }

        public static void CreateSampleScene_Spiral(SceneRenderingControl scene_rendering_control)
        {
            CreateSampleScene(scene_rendering_control, 0, 0, 0);
            //CreateSampleScene(scene_rendering_control, +2000, +2000, -1);
            //CreateSampleScene(scene_rendering_control, -5000, +5000, +1);
        }

        private static void CreateSampleScene(SceneRenderingControl scene_rendering_control, double x, double y, double skew)
        {
            int N = 300;

            System.Random random = new System.Random();
            string[] icon_filenames = new string[] { "papers/binoculars", "papers/boat", "papers/flame", "papers/radioactive", "papers/bread", "papers/news", "papers/music", "papers/percentage", "papers/postit" };

            NodeControl[] ncs = new NodeControl[N];
            for (int i = 0; i < N; ++i)
            {
                object node_content;

                switch (i % 2)
                {
                    case 0:
                        {
                            IconNodeContent icon_node_content = new IconNodeContent(icon_filenames[random.Next(icon_filenames.Length)]);
                            node_content = icon_node_content;
                            break;
                        }
                    case 1:
                        {
                            StringNodeContent snc = new StringNodeContent();
                            snc.Text = String.Format("Button {0}", i);
                            node_content = snc;
                            break;
                        }
                    default:
                        node_content = null;
                        break;
                }

                double raw_angle = 6 * i * Math.PI / 100;
                double distance = 10 + i * 4 + RandomAugmented.Instance.NextDouble() * 10;
                double left = x + distance * (-2+skew) * Math.Sin(raw_angle) + RandomAugmented.Instance.NextDouble() * 30 - 15;
                double top = y + distance * (+2+skew) * Math.Cos(raw_angle) + RandomAugmented.Instance.NextDouble() * 30 - 15;

                ncs[i] = scene_rendering_control.AddNewNodeControl(node_content, left, top);

                // A line for this to the prev nc
                if (i > 3 && (i / 2) % 20 == 0)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        ConnectorControl cc = new ConnectorControl(scene_rendering_control);
                        cc.SetNodes(ncs[i - j], ncs[i]);
                        scene_rendering_control.AddNewConnectorControl(cc);
                    }
                }
            }
        }

    }
}
