using System;
using Android.Widget;
using Android.Util;
using Android.Views;
using Android.Content;
using Android.Graphics;
using Java.Interop;

namespace SmartListView
{
    public partial class SmartListView
    {
        public bool OnDown(MotionEvent e)
        {
            mScroller.ForceFinished(true);
            flinging = false;
            rails = false;
            return true;
        }

        private volatile bool flinging  = false;
        private volatile bool rails = false;
        public virtual bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (SenterViewOntoScreen())
                return true;
            flinging = true;
            mScroller.Fling(mListLeft, mListTop, (int)velocityX, (int)velocityY, int.MinValue, int.MaxValue, int.MinValue, int.MaxValue); 
            Post(FlingRun);
            return true;
        }

        public void FlingRun()
        {
            if (!mScroller.IsFinished)
            {
                mScroller.ComputeScrollOffset();
                int x = mScroller.CurrX;
                int y = mScroller.CurrY;
                if (!(mLastItemPosition == mAdapter.Count - 1 && GetChildAt(ChildCount - 1).Bottom < Height))
                {
                    mListTop = Math.Min(30, y);
                }
                if (GetChildAt(0).Right > Width)
                {
                    mListLeft = Math.Min(0, x);
                }
                if (mListTop == y || mListLeft == x)
                {
                    RequestLayout();
                    Post(FlingRun);
                }
                else
                {
                    flinging = false;
                    SenterViewOntoScreen();
                }
            }
            /*else if (!mUserInteracting)
            {
                // End of an inertial scroll and the user is not interacting.
                // The layout is stable
                View v = mChildViews.Get(mCurrent);
                if (v != null)
                    PostSettle(v);
            }*/
        }

        public void CenterListPosition()
        {
            flinging = false;
            SenterViewOntoScreen();
            flinging = true;
        }

        private bool SenterViewOntoScreen()
        {     
            if (flinging)
                return false;

            var size = new Android.Graphics.Point();
            var display = (Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>()).DefaultDisplay;
            display.GetSize(size);
            var Width = size.X;

            var view = GetChildAt(0);
            int dx = 0, dy = 0;

            var dif = (Width - view.Width) / 2;
            if (dif > 0)
            {
                dx = dif - mListLeft;
            }
            else
            {
                if (mListLeft > 0)
                    dx = -mListLeft;
                if (Width - view.Right > 0)
                {
                    dx = (Width - (mListLeft + view.Width));
                }
            } 
            if (mLastItemPosition == mAdapter.Count - 1)
            {
                dif = GetChildAt(ChildCount - 1).Bottom;
                if (dif < Height)
                {
                    dy = -(dif - Height);
                }
            }
            if (mListTop > 0 || (mListTop + dy) > 0)
            {
                dy = -mListTop;
            }           
            if (dx != 0 || dy != 0)
            {
                mScroller.StartScroll(mListLeft, mListTop, dx, dy, 800);
                Post(PositionRun);
                return true;
            }           
            return false;
        }

        public void PositionRun()
        {
            if (!mScroller.IsFinished)
            {
                mScroller.ComputeScrollOffset();
                int x = mScroller.CurrX;
                int y = mScroller.CurrY;
                mListLeft = x;
                mListTop = y;
                RequestLayout();
                Post(PositionRun);
            }
            /*else if (!mUserInteracting)
            {
                // End of an inertial scroll and the user is not interacting.
                // The layout is stable
                View v = mChildViews.Get(mCurrent);
                if (v != null)
                    PostSettle(v);
            }*/
        }


        public virtual bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return true;
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        public virtual bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
        }
    }
}


/*
                mScroller.ComputeScrollOffset();
                //int x = mScroller.CurrX;
                int y = mScroller.CurrY;
                //mListLeft += x - mScrollerLastX;
                mListTop  += y - mScrollerLastY;
                //mScrollerLastX = x;
                mScrollerLastY = y;
                RequestLayout();
                Post(Run);*/
