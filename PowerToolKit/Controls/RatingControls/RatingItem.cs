using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JISoft.Controls
{
    internal class RatingItem : Control
    {
        /// <summary>
        /// Initializes a new instance of the RatingItem type.
        /// </summary>
        public RatingItem()
        {
            DefaultStyleKey = typeof(RatingItem);
        }

        #region public double StrokeThickness
        /// <summary>
        /// Gets or sets the value indicating the thickness of a stroke
        /// around the path object.
        /// </summary>
        /// <remarks>
        /// A control element was neccessary to allow for control templating,
        /// however the default implementation uses a path, this property was
        /// created because a good substitute for StrokeThickeness of a path,
        /// which is a double type, does not exist in the default Control class.
        /// </remarks>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RatingItem), null);
        #endregion
    }
}
