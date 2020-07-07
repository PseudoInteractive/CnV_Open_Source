using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Models
{
    public class BoundCollection<T> : ObservableCollection<T>  where T : INotifyPropertyChanged
    {
        private void AddRangeI(IEnumerable<T> items)
        {
            // reentrancy should be blocked

                int id = Count;
                foreach (var i in items)
                    InsertItem(id++, i); // Todo: optimize this
        }

        public void AddRange(IEnumerable<T> items)
        {
            using (BlockReentrancy())
            {
                AddRangeI(items);
            }
        }
        public void NotifyReset()
        {
            OnCollectionChanged( new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void Reset(IEnumerable<T> src)
        {
            // catch for thread safety
   //         using (BlockReentrancy())
            {
                ClearItems();
                AddRangeI(src);
            }
//            OnCollectionChanged( new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void Add(T item)
        {
 //           using (BlockReentrancy())
            {
                int id = Count;
                InsertItem(id, item);
            }
            //OnCollectionChanged( new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, id));
        }
    }
}
