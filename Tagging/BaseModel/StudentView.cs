using System.Linq;
using System.Collections.Generic;
using FISCA.Permission;
using K12.Data;
using Tagging.ViewHelper;
using System;
using Customization.Tagging;

namespace Tagging.BaseModel
{
    /// <summary>
    /// 
    /// </summary>
    internal partial class StudentView : ViewHelper.TreeNavViewBase
    {
        public StudentView()
            : base()
        {
            NavText = "類別檢視";
            ShowRoot = true;
            StudentTag.AfterDelete += (StudentTag_AfterDelete);
            StudentTag.AfterInsert += (StudentTag_AfterInsert);
            StudentTag.AfterUpdate += (StudentTag_AfterUpdate);
            TagConfig.AfterDelete += (TagConfig_AfterDelete);
            TagConfig.AfterInsert += (TagConfig_AfterInsert);
            TagConfig.AfterUpdate += (TagConfig_AfterUpdate);
        }

        private void TagConfig_AfterUpdate(object sender, DataChangedEventArgs e)
        {
            RenderTreeView();
        }

        private void TagConfig_AfterInsert(object sender, DataChangedEventArgs e)
        {
            RenderTreeView();
        }

        private void TagConfig_AfterDelete(object sender, DataChangedEventArgs e)
        {
            RenderTreeView();
        }

        private void StudentTag_AfterUpdate(object sender, DataChangedEventArgs e)
        {
            RenderTreeView();
        }

        private void StudentTag_AfterInsert(object sender, DataChangedEventArgs e)
        {
            RenderTreeView();
        }

        private void StudentTag_AfterDelete(object sender, DataChangedEventArgs e)
        {
            RenderTreeView();
        }

        protected override string KeyCatalogTitleFormat(KeyCatalog catalog)
        {
            return base.KeyCatalogTitleFormat(catalog);
        }

        protected override int KeyCatalogComparer(KeyCatalog x, KeyCatalog y)
        {
            string x_val = x.Tag + "";
            string y_val = y.Tag + "";

            return StringComparer.CurrentCultureIgnoreCase.Compare(x_val, y_val);
        }

        protected override void GenerateTreeStruct(KeyCatalog root)
        {
            Dictionary<string, TagConfigRecord> map = TagConfig.SelectAll().Viewable().ToDictionary(x => x.ID);
            IEnumerable<StudentTagRecord> tagRecordList = StudentTag.SelectByStudentIDs(Source).Viewable();
            ISet<string> nocatalog = new HashSet<string>(Source);

            foreach (StudentTagRecord student in tagRecordList)
            {
                if (map.ContainsKey(student.RefTagID))
                {
                    TagConfigRecord config = map[student.RefTagID];
                    KeyCatalog catalog = null, parent = null;

                    if (!string.IsNullOrWhiteSpace(config.Prefix)) //如果有 Prefix 就先建立 Prefix KeyCatalog 當作 Parent。
                    {
                        parent = root[config.Prefix];
                        parent.Tag = string.Format("{0}:{1}", "0", config.Prefix);
                    }
                    else
                        parent = root;

                    catalog = parent[config.Name];
                    catalog.Tag = string.Format("{0}:{1}", "1", config.Name);

                    catalog.AddKey(student.RefEntityID);

                    if (nocatalog.Contains(student.RefEntityID))
                        nocatalog.Remove(student.RefEntityID);
                }
            }

            //加入未分類別的。
            foreach (string key in nocatalog)
                root["未分類別"].AddKey(key);
            root["未分類別"].Tag = string.Format("2:未分類別");
        }

        protected override void PreRefresh()
        {
        }
    }
}
