using System;
using Android.Views;
using Android.App;

namespace SmartListViewLibrary
{
    public partial class SmartListView
    {  
        public bool OnScale(ScaleGestureDetector detector)
        {
            float previousScale = mScale;
            mScale = Math.Min(Math.Max(mScale * detector.ScaleFactor, MinScale), MaxScale);
            {
                var factor = mScale / previousScale;
                var v = GetChildAt(0);
                if (v != null)
                {
                    mListTopOffset = (int)(mListTopOffset * factor);
                    mListTop = (int)(mListTop * factor);
                    mListLeftOffset = (int)(mListLeftOffset * factor);
                    mListLeft = (int)(mListLeft * factor);
                    var viewFocusX = (int)(detector.FocusX * factor - detector.FocusX);
                    var viewFocusY = (int)(detector.FocusY * factor - detector.FocusY);
                    mListTop -= viewFocusY;
                    mListLeft -= viewFocusX;
                    RequestLayout();
                }
            }
            return true;
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            mScaling = true;
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            mScaling = false;
            //RequestLayout();
        }  

        /// <summary>
        /// Gets the initial scale factor.
        /// </summary>
        public static float InitialScaleFactor
        {
            get
            {
                return (Application.Context.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape) ? 1f : 0.8f;
            }
        }
        /// <summary>
        /// Current scale factor
        /// </summary>
        public float mScale = InitialScaleFactor;
        /// <summary>
        /// The minimal scale factor
        /// </summary>
        private const float MinScale = 0.5f;
        /// <summary>
        /// The maximum scale factor
        /// </summary>
        private const  float MaxScale = 5.0f;
        /// <summary>
        /// The list is in scrolling
        /// </summary>
        private bool mScaling; 
    }
}

