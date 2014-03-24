using System;
using System.Collections.Generic;
using FISCA.Presentation;
using K12.Data;
using FISCA.Permission;
using FISCA.LogAgent;
using FISCA.Authentication;
using System.Linq;
using K12.Presentation;
using Customization.Tagging;

namespace Tagging.BaseModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ClassMenu : StudentMenu //直接繼承 Student 的，因為懶得另外寫 BaseClass....
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignCodes">指定類別的權限代碼。</param>
        /// <param name="manageCodes">管理類別的權限代碼。</param>
        internal ClassMenu(RibbonBarItem menuContainer)
            : base(menuContainer)
        {
        }

        protected override void SetupPermissionCode()
        {
            AssignCodes = new string[] { "JHSchool.Class.Ribbon0040" };
            ManageCodes = new string[] { "JHSchool.Class.Ribbon0050" };
        }

        protected override void OpenTagConfigForm()
        {
            TagForm tf = new TagForm();
            tf.Category = TagCategory.Class;
            tf.EntityTitle = "班級";
            tf.EntityIdField = "Class";
            tf.CounterService = "SmartSchool.Tag.GetUseClassList";
            tf.ShowDialog();
        }

        protected override List<string> GetSelectedSource()
        {
            return K12.Presentation.NLDPanels.Class.SelectedSource;
        }

        protected override List<TagConfigRecord> GetTagConfigList()
        {
            return TagConfig.SelectByCategory(TagCategory.Class).Viewable().ToList();
        }

        protected override List<GeneralTagRecord> GetTagRelationByIDs(List<string> IDs)
        {
            return ClassTag.SelectByClassIDs(IDs).ConvertAll(x => (GeneralTagRecord)x).Viewable().ToList();
        }

        protected override GeneralTagRecord NewTagRelationRecord(string entityId, string tagID)
        {
            return new ClassTagRecord(entityId, tagID);
        }

        protected override void InsertTagRelations(List<GeneralTagRecord> records)
        {
            ClassTag.Insert(records.ConvertAll(x => (ClassTagRecord)x));
        }

        protected override void RemoveTagRelations(List<GeneralTagRecord> records)
        {
            ClassTag.Delete(records.ConvertAll(x => (ClassTagRecord)x));
        }
    }
}
