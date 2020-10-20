using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SCPAK2
{
    public class blockSpinnerAdapter : BaseAdapter
    {

        Context context;
        public List<string> blocks = new List<string>();

        public blockSpinnerAdapter(Context context)
        {
            this.context = context;
        }
        public object getData(int pos)
        {
            return null;
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
            convertView = inflater.Inflate(Resource.Layout.MyDialog, parent, false);
            convertView.FindViewById<TextView>(Resource.Id.message).Text =$"{blocks[position]}";
            return convertView;
        }
        public override int Count
        {
            get
            {
                return blocks.Count;
            }
        }

    }
}