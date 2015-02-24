using System;
using Android.Widget;
using Android.Util;
using Android.Views;
using Android.Content;

namespace SmartListViewLibrary
{
    /// <summary>
    /// Special ListView with support of zoom, inertia and centering
    /// </summary>
    public partial class SmartListView : AdapterView<IAdapter>, ScaleGestureDetector.IOnScaleGestureListener, GestureDetector.IOnGestureListener, GestureDetector.IOnDoubleTapListener
    {
        public SmartListView(Context context) : this(context, null)
        {
        }

        public SmartListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mScaleGestureDetector = new ScaleGestureDetector(context, this);
            mGestureDetector = new GestureDetector(this);
            mScroller = new Scroller(context);
        }

        public void ScrollList(int distanceY, int distanceX)
        {
            mListTop = mListTopStart + distanceY;
            if (!rails)
                mListLeft = mListLeftStart + distanceX;
            RequestLayout();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {      
            //without this, the function will not be restructuring sizes already displayed views
            // Here extremely efficient
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
                RemoveNonVisibleViews(offset);
                FillList(offset);
            }
            PositionItems();
            Invalidate();
            if (SeekBarChangeEvent != null)
                SeekBarChangeEvent(this, new ScrollEventArgs(){ ScaleFactor = mScale, ScrolledFactor = mListTop });
        }

