using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using Customization.Tagging;
using System.Drawing;

namespace TestTagging
{
    public static class Program
    {
        [MainMethod(StartupPriority.FirstAsynchronized)]
        public static void Main()
        {
            SystemTag.Define("Student", "學生身份類別:資優生", Color.Red, "Code001", "可指定「學生身份類別:資優生」", "學生>系統類別");
            SystemTag.Define("Student", "學籍紫色", Color.Red, "Code002", "可指定「學籍紫色」", "學生>系統類別");
            SystemTag.Define("Student", "學生身份類別:港澳生", Color.Red, "Code003", "可指定「學生身份類別:港澳生」", "學生>系統類別");
            SystemTag.Define("Student", "學生身份類別:功勳子女", Color.Red, "Code004", "可指定「學生身份類別:功勳子女」", "學生>系統類別");
            SystemTag.Define("Student", "學生身份類別:我的類別", Color.Red, "Code005", "可指定「學生身份類別:我的類別」", "學生>系統類別");
            SystemTag.Define("Class", "超級班", Color.Red, "ClassTag001", "可指定「超級班」", "班級>系統類別");
            SystemTag.Define("Teacher", "超級師", Color.Red, "TeacherTag00X", "可指定「超級師」", "教師>系統類別");
            SystemTag.Define("Course", "超級課", Color.Red, "CourseTag001",  "可指定「超級課」", "課程>系統類別");

            //CustomizationService.Register<GetStudentStatusList>(() =>
            //{
            //    List<StatusItem> status = new List<StatusItem>();
            //    status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.一般, Text = "A狀態" });
            //    status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.休學, Text = "B狀態" });
            //    status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.刪除, Text = "C狀態" });

            //    return status;
            //});
        }
    }
}
