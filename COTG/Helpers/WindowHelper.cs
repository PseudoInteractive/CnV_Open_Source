using Microsoft.UI.Xaml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT;


using Windows.Foundation.Metadata;
using Windows.Storage.Pickers;
using System.Runtime.InteropServices;

namespace COTG
{
	internal class WindowHelper
	{
			public static void InitFileOpenPicker(FileOpenPicker picker)
			{
				if(Window.Current == null)
				{
					var initializeWithWindowWrapper = picker.As<IInitializeWithWindow>();
					var hwnd = GetActiveWindow();
					initializeWithWindowWrapper.Initialize(hwnd);
				}
			}


			[ComImport, System.Runtime.InteropServices.Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IInitializeWithWindow
			{
				void Initialize([In] IntPtr hwnd);
			}

			[DllImport("user32.dll",ExactSpelling = true,CharSet = CharSet.Auto,PreserveSig = true,SetLastError = false)]
			public static extern IntPtr GetActiveWindow();
	}
}
