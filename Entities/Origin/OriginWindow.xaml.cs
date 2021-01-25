using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GeoLib.Entities.Origin
{
	public partial class OriginWindow : Window
	{
		public OriginWindow()
		{
			this.InitializeComponent();
		}

		private void OKClick(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}
	}
}