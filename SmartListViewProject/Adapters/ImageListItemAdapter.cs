using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Graphics;
using Com.Squareup.Picasso;
using System.Drawing;
using System.Diagnostics;

namespace SmartListViewProject
{   
    public class ImageListItemAdapter : MainListItemAdapter<int>
    {
        public ImageListItemAdapter(Context context, IList<int> items) : base(context, items)
        {
            var wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var sz = new Android.Graphics.Point();
            wm.DefaultDisplay.GetSize(sz);
            Width = (int)(sz.X * 0.9);
            CulculateTotalHeight();
        }   

        public override View GetView(int position, View convertView, ViewGroup parent)
        {     
            sw.Restart();
            NViewHolder nvh;
            if (convertView == null)
            {
                convertView = Inflater.Inflate(Resource.Layout._myItem_SmartItem, parent, false);
                nvh = new NViewHolder();
                nvh.Image = convertView.FindViewById<ImageView>(Resource.Id.ActivityImage);    
                convertView.Tag = nvh;
            }
            else
            {
                nvh = convertView.Tag as NViewHolder;
                nvh.Image.SetImageBitmap(null);
            }
            var item = this[position];
            var size = GetItemSize(position);
            if (convertView.LayoutParameters == null)
            {
                convertView.LayoutParameters = new ViewGroup.LayoutParams(size.Width, size.Height);
                convertView.SetPadding(5, 5, 5, 5);  
            }
            else
            {
                convertView.LayoutParameters.Width = size.Width;
                convertView.LayoutParameters.Height = size.Height;
            }                   
            //generate thumbnail
            using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, opt))
            {
                nvh.Image.SetImageBitmap(img);
            }
            Picasso.With(Context).Load(item).NoPlaceholder().Into(nvh.Image);
            Console.WriteLine(string.Format("Position: {0}, CreatingTime: {1}", position, sw.ElapsedMilliseconds));
            sw.Stop();
            return convertView;
        }

        private class NViewHolder : Java.Lang.Object
        {
            public ImageView Image;
        }

        private readonly List<Size> heights = new List<Size>();
        public Size GetItemSize(int position)
        {
            return heights[position];
        }

        public int TotalHeight
        {
            get;
            set;
        }

        public void CulculateTotalHeight()
        {
            sw.Restart();
            foreach (var item in Items)
            {
                var op = new BitmapFactory.Options {InJustDecodeBounds = true };
                using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, op))
                {
                    var size = new Size(Width, (op.OutHeight * Width) / op.OutWidth);
                    TotalHeight += size.Height;
                    heights.Add(size);
                } 
            }
            Console.WriteLine(string.Format("Total height: {0} - {1}", sw.ElapsedMilliseconds, TotalHeight));
            sw.Stop();
        }
        /// <summary>
        /// Display width
        /// </summary>
        private readonly int Width;
        private readonly BitmapFactory.Options opt = new BitmapFactory.Options { InPurgeable = true, InPreferQualityOverSpeed = false, InJustDecodeBounds = false, InSampleSize = 20 };
        private readonly Stopwatch sw  = new Stopwatch();
    }  
}

