using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.OS.Storage;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using SCPAK;
using Xamarin.Essentials;
namespace SCPAK2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public fileListAdaper fileListAdaper;
        public ListView fileList;
        public string lastPath = "";
        public Button lastbtn;
        public int clickpos = 0;
        public MyDialog my;
        public Button infobtn;

        private SelectOptionDialog SelectOptionDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            bool iip=ContextCompat.IsDeviceProtectedStorage(this);
            while (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {//请求权限
                RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 0);
            }
            while (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {//请求权限
                RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, 0);
            }
            SetContentView(Resource.Layout.activity_main);
            fileListAdaper = new fileListAdaper(this);
            SelectOptionDialog = new SelectOptionDialog(this);
            string rootPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            fileList = FindViewById<ListView>(Resource.Id.fileList);
            lastbtn = FindViewById<Button>(Resource.Id.lastbtn);
            infobtn = FindViewById<Button>(Resource.Id.infobtn);
            infobtn.Click += new EventHandler(infoClick);
            fileList.ItemLongClick += new EventHandler<AdapterView.ItemLongClickEventArgs>(LongClick);
            fileList.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(Onclick);
            fileList.Adapter=fileListAdaper;
            my = new MyDialog(this);            
            lastbtn.Click += new EventHandler(lastClick);
            scanDir(rootPath);
        }
        public void infoClick(object obj, EventArgs arg)
        {
            my.setText("帮助", "长安文件夹打包\n点击PAk文件进行解包\n开发者：QQ1144822034");
            my.Show();
        }
        public void LongClick(object obj, AdapterView.ItemLongClickEventArgs arg)
        {            
            int pos = arg.Position;
            object item = fileListAdaper.getData(pos);
            DirectoryInfo directory = item as DirectoryInfo;
            FileInfo file = item as FileInfo;
            if (file != null) {
                my.setText("错误","文件不能进行打包");
                my.Show();
                my.SetCancelable(true);
                return;
            }
            SelectOptionDialog.adapter.Clear();
            SelectOptionDialog.actions.Clear();
            SelectOptionDialog.adapter.Add("2.1");
            SelectOptionDialog.adapter.Add("2.2");
            SelectOptionDialog.adapter.Add("删除");
            SelectOptionDialog.setText("请选择打包目标PAK版本");
            SelectOptionDialog.Show();
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectOptionDialog.Hide();
                    my.setText("提示", "目标版本:2.1\n打包中...");
                    my.Show();
                    my.SetCancelable(false);
                });
                bool error = false;
                try
                {
                    new Pak21(directory.FullName);
                }
                catch (System.Exception e)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        my.setText("打包出错", e.ToString());
                        my.Show();
                        my.SetCancelable(true);
                    });
                    error = true;
                }
                if (!error)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        my.setText("提示", "打包成功");
                        my.Show();
                        my.SetCancelable(true);
                        scanDir(lastPath);
                    });
                }
            })));
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectOptionDialog.Hide();
                    my.setText("提示", "目标版本:2.2\n打包中...");
                    my.Show();
                    my.SetCancelable(false);
                });
                bool error = false;
                try
                {
                    new Pak(directory.FullName);
                }
                catch (System.Exception e)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        my.setText("打包出错", e.ToString());
                        my.Show();
                        my.SetCancelable(true);
                    });
                    error = true;
                }
                if (!error)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        my.setText("提示", "打包成功");
                        my.Show();
                        my.SetCancelable(true);
                        scanDir(lastPath);
                    });
                }
            })));
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectOptionDialog.Hide();
                    if (file != null)
                        my.setText("提示", "删除中文件"+file.Name+"...");
                    else my.setText("提示","删除目录中"+directory.Name+"...");
                    my.Show();
                    my.SetCancelable(false);
                });
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (file != null) file.Delete();
                    else directory.Delete();
                    scanDir(lastPath);
                    my.setText("提示","删除成功");
                    my.Show();
                    my.SetCancelable(true);
                });
            })));
        }
        public void lastClick(object obj, EventArgs arg)
        {
            try
            {
                scanDir(new DirectoryInfo(lastPath).Parent.FullName);
            }
            catch (System.Exception e)
            {
                tip("已经是根目录啦");
            }
        }
        public void scanDir(string path)
        {
            lastPath = path;
            fileListAdaper.Clear();
            List<DirectoryInfo> dirlist = new List<DirectoryInfo>();
            List<FileInfo> filelist = new List<FileInfo>();
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            foreach (string mm in dirs)
            {
                if (Path.GetRelativePath(lastPath,mm).StartsWith(".")) continue;
                DirectoryInfo ddd = new DirectoryInfo(mm);
                if (ddd.Exists) dirlist.Add(ddd);
            }
            foreach (string dd in files)
            {
                if (Path.GetFileName(dd).StartsWith(".")) continue;
                FileInfo fileInfo = new FileInfo(dd);
                if(fileInfo.Exists)filelist.Add(fileInfo);
            }

            DirectoryInfo[] listd = dirlist.ToArray();
            SortAsDirName(ref listd);
            FileInfo[] listf = filelist.ToArray();
            SortAsFileName(ref listf);
            fileListAdaper.list.AddRange(listd);
            fileListAdaper.list.AddRange(listf);
            fileListAdaper.NotifyDataSetChanged();
        }
        public void tip(string txt)
        {
            Toast.MakeText(this, txt, ToastLength.Long).Show();
        }
        private void SortAsFileName(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.Name.CompareTo(y.Name); });
        }
        private void SortAsDirName(ref DirectoryInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (DirectoryInfo x, DirectoryInfo y) { return x.Name.CompareTo(y.Name); });
        }
        private int readPakVersion(string fullname) {
            FileStream fileStream= File.OpenRead(fullname);
            byte[] by = new byte[4];
            int len=fileStream.Read(by,0,4);
            fileStream.Close();
            if (len != 4)
            {
                return -1;
            }
            else if (by[0] == 0x50 && by[1] == 0x4b && by[2] == 0x32 && by[3] == 0x00)
            {
                return 2;
            }
            else if (by[0] == 0x50 && by[1] == 0x41 && by[2] == 0x4b && by[3] == 0x00)
            {
                return 1;
            }
            else {
                return -1;
            }
        }
        public void Onclick(object obj, AdapterView.ItemClickEventArgs args)
        {
            int pos = args.Position;
            object item = fileListAdaper.getData(pos);
            DirectoryInfo directory = item as DirectoryInfo;
            FileInfo file = item as FileInfo;
            if (directory != null)
            {
                scanDir(directory.FullName);
            }
            else
            {
                if (file.Extension == ".pak")
                {
                    int ver = readPakVersion(file.FullName);
                    if (ver < 0)
                    {
                        my.setText("错误", "不能识别的PAK包");
                        my.Show();
                        return;
                    }
                    else {
                        my.setText("提示", "解包中\nPAK包版本:2."+ver.ToString());
                        my.Show();
                    }
                    my.SetCancelable(false);
                    new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                        bool error = false;
                        try
                        {
                            if (ver == 2) new UnPak(file.FullName);
                            else if (ver == 1) new UnPak21(file.FullName);
                        }
                        catch (System.Exception e)
                        {
                            MainThread.BeginInvokeOnMainThread(() => {
                                my.setText("解包出错", e.ToString());
                                my.Show();
                                my.SetCancelable(true);
                            });
                            error = true;
                        }
                        if (!error)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                my.setText("提示", "解包成功");
                                my.Show();
                                my.SetCancelable(true);
                                scanDir(lastPath);
                            });
                        }
                    })).Start();
                }
                else if (file.Extension == ".csv" || file.Name == "BlocksData.txt"|| file.Name == "BlocksData_new.txt"|| file.Name == "BlocksData_new_new.txt") {//进入blocksdata编辑器界面
                    Bundle bundle = new Bundle();
                    bundle.PutString("fullname", file.FullName);
                    bundle.PutString("name", file.Name);
                    Intent intent = new Intent(this, typeof(BlockEditActivity));
                    intent.PutExtra("file", bundle);
                    StartActivity(intent);
                } else if (file.Extension==".dll") {
                    try
                    {
                        Assembly assembly = Assembly.Load(File.ReadAllBytes(file.FullName));
                        StringBuilder s=new StringBuilder();
                        foreach (TypeInfo typeInfo in assembly.DefinedTypes) {
                            s.Append(typeInfo.Name);
                            s.Append("\n");
                        }
                        my.setText("解析成功",$"程序集:{assembly.FullName}\n{s.ToString()}");
                        my.Show();
                    }
                    catch {
                        my.setText("错误","不能识别的dll文件");
                        my.Show();
                    }
                }
                else
                {
                    my.setText("提示", "不能识别的文件类型");
                    my.Show();
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back) {
                if (string.IsNullOrEmpty(lastPath)) System.Environment.Exit(0);
                if(lastPath== Android.OS.Environment.ExternalStorageDirectory.AbsolutePath) System.Environment.Exit(0);
                DirectoryInfo directoryInfo = new DirectoryInfo(lastPath);
                scanDir(directoryInfo.Parent.FullName);
                return false; 
            }
            else System.Environment.Exit(0);                        
            return base.OnKeyDown(keyCode,e);
        }
    }
}