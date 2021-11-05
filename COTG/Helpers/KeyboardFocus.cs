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

	namespace COTG
	{
		public class KeyboardFocus
		{
			public KeyboardAccelerator instance = new();
			/// <summary>
			/// Defines the <see cref="IsActive"/> dependency property.
			public string controlName => AUtil.StringIfNotNull(f,(f) => f.Name);
			public string controlType => AUtil.StringIfNotNull(f,(f) => f.GetType().Name);
			public  string controlAutomationName => AUtil.StringIfNotNull(f,(f)=>AutomationProperties.GetName(f));
			public  string parentsString;
			public  FrameworkElement focusedControl;
			public TextBlock display;

		
			public string GetFocusDescription() => $"{controlName }:{controlType} {controlAutomationName} {GetParentsString()}";
			public void Start(TextBlock _display )
			{
				display = _display;
				// Get currently focused control once when we start
				
				FocusOnControl(FocusManager.GetFocusedElement() as FrameworkElement);
				

				// Then use FocusManager event from 1809 to listen to updates
				FocusManager.GotFocus += FocusManager_GotFocus;
				display.Visibility = Visibility.Collapsed;
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
			private  void FocusOnControl(FrameworkElement focusedControl)
			{
				this.focusedControl=focusedControl;
				if(focusedControl == null)
				{
					ClearContent();
					return;
				}

				display.Text = GetFocusDescription();
			}

			public void GetParentsString()
			{

				var element = focusedControl;
				if(element is null)
					return name;
				string name = string.Empty;
				for(;;)
				{
					var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

					if(parent == null)
					{
						break;
					}

					if(!string.IsNullOrEmpty(parent.Name))
					{
						name = $"{parent.Name}=>{name}";
					}

					element = parent;
				}
				return name;
			}
		}
	}
}
