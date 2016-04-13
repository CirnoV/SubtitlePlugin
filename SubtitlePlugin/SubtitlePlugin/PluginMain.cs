using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Net;
using System.Windows;
using System.Windows.Controls;

using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

using SubtitlePlugin.Views;
using SubtitlePlugin.Models;
using SubtitlePlugin.ViewModels;

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
	[ExportMetadata("Version", "0.0.2")]
	[ExportMetadata("Author", "@CirnoV")]
	public class PluginMain : IPlugin, ITool
	{
		// Item 1: it's last modified time.
		// Item 2: the time we're expected to ask for cache control.
		private Dictionary<string, Tuple<DateTime, DateTime>> cacheControl;

		internal static Api_Mst_Shipgraph[] Shipgraph { get; private set; }
		private readonly ToolViewModel ToolViewModel;
		public static string Test;

		public PluginMain()
		{
			this.ToolViewModel = new ToolViewModel();

			cacheControl = new Dictionary<string, Tuple<DateTime, DateTime>>();
		}

		~PluginMain()
		{
			ProxyServer.BeforeRequest -= ProxyServer_BeforeRequest;
			ProxyServer.BeforeResponse -= ProxyServer_BeforeResponse;

			ProxyServer.Stop();
		}

		public void Initialize()
		{
			KanColleClient.Current.Proxy.api_start2.TryParse<kcsapi_start2>().Subscribe(s =>
			{
				Shipgraph = s.Data.api_mst_shipgraph;
			});

			ProxyServer.BeforeRequest += ProxyServer_BeforeRequest;
			ProxyServer.BeforeResponse += ProxyServer_BeforeResponse;

			var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 15347, true);
			
			ProxyServer.AddEndPoint(explicitEndPoint);
			ProxyServer.Start();

			ProxyServer.SetAsSystemHttpProxy(explicitEndPoint);
		}

		private void ProxyServer_BeforeRequest(object sender, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
		{
			var request = e.ProxySession.Request;
			string path = request.RequestUri.PathAndQuery;
			
			if (path.Contains(".mp3"))
			{
				string[] substrings = path.Split('/');
				switch (substrings[3])
				{
					case "titlecall":
						ToolViewModel.UpdateText(DialogueTranslator.Add(DialogueType.Titlecall, substrings[4], substrings[5].Split('.')[0]));
						break;
					case "kc9999":
						ToolViewModel.UpdateText(DialogueTranslator.Add(DialogueType.NPC, "npc", substrings[4].Split('.')[0]));
						break;
					default:
						ToolViewModel.UpdateText(DialogueTranslator.Add(DialogueType.Shipgirl, substrings[3].Substring(2), substrings[4].Split('.')[0]));
						break;
				}
				if (request.RequestHeaders.Where(h => h.Name == "If-Modified-Since").Count() > 0)
				{
					if (cacheControl.ContainsKey(path))
					{
						if (cacheControl[path].Item2 < DateTime.Now)
						{
							return;
						}
						else
						{
							e.NotModified(cacheControl[path].Item1);
						}
					}
				}
			}
		}

		private void ProxyServer_BeforeResponse(object sender, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
		{
			string path = e.ProxySession.Request.RequestUri.PathAndQuery;
			if (path.Contains(".mp3"))
			{
				var cacheControlHeader = e.ProxySession.Response.ResponseHeaders.Where(h => h.Name == "Cache-Control").First();
				var lastModifiedHeader = e.ProxySession.Response.ResponseHeaders.Where(h => h.Name == "Last-Modified").First();
				Regex re = new Regex("([0-9]+)");
				var match = re.Match(cacheControlHeader.Value);
				if (!string.IsNullOrWhiteSpace(match.Value))
				{
					int seconds = int.Parse(match.Value);
					cacheControl.Add(path, new Tuple<DateTime, DateTime>(DateTime.Parse(lastModifiedHeader.Value), DateTime.Now.AddSeconds(seconds)));
				}
				cacheControlHeader.Value = "public, no-cache, max-age=0";
			}
		}

		public string Name => "SubtitlePlugin";

		// 탭을 볼 때마다 new가 되어 버리지만, 지금은 이렇게 하지 않으면 멀티 윈도우에서 제대로 표시되지 않습니다.
		public object View => new ToolView() { DataContext = ToolViewModel };
	}
}
