using System;
using Android.Widget;
using Android.Util;
using Android.Views;
using Android.Content;
using Android.Graphics;
using Android.App;

namespace SmartListView
{
    public partial class SmartListView
    {     
        protected override void DispatchDraw(Android.Graphics.Canvas canvas)
        {
            base.DispatchDraw(canvas);
        }

        protected override bool DrawChild(Canvas canvas, View child, long drawingTime)
        {  
            return base.DrawChild(canvas, child, drawingTime);
        }

        public float mScale = (Application.Context.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape) ? 1f : 0.8f;
        private const  float REFLOW_SCALE_FACTOR = 0.5f;
        private const  float MIN_SCALE = 0.5f;
        private const  float MAX_SCALE = 5.0f;

        public bool OnScale(ScaleGestureDetector detector)
        {
            float previousScale = mScale;
            mScale = System.Math.Min(System.Math.Max(mScale * detector.ScaleFactor, MIN_SCALE), MAX_SCALE);
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

        bool mScaling;
        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            mScaling = true;
            // Ignore any scroll amounts yet to be accounted for: the
            // screen is not showing the effect of them, so they can
            // only confuse the user
            // Avoid jump at end of scaling by disabling scrolling
            // until the next start of gesture
            //mScrollDisabled = true;
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            mScaling = false;
            //RequestLayout();
        }              
    }
}

