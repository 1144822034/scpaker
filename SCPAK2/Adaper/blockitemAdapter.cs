using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Security;

namespace SCPAK2
{
    public class blockitemAdaper : BaseAdapter
    {

        Context context;
        public Dictionary<string,string> list = new Dictionary<string, string>();
        public BlockEditActivity editActivity;
        public int selectPos = -1;//选中的editText
        public int pos = 0;//spiner使用
        public List<View> views = new List<View>();
        public blockitemAdaper(Context context)
        {
            this.context = context;
        }
        public object getData(int pos)
        {
            return null;
        }
        public void Clear()
        {
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
            KeyValuePair<string, string> lla = list.ElementAt(position);
            ViewHolder viewHolder=null;
            var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
            if (view != null) viewHolder = view.Tag as ViewHolder;
            if (viewHolder == null) 
            {//不存在
                view = inflater.Inflate(Resource.Layout.block_item, parent, false);
                viewHolder = new ViewHolder();
                viewHolder.edit = view.FindViewById<EditText>(Resource.Id.block_item_edit);
                viewHolder.item = view.FindViewById<TextView>(Resource.Id.block_item_title);
                viewHolder.edit.Text = lla.Value;
                viewHolder.edit.Tag = position;
                viewHolder.edit.TextChanged += new EventHandler<TextChangedEventArgs>(textChange);
                viewHolder.item.Text = lla.Key;
                view.Tag = viewHolder;
            }
            viewHolder.edit.Text = lla.Value;
            viewHolder.item.Text = lla.Key;

            return view;
        }
        public void textChange(object obj, TextChangedEventArgs args)
        {
            EditText editText = (EditText)obj;
            int pos = (int)editText.Tag;
            string lp = list.Keys.ElementAt(pos);
            if (!list[lp].Equals(editText.Text))
            {
                list[lp] = editText.Text;
                NotifyDataSetChanged();
            }
        }
        public override int Count
        {
            get
            {
                return list.Count;
            }
        }
    }
    class touchListener : Java.Lang.Object, View.IOnTouchListener {
        private blockitemAdaper blockitemAdaper;
        public touchListener(blockitemAdaper blockitem) {
            blockitemAdaper = blockitem;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            EditText editText = (EditText)v;
            if (e.Action == MotionEventActions.Up)blockitemAdaper.selectPos = (int)(editText.Tag);
            return false;
        }
    }
    class focusChangeListener : Java.Lang.Object, View.IOnFocusChangeListener
    {
        public blockitemAdaper blockitemAdaper;
        public focusChangeListener(blockitemAdaper blockitem) {
            blockitemAdaper = blockitem;
        }
        public void OnFocusChange(View v, bool hasFocus)
        {
            EditText editText = (EditText)v;
            if (hasFocus) editText.AddTextChangedListener(new textWatch(editText,blockitemAdaper));
        }
    }
    class textWatch : Java.Lang.Object, ITextWatcher
    {
        private EditText editText1;
        public blockitemAdaper blockitemAdaper;
        public textWatch(EditText edit,blockitemAdaper blockitem) {
            editText1 = edit;
            blockitemAdaper = blockitem;
        }
        public void AfterTextChanged(IEditable s)
        {

        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {

        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (-1 != blockitemAdaper.selectPos && blockitemAdaper.selectPos==(int)editText1.Tag) {
                try
                {
                    blockitemAdaper.list[blockitemAdaper.list.ElementAt(blockitemAdaper.selectPos).Key] = s.ToString();
                }
                catch { 
                
                }
            }
        }
    }
    class ViewHolder : Java.Lang.Object
    {
        public TextView item { get; set; }
        public EditText edit { get; set; }

        public int position;
    }

}