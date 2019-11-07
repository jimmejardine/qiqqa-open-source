using System;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class NodeControlSceneData
    {
        internal object node_content = null;

        internal Guid guid = Guid.NewGuid();

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        private
#pragma warning disable CS0169 // The field 'NodeControlSceneData.current_scale_at_last_resize' is never used
        double current_scale_at_last_resize; // NB THIS CANT BE REMOVED OR IT WILL DESTROY THE SERIALIZATION...
#pragma warning restore CS0169 // The field 'NodeControlSceneData.current_scale_at_last_resize' is never used

        private double original_left = 20;  // NB: This is now centre_x - can't change because of serialization
        private double original_top = 20; // NB: This is now centre_y - can't change because of serialization
        private double original_width = 20;
        private double original_height = 20;

        public bool Deleted { get; set; }

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        public double CurrentScaleAtLastResize => throw new Exception("Obsolete");

        public Guid Guid => guid;

        public double CentreX => original_left;

        public void SetDeltaCentreX(double v)
        {
            original_left += v;
        }

        public void SetCentreX(double v)
        {
            original_left = v;
        }

        public double CentreY => original_top;

        public void SetDeltaCentreY(double v)
        {
            original_top += v;
        }

        public void SetCentreY(double v)
        {
            original_top = v;
        }

        public double Width => original_width;

        public void SetDeltaWidth(double v)
        {
            original_width = Math.Max(0.000001, original_width + v);
        }

        public void SetWidth(double v)
        {
            original_width = v;
        }

        public double Height => original_height;

        public void SetDeltaHeight(double v)
        {
            original_height = Math.Max(0.000001, original_height + v);
        }

        public void SetHeight(double v)
        {
            original_height = v;
        }

        // Note the DIVIDE-BY-2 below; Utilities equiv code didn't have that.
        // See also https://github.com/jimmejardine/qiqqa-open-source/issues/26
        public double Left => CentreX - Width / 2.0d;
        public double Top => CentreY - Height / 2.0d;
    }
}
