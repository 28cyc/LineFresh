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

						//回傳 hellow
						result = new List<ISendMessage>
						{
							new TextMessage($"textMessage.Text：{textMessage.Text}"),
							//new TextMessage($"ev.Source.Id：{ev.Source.Id}"),
							//new TextMessage($"ev.Source.UserId：{ev.Source.UserId}")
						};
					}
					break;
			}

			if (result != null)
				await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
		}
	}
}
