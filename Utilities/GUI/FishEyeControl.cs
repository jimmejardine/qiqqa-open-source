//  --------------------------------
//  Copyright (c) Huy Pham. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.opensource.org/licenses/ms-pl.html
//  ---------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Utilities.GUI
{
    public class FishEyeControl : StackPanel
    {
        #region Dependency Property

        // Using a DependencyProperty as the backing store for AnimationMilliseconds. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationMillisecondsProperty = DependencyProperty.Register("AnimationMilliseconds", typeof(int), typeof(FishEyeControl), new UIPropertyMetadata(65));

        #endregion

        #region Private Fields

        private double _elementWidth;

        #endregion

        #region Constructor

        public FishEyeControl()
        {
            Background = Brushes.Transparent;
            HorizontalAlignment = HorizontalAlignment.Center;
            Orientation = Orientation.Horizontal;

            MouseMove += LensPanelMouseMove;
            MouseEnter += LensPanelMouseEnter;
            MouseLeave += LensPanelMouseLeave;
            Loaded += LensPanelLoaded;
        }

        #endregion

        #region Public Properties

        public int AnimationMilliseconds
        {
            get => (int)GetValue(AnimationMillisecondsProperty);
            set => SetValue(AnimationMillisecondsProperty, value);
        }

        #endregion

        #region Public Override

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Children == null || Children.Count == 0)
            {
                return arrangeSize;
            }

            var children = Children;
            var finalRect = new Rect(arrangeSize);
            var width = 0.0;
            var num2 = 0;
            var count = children.Count;

            while (num2 < count)
            {
                var element = children[num2];

                if (element != null)
                {
                    finalRect.X += width;
                    width = element.DesiredSize.Width;
                    finalRect.Width = width;
                    finalRect.Height = Math.Max(arrangeSize.Height, element.DesiredSize.Height);
                    element.SetValue(WidthProperty, element.DesiredSize.Width);

                    if (element.RenderTransform as TransformGroup == null)
                    {
                        var group = new TransformGroup();
                        element.RenderTransform = group;
                        group.Children.Add(new ScaleTransform());
                        group.Children.Add(new TranslateTransform());
                        element.RenderTransformOrigin = new Point(0.5, 1);
                    }

                    element.Arrange(finalRect);
                }

                num2++;
            }

            AnimateAll();

            return arrangeSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var internalChildren = InternalChildren;
            var size = new Size();
            var availableSize = constraint;
            var num5 = 0;
            var count = internalChildren.Count;

            availableSize.Width = double.PositiveInfinity;

            while (num5 < count)
            {
                var element = internalChildren[num5];

                if (element != null)
                {
                    element.Measure(availableSize);
                    var desiredSize = element.DesiredSize;
                    size.Width += Math.Round(desiredSize.Width, 2);
                    size.Height = Math.Max(size.Height, desiredSize.Height);
                }

                num5++;
            }

            return size;
        }

        #endregion

        #region Private Methods

        private void LensPanelLoaded(object sender, RoutedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                if (Children == null || Children.Count == 0)
                    return;

                foreach (UIElement child in Children)
                {
                    _elementWidth += child.DesiredSize.Width;
                }

                _elementWidth /= Children.Count;

                InvalidateArrange();
            });
        }

        private void LensPanelMouseMove(object sender, MouseEventArgs e)
        {
            InvalidateArrange();
        }

        private void LensPanelMouseEnter(object sender, MouseEventArgs e)
        {
            InvalidateArrange();
        }

        private void LensPanelMouseLeave(object sender, MouseEventArgs e)
        {
            InvalidateArrange();
        }

        private void AnimateAll()
        {
            if (Children == null || Children.Count == 0)
                return;

            int? selectedElement = null;

            if (IsMouseOver)
            {
                var widthSoFar = 0.0;
                var x = Mouse.GetPosition(this).X;
                var y = Mouse.GetPosition(this).Y;

                for (var i = 0; i < Children.Count; i++)
                {
                    if (Children[i].IsMouseOver && y < Children[i].DesiredSize.Height)
                    {
                        selectedElement = i;
                        break;
                    }

                    widthSoFar += Children[i].DesiredSize.Width;

                    if (x <= widthSoFar && y < Children[i].DesiredSize.Height)
                    {
                        selectedElement = i;
                        break;
                    }
                }
            }

            for (var i = 0; i < Children.Count; i++)
            {
                if (i == selectedElement - 2)
                {
                    AnimateTo(Children[i], 1.25, (_elementWidth * 1.5), AnimationMilliseconds);
                }
                else if (i == selectedElement - 1)
                {
                    AnimateTo(Children[i], 1.5, (_elementWidth * 2.0), AnimationMilliseconds);
                }
                else if (i == selectedElement)
                {
                    AnimateTo(Children[i], 2, (_elementWidth * 2.5), AnimationMilliseconds);
                }
                else if (i == selectedElement + 1)
                {
                    AnimateTo(Children[i], 1.5, (_elementWidth * 2.0), AnimationMilliseconds);
                }
                else if (i == selectedElement + 2)
                {
                    AnimateTo(Children[i], 1.25, (_elementWidth * 1.5), AnimationMilliseconds);
                }
                else
                {
                    AnimateTo(Children[i], 1, _elementWidth, AnimationMilliseconds);
                }
            }
        }

        private static void AnimateTo(UIElement child, double scale, double width, double duration)
        {
            var group = (TransformGroup)child.RenderTransform;
            var scaleTransform = (ScaleTransform)group.Children[0];

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, MakeAnimation(scale, duration));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, MakeAnimation(scale, duration));
            child.BeginAnimation(WidthProperty, MakeAnimation(width, duration));
        }

        private static DoubleAnimation MakeAnimation(double to, double duration)
        {
            var anim = new DoubleAnimation(to, TimeSpan.FromMilliseconds(duration))
            {
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.1
            };

            return anim;
        }

        #endregion
    }
}
