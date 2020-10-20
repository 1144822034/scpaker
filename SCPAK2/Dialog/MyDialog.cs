using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Android.App;
using Android.Content;
using Android.Widget;

namespace SCPAK2
{
    public class MyDialog : Dialog
    {
        public string tip = "";
        public TextView content;
        public LinearLayout linearLayout;
        public bool Cancelable = true;
        public MyDialog(Context context):base(context) {
            SetContentView(Resource.Layout.MyDialog);
            content = FindViewById<TextView>(Resource.Id.message);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.frameLayout1);
            linearLayout.Click += new EventHandler(Click);
        }
        public override void SetCancelable(bool flag)
        {
            Cancelable = flag;
            base.SetCancelable(flag);
        }
        public void Click(object obj,EventArgs args) {
            if (Cancelable) Hide();
        }
        public void setText(string t,string msg) {
            SetTitle(t);
            content.Text = msg;
        }
    }
}