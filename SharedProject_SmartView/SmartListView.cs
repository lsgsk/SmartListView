using System;
using Android.Widget;
using Android.Util;
using Android.Views;
using Android.Content;
using System.Diagnostics;

namespace SmartListViewLibrary
{
    public struct ScrollEventArgs
    {
        public int ScrolledFactor;
        public float ScaleFactor;
    }

    public partial class SmartListView : AdapterView<IAdapter>, ScaleGestureDetector.IOnScaleGestureListener, GestureDetector.IOnGestureListener, GestureDetector.IOnDoubleTapListener
    {
        private GestureDetector mGestureDetector;
        private ScaleGestureDetector mScaleGestureDetector;
        private Scroller mScroller;
        public event EventHandler<ScrollEventArgs> ScrollEvent;

        public SmartListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mScaleGestureDetector = new ScaleGestureDetector(context, this);
            mGestureDetector = new GestureDetector(this);
            mScroller = new Scroller(context);
        }

        public void ScrollList(int distanceY, int distanceX)
        {
            mListTop = mListTopStart + distanceY;
            if(!rails)
                mListLeft = mListLeftStart + distanceX;
            RequestLayout();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {      
            //без этой функции не будет перестройка размеров уже отображаемых вьюшек
            //Вот тут крайне не рационально
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int n = ChildCount;
            for (int i = 0; i < n; i++)
                MeasureView(GetChildAt(i));
        }
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (mAdapter == null)
            {
                return;
            }
            if (ChildCount == 0)
            {
                mLastItemPosition = -1;
                FillListDown(mListTop, 0);
            }
            else
            {
                int offset = mListTop + mListTopOffset - GetChildAt(0).Top; //насколько двинули, можно понять не скрыылась ли панель
                //Console.WriteLine(mListTop + " " + mListTopOffset + " " + GetChildAt(0).Top);
                removeNonVisibleViews(offset);
                FillList(offset);
            }
            PositionItems();
            Invalidate();
            if (ScrollEvent != null)
                ScrollEvent(this, new ScrollEventArgs(){ ScaleFactor = mScale, ScrolledFactor = mListTop });
        }

        private void FillList(int offset)
        {
            int bottomEdge = GetChildAt(ChildCount - 1).Bottom;
            //Console.WriteLine(bottomEdge);
            FillListDown(bottomEdge, offset);

            int topEdge = GetChildAt(0).Top;
            FillListUp(topEdge, offset);
        }

        private void FillListDown(int bottomEdge, int offset)
        {
            //Measure вьюшку и просто вставляем ее в структуру, расположение выстраивается в PositionItems
            while (bottomEdge + offset < Height && mLastItemPosition < mAdapter.Count - 1)
            {
                mLastItemPosition++;
                Console.WriteLine("mLastItemPosition++ " + mLastItemPosition);
                View newBottomchild = mAdapter.GetView(mLastItemPosition, CachedView, this);
                AddAndMeasureChild(newBottomchild, LAYOUT_MODE_BELOW);
                //bottomEdge += (mAdapter as SmartListViewProject.ImageListItemAdapter).GetItemSize(mLastItemPosition).Height;// (newBottomchild.Height == 0) ? newBottomchild.MeasuredHeight : newBottomchild.Height;
                bottomEdge += newBottomchild.MeasuredHeight;
            }
        }

        private void FillListUp(int topEdge, int offset)
        {
            while (topEdge + offset > 0 && mFirstItemPosition > 0)
            {
                mFirstItemPosition--;
                Console.WriteLine("mFirstItemPosition-- " + mFirstItemPosition);
                View newTopCild = mAdapter.GetView(mFirstItemPosition, CachedView, this);
                AddAndMeasureChild(newTopCild, LAYOUT_MODE_ABOVE);
                //int childHeight = (mAdapter as SmartListViewProject.ImageListItemAdapter).GetItemSize(mLastItemPosition).Height; //(newTopCild.Height == 0) ? newTopCild.MeasuredHeight: newTopCild.Height;
                int childHeight = newTopCild.MeasuredHeight;
                topEdge -= childHeight;
                // update the list offset (since we added a view at the top)
                mListTopOffset -= childHeight;
                if (mListTopOffset < 0) 
                {
                    //Постыль правки бага захода за границу
                    //mListTop += mListLeftOffset;
                    //mListLeftOffset = 0;
                    mListTop += mListTopOffset;
                    mListTopOffset = 0;
                }
            }
        }

