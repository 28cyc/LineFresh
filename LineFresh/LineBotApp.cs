using Line.Messaging;
using Line.Messaging.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineFresh
{
	public class LineBotApp : WebhookApplication
	{
		private readonly LineMessagingClient _messagingClient;
		public LineBotApp(LineMessagingClient lineMessagingClient)
		{
			_messagingClient = lineMessagingClient;
		}

		protected override async Task OnMessageAsync(MessageEvent ev)
		{
			var result = null as List<ISendMessage>;

			switch (ev.Message)
			{
				//文字訊息
				case TextEventMessage textMessage:
					{
						//頻道Id
						var channelId = ev.Source.Id;
						//使用者Id
						var userId = ev.Source.UserId;

						//回傳訊息
						result = new List<ISendMessage>
						{
							//new TextMessage($"你說：{textMessage.Text}"),
						};

						//圖文選單回應
						switch (textMessage.Text)
						{
							case "老虎燈箱賓果":
								bingoGame(result);
								break;
							case "食字路口接龍":
								foodNameGame(result);
								break;
							case "小鎮散步觀察家":
								townWalk(result);
								break;
							case "查看我的集點卡":
								result.Add(new TextMessage($"您擁有的集點卡如下："));
								break;
							case "查看我的優惠券":
								result.Add(new TextMessage($"您擁有的優惠券如下："));
								break;
							case "商圈活動查詢":
								result.Add(new TextMessage($"近期商圈的活動資訊如下："));
								break;
							case "立即推薦店家":
								result.Add(new TextMessage($"你想去的店家類型為："));
								break;
							case "設定接收推播訊息":
								result.Add(new TextMessage($"選擇你想接收的推播訊息："));
								break;
						}
					}
					break;
			}

			if (result != null)
				await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
		}

		/// <summary>
		/// 老虎燈箱賓果遊戲
		/// </summary>
		/// <param name="result"></param>
		public void bingoGame(List<ISendMessage> result)
		{
			result.Add(new TextMessage($"老虎燈箱賓果遊戲規則如下："));
		}

		/// <summary>
		/// 食字路口接龍遊戲
		/// </summary>
		/// <param name="result"></param>
		public void foodNameGame(List<ISendMessage> result)
		{
			result.Add(new TextMessage($"食字路口接龍遊戲規則如下："));
		}

		/// <summary>
		/// 小鎮散步觀察家
		/// </summary>
		/// <param name="result"></param>
		public void townWalk(List<ISendMessage> result)
		{
			result.Add(new TextMessage($"小鎮散步觀察家投稿規則如下："));
		}
	}
}
