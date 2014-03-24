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
    internal class TeacherMenu : StudentMenu //直接繼承 Student 的，因為懶得另外寫 BaseClass....
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignCodes">指定類別的權限代碼。</param>
        /// <param name="manageCodes">管理類別的權限代碼。</param>
        internal TeacherMenu(RibbonBarItem menuContainer)
            : base(menuContainer)
        {
        }

        protected override void SetupPermissionCode()
        {
            AssignCodes = new string[] { "JHSchool.Teacher.Ribbon0040" };
            ManageCodes = new string[] { "JHSchool.Teacher.Ribbon0050" };
        }

        protected override void OpenTagConfigForm()
        {
            TagForm tf = new TagForm();
            tf.Category = TagCategory.Teacher;
            tf.EntityTitle = "教師";
            tf.EntityIdField = "Teacher";
            tf.CounterService = "SmartSchool.Tag.GetUseTeacherList";
            tf.ShowDialog();
        }

        protected override List<string> GetSelectedSource()
        {
            return K12.Presentation.NLDPanels.Teacher.SelectedSource;
        }

        protected override List<TagConfigRecord> GetTagConfigList()
        {
            return TagConfig.SelectByCategory(TagCategory.Teacher).Viewable().ToList();
        }

        protected override List<GeneralTagRecord> GetTagRelationByIDs(List<string> IDs)
        {
            return TeacherTag.SelectByTeacherIDs(IDs).ConvertAll(x => (GeneralTagRecord)x).Viewable().ToList();
        }

        protected override GeneralTagRecord NewTagRelationRecord(string entityId, string tagID)
        {
            return new TeacherTagRecord(entityId, tagID);
        }

        protected override void InsertTagRelations(List<GeneralTagRecord> records)
        {
            TeacherTag.Insert(records.ConvertAll(x => (TeacherTagRecord)x));
        }

        protected override void RemoveTagRelations(List<GeneralTagRecord> records)
        {
            TeacherTag.Delete(records.ConvertAll(x => (TeacherTagRecord)x));
        }
    }
}
