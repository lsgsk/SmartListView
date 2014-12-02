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
        public bool OnDoubleTap(MotionEvent e) 
        {
            mScale = (Application.Context.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape) ? 1f : 0.8f;
            RequestLayout();
            SenterViewOntoScreen();
            return true;
        } 

        public bool OnDoubleTapEvent(MotionEvent e) 
        {
            return true;
        } 

        public bool OnSingleTapConfirmed(MotionEvent e) 
        {
            return true;
        } 
    }
}

