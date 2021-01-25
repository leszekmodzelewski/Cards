using ZwSoft.ZwCAD.Geometry;
using GeoLib;
using GeoLib.Controls;
using GeoLib.Entities.Origin;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace GeoLib.Entities.Table
{
	public partial class TableWindow : Window
	{
		public TableWindow()
		{
			this.InitializeComponent();
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TableWindowViewModel dataContext = base.DataContext as TableWindowViewModel;
			OriginItem selectedItem = (sender as ComboBox).SelectedItem as OriginItem;
			if (selectedItem == null)
			{
				return;
			}
			if (!selectedItem.AutoUpdate)
			{
				return;
			}
			TableSectionTypes sectionTypes = (TableSectionTypes)selectedItem.Data.SectionTypes;
			Point3d vector3d = TableUtils.Transform(selectedItem.Position, sectionTypes, dataContext.OriginPosition);
			vector3d += new Vector3d(selectedItem.Data.X, selectedItem.Data.Y, selectedItem.Data.Z);
			DataXYZModel firstShipDrawing = dataContext.Model.FirstShipDrawing;
			firstShipDrawing.X = new double?(vector3d.X);
			firstShipDrawing.Y = new double?(vector3d.Y);
			firstShipDrawing.Z = new double?(vector3d.Z);
			dataContext.FirePropertyChanged("FirstShipDrawing");
		}

		private void OKClick(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		private void SelectObjectA(object sender, RoutedEventArgs e)
		{
			base.DialogResult = null;
			base.Close();
		}

		private void SelectObjectB(object sender, RoutedEventArgs e)
		{
			base.DialogResult = null;
			base.Close();
		}
	}
}