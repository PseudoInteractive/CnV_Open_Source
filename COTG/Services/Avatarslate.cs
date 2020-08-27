﻿using Microsoft.Toolkit.Services.MicrosoftTranslator;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using static COTG.Debug;

namespace COTG.Services
{
    class Avatarslate : ICommand
    {
        public static Avatarslate instance = new Avatarslate();
        public const string endpoint= "https://api.cognitive.microsofttranslator.com/";
        public const string apiKey = "c21130319da44a4694b1a6bf47699f5f";
        public const string getLangauges = "/languages?api-version=3.0";
        public static string[] languages;
       // public const string displayLanguage = "en";
        public static TranslatorService service;
        public static bool initialized => service != null;

        public event EventHandler CanExecuteChanged;

        public async static Task<TranslatorService> TouchAsync()
        {
            if (service == null)
            {
                Assert(service == null);
                await TranslatorService.Instance.InitializeAsync(apiKey, null);
                service = TranslatorService.Instance;
            }
            return service;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var item = parameter as MenuFlyoutItem;
            var markdown = item.Tag as MarkdownTextBlock;
            var textblock = item.Tag as TextBox;
            var txt = markdown!=null ? markdown.Text : textblock.Text;
            var src = txt;
            if (txt.IsNullOrEmpty())
                return;
            var id = src.IndexOf('{');
            if (id > 2)
                txt = txt.Substring(0, id-1);
            var xl = await service.TranslateAsync(txt, item.Text);
            var result = $"{src} {{ {item.Text}: {xl} }}";
            if (markdown != null)
            {
                markdown.Text = result;
            }
            else
            {
                textblock.Text = result;

            }
        }
        public static async Task<string[]> GetLanguagesAsync()
        {
            if (languages == null)
            {
                var i = await TouchAsync();
                languages = (await i.GetLanguagesAsync()).ToArray();
            }
            return languages;
        }
        //using (var request = new HttpRequestMessage())
        //{
        //  // In the next few sections you'll add code to construct the request.
        //}
    }
}