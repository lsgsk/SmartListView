using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace SmartListViewProject
{   
    public abstract class MainListItemAdapter<T> : MainListItemAdapter
    {
        protected List<T> Items;
        protected MainListItemAdapter(Context context, IList<T> items) : base(context)
        {           
            this.Items = new List<T>(items);
        }

        public void ChangeItems(IList<T> items)
        {
            Items.Clear();
            Items.AddRange(items);
            NotifyDataSetChanged();
        }  

        public void AddItems(IList<T> items)
        {
            Items.AddRange(items);
            NotifyDataSetChanged();
        }

        public void InsertItems(int index, IList<T> items)
        {
            Items.InsertRange(index, items);
            NotifyDataSetChanged();
        }
        public override int Count
        {
            get { return (Items != null) ? Items.Count : 0; }
        }

        public T this [int position]
        {
            get 
            { 
                return Items[position];
            }
        }
    }

    public abstract class MainListItemAdapter: BaseAdapter
    {
        protected Context Context;
        protected LayoutInflater Inflater;  

        protected MainListItemAdapter(Context context)
        {           
            Inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            this.Context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotSupportedException("Not use GetItem");
        }
        public void SimpleInvalidate()
        {
            NotifyDataSetChanged();
        } 
    }
    public static class IListAdapterExtensions
    {
        public static void SimpleInvalidate(this IListAdapter adapter)
        {
            var adp = (adapter as BaseAdapter);
            if (adp != null)
                adp.NotifyDataSetChanged();
        }
    }
}
