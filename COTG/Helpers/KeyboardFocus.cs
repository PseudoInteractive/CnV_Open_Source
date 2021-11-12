using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Helpers
{
	// Licensed to the .NET Foundation under one or more agreements.
	// The .NET Foundation licenses this file to you under the MIT license.
	// See the LICENSE file in the project root for more information.

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Microsoft.UI.Dispatching;
	using Microsoft.UI.Xaml;
	using Microsoft.UI.Xaml.Automation;
	using Microsoft.UI.Xaml.Controls;
	using Microsoft.UI.Xaml.Controls.Primitives;
	using Microsoft.UI.Xaml.Input;
	using Microsoft.UI.Xaml.Media;
	using Microsoft.UI.Xaml.Shapes;


	public class KeyboardFocus
	{
		public static KeyboardFocus instance;
		/// <summary>
		/// Defines the <see cref="IsActive"/> dependency property.
		public string controlName => focusedControl.StringIfNotNull((f) => f.Name);
		public string controlType => focusedControl.StringIfNotNull((f) => f.GetType().Name);
		public string controlAutomationName => focusedControl.StringIfNotNull((f) => AutomationProperties.GetName(f));
		public string parentsString;
		public FrameworkElement focusedControl;
		public TextBlock display;
		public XamlRoot extraXamlRoot;

		public string GetFocusDescription() => $"{controlName }:{controlType} {controlAutomationName} {GetParentsString()}";
		public static KeyboardFocus Start(TextBlock _display, XamlRoot _extraXamlRoot)
		{
			instance=new();
			instance.display = _display;
			_display.Visibility = Visibility.Visible;
			// Get currently focused control once when we start

			var focused = FocusManager.GetFocusedElement(_extraXamlRoot) as FrameworkElement;
			if(focused is null)
				focused = FocusManager.GetFocusedElement()  as FrameworkElement;
			instance.FocusOnControl(focused as FrameworkElement);
			instance.extraXamlRoot = _extraXamlRoot;
			// Then use FocusManager event from 1809 to listen to updates

			FocusManager.GotFocus -= instance.FocusManager_GotFocus;
			FocusManager.GotFocus += instance.FocusManager_GotFocus;

			return instance;

		}
		
		private void Stop()
		{
			FocusManager.GotFocus -= FocusManager_GotFocus;
			ClearContent();
		}

		private void FocusManager_GotFocus(object sender,FocusManagerGotFocusEventArgs e)
		{
			FocusOnControl(e.NewFocusedElement as FrameworkElement);
		}

		private void ClearContent()
		{
			ClearDisplay();
		}

		private void ClearDisplay() => display.Text = "[None]";
		private void FocusOnControl(FrameworkElement focusedControl)
		{
			if(focusedControl == this.focusedControl)
				return;
			this.focusedControl=focusedControl;
	//		Trace($"Focus on {focusedControl?.Name}");
			if(focusedControl == null)
			{
				ClearContent();
				return;
			}

			display.Text = GetFocusDescription();
		}

		public string GetParentsString()
		{

			var element = focusedControl;
			string name = string.Empty;
			if(element is null)
				return name;

			for(;;)
			{
				try
				{
					var parent = element.Parent as FrameworkElement;

					if(parent == null)
					{
						break;
					}

					if(!string.IsNullOrEmpty(parent.Name))
					{
						name = $"{parent.Name}=>{name}";
					}

					element = parent;
					if(element == null)
						break;
				}
				catch(Exception ex)
				{
					break;
				}
			}
			return name;
		}
	}
}

