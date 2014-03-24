using System;
using FISCA.Presentation;
using K12.Data;
using DevComponents.DotNetBar;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Customization.Tagging;
using System.Collections;
using System.Linq;

namespace Tagging.BaseModel
{
    internal partial class ClassBar : DescriptionPane
    {
        private BackgroundWorker TaggingWorker = new BackgroundWorker();

        private bool HasPadding = false;

        private List<GeneralTagRecord> CurrentTags;

        private LabelX DescriptionLabel;

        public ClassBar()
        {
            InitializeComponent();

            #region InitializeComponent Manual
            //
            // DescriptionLabel
            //
            DescriptionLabel = new LabelX();
            DescriptionLabel.Text = string.Empty;
            DescriptionLabel.Dock = DockStyle.Left;
            DescriptionLabel.AutoSize = true;
            DescriptionLabel.Font = new Font(Font.FontFamily, 13);
            this.Name = "StudentDescription";

            this.ResumeLayout(false);

            DescriptionPanel.Controls.Add(DescriptionLabel);
            #endregion

            TaggingWorker.DoWork += (TaggingWorker_DoWork);
            TaggingWorker.RunWorkerCompleted += (TaggingWorker_RunWorkerCompleted);

            TagConfig.AfterInsert += TagRecordChangedEventHandler;
            TagConfig.AfterUpdate += TagRecordChangedEventHandler;
            TagConfig.AfterDelete += TagRecordChangedEventHandler;
            K12.Data.Class.AfterUpdate += Class_AfterUpdate;

            ClassTag.AfterInsert += TagRecordChangedEventHandler;
            ClassTag.AfterUpdate += TagRecordChangedEventHandler;
            ClassTag.AfterDelete += TagRecordChangedEventHandler;

            Disposed += delegate
            {
                ClassTag.AfterInsert -= TagRecordChangedEventHandler;
                ClassTag.AfterUpdate -= TagRecordChangedEventHandler;
                ClassTag.AfterDelete -= TagRecordChangedEventHandler;
                TagConfig.AfterInsert -= TagRecordChangedEventHandler;
                TagConfig.AfterUpdate -= TagRecordChangedEventHandler;
                TagConfig.AfterDelete -= TagRecordChangedEventHandler;
                K12.Data.Class.AfterUpdate -= Class_AfterUpdate;
            };
        }

        private void Class_AfterUpdate(object sender, DataChangedEventArgs e)
        {
            if (e.PrimaryKeys.Contains(PrimaryKey))
            {
                if (this.InvokeRequired)
                {
                    BeginInvoke(new Action(() => OnPrimaryKeyChanged(EventArgs.Empty)));
                }
                else
                    OnPrimaryKeyChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 取得某個 PrimaryKey 的 Tag 清單。
        /// </summary>
        public List<GeneralTagRecord> SelectTags(string primaryKey)
        {
            List<ClassTagRecord> stus;
            stus = K12.Data.ClassTag.SelectByClassID(primaryKey);
            return stus.ConvertAll<GeneralTagRecord>(x => x).Viewable().ToList();
        }

        public void TagRecordChangedEventHandler(object sender, DataChangedEventArgs args)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => OnPrimaryKeyChanged(EventArgs.Empty)));
            }
            else
                OnPrimaryKeyChanged(EventArgs.Empty);
        }

        public string GetDescriptionDelegate(string primaryKey)
        {
            Class.RemoveByIDs(new string[] { primaryKey });
            ClassRecord cls = Class.SelectByID(primaryKey);

            string desc = cls.Name;
            if (cls.Teacher != null)
            {
                if (cls.Teacher.Status == TeacherRecord.TeacherStatus.一般)
                    desc = string.Format("{0} (導師：{1})", cls.Name, cls.Teacher.Name);
            }

            return desc;
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            if (string.IsNullOrEmpty(PrimaryKey)) return;

            EmptyItems();

            if (TaggingWorker.IsBusy)
                HasPadding = true;
            else
                TaggingWorker.RunWorkerAsync();
        }

        private void TaggingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CurrentTags = SelectTags(PrimaryKey);

            //以下還要改寫。
            string desc = GetDescriptionDelegate(PrimaryKey);

            Invoke(new Action(() =>
            {
                DescriptionLabel.Text = desc;
            }));
        }

        private void TaggingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (HasPadding)
                TaggingWorker.RunWorkerAsync();
            else
            {
                if (e.Error != null)
                    FISCA.RTOut.WriteError(e.Error);
                else
                {
                    ClearItems();

                    foreach (GeneralTagRecord tag in CurrentTags)
                        AddItem(CreateItem(tag.FullName, tag.Color));
                    AverageItemsSzie();
                }
            }
            HasPadding = false;
        }

        private void EmptyItems()
        {
            //會閃....
            //foreach (PanelEx each in TaggingContainer.Controls)
            //    each.Text = string.Empty;
        }

        private void ClearItems()
        {
            foreach (Control ctl in TaggingContainer.Controls)
                Tip.SetSuperTooltip(ctl, null);

            TaggingContainer.Controls.Clear();
        }

        private void AddItem(PanelEx panel)
        {
            TaggingContainer.Controls.Add(panel);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent != null)
                Parent.SizeChanged += delegate //當 Parent 的大小改變時，要重新計算控制項大小。
                {
                    AverageItemsSzie();
                };
        }

        private PanelEx CreateItem(string title, Color color)
        {
            PanelEx objPanel = new PanelEx();

            objPanel.Margin = new System.Windows.Forms.Padding(2);
            objPanel.CanvasColor = System.Drawing.SystemColors.Control;
            objPanel.ColorSchemeStyle = eDotNetBarStyle.Office2010;
            objPanel.Size = new System.Drawing.Size(91, 20);
            objPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
            objPanel.Style.BorderColor.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.BorderColor.Color = Color.LightBlue;
            objPanel.Style.BackColor2.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.BackColor2.Color = color;
            objPanel.Style.BackColor1.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.BackColor1.Color = Color.White;
            objPanel.Style.ForeColor.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.ForeColor.Color = Color.Black;
            objPanel.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            objPanel.Style.CornerDiameter = 3;
            objPanel.Style.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal;
            objPanel.Style.GradientAngle = 90;
            objPanel.Style.TextTrimming = System.Drawing.StringTrimming.None;
            objPanel.Text = title;
            Tip.DefaultFont = Font;
            Tip.SetSuperTooltip(objPanel, new SuperTooltipInfo("", "", title, null, null, eTooltipColor.Office2003));

            return objPanel;
        }

        /// <summary>
        /// 計算 Tag 的大小，使其剛好填滿 Tag Panel。
        /// </summary>
        private void AverageItemsSzie()
        {
            List<PanelEx> tags = new List<PanelEx>();
            foreach (PanelEx panel in TaggingContainer.Controls)
                tags.Add(panel);

            if (Parent == null || tags.Count <= 0) return;

            //TableLayout.Width = Parent.Width;
            //Width = Parent.Width;

            int partsize = TaggingContainer.Width / tags.Count;

            foreach (PanelEx each in tags)
                each.Size = new Size(partsize - 4, each.Size.Height);
        }
    }
}
