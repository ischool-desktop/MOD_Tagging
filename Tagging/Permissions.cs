using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Permission;

namespace Tagging
{
    /// <summary>
    /// 代表目前使用者的相關權限資訊。
    /// </summary>
    public static class Permissions
    {
        public static string 匯出學生類別 { get { return "JHSchool.Student.Ribbon0300"; } }
        public static string 匯入學生類別 { get { return "JHSchool.Student.ImportExport.StudentTag.ImportStudentTag"; } }
        public static string 高中匯出學生類別 { get { return "Button0205"; } }
        public static string 高中匯入學生類別 { get { return "Button0285"; } }

        public static string 匯出班級類別 { get { return "JHSchool.Class.Ribbon0566"; } }
        public static string 匯入班級類別 { get { return "JHSchool.Class.Ribbon0566.55"; } }

        public static string 匯出教師類別 { get { return "JHSchool.Teacher.Ribbon0300"; } }
        public static string 匯入教師類別 { get { return "JHSchool.Teacher.Ribbon0300.55"; } }

        public static string 匯出課程類別 { get { return "JHSchool.Course.Ribbon0666"; } }
        public static string 匯入課程類別 { get { return "JHSchool.Course.Ribbon0666.55"; } }

        #region 學生

        public static bool 匯出學生類別權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[匯出學生類別].Executable ||
                    FISCA.Permission.UserAcl.Current[高中匯出學生類別].Executable;
            }
        }

        public static bool 匯入學生類別權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[匯入學生類別].Executable ||
                    FISCA.Permission.UserAcl.Current[高中匯入學生類別].Executable;
            }
        }

        #endregion

        #region 班級

        public static bool 匯出班級類別權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯出班級類別].Executable; }
        }

        public static bool 匯入班級類別權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯入班級類別].Executable; }
        }

        #endregion

        #region 教師

        public static bool 匯出教師類別權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯出教師類別].Executable; }
        }

        public static bool 匯入教師類別權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯入教師類別].Executable; }
        }

        #endregion

        #region 課程

        public static bool 匯出課程類別權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯出課程類別].Executable; }
        }

        public static bool 匯入課程類別權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯入課程類別].Executable; }
        }
        #endregion

        #region 是否有類別變更權限

        public static string 變更學生狀態 { get { return "JHSchool.Student.Ribbon04150.Change20130315"; } }
        public static bool 變更學生狀態權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[變更學生狀態].Executable;
            }
        }

        public static string 變更教師狀態 { get { return "JHSchool.Teacher.Ribbon04150.Change20130315"; } }
        public static bool 變更教師狀態權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[變更教師狀態].Executable;
            }
        }

        //public static string 變更班級類別 { get { return "JHSchool.Class.Ribbon04150.Change20130315"; } }
        //public static bool 變更班級類別權限
        //{
        //    get
        //    {
        //        return FISCA.Permission.UserAcl.Current[變更班級類別].Executable;
        //    }
        //}

        //public static string 變更課程類別 { get { return "JHSchool.Course.Ribbon04150.Change20130315"; } }
        //public static bool 變更課程類別權限
        //{
        //    get
        //    {
        //        return FISCA.Permission.UserAcl.Current[變更課程類別].Executable;
        //    }
        //}

        #endregion

        #region 暫時註解

        //public static string 清空學生類別 { get { return "JHSchool.Student.ListPane0300.88"; } }
        //public static string 清空班級類別 { get { return "JHSchool.Class.ListPane0566.88"; } }
        //public static string 清空教師類別 { get { return "JHSchool.Teacher.ListPane0300.88"; } }
        //public static string 清空課程類別 { get { return "JHSchool.Course.ListPane0666.88"; } }

        //public static bool 清空學生類別權限
        //{
        //    get { return FISCA.Permission.UserAcl.Current[清空學生類別].Executable; }
        //}

        //public static bool 清空班級類別權限
        //{
        //    get { return FISCA.Permission.UserAcl.Current[清空班級類別].Executable; }
        //}

        //public static bool 清空教師類別權限
        //{
        //    get { return FISCA.Permission.UserAcl.Current[清空教師類別].Executable; }
        //}  

        //public static bool  清空課程類別權限
        //{
        //    get { return FISCA.Permission.UserAcl.Current[清空課程類別].Executable; }
        //} 

        #endregion
    }
}
