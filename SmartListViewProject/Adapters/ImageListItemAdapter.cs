using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Graphics;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace SmartListViewProject
{   
    public class ImageListItemAdapter : MainListItemAdapter<string>
    {
        public ImageListItemAdapter(Context context, IList<string> items) : base(context, items)
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
                nvh.nImage = convertView.FindViewById<ImageView>(Resource.Id.ActivityImage);    
                convertView.Tag = nvh;
            }
            else
            {
                nvh = convertView.Tag as NViewHolder;
                nvh.nImage.SetImageBitmap(null);
            }
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
            using (var stream = Context.Assets.Open(this[position]))
            {
                using (var img = BitmapFactory.DecodeStream(stream, null, opt))
                {
                    nvh.nImage.SetImageBitmap(img);
                }
            }

            if (nvh.nTaskToken != null)
            {
                nvh.nTaskToken.Cancel();
                nvh.nTask = null;
                nvh.nTaskToken = null;
            }
            nvh.nTaskToken = new CancellationTokenSource();
            nvh.nTask =Task<Bitmap>.Factory.StartNew(token =>
                {
                    var tkn = nvh.nTaskToken;
                     if (tkn.IsCancellationRequested)
                        return null;

                    Bitmap bm = null;
                    using (var stream =  Context.Assets.Open(this[position]))
                    {
                        bm = BitmapFactory.DecodeStream(stream, null, opts);
                    }
                    return (tkn.IsCancellationRequested) ? null : bm;
                }, TaskContinuationOptions.NotOnCanceled, nvh.nTaskToken.Token);
            nvh.nTask.ContinueWith((bm) =>
                { 
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!" + bm.Result.ToString());
                    nvh.nImage.SetImageBitmap(bm.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());


            //Picasso.With(Context).Load(item).NoPlaceholder().Into(nvh.Image);
            Console.WriteLine(string.Format("Position: {0}, CreatingTime: {1}", position, sw.ElapsedMilliseconds));
            sw.Stop();
            return convertView;
        }

        private class NViewHolder : Java.Lang.Object
        {
            public ImageView nImage;
            public Task<Bitmap> nTask;
            public CancellationTokenSource nTaskToken;
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
                using (var stream = Context.Assets.Open(item))
                {
                    using (var img = BitmapFactory.DecodeStream(stream, null, op))
                    {
                        var size = new Size(Width, (op.OutHeight * Width) / op.OutWidth);
                        TotalHeight += size.Height;
                        heights.Add(size);
                    }
                }
            }
            Console.WriteLine(string.Format("Total height: {0} - {1}", sw.ElapsedMilliseconds, TotalHeight));
            sw.Stop();
        }
        private readonly int Width;
        private readonly BitmapFactory.Options opt = new BitmapFactory.Options { InPurgeable = true, InPreferQualityOverSpeed = false, InJustDecodeBounds = false, InSampleSize = 20 };
        private readonly BitmapFactory.Options opts = new BitmapFactory.Options() { InPurgeable = true, InInputShareable = true };
        private readonly Stopwatch sw  = new Stopwatch();
    }  
}

