using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace SCPAK2
{
    [Activity(Label = "BlocksData编辑器", Theme = "@style/AppTheme")]
    public class BlockEditActivity:Activity
    {
        public static Dictionary<string, string> tranlates = new Dictionary<string, string>();
        private TextView title;
        private Spinner spinner;
        private MyDialog dialog;
        private Button button;
        public LinearLayout containView;
        public List<List<string>> blocklist=new List<List<string>>();
        private blockSpinnerAdapter spinnerAdapter;
        private int lastClickPos = 0;
        private ArrayAdapter arrayAdapter ;
        public void initTranslate() {
            if (tranlates.Count > 0) return;
            tranlates.Add("Class Name", "类名");
            tranlates.Add("DefaultDisplayName", "默认显示名称");
            tranlates.Add("DefaultCategory", "默认类别");
            tranlates.Add("Behaviors", "行为");
            tranlates.Add("DisplayOrder", "显示顺序");
            tranlates.Add("DefaultIconBlockOffset", "默认图标块偏移");
            tranlates.Add("DefaultIconViewOffset", "默认图标视图偏移");
            tranlates.Add("DefaultIconViewScale", "默认图标视图缩放");
            tranlates.Add("FirstPersonScale", "第一人称缩放");
            tranlates.Add("FirstPersonOffset", "第一人称偏移");
            tranlates.Add("FirstPersonRotation", "第一人称旋转");
            tranlates.Add("InHandScale", "手持缩放");
            tranlates.Add("InHandOffset", "手持偏移");
            tranlates.Add("InHandRotation", "手持旋转");
            tranlates.Add("CraftingId", "合成谱标识");
            tranlates.Add("DefaultCreativeData", "默认为创造数据");
            tranlates.Add("IsCollidable", "是否可碰撞");
            tranlates.Add("IsPlaceable", "是可放置");
            tranlates.Add("IsDiggingTransparent", "挖掘是否透明");
            tranlates.Add("IsPlacementTransparent", "放置是否透明");
            tranlates.Add("DefaultIsInteractive", "是否可交互");
            tranlates.Add("IsEditable", "是否可编辑");
            tranlates.Add("IsNonDuplicable", "是否可堆叠");
            tranlates.Add("IsGatherable", "是否可收集");
            tranlates.Add("HasCollisionBehavior", "是否有碰撞行为");
            tranlates.Add("KillsWhenStuck", "卡住是否会死亡");
            tranlates.Add("IsFluidBlocker", "是否流体方块");
            tranlates.Add("IsTransparent", "是否透明");
            tranlates.Add("DefaultShadowStrength", "默认阴影强度");
            tranlates.Add("LightAttenuation", "光衰减强度");
            tranlates.Add("DefaultEmittedLightAmount", "默认发光强度");
            tranlates.Add("ObjectShadowStrength", "物体阴影强度");
            tranlates.Add("DefaultDropContent", "默认掉落内容");
            tranlates.Add("DefaultDropCount", "默认掉落数量");
            tranlates.Add("DefaultExperienceCount", "默认掉落经验");
            tranlates.Add("RequiredToolLevel", "所需工具级别");
            tranlates.Add("MaxStacking", "最大堆积");
            tranlates.Add("SleepSuitability", "适合睡眠");
            tranlates.Add("FrictionFactor", "摩擦系数");
            tranlates.Add("Density", "密度");
            tranlates.Add("NoAutoJump", "禁止自动跳跃");
            tranlates.Add("NoSmoothRise", "非平稳上升");
            tranlates.Add("FuelHeatLevel", "燃料加热水平");
            tranlates.Add("FuelFireDuration", "燃料持续时间");
            tranlates.Add("DefaultSoundMaterialName", "默认声音材质名称");
            tranlates.Add("ShovelPower", "铲子效率");
            tranlates.Add("QuarryPower", "稿子效率");
            tranlates.Add("HackPower", "斧子效率");
            tranlates.Add("DefaultMeleePower", "默认近战攻击力");
            tranlates.Add("DefaultMeleeHitProbability", "默认近战命中概率");
            tranlates.Add("DefaultProjectilePower", "默认射击攻击力");
            tranlates.Add("ToolLevel", "工具等级");
            tranlates.Add("PlayerLevelRequired", "需要玩家等级");
            tranlates.Add("Durability", "耐久性");
            tranlates.Add("IsAimable", "是否可瞄准");
            tranlates.Add("IsStickable", "是否可粘贴");
            tranlates.Add("AlignToVelocity", "与速度对齐");
            tranlates.Add("ProjectileSpeed", "投掷速度");
            tranlates.Add("ProjectileDamping", "投掷阻力");
            tranlates.Add("ProjectileTipOffset", "投掷倾摆");
            tranlates.Add("DisintegratesOnHit", "击中后消失");
            tranlates.Add("ProjectileStickProbability", "投掷粘上概率");
            tranlates.Add("DefaultHeat", "默认热量");
            tranlates.Add("FireDuration", "燃烧持续时间");
            tranlates.Add("ExplosionResilience", "爆炸抗性");
            tranlates.Add("DefaultExplosionPressure", "默认爆炸压力");
            tranlates.Add("DefaultExplosionIncendiary", "默认爆炸燃烧弹");
            tranlates.Add("IsExplosionTransparent", "爆炸透明");
            tranlates.Add("DigMethod", "挖掘方法");
            tranlates.Add("DigResilience", "挖掘抗性");
            tranlates.Add("ProjectileResilience", "投掷弹性");
            tranlates.Add("DefaultNutritionalValue", "默认营养值");
            tranlates.Add("DefaultSicknessProbability", "默认患病概率");
            tranlates.Add("FoodType", "食物类型");
            tranlates.Add("DefaultRotPeriod", "默认旋转周期");
            tranlates.Add("DefaultTextureSlot", "默认纹理位置");
            tranlates.Add("DestructionDebrisScale", "掉落缩放");
            tranlates.Add("DefaultDescription", "默认描述");
            tranlates.Add("MaxInHandStacking","最大手持堆叠");
            tranlates.Add("EmittedLightAmount","发光强度");
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            initTranslate();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.block_edit);
            dialog = new MyDialog(this);
            spinnerAdapter = new blockSpinnerAdapter(this);
            arrayAdapter = new ArrayAdapter(this, Resource.Layout.MyDialog, Resource.Id.message);
            Bundle config = Intent.GetBundleExtra("file");
            string fullname = config.GetString("fullname");
            string name = config.GetString("name");
            title = FindViewById<TextView>(Resource.Id.block_edit_title);
            spinner = FindViewById<Spinner>(Resource.Id.block_edit_spinner);
            containView = FindViewById<LinearLayout>(Resource.Id.containView);
            button = FindViewById<Button>(Resource.Id.block_edit_save);
            button.Click += new EventHandler(saveFile);
            spinner.Adapter = arrayAdapter;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerClick);
            spinner.Clickable = true;
            title.Text=$"{name}导入成功";
            StreamReader stream = new StreamReader(File.OpenRead(fullname));
            string content = stream.ReadToEnd();
            if (!checkValid(content))
            {
                dialog.setText("错误","不是有效的BlocksData文件");
                dialog.Show();
            }
            else {
                parseBlocks(content);
                parseToView();
            }
        }        
        public void saveFile(object obj,EventArgs args) {
            Bundle config = Intent.GetBundleExtra("file");
            string fullname = config.GetString("fullname");
            StringBuilder stringBuilder = new StringBuilder();
            int yy = 0;
            foreach (List<string> items in blocklist) {
                int dd = 0;
                foreach (string item in items) {
                    stringBuilder.Append(item);
                    if(dd<items.Count()-1)stringBuilder.Append(';');
                    ++dd;
                }
                if(yy<blocklist.Count-1)stringBuilder.Append('\n');
                ++yy;
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 1);//移除最后的换行
            string newfile = System.IO.Path.GetFileNameWithoutExtension(fullname) + "_new" + System.IO.Path.GetExtension(fullname);
            string newpath =System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullname),newfile);
            File.WriteAllText(newpath, stringBuilder.ToString());
            dialog.setText("提示","已保存到该目录的新文件"+newfile);
            dialog.Show();
        }
        public void textChange(object obj,TextChangedEventArgs args) {
            EditText edit = (EditText)obj;
            int posaa=(int)edit.Tag;
            string soo = args.Text.ToString();
            if (!soo.Equals(blocklist[lastClickPos][posaa]))blocklist[lastClickPos][posaa] = soo;
        }
        public void refreshView(int where) { //根据spinner选择的pos绘制视图
            int ii = 0;
            foreach (string item in blocklist[where])
            {
                View view = containView.FindViewWithTag(ii);
                if (view == null)
                {
                    LinearLayout linearLayout = new LinearLayout(this);
                    linearLayout.Orientation = Orientation.Horizontal;
                    LinearLayout.LayoutParams lineParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    LinearLayout.LayoutParams editParam = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f);
                    LinearLayout.LayoutParams textParam = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 1f);
                    lineParams.SetMargins(0, 1, 0, 1);
                    editParam.SetMargins(10, 0, 0, 0);
                    linearLayout.LayoutParameters = lineParams;
                    EditText editText = new EditText(this);
                    editText.TextChanged += new EventHandler<TextChangedEventArgs>(textChange);
                    editText.Tag = ii;
                    TextView textView = new TextView(this);
                    tranlates.TryGetValue(blocklist[0][ii],out string title);
                    if (string.IsNullOrEmpty(title)) title = blocklist[0][ii];
                    textView.Text = title;
                    textView.Gravity = GravityFlags.Center;
                    textView.LayoutParameters = textParam;
                    textView.SetBackgroundColor(Color.ParseColor("#a88888"));
                    editText.SetBackgroundColor(Color.ParseColor("#a99999"));
                    editText.LayoutParameters = editParam;
                    editText.Gravity = GravityFlags.Center;
                    editText.Id = Resource.Id.block_item_edit;
                    editText.Text = item;
                    linearLayout.Tag = ii;
                    linearLayout.AddView(textView);
                    linearLayout.AddView(editText);
                    containView.AddView(linearLayout);
                }
                else
                {
                    EditText editText = view.FindViewById<EditText>(Resource.Id.block_item_edit);
                    editText.Text = item;
                }
                ++ii;
            }
        }
        public void spinnerClick(object obj,AdapterView.ItemSelectedEventArgs args) {
            lastClickPos =args.Position;//处理位置
            refreshView(lastClickPos);
        }
        public bool checkValid(string content) {
            if (content.Contains(";")) {
                string[] ll = content.Split(new char[] { ';' });
                if (ll.Length > 10) {
                    return true;
                }
                else return false;
            }
            return false;
        }
        public void parseToView() {
            spinnerAdapter.blocks.Clear();
            foreach (List<string> item in blocklist) {
                arrayAdapter.Add($"[{item[0]}]{item[1]}");
            }
            refreshView(0);
            spinnerAdapter.NotifyDataSetChanged();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            FinishActivity(0);
        }
        //解析文件
        public List<List<string>> parseBlocks(string content) {
            string[] lines = null;
            if (content.Contains("\r\n"))
            {
                lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            else {
                lines = content.Split(new string[] { "\n" }, StringSplitOptions.None);
            }
            foreach (string blockline in lines) {
                string[] items = blockline.Split(new char[] { ';'});
                if (items.Length > 2) if (string.IsNullOrEmpty(items[0])&& string.IsNullOrEmpty(items[1])&& string.IsNullOrEmpty(items[2])) continue;//去除空的
                List<string> block= new List<string>();
                foreach (string item in items) {
                    block.Add(item);                
                }
                if(block.Count>10)blocklist.Add(block);
            }
            return blocklist;
        }
        public string GetModelValue(string FieldName, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                object o = Ts.GetProperty(FieldName).GetValue(obj, null);
                string Value = Convert.ToString(o);
                if (string.IsNullOrEmpty(Value))
                    return null;
                return Value;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 设置对象中的属性值
        /// </summary>
        /// <param name="FieldName">属性名</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public bool SetModelValue(string FieldName, string Value, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                object v = Convert.ChangeType(Value, Ts.GetProperty(FieldName).PropertyType);
                Ts.GetProperty(FieldName).SetValue(obj, v, null);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}