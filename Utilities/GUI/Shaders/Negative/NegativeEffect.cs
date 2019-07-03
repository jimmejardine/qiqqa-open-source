using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Utilities.GUI.Shaders.Negative
{
    /// <summary>
    /// Inverts the image colours using a precompiled pixel shader.
    /// See http://joyfulwpf.blogspot.com/2009/03/writing-custom-pixel-shader-effects.html
    /// </summary>
    public class NegativeEffect : ShaderEffect
    {
        private static readonly PixelShader PixelShaderStatic = new PixelShader()
        {
            UriSource = new Uri(
                string.Format("pack://application:,,,/{0};component/GUI/Shaders/Negative/NegativeEffect.ps", typeof(NegativeEffect).Assembly.GetName().Name),
                UriKind.RelativeOrAbsolute)
        };

        public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty("Input", typeof(NegativeEffect), 0);
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public NegativeEffect()
        {
            PixelShader = PixelShaderStatic;
            UpdateShaderValue(InputProperty);
        }
    }
}
