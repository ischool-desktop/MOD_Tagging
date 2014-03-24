using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FISCA;
using K12.Data;

namespace Customization.Tagging
{
    /// <summary>
    /// 還沒寫好...
    /// </summary>
    internal abstract class ObjectCache<T>
    {
        private ManualResetEvent Sync = new ManualResetEvent(true);

        private Dictionary<string, T> _cache = null;
        private Dictionary<string, T> Cache
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (_cache == null)
                    _cache = new Dictionary<string, T>();
                return _cache;
            }
        }

        //public ObjectCache()
        //{
        //TagConfig.AfterInsert += (TagConfig_AfterInsert);
        //TagConfig.AfterUpdate += (TagConfig_AfterUpdate);
        //TagConfig.AfterDelete += (TagConfig_AfterDelete);
        //}

        //private void TagConfig_AfterDelete(object sender, DataChangedEventArgs e)
        //{
        //    Remove(e.PrimaryKeys);
        //}

        //private void TagConfig_AfterUpdate(object sender, DataChangedEventArgs e)
        //{
        //    Refresh(e.PrimaryKeys);
        //}

        //private void TagConfig_AfterInsert(object sender, DataChangedEventArgs e)
        //{
        //    Refresh(e.PrimaryKeys);
        //}

        protected virtual IDictionary<string, T> GetData(params string[] idList)
        {
            return new Dictionary<string, T>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void RefreshObjects(List<string> list)
        {
            Sync.Reset();

            try
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        lock (Cache)
                        {
                            foreach (KeyValuePair<string, T> pair in GetData(list.ToArray()))
                                Cache[pair.Key] = pair.Value;
                        }
                        Sync.Set();
                    }
                    catch (Exception ex)
                    {
                        Sync.Set();
                        RTOut.WriteError(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Sync.Set();
                RTOut.WriteError(ex);
            }
        }

        protected void RemoveObjects(List<string> list)
        {
            lock (Cache)
            {
                foreach (string id in list)
                {
                    if (Cache.ContainsKey(id))
                        Cache.Remove(id);
                }
            }
        }

        /// <summary>
        /// 取得 TagConfig 的 AccessControlCode。
        /// </summary>
        /// <param name="tagConfigIDs"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<T> Get(params string[] tagConfigIDs)
        {
            Sync.WaitOne(); //等待同步完成再繼續執行。

            List<T> result = new List<T>();
            List<string> unknows = new List<string>();

            lock (Cache)
            {
                foreach (string id in tagConfigIDs)
                {
                    if (Cache.ContainsKey(id))
                        result.Add(Cache[id]);
                    else
                        unknows.Add(id);
                }
            }

            if (unknows.Count > 0)
            {
                lock (Cache)
                {
                    foreach (KeyValuePair<string, T> pair in GetData(unknows.ToArray()))
                    {
                        result.Add(pair.Value);
                        Cache.Add(pair.Key, pair.Value);
                    }
                }
            }
            return result;
        }

        //internal class TagACC
        //{
        //    public TagACC()
        //    {
        //    }

        //    public string Id { get; set; }

        //    public string ACCode { get; set; }
        //}
    }
}
