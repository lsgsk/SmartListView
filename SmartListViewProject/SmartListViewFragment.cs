using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using System;
using SmartListViewLibrary;

namespace SmartListViewProject
{
    public class SmartListViewFragment : Fragment
    {
        public static SmartListViewFragment NewInstance()
        {
            var frag = new SmartListViewFragment();
            frag.RetainInstance = true;
            return frag;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var a = new []{ Resource.Drawable.img0, Resource.Drawable.img1, Resource.Drawable.img2, Resource.Drawable.img3, Resource.Drawable.img4, Resource.Drawable.img5, Resource.Drawable.img6,Resource.Drawable.img7, Resource.Drawable.img8, Resource.Drawable.img9, Resource.Drawable.img10, Resource.Drawable.img11, Resource.Drawable.img12, Resource.Drawable.img13, Resource.Drawable.img14, Resource.Drawable.img15};
            var rd = new Random();
            var list = new List<int>();
            for (int i = 0; i < 30; i++)
            {
                list.Add(a[rd.Next(a.Length - 1)]);
            }
            adapter = new ImageListItemAdapter(Activity, list);
        }

        private ImageListItemAdapter /*IAdapter*/ adapter;
        private SmartListView mListView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout._Fragment_SmartListView, container, false);
            mListView = view.FindViewById<SmartListView>(Resource.Id.SmartListView);
            mListView.Adapter = adapter;
            var scr = view.FindViewById<VerticalSeekBar>(Resource.Id.mainVerticalSeekBar);
            scr.Max = adapter.TotalHeight;
            mListView.ScrollEvent += delegate(object sender, ScrollEventArgs e)
            {
                scr.Max = (int)(adapter.TotalHeight * e.ScaleFactor) - Activity.WindowManager.DefaultDisplay.Height;
                scr.Progress = scr.Max - (-1 * e.ScrolledFactor);
            };
            scr.OnSeek += delegate(object sender, int e)
            {
                mListView.mListTop = -1 * e;
                mListView.RequestLayout();
            };
            return view;
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            using (var h = new Handler(Looper.MainLooper))
            {
                h.PostDelayed(mListView.CenterListPosition, 0);
            }
            
        }

        /*public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mListTop = mListView.mListTop;
            var view = mListView.GetChildAt(0);
            mListLeftProportion = (view == null || mListView.Width > view.Width) ? -1 : (float)mListView.mListLeft / (float)view.Width;
            mScale = mListView.mScale;
        }*/

        public override void OnDestroy()
        {
            base.OnDestroy();
            adapter.Dispose();
        }
    }
}

