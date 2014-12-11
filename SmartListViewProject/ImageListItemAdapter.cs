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
using Android.OS;

namespace SmartListViewProject
{   
    public class ImageListItemAdapter : MainListItemAdapter<int>
    {
        private readonly int width;
        private readonly BitmapFactory.Options opt = new BitmapFactory.Options { InPurgeable = true, InPreferQualityOverSpeed = false, InJustDecodeBounds = false, InSampleSize = 20 };
        private readonly Stopwatch sw  = new Stopwatch();

        public ImageListItemAdapter(Context context, IList<int> items) : base(context, items)
        {
            var wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var sz = new Android.Graphics.Point();
            wm.DefaultDisplay.GetSize(sz);
            width = (int)(sz.X * 0.9);
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

            if (nvh.Task != null && !nvh.Task.IsCancelled)
            {
                nvh.Task.Cancel(true);
                nvh.Task = null;
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
           
            using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, opt))
            {
                nvh.Image.SetImageBitmap(img);
            }
            Picasso.With(Context).Load(item).NoPlaceholder().Into(nvh.Image);
       
            //var task = new ActDecodeTask(nvh.Image, item);
            //task.Execute();
            //nvh.Task = task;

            Console.WriteLine(string.Format("Position: {0}, CreatingTime: {1}", position, sw.ElapsedMilliseconds));
            sw.Stop();
            return convertView;
        }

        public class NViewHolder : Java.Lang.Object
        {
            public ImageView Image;
            public AsyncTask Task;
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
                var op = new BitmapFactory.Options { InJustDecodeBounds = true };
                using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, op))
                {
                    var size = new Size(width, (op.OutHeight * width) / op.OutWidth);
                    TotalHeight += size.Height;
                    heights.Add(size);
                } 
            }
            Console.WriteLine(string.Format("Total height: {0} - {1}", sw.ElapsedMilliseconds, TotalHeight));
            sw.Stop();
        }
    }

    public class ActDecodeTask: AsyncTask
    {
        private readonly WeakReference<ImageView> imageViewReference;
        private readonly int item;
        private readonly BitmapFactory.Options opt = new BitmapFactory.Options { InPurgeable = true, InPreferQualityOverSpeed = false, InJustDecodeBounds = false, InSampleSize = 20 };
    
        public ActDecodeTask(ImageView imageView, int item)
        {
            this.item = item;
            this.imageViewReference = new WeakReference<ImageView>(imageView);
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            if (IsCancelled)                
                return null;

            if (!IsCancelled)
            {
                return BitmapFactory.DecodeResource(Application.Context.Resources, item, opt);
            }
            else
            {
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@");
                return null;
            }
        }           

        protected override void OnPostExecute(Java.Lang.Object bitmap)
        {
            if (IsCancelled)
                return;

            ImageView imageView;
            imageViewReference.TryGetTarget(out imageView);
            if (imageView != null)
                imageView.SetImageBitmap(bitmap as Bitmap);
        }
    }
}
    /*var task = Task.Factory.StartNew(() =>
            {    
                //http://www.javaspecialist.ru/2014/04/android-tricks.html
                var op = new BitmapFactory.Options();
                op.InPreferredConfig = Bitmap.Config.Rgb565;
                op.InMutable = true;
                op.InPurgeable = true;
                op.InInputShareable = true;
                return BitmapFactory.DecodeResource(Application.Context.Resources, item, op);
            }).ContinueWith((res) =>
            {
                nvh.Image.SetImageBitmap(res.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());*/
    /*opt.InJustDecodeBounds = false;
    opt.InSampleSize = 10;
    using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, opt))
    {
        nvh.Image.SetImageBitmap(img);
    }*/
/*var task = Task.Factory.StartNew(() =>
            {    
                //http://www.javaspecialist.ru/2014/04/android-tricks.html
                var op = new BitmapFactory.Options();
                op.InPreferredConfig = Bitmap.Config.Rgb565;
                op.InMutable = true;
                op.InPurgeable = true;
                op.InInputShareable = true;
                return BitmapFactory.DecodeResource(Application.Context.Resources, item, op);
            }).ContinueWith((res) =>
            {
                nvh.Image.SetImageBitmap(res.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());*/
/*opt.InJustDecodeBounds = false;
opt.InSampleSize = 10;
using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, opt))
{
    nvh.Image.SetImageBitmap(img);
}*/

/*public class ActDecodeTask: AsyncTask
{
    private readonly int MaxTextureSize = Math.Min(2048, MyGlobalSettings.MaxBitmapSize); 
    private readonly WeakReference<ImageView> imageViewReference;
    private readonly string path;
    private int width;
    private int height;
    private Bitmap bitmap;

    private bool reuse = false;
    public ActDecodeTask(ImageView imageView, string imagePath, int width, int height)
    {
        this.imageViewReference = new WeakReference<ImageView>(imageView);
        this.path = imagePath;
        this.width = width;
        this.height = height;

        var dr = (imageView.Drawable as BitmapDrawable);
        if (dr != null) 
        {
            bitmap = dr.Bitmap;
            if (bitmap != null && bitmap.Width == width && bitmap.Height == height)
            {
                reuse = true;  
            }
            imageView.SetImageBitmap(null);
        }
    }
    protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
    {
        if (IsCancelled)                
            return null;

        int maxSize = Math.Min(2048, MyGlobalSettings.MaxBitmapSize);
        var max = Math.Max(width, height);
        if (max > maxSize)
        {
            var df = (float)maxSize / max;
            width = (int)(width * df);
            height = (int)(height * df);
        }

        var opt = new BitmapFactory.Options();
        opt.InMutable = true;
        {
            if (bitmap != null)
                bitmap.Recycle();
            opt.OutHeight = height;
            opt.OutWidth = width;
            Console.WriteLine("================ " + this.GetHashCode()+" 3");
            var a =  BitmapFactory.DecodeFile(path, opt);
            Console.WriteLine("================ " + this.GetHashCode()+" 4");
            return a;
        }               
    }           

    protected override void OnPostExecute(Java.Lang.Object bitmap)
    {
        Console.WriteLine("================ " + this.GetHashCode()+" 5");
        if (!IsCancelled && imageViewReference != null && bitmap != null && bitmap is Bitmap)
        {
            ImageView imageView;
            imageViewReference.TryGetTarget(out imageView);
            Console.WriteLine("================ " + this.GetHashCode()+" 6");
            if (imageView != null)
                imageView.SetImageBitmap(bitmap as Bitmap);
            Console.WriteLine("================ " + this.GetHashCode()+" 7");
        }
        else
        {
            if(bitmap != null)
            {
                (bitmap as Bitmap).Recycle();
            }
        }
    }

}*/
