using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities.GUI
{
    /// <summary>
    /// Use this class to access "cached" values for some of the common dependency properties. 
    /// It is a lot faster setting them using this if you very frequently set them to the same value (i.e. don't actually change them...)
    /// This specifically tailors PropertyShadow for Canvas so we can also cache Backgrounds, etc.
    /// </summary>
    public class PropertyShadowForPanels : PropertyShadow
    {
        private Panel panel;
        
        public PropertyShadowForPanels(Panel panel) : base(panel)
        {            
            this.panel = panel;

            this._shadow_Background = panel.Background;
        }

        private Brush _shadow_Background = null;
        public bool SetShadowBackground(Brush value)
        {
            if (_shadow_Background != value)
            {
                panel.Background = _shadow_Background = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
