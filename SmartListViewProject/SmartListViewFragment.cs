using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using System;
using SmartListViewLibrary;
using Android.Graphics;

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
            var list = new List<string>();
            for (int i = 0; i < 30; i++)
            {
                list.Add("Images/img" + (i % 15) + ".jpg");
            }
            adapter = new ImageListItemAdapter(Activity, list);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout._Fragment_SmartListView, container, false);
            mListView = view.FindViewById<SmartListView>(Resource.Id.SmartListView);
            mListView.Adapter = adapter;
            var scr = view.FindViewById<VerticalSeekBar>(Resource.Id.mainVerticalSeekBar);
            scr.Max = adapter.TotalHeight;
            mListView.SeekBarChangeEvent += delegate(object sender, ScrollEventArgs e)
            {
                Activity.WindowManager.DefaultDisplay.GetSize(sz);
                var height = sz.Y;
                scr.Max = (int)(adapter.TotalHeight * e.ScaleFactor) - height;
                scr.Progress = scr.Max - (-1 * e.ScrolledFactor);
            };
            scr.OnSeek += delegate(object sender, int e)
            {
                mListView.mListTop = -1 * e;
                mListView.RequestLayout();
            };
            return view;
        }

        /*public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            using (var h = new Handler(Looper.MainLooper))
            {
                h.PostDelayed(mListView.CenterListPosition, 0);
            }            
        }*/
        private ImageListItemAdapter adapter;
        private SmartListView mListView;
        private readonly Point sz = new Point();
    }
}

