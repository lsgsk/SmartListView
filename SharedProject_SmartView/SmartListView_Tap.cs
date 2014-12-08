using System;
using Android.Widget;
using Android.Util;
using Android.Views;
using Android.Content;
using Android.Graphics;
using Android.App;

namespace SmartListViewLibrary
{
    public partial class SmartListView
    {     

        public bool OnDoubleTap(MotionEvent e) 
        {
            mScale = BaseScaleValue;
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

