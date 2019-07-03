using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Utilities.GUI.DualTabbedLayoutStuff
{
    internal class DualTabbedLayoutItem
    {
        internal enum Location
        {
            Left,
            Right,
            Bottom,
            Floating
        }


        internal string reference_key;
        internal DualTabbedLayout layout;
        internal Location location;
        internal string header;
        internal BitmapSource icon;
        internal FrameworkElement content;
        internal bool can_close;
        internal bool can_floating;
        internal Color? background_color;

        internal DualTabbedLayoutItem(string reference_key, string header, BitmapSource icon, bool can_close, bool can_floating, FrameworkElement content, DualTabbedLayout layout, Color? background_color)
        {
            this.reference_key = reference_key;
            this.layout = layout;
            this.location = Location.Left;

            this.header = header;
            this.icon = icon;
            this.can_close = can_close;
            this.can_floating = can_floating;
            this.content = content;
            this.background_color = background_color;
        }

        internal void WantsLocation(Location location)
        {
            switch (location)
            {
                case Location.Left:
                    WantsLeft();
                    break;
                case Location.Right:
                    WantsRight();
                    break;
                case Location.Bottom:
                    WantsBottom();
                    break;
                case Location.Floating:
                    WantsFloating();
                    break;
                default:
                    throw new Exception("Should never get here");
            }
        }
        
        internal void WantsLeft()
        {
            location = Location.Left;
            layout.WantsLeft(this);
        }

        internal void WantsRight()
        {
            location = Location.Right;
            layout.WantsRight(this);
        }

        internal void WantsBottom()
        {
            location = Location.Bottom;
            layout.WantsBottom(this);
        }

        internal void WantsFloating()
        {
            location = Location.Floating;
            layout.WantsFloating(this);
        }

        internal void WantsClose()
        {
            layout.WantsClose(this); 
        }

        internal void MarkAsRecentlyUsed()
        {
            layout.MarkAsRecentlyUsed(this);
        }

        public FrameworkElement Content
        {
            get
            {
                return content;
            }
        }

        public override string ToString()
        {
            return String.Format("Tab '{0}' at {1}", this.header, this.location);
        }
    }
}
