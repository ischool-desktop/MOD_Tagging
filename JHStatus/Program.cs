using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using K12.Data;
using Customization.Tagging;

namespace JHStatus
{
    public static class Program
    {
        [MainMethod(StartupPriority.FirstAsynchronized)]
        public static void Main()
        {
            CustomizationService.Register<GetStudentStatusList>(() =>
            {
                List<StatusItem> items = new List<StatusItem>();

                items.Add(new StatusItem() { Status = StudentRecord.StudentStatus.一般, Text = "一般" });
                items.Add(new StatusItem() { Status = StudentRecord.StudentStatus.畢業或離校, Text = "畢業或離校" });
                items.Add(new StatusItem() { Status = StudentRecord.StudentStatus.休學, Text = "休學" });
                items.Add(new StatusItem() { Status = StudentRecord.StudentStatus.輟學, Text = "輟學" });
                items.Add(new StatusItem() { Status = StudentRecord.StudentStatus.刪除, Text = "刪除" });

                return items;
            });
        }
    }
}
