using System;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace SCPAK2
{
    public class spinnerClickListener : Java.Lang.Throwable, AdapterView.IOnItemClickListener
    {
        public blockitemAdaper blockitem;
        public int offset = 0;
        public List<List<string>> Datas=new List<List<string>>();
        public void setOffset(int off) {
            offset = off;
        }
        public spinnerClickListener(blockitemAdaper blockitemAdaper) {
            blockitem = blockitemAdaper;
        }
        public void setData(ref List<List<string>> dat) {
            Datas = dat;
        }
        public void Disposed()
        {
        }

        public void DisposeUnlessReferenced()
        {
        }

        public void Finalized()
        {
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {//更新方块属性列表
            
            List<string> tmp = Datas[position+offset];
            int i = 0;
            blockitem.list.Clear();
            foreach (string tmpa in tmp)
            {
                blockitem.list.Add(BlockEditActivity.tranlates[BlockEditActivity.tranlates.Keys.ElementAt(i)], tmpa);
                ++i;
            }
            blockitem.NotifyDataSetChanged();
        }

        public void SetJniIdentityHashCode(int value)
        {
        }

        public void SetJniManagedPeerState(JniManagedPeerStates value)
        {
        }

        public void SetPeerReference(JniObjectReference reference)
        {
        }
    }
}