using Line.Messaging;
using Line.Messaging.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
			//回傳訊息
			var result = new List<ISendMessage>();

			switch (ev.Message)
			{
				//文字訊息
				case TextEventMessage textMessage:
					{
						//圖文選單回應
						switch (textMessage.Text)
						{
							case "老虎燈箱賓果":
								var bingoContainer = textTemp("老虎燈箱賓果遊戲規則",
									"巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉");
								var bingoMessage = new FlexMessage("老虎燈箱賓果遊戲規則") { Contents = bingoContainer };
								bingoMessage.QuickReply = new QuickReply
								{
									Items = new List<QuickReplyButtonObject>
									{
										new QuickReplyButtonObject(new PostbackTemplateAction("開始遊戲", $"readRule=bingo"))
									}
								};
								result.Add(bingoMessage);
								break;

							case "食字路口接龍":
								var foodNameContainer = textTemp("食字路口接龍遊戲規則",
									"巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉");
								var foodNameMessage = new FlexMessage("食字路口接龍遊戲規則") { Contents = foodNameContainer };
								foodNameMessage.QuickReply = new QuickReply
								{
									Items = new List<QuickReplyButtonObject>
									{
										new QuickReplyButtonObject(new PostbackTemplateAction("開始遊戲", $"readRule=foodName"))
									}
								};
								result.Add(foodNameMessage);
								break;

							case "小鎮散步觀察家":
								var townWalkContainer = textTemp("小鎮散步觀察家投稿規則",
									"巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉巴拉");
								var townWalkMessage = new FlexMessage("食字路口接龍遊戲規則") { Contents = townWalkContainer };
								townWalkMessage.QuickReply = new QuickReply
								{
									Items = new List<QuickReplyButtonObject>
									{
										new QuickReplyButtonObject(new PostbackTemplateAction("查看本期主題", $"readRule=townWalk"))
									}
								};
								result.Add(townWalkMessage);
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
				default:
					result.Add(new TextMessage($"訊息類型：{ev.Message.Type}"));
					break;
			}
			if (result != null) await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
		}

		/// <summary>
		/// postback事件
		/// </summary>
		/// <param name="ev"></param>
		/// <returns></returns>
		protected override async Task OnPostbackAsync(PostbackEvent ev)
		{
			//將 data 資料轉成 QueryString
			var query = HttpUtility.ParseQueryString(ev.Postback.Data);
			//回傳訊息
			var result = new List<ISendMessage>();

			if (query["readRule"] != null)
			{
				#region 遊戲規則下一步
				switch (query["readRule"])
				{
					case "bingo":
						bingoGame(result);
						break;
					case "foodName":
						foodNameGame(result);
						break;
					case "townWalk":
						townWalk(result);
						break;
				}
				#endregion
			}
			if (query["foodName"] != null)
			{
				#region 食字路口接龍
				string topic = query["foodName"];
				result.Add(new TextMessage($"題目是：{topic}"));
				#endregion
			}

			if (result != null) await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
		}

		/// <summary>
		/// 老虎燈箱賓果遊戲
		/// </summary>
		/// <param name="result"></param>
		public void bingoGame(List<ISendMessage> result)
		{
			result.Add(new TextMessage($"讀完老虎燈箱賓果的規則"));
		}

		/// <summary>
		/// 食字路口接龍遊戲開頭題目
		/// </summary>
		/// <param name="result"></param>
		public void foodNameGame(List<ISendMessage> result)
		{
			string topic = "綠豆";
			result.Add(new TextMessage($"題目：{topic}"));
			var button = new ButtonComponent
			{
				Style = ButtonStyle.Secondary,
				Height = ButtonHeight.Sm,
				Action = new PostbackTemplateAction("接龍", $"foodName={topic}")
			};
			var container = buttonTemp("食字路口接龍", $"題目：{topic}", button);
			result.Add(new FlexMessage("食字路口接龍遊戲題目")
			{
				Contents = container
			});
		}

		/// <summary>
		/// 小鎮散步觀察家
		/// </summary>
		/// <param name="result"></param>
		public void townWalk(List<ISendMessage> result)
		{
			var button = new ButtonComponent
			{
				Style = ButtonStyle.Secondary,
				Height = ButtonHeight.Sm,
				Action = new UriTemplateAction("加入社群", "https://line.me/ti/g2/XY6wleDShv0_w0YHwDdtfDh773qT4-nCFtpIqw?utm_source=invitation&utm_medium=link_copy&utm_campaign=default")
			};
			var container = buttonTemp("小鎮散步觀察家", "本期投稿主題：TEST", button);
			result.Add(new FlexMessage("小鎮散步觀察家本期投稿主題")
			{
				Contents = container
			});
		}

		#region 標題/內容 模板 textTemp
		public BubbleContainer textTemp(string title, string content)
		{
			var container = new BubbleContainer
			{
				Header = new BoxComponent
				{
					Layout = BoxLayout.Vertical,
					Contents = new IFlexComponent[]
					{
						new TextComponent
						{
							Text = $"{title}",
							Size = ComponentSize.Lg,
							Weight = Weight.Bold
						}
					},
				},
				Body = new BoxComponent
				{
					Layout = BoxLayout.Vertical,
					Contents = new IFlexComponent[]
					{
						new TextComponent
						{
							Text = $"{content}",
							Wrap = true,
						}
					},
				},
			};
			return container;
		}
		#endregion

		#region 標題/內容/按鈕 模板 buttonTemp
		public BubbleContainer buttonTemp(string title, string content, ButtonComponent button)
		{
			var container = new BubbleContainer
			{
				Header = new BoxComponent
				{
					Layout = BoxLayout.Vertical,
					Contents = new IFlexComponent[]
					{
						new TextComponent
						{
							Text = $"{title}",
							Size = ComponentSize.Lg,
							Weight = Weight.Bold
						}
					},
				},
				Body = new BoxComponent
				{
					Layout = BoxLayout.Vertical,
					Contents = new IFlexComponent[]
					{
						new TextComponent
						{
							Text = $"{content}",
							Wrap = true,
						}
					},
				},
				Footer = new BoxComponent
				{
					Layout = BoxLayout.Vertical,
					Spacing = Spacing.Sm,
					Contents = new IFlexComponent[]
					{
						button,
						new SpacerComponent
						{
							Size = ComponentSize.Sm
						}
					},
					Flex = 0
				}
			};
			return container;
		}
		#endregion
	}
}
