using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Graphics;
using Com.Squareup.Picasso;

namespace SmartListViewProject
{
    public class ImageListItemAdapter : MainListItemAdapter<int>
    {
        private readonly int width;
        public ImageListItemAdapter(Context context, IList<int> items) : base(context, items)
        {
            var wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var sz = new Point();
            wm.DefaultDisplay.GetSize(sz);
            width = (int)(sz.X * 0.9);
        }      
        public override View GetView(int position, View convertView, ViewGroup parent)
        {      
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();   
            NViewHolder nvh;
            if (convertView == null)
            {
                convertView = Inflater.Inflate(Resource.Layout._myItem_SmartItem, parent, false);
                nvh = new NViewHolder();
                nvh.Image = convertView.FindViewById<ImageView>(Resource.Id.ActivityImage);    
                convertView.Tag = nvh;
            } else
            {
                nvh = convertView.Tag as NViewHolder;
            }
            var item = this[position];
            var opt = new BitmapFactory.Options { InPurgeable = true, InJustDecodeBounds = true, InPreferQualityOverSpeed = false};
            using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, opt))
            {
                convertView.LayoutParameters = new ViewGroup.LayoutParams(width,(opt.OutHeight*width)/opt.OutWidth);
                convertView.SetPadding(5,5,5,5);
            }  
            /*opt.InJustDecodeBounds = false;
            opt.InSampleSize = 10;
            using (var img = BitmapFactory.DecodeResource(Application.Context.Resources, item, opt))
            {
                nvh.Image.SetImageBitmap(img);
            }*/
            Picasso.With(Application.Context).Load(item).Into(nvh.Image);
            Console.WriteLine(string.Format("Position: {0}, CreatingTime: {1}", position, sw.ElapsedMilliseconds));
            sw.Stop();
            return convertView;
        }      

        public class NViewHolder : Java.Lang.Object
        {
            public ImageView Image;
        }
    } 

    public abstract class MainListItemAdapter<T>: BaseAdapter
    {
        protected List<T> Items;
        protected float Density;
        protected LayoutInflater Inflater;

        protected MainListItemAdapter(Context context, IList<T> items = null)
        {           
            Density  = Application.Context.Resources.DisplayMetrics.Density;
            Inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            Items = new List<T>(items);
        }

        public T this [int position]
        {
            get 
            { 
                return Items[position];
            }
        }

        /// <exception cref="T:System.NotImplementedException"></exception>
        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException("Not use GetItem");
        }

        public virtual void ChangeItems(IList<T> items)
        {
            if(items == null)
                Items = new List<T>(items);
            Items.Clear();
            Items.AddRange(items);
            NotifyDataSetChanged();
        } 

        public virtual void AddItems(IList<T> items)
        {
            Items.AddRange(items);
            NotifyDataSetChanged();
        } 
        public virtual void SimpleInvalidate()
        {
            NotifyDataSetChanged();
        } 

        public override int Count
        {
            get { return (Items != null) ? Items.Count : 0; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public int ConvertDpToPixels(float dpValue)
        {
            return (int) ((dpValue)*Density);
        }
    }

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
}
