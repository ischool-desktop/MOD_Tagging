using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.AdvTree;
using FISCA.Presentation;
using System.Collections.Generic;
using System.Linq;

namespace Tagging.ViewHelper
{
    /// <summary>
    /// 提供 NavView 的基本功能。
    /// </summary>
    public partial class TreeNavViewBase : NavView
    {
        private Node Loading { get; set; }

        private KeyCatalogFactory KCFactory { get; set; }

        /// <summary>
        /// 是否顯示根項目。
        /// </summary>
        protected bool ShowRoot { get; set; }

        /// <summary>
        /// 根項目標題。
        /// </summary>
        protected string RootCaption { get; set; }

        protected TaskScheduler UISyncContext { get; set; }

        /// <summary>
        /// 建立年班檢視。
        /// </summary>
        public TreeNavViewBase()
        {
            InitializeComponent();

            NameComparer = StringComparer.CurrentCultureIgnoreCase;
            KCFactory = new KeyCatalogFactory() { NameSorter = KeyCatalogComparer, ToStringFormatter = KeyCatalogTitleFormat };
            ShowRoot = true;
            RootCaption = "所有項目";
            Loading = new Node("讀取中...");
            UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();

            //當 Student.SetFilteredSource 被呼叫時。
            SourceChanged += new EventHandler(StudentGradeClassView_SourceChanged);
        }

        #region KeyCatalog 排序處理
        /// <summary>
        /// 排序 KeyCatalog。
        /// </summary>
        private StringComparer NameComparer { get; set; }

        /// <summary>
        /// 排序 KeyCatalog，如果不改寫則使用 Name 屬性排序。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int KeyCatalogComparer(KeyCatalog x, KeyCatalog y)
        {
            return NameComparer.Compare(x.Name, y.Name);
        }

        /// <summary>
        /// 提供 KeyCatalog 標題的格式化方法。
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        protected virtual string KeyCatalogTitleFormat(KeyCatalog catalog)
        {
            return string.Format("{0}({1})", catalog.Name, catalog.TotalUniqueKeyCount);
        }
        #endregion

