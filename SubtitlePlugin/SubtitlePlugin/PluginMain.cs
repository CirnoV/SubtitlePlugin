using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace SubtitlePlugin
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ITool))]
	[ExportMetadata("Guid", "1207972E-79CD-48F8-A4E2-94EEA498C947")]
	[ExportMetadata("Title", "SubtitlePlugin")]
	[ExportMetadata("Description", "자막을 추가해주는 플러그인 입니다.")]
	[ExportMetadata("Version", "1.2.0")]
	[ExportMetadata("Author", "@CirnoV")]
	public class HelperPlugin : IPlugin, ITool
	{
		public void Initialize()
		{

		}

		public string Name => "SubtitlePlugin";

		// 탭을 볼 때마다 new가 되어 버리지만, 지금은 이렇게 하지 않으면 멀티 윈도우에서 제대로 표시되지 않습니다.
		public object View => new ToolView();
	}
}