        private void FillList(int offset)
        {
            int bottomEdge = GetChildAt(ChildCount - 1).Bottom;
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
                Console.WriteLine("SmartListView. mLastItemPosition++ " + mLastItemPosition);
                View newBottomchild = mAdapter.GetView(mLastItemPosition, CachedView, this);
                AddAndMeasureChild(newBottomchild, LAYOUT_MODE_BELOW);
                bottomEdge += newBottomchild.MeasuredHeight;
            }
        }

        private void FillListUp(int topEdge, int offset)
        {
            while (topEdge + offset > 0 && mFirstItemPosition > 0)
            {
                mFirstItemPosition--;
                Console.WriteLine("SmartListView. mFirstItemPosition-- " + mFirstItemPosition);
                View newTopCild = mAdapter.GetView(mFirstItemPosition, CachedView, this);
                AddAndMeasureChild(newTopCild, LAYOUT_MODE_ABOVE);
                int childHeight = newTopCild.MeasuredHeight;
                topEdge -= childHeight;
                // update the list offset (since we added a view at the top)
                mListTopOffset -= childHeight;
                if (mListTopOffset < 0)
                {
                    //bug fix output abroad. Костыль правки бага захода за границу
                    mListTop += mListTopOffset;
                    mListTopOffset = 0;
                }
            }
        }

        private void RemoveNonVisibleViews(int offset)
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
                    Console.WriteLine("SmartListView. mFirstItemPosition++ " + mFirstItemPosition);
                    // update the list offset (since we've removed the top child)
                    mListTopOffset += firstChild.MeasuredHeight;
                    // Continue to check the next child only if we have more than one child left
                    firstChild = childCount > 1 ? GetChildAt(0) : null;
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
                    Console.WriteLine("SmartListView. mLastItemPosition-- " + mLastItemPosition);
                    // Continue to check the next child only if we have more than one child left
                    lastChild = childCount > 1 ? GetChildAt(childCount - 1) : null;
                }
            }
        }

        /// <summary>
        /// Locate all views on each positions
        /// </summary>
        private void PositionItems()
        { 
            int top = mListTop + mListTopOffset;
            for (int index = 0; index < ChildCount; index++)
            {
                View child = GetChildAt(index);
                int width = child.MeasuredWidth;
                int height = child.MeasuredHeight;

                if (!flinging && !mScaling && mTouchState == TOUCH_FIRST_STATE)
                {
                    //Initial alignment of the sheet
                    var dx = (Width - width) / 2;
                    if (dx > 0)
                    {
                        mListLeft = dx;
                    }
                }
                var left = mListLeft;
                child.Layout(left, top, left + width, top + height);
                top += height;
            }
        }

        private void AddAndMeasureChild(View child, int layoutMode)
        {
            int index = layoutMode == LAYOUT_MODE_ABOVE ? 0 : -1;
            AddViewInLayout(child, index, child.LayoutParameters, true);
            MeasureView(child);
        }

        private void MeasureView(View v)
        {
            if (v.LayoutParameters != null)
            {
                int childWidthSpec = MeasureSpec.MakeMeasureSpec(v.LayoutParameters.Width, MeasureSpecMode.Exactly);//(v.Width, MeasureSpecMode.Exactly);
                int childHeightSpec = MeasureSpec.MakeMeasureSpec(v.LayoutParameters.Height, MeasureSpecMode.Exactly);//(v.Height, MeasureSpecMode.Unspecified);
                v.Measure(childWidthSpec, childHeightSpec);
            }
            else
            {
                v.Measure(0, 0);
            }
            v.Measure(MeasureSpec.MakeMeasureSpec((int)(v.MeasuredWidth * mScale), MeasureSpecMode.Exactly), MeasureSpec.MakeMeasureSpec((int)(v.MeasuredHeight * mScale), MeasureSpecMode.Exactly));
        }

        private View CachedView
        {
            get
            {
                return mCachedItemViews.Size() != 0 ? (View)mCachedItemViews.RemoveFirst() : null;
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
            throw new NotSupportedException("Not supported");
        }

        public override View SelectedView
        {
            get
            {
                throw new NotSupportedException("Not supported");
            }
        }

        /// <summary>
        /// The gesture detector.
        /// </summary>
        private GestureDetector mGestureDetector;
        /// <summary>
        /// Listener of the scale gestures
        /// </summary>
        private ScaleGestureDetector mScaleGestureDetector;
        /// <summary>
        /// Is used to inertia scrolling and centering after manipulation
        /// </summary>
        private Scroller mScroller;
        /// <summary>
        /// Is used to update the SeekBar status
        /// </summary>
        public event EventHandler<ScrollEventArgs> SeekBarChangeEvent;
        /// <summary>
        /// Adapter of output data
        /// </summary>
        private IAdapter mAdapter;
        /// <summary>
        /// The adaptor position of the first visible item
        /// </summary>
        private int mFirstItemPosition;
        /// <summary>
        /// The adaptor position of the last visible item 
        /// Индекс самого нижнего отображаемого элемента на экране
        /// </summary>
        public int mLastItemPosition;
        /// <summary>
        /// The current top of the first item 
        /// Вертикальное смешение по оси y самой первой вьюшки
        /// </summary>
        public int mListTop;
        /// <summary>
        /// The current left of the first item 
        /// </summary>
        public int mListLeft;
        /// <summary>
        /// A list of cached (re-usable) item views
        /// Кэш вьюшек. ощущение, что он не работает / 
        /// </summary>
        private Java.Util.LinkedList mCachedItemViews = new Java.Util.LinkedList();
        /// <summary>
        /// Children added with this layout mode will be added below the last child
        /// </summary>
        private const int LAYOUT_MODE_BELOW = 0;
        /// <summary>
        /// Children added with this layout mode will be added above the first child
        /// </summary>
        private const int LAYOUT_MODE_ABOVE = 1;
        /// <summary>
        /// The offset from the top of the currently first visible item to the top of the first item
        /// расстояние между верхней точкой первой видимой вьюшки и топом самой первой вьюшки во всей структуре
        /// </summary>
        private int mListTopOffset;
        /// <summary>
        /// The offset from the left of the currently first visible item to the top of the first item
        /// </summary>
        private int mListLeftOffset;
        /// <summary>
        /// The top of the first item when the touch down event was received
        /// Насколько в момент касания первая видимая вьюшка была сдвинута вверх за границу экрана
        /// </summary>
        private int mListTopStart;
        /// <summary>
        /// The left of the first item when the touch down event was received
        /// </summary>
        private int mListLeftStart;
    }
}