        private void removeNonVisibleViews(int offset)
        {
            // We need to keep close track of the child count in this function. We
            // should never remove all the views, because if we do, we loose track
            // of were we are.
            int childCount = ChildCount;

            // if we are not at the bottom of the list and have more than one child
            if (mLastItemPosition != mAdapter.Count - 1 && childCount > 1)
            {
                // check if we should remove any views in the top
                View firstChild = GetChildAt(0);
                while (firstChild != null && firstChild.Bottom + offset < 0)
                {
                    // remove the top view
                    RemoveViewInLayout(firstChild);
                    childCount--;
                    mCachedItemViews.AddLast(firstChild);
                    mFirstItemPosition++;
                    Console.WriteLine("mFirstItemPosition++ " + mFirstItemPosition);

                    // update the list offset (since we've removed the top child)
                    mListTopOffset += firstChild.MeasuredHeight;

                    // Continue to check the next child only if we have more than
                    // one child left
                    if (childCount > 1)
                    {
                        firstChild = GetChildAt(0);
                    }
                    else
                    {
                        firstChild = null;
                    }
                }
            }

            // if we are not at the top of the list and have more than one child
            if (mFirstItemPosition != 0 && childCount > 1)
            {
                // check if we should remove any views in the bottom
                View lastChild = GetChildAt(childCount - 1);
                while (lastChild != null && lastChild.Top + offset > Height)
                {
                    // remove the bottom view
                    RemoveViewInLayout(lastChild);
                    childCount--;
                    mCachedItemViews.AddLast(lastChild);
                    mLastItemPosition--;
                    Console.WriteLine("mLastItemPosition-- " + mLastItemPosition);

                    // Continue to check the next child only if we have more than one child left
                    if (childCount > 1)
                    {
                        lastChild = GetChildAt(childCount - 1);
                    }
                    else
                    {
                        lastChild = null;
                    }
                }
            }
        }
        //здесь отстраиваем, где находится каждая вьюшка
        private void PositionItems()
        { 
            int top = mListTop + mListTopOffset;
            for (int index = 0; index < ChildCount; index++)
            {
                View child = GetChildAt(index);
                int width = child.MeasuredWidth;
                int height = child.MeasuredHeight;

                if (!flinging && !mScaling && mTouchState == TOUCH_FIRST_STATE )
                {
                    //Первичное выравнивание листа
                    var dx = (Width - width) / 2;
                    if (dx > 0)
                    {
                        mListLeft = dx;
                    }
                    /*else
                    {
                        if(Proportion != -1)
                            mListLeft = (int)(Proportion * child.MeasuredWidth);
                    }*/
                }
                var left = mListLeft;
                child.Layout(left, top, left + width, top + height);
                top += height;
            }
        }


        private void AddAndMeasureChild(View child, int layoutMode)
        {
            //LayoutParams layparams = child.LayoutParameters ?? new LayoutParams(LayoutParams.FillParent, LayoutParams.WrapContent);
            int index = layoutMode == LAYOUT_MODE_ABOVE ? 0 : -1;
            AddViewInLayout(child, index, child.LayoutParameters, true);
            MeasureView(child);
        }

