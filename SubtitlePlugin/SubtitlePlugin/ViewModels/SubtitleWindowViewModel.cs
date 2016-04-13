using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

using Livet;

namespace SubtitlePlugin.ViewModels
{
	class SubtitleWindowViewModel : ViewModel
	{
		#region Text 변경 통지 프로퍼티
		private string _Text;

		public string Text
		{
			get
			{ return this._Text; }
			set
			{
				if (this._Text == value)
					return;
				this._Text = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion
	}
}
