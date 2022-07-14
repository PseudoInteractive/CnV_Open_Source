﻿using Microsoft.UI.Xaml.Media;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CnV;
public class ACommandCategory 
{
	public string name;
	public string icon;
	public string label;
	public string description;
	
	public ACommandCategory parent;
	public List<ACommandCategory> children = new();

	

	public ACommandCategory(string name,string label,string icon,string description,ACommandCategory parent) {
		this.name = name;
		this.icon=icon;
		this.label=label;
		this.description=description;
		this.parent=parent;
		if(parent is not null)
			parent.children.Add(this);
	}
}


	[AttributeUsage(AttributeTargets.Method)]
public class ACommandAttribute  : Attribute {

	public string? label;
	public string icon;
	public string desc;
	public string category;
	public string? canExecute;
	public ACommandAttribute(string category,string icon,string desc,string? label= null,string? canExecute = null) {
		this.category=category;
		this.label=label;
		this.icon=icon;
		this.desc=desc;
		this.canExecute=canExecute;
	}
}

internal class ACommand {
	public MethodInfo method;
	public PropertyInfo canExecult;
	public string? label;
	public string? desc;
	public string category;
	public string[]? tags;
	public string icon;
	public ACommand(MethodInfo method,string? label,string desc,string category,string icon,PropertyInfo canExecult) {
		this.method=method;
		this.label=label;
		this.desc=desc;
		this.category=category;
		this.icon=icon;
		this.canExecult=canExecult;	
	}
	static int MatchScore( string search, string label) {
		if(!label.Contains(search,StringComparison.OrdinalIgnoreCase)) {
			return 0;
		}
		var rv = (2 + search.Length);
		if(!label.StartsWith(search,StringComparison.OrdinalIgnoreCase)) {
			rv += (2 + search.Length)*2;
		}
		return rv;
	}

	internal int GetSearchScore(string search) {
		var ss = search.Split(' ',';','.','-',',');
		var rv = 0;
		foreach(var s in ss) {
			rv += MatchScore(s,Label)*8;

			if(desc is not null) 
				rv += MatchScore(s,desc)*3;
			if(tags is not null) {
					foreach(var t in tags) {
						rv += MatchScore(s,t)*4;
					}
			}
			rv += MatchScore(s,category)*1;

		}
		return rv;

	}

	const int imageSize = 32;
	public string Label => label?? method.Name;

	public ACommandInstance CreateInstance(City city) => new ACommandInstance(this,city);

	internal static ACommand[] commands;
	static ACommand() {
		var c = new List<ACommand>();
		foreach(var m in typeof(City).GetMethods( BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static)) {
			var ac = ACommandAttribute.GetCustomAttribute(m , typeof(ACommandAttribute) ) as ACommandAttribute;
			if(ac is null)
				continue;
			PropertyInfo canExecute = null;
			if(ac.canExecute is not null) {
				canExecute = typeof(City).GetProperty(ac.canExecute,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static |BindingFlags.GetProperty);
				Assert(canExecute is not null);
				Assert(canExecute.PropertyType == typeof(bool));
			}
			c.Add( new( m,ac.label,ac.desc,ac.category,ac.icon,canExecute)) ;

		}
		commands = c.ToArray();
	}

	
}

internal class ACommandInstance  :ICommand  {
	public ACommand command;
	public City city;

	public string toolTip => command.desc ?? command.Label;

	public ACommandInstance(ACommand command,City city) {
		this.command=command;
		this.city=city;
	}

	public event EventHandler? CanExecuteChanged;

	public bool CanExecute(object? parameter) {
		if(!city.IsValid())
			return false;
		if(command.canExecult is not null)
			return (bool) command.canExecult.GetValue(city);
		return true;
	}
	internal bool isEnabled => CanExecute(city);

	internal Brush isEnabledColor => AppS.Brush(isEnabled ? UIColors.Transparent : UIColors.Maroon );  
	public void Execute(object? parameter) {

		try {
			command.method.Invoke(city,null);
		}
		catch(Exception ex) {
			LogEx(ex);
			;
		}
	}
}
