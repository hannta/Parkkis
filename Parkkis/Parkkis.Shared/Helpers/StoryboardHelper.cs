using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Parkkis.Helpers
{
    /// <summary>
    /// Helper to start storyboards
    /// </summary>
    public class StoryboardHelper : DependencyObject
    {

        public static readonly DependencyProperty BeginIfTrueProperty =
                                DependencyProperty.RegisterAttached("BeginIfTrue",
                                typeof(bool),
                                typeof(StoryboardHelper),
                                new PropertyMetadata(false, OnBeginIfTrueChanged));


        public static bool GetBeginIfTrue(DependencyObject obj)
        {
            return (bool)obj.GetValue(BeginIfTrueProperty);
        }


        public static void SetBeginIfTrue(DependencyObject obj, bool value)
        {
            obj.SetValue(BeginIfTrueProperty, value);
        }


        private static void OnBeginIfTrueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Storyboard storyboard = d as Storyboard;

            if (storyboard == null)
            {
                throw new InvalidOperationException("StoryboardHelper supports only storyboards");
            }
                
            bool begin = (bool)e.NewValue;

            if (begin)
            {
                storyboard.Begin();
            }

            else
            {
                storyboard.Stop();
            }
        }
    }
}
