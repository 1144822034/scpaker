using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Android.App;
using Android.Content;
using Android.Widget;
using Java.Lang;

namespace SCPAK2
{
    public class SelectOptionDialog : Dialog
    {
        public ListView listView;
        public List<string> data=new List<string>();
        public ArrayAdapter adapter;
        public List<System.Threading.Thread> actions = new List<System.Threading.Thread>();
        public SelectOptionDialog(Context context) : base(context)
        {
            SetContentView(Resource.Layout.optionslist);
            adapter = new ArrayAdapter(context,Resource.Layout.MyDialog,Resource.Id.message);
            listView = FindViewById<ListView>(Resource.Id.optionlist);
            listView.Adapter = adapter;
            listView.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(click);
        }
        public void click(object obj,AdapterView.ItemClickEventArgs args) {
            if (args.Position < actions.Count)
                actions[args.Position].Start();
            else Hide();
        }
        public void setText(string t)
        {
            SetTitle(t);
        }
    }
}