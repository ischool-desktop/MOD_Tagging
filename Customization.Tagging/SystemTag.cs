using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K12.Data;
using FISCA.Permission;

namespace Customization.Tagging
{
    /// <summary>
    /// 提供定義 SystemTag 的 API。
    /// </summary>
    public static class SystemTag
    {
        /// <summary>
        /// 定義系統類別，如果系統中不存在該類別，則會自動建立。
        /// </summary>
        /// <param name="catalog">類別分類，例：Student、Class、Teacher、Course</param>
        /// <param name="tagFullName">類別全名，例：一般生、輔導:輔導主任</param>
        /// <param name="color">類別顏色。</param>
        public static void Define(string catalog, string tagFullName, Color color)
        {
            Define(catalog, tagFullName, color, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// 定義系統類別，如果系統中不存在該類別，則會自動建立。
        /// </summary>
        /// <param name="catalog">類別分類，例：Student、Class、Teacher、Course</param>
        /// <param name="tagFullName">類別全名，例：一般生、輔導:輔導主任</param>
        /// <param name="color">類別顏色。</param>
        /// <param name="code">類別權限代碼，如果不指定代表是一般類別，無法進行權限設定。</param>
        /// <param name="title">權限畫面的描述，例：可為學生指定「一般生」類別。</param>
        /// <param name="path">權限畫面放置路徑，例：學生>系統類別</param>
        public static void Define(string catalog, string tagFullName, Color color, string code, string title, string path)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                try
                {
                    TagCategory tc = GetCategory(catalog);
                    string[] tagParts = Parse(tagFullName);

                    TagConfigRecord found = null; //如果有找到的話...

                    //如果不存在就新增。
                    if (!IsTagExists(tc, tagParts[0], tagFullName, out found))
                    {
                        TagConfigRecord tcr = new TagConfigRecord();
                        tcr.Prefix = tagParts[0];
                        tcr.Name = tagParts[1];
                        tcr.Category = catalog;
                        tcr.Color = color;
                        tcr.AccessControlCode = code;

                        TagConfig.Insert(tcr);
                    }
                    else
                    {
                        if (found.AccessControlCode != code)
                        {
                            found.AccessControlCode = code;
                            TagConfig.Update(found);
                        }
                    }

                    GetCatalog(path).Add(new DetailItemFeature(code, title));
                }
                catch (Exception ex)
                {
                    FISCA.RTOut.WriteError(ex);
                }
            });
        }

        private static bool IsTagExists(TagCategory tc, string prefix, string fn, out TagConfigRecord found)
        {
            List<TagConfigRecord> tags = TagConfig.SelectByCategoryAndPrefix(tc, prefix);

            //找找看指定的 Tag 是否存在。
            foreach (TagConfigRecord each in tags)
                if (each.FullName == fn)
                {
                    found = each;
                    return true;
                }

            found = null;
            return false;
        }

        private static TagCategory GetCategory(string catalog)
        {
            return (TagCategory)Enum.Parse(typeof(TagCategory), catalog);
        }

        private static string[] Parse(string tagFullName)
        {
            string[] tagParts = tagFullName.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (tagParts.Length <= 1)
                return new string[] { "", tagParts[0] };
            else
                return new string[] { tagParts[0], tagParts[1] };
        }

        private static Catalog GetCatalog(string path)
        {
            string[] parts = path.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);

            Catalog result = RoleAclSource.Instance.Root;
            foreach (string name in parts)
                result = result[name];

            return result;
        }
    }
}
