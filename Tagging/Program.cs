using FISCA;
using FISCA.Presentation;
using K12.Data;
using K12.Presentation;
using System.Collections.Generic;
using System;
using FISCA.Permission;
using System.Linq;
using FISCA.Presentation.Controls;
using Customization.Tagging;

namespace Tagging
{
    /// <summary>
    /// 
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        [MainMethod()]
        [Dependency("Customization.Tagging")]
        public static void Main()
        {
            #region RibbonBar 學生
            new Tagging.BaseModel.StudentMenu(K12.Presentation.NLDPanels.Student.RibbonBarItems["指定"]);

            //指定 DescriptionPane。
            NLDPanels.Student.SetDescriptionPaneBulider(new Tagging.BaseModel.StudentBarBuilder());

            //新增 View。
            NLDPanels.Student.AddView(new Tagging.BaseModel.StudentView());
            #endregion

            #region RibbonBar 班級
            new Tagging.BaseModel.ClassMenu(K12.Presentation.NLDPanels.Class.RibbonBarItems["指定"]);

            //指定 DescriptionPane。
            NLDPanels.Class.SetDescriptionPaneBulider(new Tagging.BaseModel.ClassBarBuilder());

            //新增 View。
            NLDPanels.Class.AddView(new Tagging.BaseModel.ClassView());
            #endregion

            #region RibbonBar 教師
            new Tagging.BaseModel.TeacherMenu(K12.Presentation.NLDPanels.Teacher.RibbonBarItems["指定"]);

            //指定 DescriptionPane。
            NLDPanels.Teacher.SetDescriptionPaneBulider(new Tagging.BaseModel.TeacherBarBuilder());

            //新增 View。
            NLDPanels.Teacher.AddView(new Tagging.BaseModel.TeacherView());
            #endregion

            #region RibbonBar 課程
            new Tagging.BaseModel.CourseMenu(K12.Presentation.NLDPanels.Course.RibbonBarItems["指定"]);

            //指定 DescriptionPane。
            NLDPanels.Course.SetDescriptionPaneBulider(new Tagging.BaseModel.CourseBarBuilder());

            //新增 View。
            NLDPanels.Course.AddView(new Tagging.BaseModel.CourseView());
            #endregion

            #region 基本功能權限註冊
            RoleAclSource.Instance["學生"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Student.Ribbon0040", "指定學生類別"));
            RoleAclSource.Instance["學生"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Student.Ribbon0050", "管理學生類別清單"));

            RoleAclSource.Instance["班級"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Class.Ribbon0040", "指定班級類別"));
            RoleAclSource.Instance["班級"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Class.Ribbon0050", "管理班級類別清單"));

            RoleAclSource.Instance["教師"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Teacher.Ribbon0040", "指定教師類別"));
            RoleAclSource.Instance["教師"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Teacher.Ribbon0050", "管理教師類別清單"));

            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Course.Ribbon0040", "指定課程類別"));
            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("JHSchool.Course.Ribbon0050", "管理課程類別清單"));
            #endregion

            #region 學生類別匯出匯入
            RibbonBarItem student_ribb = MotherForm.RibbonBarItems["學生", "資料統計"];

            student_ribb["匯出"].Image = Properties.Resources.Export_Image;
            student_ribb["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            student_ribb["匯出"]["學籍相關匯出"]["匯出學生類別"].Enable = Permissions.匯出學生類別權限;
            student_ribb["匯出"]["學籍相關匯出"]["匯出學生類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new Tagging.ExportStudentTag();
                ExportStudentV2 wizard = new ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            student_ribb["匯入"].Image = Properties.Resources.Import_Image;
            student_ribb["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            student_ribb["匯入"]["學籍相關匯入"]["匯入學生類別"].Enable = Permissions.匯入學生類別權限;
            student_ribb["匯入"]["學籍相關匯入"]["匯入學生類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer Importer = new ImportStudentTag();
                ImportStudentV2 wizard = new ImportStudentV2(Importer.Text, Importer.Image);
                Importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            #endregion

            #region 班級類別匯出匯入
            RibbonBarItem Class_ribb = MotherForm.RibbonBarItems["班級", "資料統計"];

            Class_ribb["匯出"].Image = Properties.Resources.Export_Image;
            Class_ribb["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            Class_ribb["匯出"]["匯出班級類別"].Enable = Permissions.匯出班級類別權限;
            Class_ribb["匯出"]["匯出班級類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new Tagging.ExportClassTag();
                ExportClassV2 wizard = new ExportClassV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();

            };

            Class_ribb["匯入"].Image = Properties.Resources.Import_Image;
            Class_ribb["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            Class_ribb["匯入"]["匯入班級類別"].Enable = Permissions.匯入班級類別權限;
            Class_ribb["匯入"]["匯入班級類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer Importer = new ImportClassTag();
                ImportClassV2 wizard = new ImportClassV2(Importer.Text, Importer.Image);
                Importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            #endregion

            #region 教師類別匯出匯入

            RibbonBarItem teacher_ribb = MotherForm.RibbonBarItems["教師", "資料統計"];

            teacher_ribb["匯出"].Image = Properties.Resources.Export_Image;
            teacher_ribb["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            teacher_ribb["匯出"]["匯出教師類別"].Enable = Permissions.匯出教師類別權限;
            teacher_ribb["匯出"]["匯出教師類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new Tagging.ExportTeacherTag();
                ExportTeacherV2 wizard = new ExportTeacherV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();

            };

            teacher_ribb["匯入"].Image = Properties.Resources.Import_Image;
            teacher_ribb["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            teacher_ribb["匯入"]["匯入教師類別"].Enable = Permissions.匯入教師類別權限;
            teacher_ribb["匯入"]["匯入教師類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer Importer = new ImportTeacherTag();
                ImportTeacherV2 wizard = new ImportTeacherV2(Importer.Text, Importer.Image);
                Importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            #endregion

            #region 課程類別匯出匯入
            RibbonBarItem Course_ribb = MotherForm.RibbonBarItems["課程", "資料統計"];

            Course_ribb["匯出"].Image = Properties.Resources.Export_Image;
            Course_ribb["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            Course_ribb["匯出"]["匯出課程類別"].Enable = Permissions.匯出課程類別權限;
            Course_ribb["匯出"]["匯出課程類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new Tagging.ExportCourseTag();
                ExportCourseV2 wizard = new ExportCourseV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            Course_ribb["匯入"].Image = Properties.Resources.Import_Image;
            Course_ribb["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            Course_ribb["匯入"]["匯入課程類別"].Enable = Permissions.匯入課程類別權限;
            Course_ribb["匯入"]["匯入課程類別"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer Importer = new ImportCourseTag();
                ImportCourseV2 wizard = new ImportCourseV2(Importer.Text, Importer.Image);
                Importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            #endregion

            #region 匯入/匯出權限註冊部份

            Catalog ButtonRoleAcl = RoleAclSource.Instance["學生"]["匯出/匯入"];
            ButtonRoleAcl.Add(new RibbonFeature(Permissions.匯出學生類別, "匯出學生類別"));
            ButtonRoleAcl.Add(new RibbonFeature(Permissions.匯入學生類別, "匯入學生類別"));

            Catalog ButtonRoleAc3 = RoleAclSource.Instance["班級"]["匯出/匯入"];
            ButtonRoleAc3.Add(new RibbonFeature(Permissions.匯出班級類別, "匯出班級類別"));
            ButtonRoleAc3.Add(new RibbonFeature(Permissions.匯入班級類別, "匯入班級類別"));

            Catalog ButtonRoleAc5 = RoleAclSource.Instance["教師"]["匯出/匯入"];
            ButtonRoleAc5.Add(new RibbonFeature(Permissions.匯出教師類別, "匯出教師類別"));
            ButtonRoleAc5.Add(new RibbonFeature(Permissions.匯入教師類別, "匯入教師類別"));

            Catalog ButtonRoleAc7 = RoleAclSource.Instance["課程"]["匯出/匯入"];
            ButtonRoleAc7.Add(new RibbonFeature(Permissions.匯出課程類別, "匯出課程類別"));
            ButtonRoleAc7.Add(new RibbonFeature(Permissions.匯入課程類別, "匯入課程類別"));

            Catalog ButtonRoleAc01 = RoleAclSource.Instance["學生"]["功能按鈕"];
            ButtonRoleAc01.Add(new RibbonFeature(Permissions.變更學生狀態, "變更學生狀態"));

            Catalog ButtonRoleAc02 = RoleAclSource.Instance["教師"]["功能按鈕"];
            ButtonRoleAc02.Add(new RibbonFeature(Permissions.變更教師狀態, "變更教師狀態"));

            //ButtonRoleAc01.Add(new RibbonFeature(Permissions.變更班級類別, "變更班級類別"));
            //ButtonRoleAc01.Add(new RibbonFeature(Permissions.變更課程類別, "變更課程類別"));
            #endregion

            #region 清空類別功能(註解)
            //暫不使用之功能
            //NLDPanels.Student.ListPaneContexMenu["清空學生類別"].Enable = Permissions.清空學生類別權限;
            //NLDPanels.Student.ListPaneContexMenu["清空學生類別"].BeginGroup = true;
            //NLDPanels.Student.ListPaneContexMenu["清空學生類別"].Click += delegate
            //{
            //    List<StudentTagRecord> list = StudentTag.SelectByStudentIDs(NLDPanels.Student.SelectedSource);
            //    StudentTag.Delete(list);
            //};

            //暫不使用之功能
            //NLDPanels.Class.ListPaneContexMenu["清空班級類別"].Enable = Permissions.清空班級類別權限;
            //NLDPanels.Class.ListPaneContexMenu["清空班級類別"].BeginGroup = true;
            //NLDPanels.Class.ListPaneContexMenu["清空班級類別"].Click += delegate
            //{
            //    List<ClassTagRecord> list = ClassTag.SelectByClassIDs(NLDPanels.Class.SelectedSource);
            //    ClassTag.Delete(list);
            //};

            //暫不使用之功能
            //NLDPanels.Teacher.ListPaneContexMenu["清空教師類別"].Enable = Permissions.清空教師類別權限;
            //NLDPanels.Teacher.ListPaneContexMenu["清空教師類別"].BeginGroup = true;
            //NLDPanels.Teacher.ListPaneContexMenu["清空教師類別"].Click += delegate
            //{
            //    List<TeacherTagRecord> list = TeacherTag.SelectByTeacherIDs(NLDPanels.Teacher.SelectedSource);
            //    TeacherTag.Delete(list);
            //};

            ////暫不使用之功能
            //NLDPanels.Course.ListPaneContexMenu["清空課程類別"].Enable = Permissions.清空課程類別權限;
            //NLDPanels.Course.ListPaneContexMenu["清空課程類別"].BeginGroup = true;
            //NLDPanels.Course.ListPaneContexMenu["清空課程類別"].Click += delegate
            //{
            //    List<CourseTagRecord> list = CourseTag.SelectByCourseIDs(NLDPanels.Course.SelectedSource);
            //    CourseTag.Delete(list);
            //};

            //課程選取變更時...
            //NLDPanels.Course.SelectedSourceChanged += delegate
            //{
            //    NLDPanels.Course.ListPaneContexMenu["清空課程類別"].Enable = Permissions.清空課程類別權限 && (NLDPanels.Course.SelectedSource.Count > 0);
            //};

            //Catalog ButtonRoleAc2 = RoleAclSource.Instance["學生"]["資料清單"];
            //ButtonRoleAc2.Add(new RibbonFeature(Permissions.清空學生類別, "清空學生類別"));

            //Catalog ButtonRoleAc4 = RoleAclSource.Instance["班級"]["資料清單"];
            //ButtonRoleAc4.Add(new RibbonFeature(Permissions.清空班級類別, "清空班級類別"));

            //Catalog ButtonRoleAc6 = RoleAclSource.Instance["教師"]["資料清單"];
            //ButtonRoleAc6.Add(new RibbonFeature(Permissions.清空教師類別, "清空教師類別"));

            //Catalog ButtonRoleAc8 = RoleAclSource.Instance["課程"]["資料清單"];
            //ButtonRoleAc8.Add(new RibbonFeature(Permissions.清空課程類別, "清空課程類別")); 
            #endregion
        }

        #region Customization Status List
        private static List<StatusItem> _status_list = null;

        public static List<StatusItem> StatusList
        {
            get
            {
                if (_status_list != null)
                    return _status_list;

                _status_list = new List<StatusItem>();
                GetStudentStatusList getter = CustomizationService.Discover<GetStudentStatusList>();
                if (getter == null)
                {
                    StatusList.Add(new StatusItem() { Status = StudentRecord.StudentStatus.一般, Text = "一般" });
                    StatusList.Add(new StatusItem() { Status = StudentRecord.StudentStatus.休學, Text = "休學" });
                    StatusList.Add(new StatusItem() { Status = StudentRecord.StudentStatus.輟學, Text = "輟學" });
                    StatusList.Add(new StatusItem() { Status = StudentRecord.StudentStatus.畢業或離校, Text = "畢業或離校" });
                }
                else
                    _status_list = getter();

                return _status_list;
            }
        }
        #endregion

    }
}
