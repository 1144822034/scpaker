using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SCPAK2
{
    public class fileListAdaper : BaseAdapter
    {

        Context context;
        public List<object> list = new List<object>();

        public fileListAdaper(Context context)
        {
            this.context = context;
        }
        public object getData(int pos) {
            return list[pos];
        }
        public void Add(object data) {
            list .Add(data);
        }
        public void Clear() {
            list.Clear();
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View view, ViewGroup parent)
        {
            var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
            view = inflater.Inflate(Resource.Layout.fileList_item, parent, false);
            DirectoryInfo directory = list[position] as DirectoryInfo;
            FileInfo fileInfo = list[position] as FileInfo;
            if (directory != null)
            {
                view.FindViewById<TextView>(Resource.Id.itemName).Text = directory.Name;
                view.FindViewById<ImageView>(Resource.Id.fileIcon).SetImageResource(Resource.Drawable.folder_regular);
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.itemName).Text = fileInfo.Name;
                view.FindViewById<ImageView>(Resource.Id.fileIcon).SetImageResource(Resource.Drawable.file_regular);
            }
            return view;
        }
        public void iteamTouch(object obj,View.TouchEventArgs args) {
            LinearLayout ll = (LinearLayout)obj;
            ll.SetBackgroundColor(Color.Bisque);
        }
        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return list.Count;
            }
        }

    }
}