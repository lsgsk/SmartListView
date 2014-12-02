using System;
using Android.Widget;
using Android.Util;
using Android.Views;
using Android.Content;
using Android.Graphics;
using Android.Views.Animations;

namespace SmartListView
{
    public partial class SmartListView 
    {
        public override bool OnTouchEvent(MotionEvent e)
        {
            mScaleGestureDetector.OnTouchEvent(e);
            if (!mScaling)
                mGestureDetector.OnTouchEvent(e);

            if (ChildCount == 0)
            {
                return false;
            }
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:                
                    startTouch(e);
                    break;
                    /*case MotionEventActions.PointerDown:
                    mTouchState = TOUCH_STATE_RESTING;
                    break;
                case MotionEventActions.PointerUp:
                    mTouchState = TOUCH_STATE_CLICK;
                    break;*/
                case MotionEventActions.Up:
                    if (mTouchState == TOUCH_STATE_CLICK)
                    {
                        clickChildAt((int)e.GetX(), (int)e.GetY());
                    }
                    endTouch();
                    SenterViewOntoScreen();
                    break; 
                case MotionEventActions.Move:
                    //if (mScaling)
                    //    break;
                    if (mTouchState == TOUCH_STATE_CLICK)
                    {
                        startScrollIfNeeded(e);
                    }
                    if (mTouchState == TOUCH_STATE_SCROLL)
                    {
                        ScrollList((int)e.GetY() - mTouchStartY, (int)e.GetX() - mTouchStartX);
                    }
                    break;  
                default:
                    endTouch();
                    break;
            }
            return true;
        }
               
      /*public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    startTouch(e);
                    return false;
                case MotionEventActions.Move:
                    return startScrollIfNeeded(e);

                case MotionEventActions.PointerDown:
                case MotionEventActions.PointerUp:
                    return true;

                default:
                    endTouch();
                    return false;
            }
        }*/
        private static int TOUCH_FIRST_STATE = -1;
        private int mTouchState = TOUCH_FIRST_STATE;
        private int mTouchStartX;
        /// <summary>
        /// Точка начала касания относительно listview
        /// </summary>
        private int mTouchStartY;
        private static int TOUCH_SCROLL_THRESHOLD = 10;
        private static int TOUCH_STATE_SCROLL = 2;

        private bool startScrollIfNeeded(MotionEvent e)
        {
            int xPos = (int)e.GetX();
            int yPos = (int)e.GetY();

            var direction = directionOfTravel(xPos - mTouchStartX, yPos - mTouchStartY);
            if (direction == 3 || direction == 4)
                rails = true;

            if (xPos < mTouchStartX - TOUCH_SCROLL_THRESHOLD || xPos > mTouchStartX + TOUCH_SCROLL_THRESHOLD || yPos < mTouchStartY - TOUCH_SCROLL_THRESHOLD || yPos > mTouchStartY + TOUCH_SCROLL_THRESHOLD)
            {
                mTouchState = TOUCH_STATE_SCROLL;
                return true;
            }
            return false;
        }

        private const  int MOVING_DIAGONALLY = 0;
        private const  int MOVING_LEFT = 1;
        private const  int MOVING_RIGHT = 2;
        private const  int MOVING_UP = 3;
        private const  int MOVING_DOWN = 4;
        private static int directionOfTravel(float vx, float vy)
        {
            if (Math.Abs(vy) > 1.5f * Math.Abs(vx))
                return (vy > 0) ? MOVING_DOWN : MOVING_UP;
            if (Math.Abs(vx) > 1.5f * Math.Abs(vy))
                return (vx > 0) ? MOVING_RIGHT : MOVING_LEFT;
            return MOVING_DIAGONALLY;
        }

        private void startTouch(MotionEvent e)
        {
            // save the start place
            mTouchStartX = (int)e.GetX();
            mTouchStartY = (int)e.GetY();
            mListTopStart = GetChildAt(0).Top - mListTopOffset;
            mListLeftStart = GetChildAt(0).Left - mListLeftOffset;

            // we don't know if it's a click or a scroll yet, but until we know
            // assume it's a click
            mTouchState = TOUCH_STATE_CLICK;
        }

        private static int TOUCH_STATE_RESTING = 0;

        private void endTouch()
        {
            // reset touch state
            mTouchState = TOUCH_STATE_RESTING;
        }

        private const int TOUCH_STATE_CLICK = 1;
       
        private void clickChildAt(int x, int y)
        {
        }
    }
}

