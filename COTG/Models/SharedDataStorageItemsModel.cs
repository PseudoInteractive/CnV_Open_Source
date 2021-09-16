using System.Collections.Generic;

using Microsoft.UI.Xaml.Media;

namespace COTG.Models
{
    public class SharedDataStorageItemsModel : SharedDataModelBase
    {
        public List<ImageSource> Images { get; } = new List<ImageSource>();

        public SharedDataStorageItemsModel()
            : base()
        {
        }
    }
}
