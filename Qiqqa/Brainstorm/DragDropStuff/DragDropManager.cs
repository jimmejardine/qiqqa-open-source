using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Qiqqa.Brainstorm.SceneManager;

namespace Qiqqa.Brainstorm.DragDropStuff
{
    public class DragDropManager
    {
        public delegate void DragDropTypeHandler(object dropped_object, Point mouse_current_virtual);

        private SceneRenderingControl scene_rendering_control;
        private Dictionary<Type, DragDropTypeHandler> drag_drop_type_handlers = new Dictionary<Type, DragDropTypeHandler>();

        internal DragDropManager(SceneRenderingControl scene_rendering_control)
        {
            this.scene_rendering_control = scene_rendering_control;
        }

        public SceneRenderingControl SceneRenderingControl => scene_rendering_control;

        public void RegisterDropType(Type type, DragDropTypeHandler handler)
        {
            if (drag_drop_type_handlers.ContainsKey(type))
            {
                throw new Exception(String.Format("The type {0} already has a drop handler", type));
            }

            drag_drop_type_handlers[type] = handler;
        }


        public bool CanDrop(DragEventArgs e)
        {
            foreach (Type type in drag_drop_type_handlers.Keys)
            {
                if (e.Data.GetDataPresent(type))
                {
                    return true;
                }
            }

            // If we get here, we don't know how to handle this drop item
            return false;
        }

        public bool OnDrop(DragEventArgs e, Point mouse_current_virtual)
        {
            foreach (Type type in drag_drop_type_handlers.Keys)
            {
                if (e.Data.GetDataPresent(type))
                {
                    object dropped_object = e.Data.GetData(type);
                    DragDropTypeHandler drag_drop_type_handler = drag_drop_type_handlers[type];
                    drag_drop_type_handler(dropped_object, mouse_current_virtual);
                    return true;
                }
            }

            // If we get here, we don't know how to handle this drop item
            return false;
        }

        public static string DumpUnknownDropTypes(DragEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Brainstorm does not support the dropping of any of:");
            foreach (string format in e.Data.GetFormats(true))
            {
                sb.Append(format);
                sb.Append(' ');
            }

            return sb.ToString();
        }
    }
}
