﻿using System;

using COTG.Models;

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DataTemplateSelector = Windows.UI.Xaml.Controls.DataTemplateSelector;

namespace COTG.TemplateSelectors
{
    public class SharedContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate StorageItemsTemplate { get; set; }

        public DataTemplate WebLinkTemplate { get; set; }

        public SharedContentTemplateSelector()
        {
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var sharedData = item as SharedDataModelBase;
            if (sharedData != null)
            {
                if (sharedData.DataFormat == StandardDataFormats.WebLink)
                {
                    return WebLinkTemplate;
                }
                else if (sharedData.DataFormat == StandardDataFormats.StorageItems)
                {
                    return StorageItemsTemplate;
                }
            }

            return DefaultTemplate;
        }
    }
}
