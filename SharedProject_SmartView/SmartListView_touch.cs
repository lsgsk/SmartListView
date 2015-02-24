using System;
using Android.Views;

namespace SmartListViewLibrary
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
                    StartTouch(e);
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
                        ClickChildAt((int)e.GetX(), (int)e.GetY());
                    }
                    EndTouch();
                    SenterViewOntoScreen();
                    break; 
                case MotionEventActions.Move:
                    if (mTouchState == TOUCH_STATE_CLICK)
                    {
                        StartScrollIfNeeded(e);
                    }
                    if (mTouchState == TOUCH_STATE_SCROLL)
                    {
                        ScrollList((int)e.GetY() - mTouchStartY, (int)e.GetX() - mTouchStartX);
                    }
                    break;  
                default:
                    EndTouch();
                    break;
            }
            return true;
        }

        private bool StartScrollIfNeeded(MotionEvent e)
        {
            int xPos = (int)e.GetX();
            int yPos = (int)e.GetY();

            var direction = DirectionOfTravel(xPos - mTouchStartX, yPos - mTouchStartY);
            if (direction == Moving.Up || direction == Moving.Down)
                rails = true;

            if (xPos < mTouchStartX - TOUCH_SCROLL_THRESHOLD || xPos > mTouchStartX + TOUCH_SCROLL_THRESHOLD || yPos < mTouchStartY - TOUCH_SCROLL_THRESHOLD || yPos > mTouchStartY + TOUCH_SCROLL_THRESHOLD)
            {
                mTouchState = TOUCH_STATE_SCROLL;
                return true;
            }
            return false;
        }


        private static Moving DirectionOfTravel(float vx, float vy)
        {
            if (Math.Abs(vy) > 1.5f * Math.Abs(vx))
                return (vy > 0) ? Moving.Down : Moving.Up;
            if (Math.Abs(vx) > 1.5f * Math.Abs(vy))
                return (vx > 0) ? Moving.Right : Moving.Left;
            return Moving.Diagonally;
        }

        private void StartTouch(MotionEvent e)
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

        private void EndTouch()
        {
            // reset touch state
            mTouchState = TOUCH_STATE_RESTING;
        }
       
        private void ClickChildAt(int x, int y)
        {
            //something event
        }

        /// <summary>
        /// The touch start X position relatively ListView
        /// </summary>
        private int mTouchStartX;
        /// <summary>
        /// The touch start Y position relatively ListView
        /// Точка начала касания относительно listview
        /// </summary>
        private int mTouchStartY;
        /// <summary>
        /// Current touch state
        /// </summary>
        private int mTouchState = TOUCH_FIRST_STATE;
        private static int TOUCH_SCROLL_THRESHOLD = 10;
        private static int TOUCH_STATE_SCROLL = 2;
        private static int TOUCH_FIRST_STATE = -1;
        private static int TOUCH_STATE_RESTING = 0;
        private const int TOUCH_STATE_CLICK = 1;

        private enum Moving
        {
            Diagonally = 0,
            Left = 1,
            Right = 2,
            Up = 3,
            Down = 4
        }
    }
}

