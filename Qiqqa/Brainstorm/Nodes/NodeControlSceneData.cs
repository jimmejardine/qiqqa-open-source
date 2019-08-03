using System;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class NodeControlSceneData
    {
        internal object node_content = null;

        internal Guid guid = Guid.NewGuid();

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        double current_scale_at_last_resize; // NB THIS CANT BE REMOVED OR IT WILL DESTROY THE SERIALIZATION...

        double original_left = 20;  // NB: This is now centre_x - can't change because of serialization
        double original_top = 20; // NB: This is now centre_y - can't change because of serialization
        double original_width = 20;
        double original_height = 20;

        public bool Deleted { get; set; }

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        public double CurrentScaleAtLastResize
        {
            get
            {
                throw new Exception("Obsolete");
            }
        }

        public Guid Guid
        {
            get
            {
                return guid;
            }
        }

        public double CentreX
        {
            get
            {
                return original_left;
            }
        }

        public void SetDeltaCentreX(double v)
        {
            original_left += v;
        }
        
        public void SetCentreX(double v)
        {
            original_left = v;
        }

        public double CentreY
        {
            get
            {
                return original_top;
            }
        }

        public void SetDeltaCentreY(double v)
        {
            original_top += v;
        }

        public void SetCentreY(double v)
        {
            original_top = v;
        }

        public double Width
        {
            get
            {
                return original_width;
            }
        }

        public void SetDeltaWidth(double v)
        {
            original_width = Math.Max(0.000001, original_width+v);
        }

        public void SetWidth(double v)
        {
            original_width = v;
        }

        public double Height
        {
            get
            {
                return original_height;
            }
        }

        public void SetDeltaHeight(double v)
        {
            original_height = Math.Max(0.000001, original_height+v);
        }

        public void SetHeight(double v)
        {
            original_height = v;
        }

        public double Left { get { return CentreX - Width/2.0d; } }
        public double Top { get { return CentreY - Height / 2.0d; } }
    }
}
