using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using SCPAK;
using Engine.Media;
using pakdll;

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
        public Button refreshbutton;
        public Handler handler;
        private SelectOptionDialog SelectOptionDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            bool iip = ContextCompat.IsDeviceProtectedStorage(this);
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
            refreshbutton = FindViewById<Button>(Resource.Id.refreshbtn);
            infobtn.Click += new EventHandler(infoClick);
            refreshbutton.Click += new EventHandler(refreshClick);
            fileList.ItemLongClick += new EventHandler<AdapterView.ItemLongClickEventArgs>(LongClick);
            fileList.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(Onclick);
            fileList.Adapter = fileListAdaper;
            my = new MyDialog(this);
            lastbtn.Click += new EventHandler(lastClick);
            handler = new Handler(handleMessage);
            scanDir(rootPath);
        }
        public void refreshClick(object obj, EventArgs arg)
        {
            scanDir(lastPath);
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
            SelectOptionDialog.adapter.Clear();
            SelectOptionDialog.actions.Clear();
            SelectOptionDialog.setText("请选择操作");
            SelectOptionDialog.adapter.Add("打包为2.1版Pak");
            SelectOptionDialog.adapter.Add("打包为2.2版Pak");
            SelectOptionDialog.adapter.Add("删除此项");
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                sendDialogHiden();
                sendDialog("提示", "目标版本:2.1\n打包中...");
                sendDialogCancelable(false);
                try
                {
                    new Pak21(directory.FullName,this);
                    sendDialog("提示", "打包成功");
                    sendDialogCancelable(true);
                    sendRefreshFileList();
                }
                catch (System.Exception e)
                {
                    sendDialogCancelable(true);
                    sendDialog("打包出错", e.ToString());
                }
            })));
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                sendDialogHiden();
                sendDialog("提示", "目标版本:2.2\n打包中...");
                sendDialogCancelable(false);
                try
                {
                    new Pak(directory.FullName,this);
                    sendDialogCancelable(true);
                    sendDialog("提示", "打包成功");
                    sendRefreshFileList();
                }
                catch (System.Exception e)
                {
                    sendDialogCancelable(true);
                    sendDialog("打包出错", e.ToString());
                }
            })));
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                sendDialogHiden();
                sendDialogCancelable(false);
                if (file != null)sendDialog("提示", "删除中文件" + file.Name + "...");
                else sendDialog("提示", "删除目录中" + directory.Name + "...");
                if (file != null) file.Delete();
                else directory.Delete();
                sendDialog("提示", "删除成功");
                sendDialogCancelable(true);
                sendRefreshFileList();
            })));
            if (file != null)
            {
                if (file.Extension == ".png")
                {
                    Image image = Png.Load(file.Open(FileMode.Open));
                    SelectOptionDialog.adapter.Add("将该图片转换到32位Png");
                    SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                    {
                        sendDialogHiden();
                        sendDialog("提示", "转换图片中...");
                        sendDialogCancelable(false);
                        savepng(image, file);
                        sendDialog("提示", "转换成功");
                        sendDialogCancelable(true);
                        sendRefreshFileList();
                    })));
                }
                else if (file.Extension == ".jpg" || file.Extension == ".jpeg")
                {
                    Image image = Png.Load(file.Open(FileMode.Open));
                    SelectOptionDialog.adapter.Add("将该图片转换到32位Png");
                    SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                    {
                        sendDialogCancelable(false);
                        sendDialog("提示", "转换图片中...");
                        savepng(image, file);
                        sendDialog("提示", "转换成功");
                        sendDialogCancelable(true);
                        sendRefreshFileList();
                    })));
                }
                else if (file.Extension == ".dll" || file.Extension == ".exe") {
                    SelectOptionDialog.adapter.Add("尝试加载这个dll");
                    SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                    {
                        sendDialogHiden();
                        loadDll(file);
                    })));
                }
            }
            SelectOptionDialog.adapter.Add("打包为scmod");
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                sendDialogHiden();
                sendDialogCancelable(false);
                makezip(item,"scmod");
                sendDialogCancelable(true);
                sendRefreshFileList();
            })));
            SelectOptionDialog.adapter.Add("打包为zip");
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                sendDialogHiden();
                sendDialogCancelable(false);
                makezip(item,"zip");
                sendDialogCancelable(true);
            })));
            SelectOptionDialog.Show();
            SelectOptionDialog.adapter.Add("查看已加载的dll");
            SelectOptionDialog.actions.Add(new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                sendDialogHiden();
                sendShowdlls();
            })));
            SelectOptionDialog.Show();
        }
        public void listDlls() {
            SelectOptionDialog.actions.Clear();
            SelectOptionDialog.adapter.Clear();
            SelectOptionDialog.SetTitle("已加载的dll");
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                string name = assembly.GetName().Name;
                if(!name.StartsWith("Xamarin"))SelectOptionDialog.adapter.Add(name+"["+assembly.GetName().Version+"]");            
            }
            SelectOptionDialog.Show();
        }
        public void savepng(Image from, FileInfo file) {
            Image.Save(from, File.Create(Path.Combine(file.Directory.FullName, file.Name.Split(new string[] { file.Extension }, StringSplitOptions.RemoveEmptyEntries)[0] + "1" + file.Extension)), ImageFileFormat.Png, true);
        }
        public void makezip(object obj,string type) {
            FileInfo file = obj as FileInfo;
            DirectoryInfo dir = obj as DirectoryInfo;
            sendDialog("提示","打包中...目标:"+type);
            try {

                if (file != null)
                {
                    ZipArchive zipArchive = ZipArchive.Create(File.Create(Path.Combine(file.Directory.FullName, file.Name + "." + type)));
                    zipArchive.AddStream(file.Name, file.OpenRead());
                    zipArchive.Close();
                }
                else
                {
                    ZipArchive zipArchive = ZipArchive.Create(File.Create(Path.Combine(dir.Parent.FullName, dir.Name + "." + type)));
                    AddZip(dir, zipArchive, dir.FullName);
                    zipArchive.Close();
                }
                sendDialog("提示", "打包完成");
            }
            catch (System.Exception e){
                sendDialog("打包出错",e.ToString());
            }
        }
        public void AddZip(DirectoryInfo dir,ZipArchive zip,string needDel) {
            foreach (DirectoryInfo directory in dir.GetDirectories()) {
                AddZip(directory,zip,needDel);
            }
            foreach (FileInfo file in dir.GetFiles()) {
                zip.AddStream(file.FullName.Split(new string[]{ needDel},StringSplitOptions.RemoveEmptyEntries)[0],file.OpenRead());
            }        
        }
        public void handleMessage(Message msg) {
            switch (msg.What) {
                case 0:
                    bool dp = (bool)msg.Obj;
                    my.SetCancelable(dp);
                    break;
                case 1:
                    string[] tmp = (string[]) msg.Obj;
                    my.setText(tmp[0],tmp[1]);
                    if(!my.IsShowing)my.Show();
                    break;
                case 2:scanDir(lastPath); break;
                case 3:SelectOptionDialog.Hide(); break;
                case 4:listDlls();break;
            }
        }
        public void sendShowdlls() {
            Message message = new Message();
            message.What = 4;
            handler.SendMessage(message);
        }
        public void sendDialogCancelable(bool flag) {
            Message message = new Message();
            message.What = 0;
            message.Obj = flag;
            handler.SendMessage(message);
            
        }
        public void sendDialog(string t,string m) {
            Message message = new Message();
            message.What = 1;
            message.Obj = new string[] {t,m};
            handler.SendMessage(message);
        }
        public void sendRefreshFileList()
        {
            Message message = new Message();
            message.What = 2;
            handler.SendMessage(message);
        }

        public void sendDialogHiden()
        {
            Message message = new Message();
            message.What = 3;
            handler.SendMessage(message);
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
                            if (ver == 2) new UnPak(file.FullName,this);
                            else if (ver == 1) new UnPak21(file.FullName,this);
                        }
                        catch (System.Exception e)
                        {
                            sendDialogCancelable(true);
                            sendDialog("解包出错", e.ToString());
                            error = true;
                        }
                        if (!error)
                        {
                            sendDialogCancelable(true);
                            sendDialog("提示", "解包成功");
                            sendRefreshFileList();
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
                } else if (file.Extension==".dll"||file.Extension==".exe") {
                    my.setText("反编译功能尚在开发中", "尽情期待");
                    my.Show();
                }
                else
                {
                    my.setText("提示", "不能识别的文件类型");
                    my.Show();
                }
            }
        }
        public void loadDll(FileInfo file) {
            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(file.FullName));
                sendDialog("解析成功", $"程序集:{assembly.FullName}");
            }
            catch (System.Exception e)
            {
                sendDialog("错误", "加载程序集失败" + e.ToString());
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