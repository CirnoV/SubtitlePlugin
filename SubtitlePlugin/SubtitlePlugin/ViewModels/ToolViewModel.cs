using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

using Livet;
using Livet.Messaging;

namespace SubtitlePlugin.ViewModels
{
	class ToolViewModel : ViewModel
	{
		private readonly static SubtitleWindowViewModel SubtitleViewModel = new SubtitleWindowViewModel();
		public void OpenSubtitleWindow()
		{
			var message = new TransitionMessage(SubtitleViewModel, TransitionMode.Normal, "SubtitleWindow.Show");
			this.Messenger.Raise(message);
		}
		
		public void UpdateText(string text)
		{
			SubtitleViewModel.Text = text;
		}
	}
}
