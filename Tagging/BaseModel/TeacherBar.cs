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
using FISCA;

namespace Tagging.BaseModel
{
    internal partial class TeacherBar : DescriptionPane
    {
        private BackgroundWorker TaggingWorker = new BackgroundWorker();

        private bool HasPadding = false;

        private List<GeneralTagRecord> CurrentTags;

        private LabelX DescriptionLabel;
        private PanelEx StatusPanel;
        private ContextMenuBar StatusMenuBar;

        /// <summary>
        /// 存放 Status MenuItem 的容器。
        /// </summary>
        private ButtonItem StatusContainer;
        /// <summary>
        /// 所有 Status 的項目。
        /// </summary>
        private List<ButtonItem> StatusList;

        public TeacherBar()
        {
            InitializeComponent();

            #region InitializeComponent Manual
            this.StatusMenuBar = new DevComponents.DotNetBar.ContextMenuBar();

            StatusList = new List<ButtonItem>();

            //依據設定動態建立功能表。
            foreach (string name in Enum.GetNames(typeof(TeacherRecord.TeacherStatus)))
            {
                ButtonItem item = new ButtonItem();

                item.AutoCheckOnClick = true;
                item.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
                item.ImagePaddingHorizontal = 8;
                item.OptionGroup = "status";
                item.Text = name;
                if (Permissions.變更教師狀態權限)
                {
                    item.CheckedChanged += new System.EventHandler(this.StatusMenu_CheckedChanged);
                    item.Enabled = true;
                }
                else
                {
                    item.Enabled = false;
                }
                StatusList.Add(item);
            }

            this.StatusContainer = new DevComponents.DotNetBar.ButtonItem();
            this.StatusPanel = new DevComponents.DotNetBar.PanelEx();
            ((System.ComponentModel.ISupportInitialize)(this.StatusMenuBar)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuBar1
            // 
            this.StatusMenuBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.StatusContainer});
            this.StatusMenuBar.Location = new System.Drawing.Point(36, 12);
            this.StatusMenuBar.Size = new System.Drawing.Size(123, 25);
            this.StatusMenuBar.Stretch = true;
            this.StatusMenuBar.TabIndex = 184;
            this.StatusMenuBar.TabStop = false;
            this.StatusMenuBar.Text = "StatusMenuBar";
            // 
            // buttonItem1
            // 
            this.StatusContainer.AutoExpandOnClick = true;
            this.StatusContainer.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
            this.StatusContainer.ImagePaddingHorizontal = 8;
            this.StatusContainer.SubItems.AddRange(StatusList.ToArray());
            this.StatusContainer.Text = "statusMenu";
            // 
            // panelEx1
            // 
            this.StatusPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.StatusPanel.CanvasColor = System.Drawing.SystemColors.Control;
            this.StatusPanel.Location = new System.Drawing.Point(66, 3);
            this.StatusPanel.Margin = new System.Windows.Forms.Padding(0);
            this.StatusPanel.Size = new System.Drawing.Size(95, 20);
            this.StatusPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.StatusPanel.Style.BackColor1.Color = System.Drawing.Color.LightBlue;
            this.StatusPanel.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.StatusPanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.StatusPanel.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.None;
            this.StatusPanel.Style.BorderWidth = 0;
            this.StatusPanel.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.StatusPanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.StatusPanel.Style.GradientAngle = 90;
            this.StatusPanel.Style.TextTrimming = System.Drawing.StringTrimming.Word;
            this.StatusPanel.TabIndex = 184;
            this.StatusPanel.Text = "一般";
            this.StatusPanel.Click += new System.EventHandler(this.StatusPanel_Click);
            //
            // DescriptionLabel
            //
            DescriptionLabel = new LabelX();
            DescriptionLabel.Text = string.Empty;
            DescriptionLabel.Dock = DockStyle.Left;
            DescriptionLabel.AutoSize = true;
            DescriptionLabel.Font = new Font(Font.FontFamily, 13);
            // 
            // StudentDescription
            // 
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            //this.AutoSize = true;
            this.Controls.Add(this.StatusMenuBar);
            this.Name = "StudentDescription";

            ((System.ComponentModel.ISupportInitialize)(this.StatusMenuBar)).EndInit();
            this.ResumeLayout(false);

            DescriptionPanel.Controls.Add(DescriptionLabel);
            DescriptionPanel.Controls.Add(StatusPanel);
            #endregion

            TaggingWorker.DoWork += (TaggingWorker_DoWork);
            TaggingWorker.RunWorkerCompleted += (TaggingWorker_RunWorkerCompleted);

            TagConfig.AfterInsert += TagRecordChangedEventHandler;
            TagConfig.AfterUpdate += TagRecordChangedEventHandler;
            TagConfig.AfterDelete += TagRecordChangedEventHandler;
            K12.Data.Teacher.AfterUpdate += Teacher_AfterUpdate;

