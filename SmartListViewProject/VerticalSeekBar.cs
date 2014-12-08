using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Runtime;
using System.Runtime.CompilerServices;
using Android.Graphics;
using Android.Views;

namespace SmartListViewProject
{
    public class VerticalSeekBar : SeekBar 
    {
        public VerticalSeekBar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }
        public VerticalSeekBar(IntPtr a, JniHandleOwnership b) : base(a,b)
        {
            //Этот конструктор очень важен
            //http://stackoverflow.com/questions/10593022/monodroid-error-when-calling-constructor-of-custom-view-twodscrollview
        }
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh) 
        {
            //Обратить внимание, что высота и ширина меняются местами 
            base.OnSizeChanged(h, w, oldh, oldw);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(heightMeasureSpec, widthMeasureSpec);
            SetMeasuredDimension(MeasuredHeight, MeasuredWidth);
        }
        protected override void OnDraw(Canvas c) 
        {
            c.Rotate(-90);
            c.Translate(-Height, 0);
            base.OnDraw(c);
        }      

        public override int Progress
        {
            get
            {
                return base.Progress;
            }
            set
            {
                base.Progress = value;
                OnSizeChanged(Width, Height , 0, 0);
            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!Enabled)
            {
                return false;
            }
            switch (e.Action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Move:
                case MotionEventActions.Up:
                    Progress = Max - (int)(Max * e.GetY() / Height);
                    if (OnSeek != null)
                        OnSeek(this, Max - Progress);
                    break;
                case MotionEventActions.Cancel:
                    break;
            }

            return true;
        }
        public event EventHandler<int> OnSeek;
    }
}

