using COTG.Game;
using COTG.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static COTG.Debug;
namespace COTG.Helpers
{
    // All updates should happen on the UI thread atomically and synchronously which will reduce the need for synchronization
    // we do not track individual properties
    public class DumbCollection<T> : List<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DumbCollection(IList<T> collection) 
        {
            Set(collection);
        }

        public DumbCollection()
        {
        }



        //public void NotifyChange(T changedItem)
        //{
        //    var dummy = PropertyChanged;
        //    PropertyChanged?.Invoke(changedItem, new PropertyChangedEventArgs(string.Empty));

        //    //            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, changedItem, changedItem));
        //}


        public void NotifyReset()
            {
                Assert(App.IsOnUIThread());
                if (CollectionChanged != null)
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

        public void Set(IEnumerable<T> src)
          {
            App.DispatchOnUIThreadSneaky(() =>
            {
                // catch for thread safety
                base.Clear();
                if(src!=null)
                    base.AddRange(src);
                NotifyReset();
            });
          }
        public new void Clear()
        {
            Set(null);
        }

            public void OnPropertyChanged(T city, string propertyName) => PropertyChanged?.Invoke(city, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;

 //       public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public new void Add(T item)
        {
            int id = Count;
            base.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, id));
        }

        // Use Reset if you are clearning first
        public void AddRange(IList<T> src)
        {
            var id = Count;
            base.AddRange(src);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,src ,id ));
        }

    }
    public static class DumbHelpers
	{
        public static void NotifyChange(this HashSet<City> items,string memberName="")
        {
            if (items.Count == 0)
                return;

            // defer the call, we don't need it right away
            App.DispatchOnUIThreadSneaky( ()=>
            {
                try
                {

                    //       MainPage.instance.gridCitySource

                    foreach (var i in items)
                    {
                        i.OnPropertyChanged((memberName));

                    }
                }
                catch (Exception e)
                {
                    Log(e);
                }
            }    );
        }
        

    }
}