            TeacherTag.AfterInsert += TagRecordChangedEventHandler;
            TeacherTag.AfterUpdate += TagRecordChangedEventHandler;
            TeacherTag.AfterDelete += TagRecordChangedEventHandler;

            Disposed += delegate
            {
                TeacherTag.AfterInsert -= TagRecordChangedEventHandler;
                TeacherTag.AfterUpdate -= TagRecordChangedEventHandler;
                TeacherTag.AfterDelete -= TagRecordChangedEventHandler;
                TagConfig.AfterInsert -= TagRecordChangedEventHandler;
                TagConfig.AfterUpdate -= TagRecordChangedEventHandler;
                TagConfig.AfterDelete -= TagRecordChangedEventHandler;
                K12.Data.Teacher.AfterUpdate -= Teacher_AfterUpdate;
            };

            FISCA.Features.TryRegister("教師.Tag.Reload", args =>
            {
                OnPrimaryKeyChanged(EventArgs.Empty);
            });
        }

        private void Teacher_AfterUpdate(object sender, DataChangedEventArgs e)
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
            List<TeacherTagRecord> stus;
            stus = K12.Data.TeacherTag.SelectByTeacherID(primaryKey);
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
            Teacher.RemoveByIDs(new string[] { primaryKey });
            TeacherRecord record = Teacher.SelectByID(primaryKey);
            return string.Format("{0}", record.Name);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            if (string.IsNullOrEmpty(PrimaryKey)) return;

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

            Teacher.RemoveByIDs(new string[] { PrimaryKey }); //把快取刪掉。
            TeacherRecord stu = Teacher.SelectByID(PrimaryKey);

            Invoke(new Action(() =>
            {
                DisplayStatus(stu);
            }));
        }

        private void DisplayStatus(TeacherRecord stu)
        {

            if (stu != null)
            {
                Color s;
                switch (stu.Status)
                {
                    case TeacherRecord.TeacherStatus.一般:
                        s = Color.FromArgb(255, 255, 255);
                        break;
                    case TeacherRecord.TeacherStatus.刪除:
                        s = Color.FromArgb(254, 128, 155);
                        break;
                    default:
                        s = Color.Transparent;
                        break;
                }

                StatusPanel.Text = stu.StatusStr;
                StatusPanel.Style.BackColor1.Color = s;
                StatusPanel.Style.BackColor2.Color = s;

                foreach (var item in StatusList)
                {
                    if (item.Text == stu.StatusStr)
                        item.Checked = true;
                    else
                        item.Checked = false;
                }
            }
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

        //private void EmptyItems()
        //{
        //    //會閃....
        //    //foreach (PanelEx each in TaggingContainer.Controls)
        //    //    each.Text = string.Empty;
        //}

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

        private void StatusMenu_CheckedChanged(object sender, EventArgs e)
        {
            var button = (DevComponents.DotNetBar.ButtonItem)sender;
            StatusCheckedChanged(button, PrimaryKey);
        }

        private void StatusCheckedChanged(ButtonItem button, string key)
        {
            if (button.Checked)
            {
                Teacher.RemoveByIDs(new string[] { key });
                TeacherRecord teacher = Teacher.SelectByID(key);

                if (teacher != null)
                {
                    if (button.Text != teacher.StatusStr)
                    {
                        try
                        {
                            string log = "";

                            if (string.IsNullOrEmpty(teacher.Nickname))
                            {
                                log = "教師「" + teacher.Name + "」狀態已由「";
                                log += teacher.StatusStr + "」變更為「" + button.Text + "」";
                            }
                            else
                            {
                                log = "教師「" + teacher.Name + "(" + teacher.Nickname + ")" + "」狀態已由「";
                                log += teacher.StatusStr + "」變更為「" + button.Text + "」";
                            }



                            teacher.Status = (TeacherRecord.TeacherStatus)Enum.Parse(typeof(TeacherRecord.TeacherStatus), button.Text);

                            K12.Data.Teacher.Update(teacher);
                            try
                            {
                                ArgDictionary args = new ArgDictionary();
                                args.Add("TeacherIDList", new string[] { teacher.ID });//參數。
                                FISCA.Features.Invoke("教師.CacheManager", args);
                            }
                            catch (Exception ex)
                            {
                                RTOut.WriteError(ex);
                            }

                            FISCA.LogAgent.ApplicationLog.Log("教師狀態", "變更", "teacher", teacher.ID, log);
                        }
                        catch (ArgumentException ex)
                        {
                            RTOut.WriteError(ex);
                            MessageBox.Show("目前無法移到刪除");
                        }
                        catch (Exception ex)
                        {
                            RTOut.WriteError(ex);
                            MotherForm.SetStatusBarMessage("變更狀態失敗。");
                            return;
                        }
                    }
                }
            }
        }

        private void StatusPanel_Click(object sender, EventArgs e)
        {
            StatusContainer.Popup(StatusPanel.PointToScreen(new Point(0, StatusPanel.Height)));
        }
    }
}
