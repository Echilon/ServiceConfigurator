using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ServiceConfigurator
{
    /// <summary>
    /// A button which can be assigned an image
    /// </summary>
    public class ImageButton : Button
    {
        public static readonly DependencyProperty ImageLocationProperty = DependencyProperty.Register("ImageLocation", typeof(PlacementMode), typeof(ImageButton), new UIPropertyMetadata(PlacementMode.Left));
        /// <summary>
        /// Gets or sets the position of the image in relative to the text.
        /// </summary>
        /// <value>The image.</value>
        public PlacementMode ImageLocation
        {
            get { return (PlacementMode)base.GetValue(ImageLocationProperty); }
            set { base.SetValue(ImageLocationProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton));
        /// <summary>
        /// Gets or sets the image to display.
        /// </summary>
        /// <value>The image.</value>
        public ImageSource Image
        {
            get { return base.GetValue(ImageProperty) as ImageSource; }
            set { base.SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ImageButton));
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return base.GetValue(TextProperty) as string; }
            set { base.SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ShowTextProperty = DependencyProperty.Register("ShowText", typeof(bool), typeof(ImageButton), new UIPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether to show the text.
        /// </summary>
        /// <value><c>true</c> if the text should be shown, otherwise <c>false</c>.</value>
        public bool ShowText
        {
            get { return (bool)base.GetValue(ShowTextProperty); }
            set { base.SetValue(ShowTextProperty, value); }
        }

        public static readonly DependencyProperty AlwaysShowBorderProperty = DependencyProperty.Register("AlwaysShowBorder", typeof(bool), typeof(ImageButton));
        /// <summary>
        /// Gets or sets whether to always show the border.
        /// </summary>
        /// <value><c>true</c> if the border should be shown, otherwise <c>false</c>.</value>
        public bool AlwaysShowBorder
        {
            get { return (bool)base.GetValue(AlwaysShowBorderProperty); }
            set { base.SetValue(AlwaysShowBorderProperty, value); }
        }
    }
}
