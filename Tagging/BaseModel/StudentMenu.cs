using System;
using System.Collections.Generic;
using System.Linq;
using FISCA.Authentication;
using FISCA.LogAgent;
using FISCA.Permission;
using FISCA.Presentation;
using K12.Data;
using K12.Presentation;
using Customization.Tagging;

namespace Tagging.BaseModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StudentMenu
    {
        /// <summary>
        /// 指定類別的權限代碼。
        /// </summary>
        protected string[] AssignCodes { get; set; }
        /// <summary>
        /// 管理類別的權限代碼。
        /// </summary>
        protected string[] ManageCodes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignCodes">指定類別的權限代碼。</param>
        /// <param name="manageCodes">管理類別的權限代碼。</param>
        internal StudentMenu(RibbonBarItem menuContainer)
        {
            SetupPermissionCode();

            menuContainer["類別"].Image = Tagging._TagForm.PResource.ctxTag_Image;
            menuContainer["類別"].SupposeHasChildern = true;
            menuContainer["類別"].PopupOpen += MenuOpen;
        }

        protected virtual void SetupPermissionCode()
        {
            //"Button0100" 是高中的權限代碼。
            string[] assignCode = new string[] { "JHSchool.Student.Ribbon0040", "Button0100" };
            string[] manageCode = new string[] { "JHSchool.Student.Ribbon0050", "Button0100" };

            AssignCodes = assignCode;
            ManageCodes = manageCode;
        }

        /// <summary>
        /// 打開類別管理的畫面。
        /// </summary>
        protected virtual void OpenTagConfigForm()
        {
            TagForm tf = new TagForm();
            tf.Category = TagCategory.Student;
            tf.EntityTitle = "學生";
            tf.EntityIdField = "Student";
            tf.CounterService = "SmartSchool.Tag.GetUseStudentList";
            tf.ShowDialog();
        }

        /// <summary>
        /// 取得目前選擇的 ID 清單。
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> GetSelectedSource()
        {
            return K12.Presentation.NLDPanels.Student.SelectedSource;
        }

        protected virtual List<TagConfigRecord> GetTagConfigList()
        {
            return TagConfig.SelectByCategory(TagCategory.Student).Viewable().ToList();
        }

        protected virtual List<GeneralTagRecord> GetTagRelationByIDs(List<string> IDs)
        {
            return StudentTag.SelectByStudentIDs(IDs).ConvertAll(x => (GeneralTagRecord)x).Viewable().ToList();
        }

        protected virtual GeneralTagRecord NewTagRelationRecord(string entityId, string tagID)
        {
            return new StudentTagRecord(entityId, tagID);
        }

        protected virtual void InsertTagRelations(List<GeneralTagRecord> records)
        {
            StudentTag.Insert(records.ConvertAll(x => (StudentTagRecord)x));
        }

        protected virtual void RemoveTagRelations(List<GeneralTagRecord> records)
        {
            StudentTag.Delete(records.ConvertAll(x => (StudentTagRecord)x));
        }

        internal void MenuOpen(object sender, PopupOpenEventArgs e)
        {
            List<string> selectedIDs = GetSelectedSource();
            if (selectedIDs.Count > 0)
            {
                //建立指定 Category 的所有 Tag 項目。
                List<TagConfigRecord> noprefix = new List<TagConfigRecord>();
                List<string> prefixList = new List<string>();
                IEnumerable<TagConfigRecord> viewables = GetSortedTags().Viewable();
                foreach (TagConfigRecord record in viewables)
                {
                    if (string.IsNullOrEmpty(record.Prefix))
                        noprefix.Add(record);
                    else
                    {
                        if (!prefixList.Contains(record.Prefix))
                        {
                            SetPrefixEvent(selectedIDs, e.VirtualButtons[record.Prefix]);
                            prefixList.Add(record.Prefix);
                        }

                        CreateTagMenuItem(e, selectedIDs, record, record.AccessControlCode);
                    }
                }

                //不具有 Prefix 的要建立在根功能表。
                MenuButton topPrefixItem = e.VirtualButtons;
                foreach (TagConfigRecord record in noprefix)
                {
                    MenuButton tagItem = e.VirtualButtons[record.Name];
                    SetEvents(selectedIDs, record, tagItem, topPrefixItem, record.AccessControlCode);
                }
                PrefixMenuOpen(selectedIDs, topPrefixItem);
            }

            e.VirtualButtons["類別管理..."].BeginGroup = true;
            e.VirtualButtons["類別管理..."].Enable = Accept(ManageCodes);
            e.VirtualButtons["類別管理..."].Click += delegate
            {
                OpenTagConfigForm();
            };
        }

        private List<TagConfigRecord> GetSortedTags()
        {
            List<TagConfigRecord> items = GetTagConfigList();
            items.Sort((x, y) =>
            {
                return x.FullName.CompareTo(y.FullName);
            });
            return items;
        }

        private void CreateTagMenuItem(PopupOpenEventArgs e, List<string> selected, TagConfigRecord tcRecord, string acceptCode)
        {
            string prefix = tcRecord.Prefix;

            MenuButton tagItem = e.VirtualButtons[prefix][tcRecord.Name];
            MenuButton prefixItem = e.VirtualButtons[prefix];

            SetEvents(selected, tcRecord, tagItem, prefixItem, acceptCode);
        }

        private void SetPrefixEvent(List<string> selected, MenuButton prefixItem)
        {
            prefixItem.PopupOpen += delegate
            {
                PrefixMenuOpen(selected, prefixItem);
            };
        }

        private void SetEvents(List<string> selected,
            TagConfigRecord tcRecord,
            MenuButton tagItem,
            MenuButton prefixItem,
            string acceptCode)
        {
            tagItem.Tag = tcRecord; //TagConfigRecord。
            tagItem.AutoCheckOnClick = true;
            tagItem.AutoCollapseOnClick = false;
            if (!string.IsNullOrWhiteSpace(acceptCode))
            {
                tagItem.Visible = AcceptView(acceptCode);
                tagItem.Enable = AcceptEdit(acceptCode);
            }
            else
                tagItem.Visible = Accept(AssignCodes);

            tagItem.Click += delegate(object sender, EventArgs e1)
            {
                TagMenuCheckChanged(selected, sender as MenuButton);
                CalcCheckedState(selected, prefixItem);
            };
        }

        private void PrefixMenuOpen(List<string> selected, MenuButton prefixItem)
        {
            //當沒算過時，計算 Checked State。
            if (string.IsNullOrEmpty("" + prefixItem.Tag))
                CalcCheckedState(selected, prefixItem);

            Dictionary<string, int> tags = prefixItem.Tag as Dictionary<string, int>;
            int selectedCount = selected.Count;
            foreach (var item in prefixItem.Items)
            {
                if (item.Tag is TagConfigRecord)
                {
                    TagConfigRecord tc = (item.Tag as TagConfigRecord);

                    //當具有 AccessControlCode 時才進行判斷。
                    if (!string.IsNullOrWhiteSpace(tc.AccessControlCode))
                    {
                        if (!AcceptView(tc.AccessControlCode))
                            continue;
                    }

                    string tagID = tc.ID;
                    if (tags.ContainsKey(tagID) && tags[tagID] == selectedCount)
                        item.Checked = true;
                    else
                        item.Checked = false;
                }
                else
                {
                    item.Visible = item.Items.Count(x => x.Visible) > 0;
                }
            }
        }

        private void CalcCheckedState(List<string> selected, MenuButton prefixMenuButton)
        {
            Dictionary<string, int> tagRefCountMap = new Dictionary<string, int>();

            foreach (GeneralTagRecord entityTag in GetTagRelationByIDs(selected))
            {
                if (!tagRefCountMap.ContainsKey(entityTag.RefTagID))
                    tagRefCountMap.Add(entityTag.RefTagID, 0);
                tagRefCountMap[entityTag.RefTagID]++;
            }
            prefixMenuButton.Tag = tagRefCountMap;
        }

        private void TagMenuCheckChanged(List<string> selectedEntityIDs, MenuButton mb)
        {
            List<GeneralTagRecord> addList = new List<GeneralTagRecord>();
            List<GeneralTagRecord> removeList = new List<GeneralTagRecord>();

            TagConfigRecord tag = mb.Tag as TagConfigRecord;

            List<GeneralTagRecord> tagRecords = GetTagRelationByIDs(selectedEntityIDs);
            Dictionary<string, Dictionary<string, GeneralTagRecord>> TagRecordMap = new Dictionary<string, Dictionary<string, GeneralTagRecord>>();
            foreach (GeneralTagRecord each in tagRecords)
            {
                if (!TagRecordMap.ContainsKey(each.RefEntityID))
                    TagRecordMap.Add(each.RefEntityID, new Dictionary<string, GeneralTagRecord>());

                TagRecordMap[each.RefEntityID].Add(each.RefTagID, each);
            }

            //LogSaver log = ApplicationLog.CreateLogSaverInstance();

            //Student.SelectByIDs(selectedEntityIDs);//先快取資料。
            foreach (string entityId in selectedEntityIDs)
            {
                if (!TagRecordMap.ContainsKey(entityId)) //防止 Key 不存在爆掉。
                    TagRecordMap.Add(entityId, new Dictionary<string, GeneralTagRecord>());

                if (mb.Checked == true)
                {
                    if (!TagRecordMap[entityId].ContainsKey(tag.ID)) //不在清單中就新增。
                    {
                        if (string.IsNullOrEmpty(entityId))
                            Console.WriteLine("Empty");
                        if (string.IsNullOrEmpty(tag.ID))
                            Console.WriteLine("Emtpy");

                        addList.Add(NewTagRelationRecord(entityId, tag.ID));

                        //string actionBy = string.Format("類別.{0}類別", EntityTitle);
                        //string action = string.Format("指定{0}類別", EntityTitle);
                        //StudentRecord student = Student.SelectByID(entityId);
                        //log.AddBatch(actionBy, action, "description:" + student.Name);
                    }
                }
                else
                {
                    if (TagRecordMap[entityId].ContainsKey(tag.ID)) //如果在清單中就移除。
                    {
                        if (string.IsNullOrEmpty(entityId))
                            Console.WriteLine("Empty");
                        if (string.IsNullOrEmpty(tag.ID))
                            Console.WriteLine("Emtpy");

                        removeList.Add(TagRecordMap[entityId][tag.ID]);
                        //string actionBy = string.Format("類別.{0}類別", EntityTitle);
                        //string action = string.Format("移除{0}類別", EntityTitle);
                        //StudentRecord student = Student.SelectByID(entityId);
                        //log.AddBatch(actionBy, action, "description:" + student.Name);
                    }
                }
            }

            if (addList.Count > 0)
                InsertTagRelations(addList);

            if (removeList.Count > 0)
                RemoveTagRelations(removeList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes">用於指定類別的 Code。</param>
        /// <param name="editCode">用於 SystemTag 的 Code。</param>
        /// <returns></returns>
        private bool Accept(string[] codes)
        {
            bool accept = false;

            foreach (string code in codes)
            {
                if (string.IsNullOrWhiteSpace(code)) continue;
                accept |= UserAcl.Current[code].Executable;
            }

            return accept;
        }

        private bool AcceptEdit(string editCode)
        {
            return UserAcl.Current[editCode].Editable;
        }

        private bool AcceptView(string vCode)
        {
            return UserAcl.Current[vCode].Viewable;
        }
    }
}