        private void StudentGradeClassView_SourceChanged(object sender, EventArgs e)
        {
            RenderTreeView(true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RenderTreeView()
        {
            RenderTreeView(true);
        }

        /// <summary>
        /// 產生資料到畫面上，請透過 GenerateDataModel 產生資料結構。
        /// </summary>
        protected void RenderTreeView(bool reserveSelection)
        {
            if (reserveSelection)
                ReserveTreeSelection();

            KeyCatalog userroot = new KeyCatalog(RootCaption, KCFactory);
            Task task = Task.Factory.StartNew((x) =>
            {
                GenerateTreeStruct(x as KeyCatalog);
            }, userroot);

            task.ContinueWith(x =>
            {
                ATree.Nodes.Clear();
                KeyCatalog rkc = x.AsyncState as KeyCatalog;

                if (x.Exception != null)
                {
                    KeyNode kn = new KeyNode("請重新整理資料");
                    kn.Catalog = new KeyCatalog("請重新整理資料", null);
                    ATree.Nodes.Add(kn);
                }
                else
                {
                    if (ShowRoot)
                    {
                        KeyCatalog root = new KeyCatalog("", KCFactory);
                        root.Subcatalogs.Add(rkc);
                        RenderNodes(root, ATree.Nodes, RestoreLevel);
                    }
                    else
                        RenderNodes(rkc, ATree.Nodes, RestoreLevel);

                    //第一層節點會都打開。
                    foreach (Node n in ATree.Nodes)
                        n.Expand();

                    RestoreExpandedNodes();
                }
            }, UISyncContext);
        }

        /// <summary>
        /// 顯示「載入中...」節點。
        /// </summary>
        public void ShowLoading()
        {
            ReserveTreeSelection();
            ATree.Nodes.Clear();
            ATree.Nodes.Add(Loading);
        }

        /// <summary>
        /// 產生資料模型，將資料產生在 Root 屬性上。
        /// </summary>
        protected virtual void GenerateTreeStruct(KeyCatalog root)
        {
            throw new NotImplementedException("您應該實作此方法。");
        }

        /// <summary>
        /// 當使用者按下「重新整理」功能，要開始重新產生資料結構之前。
        /// </summary>
        protected virtual void PreRefresh()
        {
        }

        /// <summary>
        /// 選擇的節點名稱集合，一個項目一個層次。
        /// </summary>
        private string SelectionNodeName = string.Empty;
        /// <summary>
        /// 目前還原到第幾層。
        /// </summary>
        private int RestoreLevel = 0;

        private List<string> ExpandedNodes = new List<string>();

        /// <summary>
        /// 保留目前在 TreeView 上的選擇項目。
        /// </summary>
        private void ReserveTreeSelection()
        {
            KeepExpandedNodes();
            SelectionNodeName = string.Empty;
            RestoreLevel = 0;
            KeyNode kn = ATree.SelectedNode as KeyNode;

            if (kn == null) return; //如果選擇的不是 KeyNode 就不需要處理了。

            //記錄選擇的 Node 名稱與他的層級。
            SelectionNodeName = kn.Catalog.Name;
            RestoreLevel = kn.Level;
        }

        /// <summary>
        /// 保留畫面上已經 Expand 的 Node。
        /// </summary>
        private void KeepExpandedNodes()
        {
            ExpandedNodes = new List<string>();
            KeepExpandedNodes(ATree.Nodes);
        }

        private void KeepExpandedNodes(NodeCollection nodes)
        {
            foreach (Node n in nodes)
            {
                if (n.Expanded)
                    ExpandedNodes.Add(GetNodePath(n));

                KeepExpandedNodes(n.Nodes);
            }
        }

        private static string GetNodePath(Node n)
        {
            List<string> path = new List<string>();
            KeyNode kn = n as KeyNode;

            if (kn == null) return string.Empty;

            do
            {
                path.Add(kn.Catalog.Name);
            } while ((kn = kn.Parent as KeyNode) != null);

            path.Reverse();
            return string.Join("/", path.ToArray());
        }

        /// <summary>
        /// 復原畫面上已經 Exapnd 的 Node。
        /// </summary>
        private void RestoreExpandedNodes()
        {
            if (ExpandedNodes == null) return;

            foreach (string path in ExpandedNodes)
                RestoreExpandedNodes(path);
        }

        private void RestoreExpandedNodes(string path)
        {
            if (ATree.Nodes.Count <= 0) return; //沒有資料就不處理了。

            string[] pathparts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathparts.Length <= 1) return; //只有一層代表是 Root，不需處理。

            int level = 1;
            Node current = ATree.Nodes[0];
            while (level < pathparts.Length)
            {
                string name = pathparts[level];
                foreach (Node node in current.Nodes)
                {
                    KeyNode kn = node as KeyNode;
                    if (kn == null) continue;

                    if (kn.Catalog.Name == name)
                    {
                        current = kn;
                        break;
                    }
                }
                level++;
            }
            current.Expanded = true;
        }

        private void RenderNodes(KeyCatalog catalog, NodeCollection nodes, int restoreLevel)
        {
            restoreLevel--;
            foreach (KeyCatalog sub in catalog.Subcatalogs.SortedValues)
            {
                KeyNode n = new KeyNode(sub.ToString()) { Catalog = sub };
                nodes.Add(n);

                if (restoreLevel < 0)
                {
                    if (SelectionNodeName == sub.Name)
                        ATree.SelectedNode = n;
                }

                if (!sub.IsLeaf)
                    RenderNodes(sub, n.Nodes, restoreLevel);
            }
        }

        private void Tree_AfterNodeSelect(object sender, AdvTreeNodeEventArgs e)
        {
            SetListPanel(e.Node as KeyNode);
        }

        private void Tree_NodeClick(object sender, TreeNodeMouseEventArgs e)
        {
            SetListPanel(e.Node as KeyNode);
        }

        private void Tree_NodeDoubleClick(object sender, TreeNodeMouseEventArgs e)
        {
            SetListPanel(e.Node as KeyNode);
        }

        private void SetListPanel(KeyNode node)
        {
            if (node == null) return;

            try
            {
                bool selAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool addToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(node.Catalog.AllKey.Distinct(), selAll, addToTemp);
            }
            catch (Exception) { }
        }

        private void btnRefreshAll_Click(object sender, EventArgs e)
        {
            ReserveTreeSelection();
            ShowLoading();
            PreRefresh();
            RenderTreeView(false);
        }
    }
}