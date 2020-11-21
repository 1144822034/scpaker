using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace SCPAK2
{
    public class OnDialogCancelListener : Java.Lang.Object,IDialogInterfaceOnCancelListener
    {
        private FileInfo finfo;
        private DirectoryInfo dinfo;
        public OnDialogCancelListener(FileInfo finfo,DirectoryInfo directoryInfo) {
            this.finfo = finfo;
            dinfo = directoryInfo;
        }
        public void OnCancel(IDialogInterface dialog)
        {
        }
    }
}