        private readonly Stopwatch sw  = new Stopwatch();
        private void MeasureView(View v) 
        {
            //sw.Restart();
            /*
            int childWidthSpec = MeasureSpec.MakeMeasureSpec(Width, MeasureSpecMode.Exactly);//(v.Width, MeasureSpecMode.Exactly);
            int childHeightSpec = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);//(v.Height, MeasureSpecMode.Unspecified);
            v.Measure(childWidthSpec, childHeightSpec);

            // Work out a scale that will fit it to this view
            //float scale = System.Math.Min((float)Width / (float)v.MeasuredWidth, (float)Height / (float)v.MeasuredHeight);
            // Use the fitting values scaled by our current scale factor
            v.Measure(MeasureSpec.MakeMeasureSpec((int)(v.MeasuredWidth * mScale), MeasureSpecMode.Exactly), MeasureSpec.MakeMeasureSpec((int)(v.MeasuredHeight * mScale), MeasureSpecMode.Exactly));
            */
            if (v.LayoutParameters != null)
            {
                int childWidthSpec = MeasureSpec.MakeMeasureSpec(v.LayoutParameters.Width, MeasureSpecMode.Exactly);//(v.Width, MeasureSpecMode.Exactly);
                int childHeightSpec = MeasureSpec.MakeMeasureSpec(v.LayoutParameters.Height, MeasureSpecMode.Exactly);//(v.Height, MeasureSpecMode.Unspecified);
                v.Measure(childWidthSpec, childHeightSpec);
            } else
            {
                v.Measure(0, 0);
            }
            // Work out a scale that will fit it to this view
            //float scale = System.Math.Min((float)Width / (float)v.MeasuredWidth, (float)Height / (float)v.MeasuredHeight);
            // Use the fitting values scaled by our current scale factor
            v.Measure(MeasureSpec.MakeMeasureSpec((int)(v.MeasuredWidth * mScale), MeasureSpecMode.Exactly), MeasureSpec.MakeMeasureSpec((int)(v.MeasuredHeight * mScale), MeasureSpecMode.Exactly));
            //Console.WriteLine(string.Format("View hash: {0}, View measure time: {1}", v.GetHashCode(), sw.ElapsedMilliseconds));
            //sw.Stop();
        }

        private View CachedView
        {
            get
            {
                if (mCachedItemViews.Size() != 0)
                {
                    return (View)mCachedItemViews.RemoveFirst();
                }
                return null;
            }
        }

        public override IAdapter Adapter
        {
            get
            {
                return mAdapter;
            }
            set
            {
                mAdapter = value;
                mCachedItemViews.Clear();
                RemoveAllViewsInLayout();
                RequestLayout();
            }
        }
       
        public override void SetSelection(int position)
        {
            throw new Java.Lang.UnsupportedOperationException("Not supported");
        }

        public override View SelectedView
        {
            get
            {
                throw new Java.Lang.UnsupportedOperationException("Not supported");
            }
        }

        private IAdapter mAdapter;

        /// <summary>
        /// Индекс самого нижнего отображаемого элемента на экране /The adaptor position of the last visible item 
        /// </summary>
        public int mLastItemPosition;
        /// <summary>
        /// Вертикальное смешение по оси y самой первой вьюшки / The current top of the first item 
        /// </summary>
        public int mListTop;
        public int mListLeft;

        /// <summary>
        /// Кэш вьюшек. ощущение, что он не работает / A list of cached (re-usable) item views
        /// </summary>
        private Java.Util.LinkedList mCachedItemViews = new Java.Util.LinkedList();
        /** Children added with this layout mode will be added below the last child */
        private const int LAYOUT_MODE_BELOW = 0;
        /** Children added with this layout mode will be added above the first child */
        private const int LAYOUT_MODE_ABOVE = 1;

        /// <summary>
        /// расстояние между верхней точкой первой видимой вьюшки и топом самой первой вьюшки во всей структуре//The offset from the top of the currently first visible item to the top of the first item
        /// </summary>
        private int mListTopOffset;
        private int mListLeftOffset;
        /* The adaptor position of the first visible item */
        private int mFirstItemPosition;


        /*The top of the first item when the touch down event was received */
        /// <summary>
        /// насколько в момент касания первая видимая вьюшка была сдвинута вверх за границу экрана
        /// + суммарная высота высота всех вьюшек до нее всерху
        /// </summary>
        private int mListTopStart;
        private int mListLeftStart;

    }
}

