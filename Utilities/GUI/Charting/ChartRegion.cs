namespace Utilities.GUI.Charting
{
    public class ChartRegion
    {
        private float _left;
        private float _top;
        private float _width;
        private float _height;

        public ChartRegion() : this(0, 0, 0, 0)
        {
        }

        public ChartRegion(float left, float top, float width, float height)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }

        public void grow(float width, float height)
        {
            _left = _left - (width / 2);
            _width = _width + width;
            _top = _top - (height / 2);
            _height = _height + height;
        }

        public void splitHorizontal(float split, out ChartRegion top, out ChartRegion bottom)
        {
            // Top region
            top = clone();
            top.Height = split;

            // Bottom region
            bottom = clone();
            bottom.Top += split;
            bottom.Height -= split;
        }

        public void splitAxes(float split, out ChartRegion chart, out ChartRegion x_axis, out ChartRegion y1_axis, out ChartRegion y2_axis)
        {
            chart = clone();
            chart._left += split;
            chart._width -= 2 * split;
            chart._height -= split;

            x_axis = clone();
            x_axis._left += split;
            x_axis._width -= 2 * split;
            x_axis._top = _top + _height - split;
            x_axis._height = split;

            y1_axis = clone();
            y1_axis._width = split;
            y1_axis._height -= split;

            y2_axis = clone();
            y2_axis._left = _left + _width - split;
            y2_axis._width = split;
            y2_axis._height -= split;
        }

        public ChartRegion clone()
        {
            return new ChartRegion(_left, _top, _width, _height);
        }

        public float Width => _width;

        public float Height
        {
            get => _height;

            set => _height = value;
        }

        public float Top
        {
            get => _top;

            set => _top = value;
        }

        public float Bottom => _top + _height;

        public float Left => _left;

        public float Right => _left + _width;

    }
}

