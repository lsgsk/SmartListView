using Android.Views;

namespace SmartListViewLibrary
{
    public partial class SmartListView
    { 
        public bool OnDoubleTap(MotionEvent e) 
        {
            //TODO: this realization is incorrect sometimes
            /*mScale = BaseScaleValue;
            RequestLayout();
            SenterViewOntoScreen();*/
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

