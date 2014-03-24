using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;
using FISCA;

namespace Customization.Tagging
{
    /// <summary>
    /// 
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        [MainMethod("Customization.Tagging")]
        public static void Main()
        {
            //Cache.TagACC = new ObjectCache();
        }
    }

    /// <summary>
    /// 代表學生狀態的內部代碼與文字。
    /// </summary>
    public struct StatusItem
    {
        /// <summary>
        /// 代碼。
        /// </summary>
        public StudentRecord.StudentStatus Status { get; set; }

        /// <summary>
        /// 顯示文字。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(StatusItem x, StatusItem y)
        {
            return x.Status == y.Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(StatusItem x, StatusItem y)
        {
            return x.Status != y.Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 提供「學生狀態」清單的自定機會。
    /// </summary>
    /// <returns></returns>
    public delegate List<StatusItem> GetStudentStatusList();
}
