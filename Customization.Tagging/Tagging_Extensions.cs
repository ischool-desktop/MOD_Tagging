using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Permission;
using K12.Data;

namespace Customization.Tagging
{
    /// <summary>
    /// 
    /// </summary>
    public static class Tagging_Extensions
    {
        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<TagConfigRecord> Viewable(this IEnumerable<TagConfigRecord> records)
        {
            HashSet<TagConfigRecord> accepts = new HashSet<TagConfigRecord>();

            foreach (TagConfigRecord record in records)
            {
                if (UserAcl.Current[record.AccessControlCode].Viewable || string.IsNullOrWhiteSpace(record.AccessControlCode))
                    accepts.Add(record);
            }

            return accepts;
        }

        /// <summary>
        /// 取得具有編輯權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<TagConfigRecord> Editable(this IEnumerable<TagConfigRecord> records)
        {
            HashSet<TagConfigRecord> accepts = new HashSet<TagConfigRecord>();

            foreach (TagConfigRecord record in records)
            {
                if (UserAcl.Current[record.AccessControlCode].Editable || string.IsNullOrWhiteSpace(record.AccessControlCode))
                    accepts.Add(record);
            }

            return accepts;
        }

        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<GeneralTagRecord> Viewable(this IEnumerable<GeneralTagRecord> records)
        {
            HashSet<GeneralTagRecord> accepts = new HashSet<GeneralTagRecord>();
            Dictionary<string, TagConfigRecord> map = TagConfig.SelectAll().ToDictionary(x => x.ID);

            foreach (GeneralTagRecord record in records)
            {
                if (string.IsNullOrWhiteSpace(record.RefTagID)) continue; //防止爆炸。

                if (map.ContainsKey(record.RefTagID))
                {
                    string acc = map[record.RefTagID].AccessControlCode;
                    if (UserAcl.Current[acc].Viewable || string.IsNullOrWhiteSpace(acc))
                        accepts.Add(record);
                }
            }

            return accepts;
        }

        /// <summary>
        /// 取得具有編輯權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<GeneralTagRecord> Editable(this IEnumerable<GeneralTagRecord> records)
        {
            HashSet<GeneralTagRecord> accepts = new HashSet<GeneralTagRecord>();
            Dictionary<string, TagConfigRecord> map = TagConfig.SelectAll().ToDictionary(x => x.ID);

            foreach (GeneralTagRecord record in records)
            {
                if (string.IsNullOrWhiteSpace(record.RefTagID)) continue; //防止爆炸。

                if (map.ContainsKey(record.RefTagID))
                {
                    string acc = map[record.RefTagID].AccessControlCode;
                    if (UserAcl.Current[acc].Editable || string.IsNullOrWhiteSpace(acc))
                        accepts.Add(record);
                }
            }

            return accepts;
        }

        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<StudentTagRecord> Viewable(this IEnumerable<StudentTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Viewable()).ConvertAll(x => (StudentTagRecord)x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<ClassTagRecord> Viewable(this IEnumerable<ClassTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Viewable()).ConvertAll(x => (ClassTagRecord)x);
        }

        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<TeacherTagRecord> Viewable(this IEnumerable<TeacherTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Viewable()).ConvertAll(x => (TeacherTagRecord)x);
        }

        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<CourseTagRecord> Viewable(this IEnumerable<CourseTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Viewable()).ConvertAll(x => (CourseTagRecord)x);
        }

        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<StudentTagRecord> Editable(this IEnumerable<StudentTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Viewable()).ConvertAll(x => (StudentTagRecord)x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<ClassTagRecord> Editable(this IEnumerable<ClassTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Editable()).ConvertAll(x => (ClassTagRecord)x);
        }

        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<TeacherTagRecord> Editable(this IEnumerable<TeacherTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Editable()).ConvertAll(x => (TeacherTagRecord)x);
        }

        /// <summary>
        /// 取得具有檢視權限的清單。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static IEnumerable<CourseTagRecord> Editable(this IEnumerable<CourseTagRecord> records)
        {
            IEnumerable<GeneralTagRecord> gen = records;
            return new List<GeneralTagRecord>(gen.Editable()).ConvertAll(x => (CourseTagRecord)x);
        }
    }
}
