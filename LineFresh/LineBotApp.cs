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
									"開起挑戰後會得到一個老虎燈箱的九宮格，在商圈內找到九宮格內對應的老虎燈箱圖後，可點擊對應的宮格進行答題賓果，若完成兩條連線則可獲得點數。");
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
									"由官方帳號出題，需利用指定食物的最後一個字（可為同音字）來進行美食接龍，並輸入接龍之美食與回傳定位，成功接龍三次後即可獲得點數。*若食物之最後一個字無法接出美食，則可利用倒數第二個字進行接龍（以此類推）答題範例：豆漿->薑汁豆花 or 包子->包心粉圓");
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
									"深入走訪虎尾魅力商圈，用心觀察每一個巷弄、每一間商家以及每一個片刻，拍下最符合投稿主題的畫面。加入社群後，即可將您的照片投稿至平台上，與社群內的好友們互相交流任何關於魅力商圈的一切。");
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
						foodNameGameBegin(result);
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
			//result.Add(new TextMessage($"讀完老虎燈箱賓果的規則"));
			var imagemapMessage = new ImagemapMessage(
				"https://github.com/28cyc/LineFresh/blob/main/LineFresh/Image/Bingo",
				"老虎燈箱賓果",
				new ImagemapSize(1020, 1020),
				new List<IImagemapAction>
				{
					new MessageImagemapAction(
						new ImagemapArea(0, 0, 340, 340), "1"),
					new MessageImagemapAction(
						new ImagemapArea(340, 0, 340, 340), "2"),
					new MessageImagemapAction(
						new ImagemapArea(680, 0, 340, 340), "3"),
					new MessageImagemapAction(
						new ImagemapArea(0, 340, 340, 340), "4"),
					new MessageImagemapAction(
						new ImagemapArea(340, 340, 340, 340), "5"),
					new MessageImagemapAction(
						new ImagemapArea(680, 340, 340, 340), "6"),
					new MessageImagemapAction(
						new ImagemapArea(0, 680, 340, 340), "7"),
					new MessageImagemapAction(
						new ImagemapArea(340, 680, 340, 340), "8"),
					new MessageImagemapAction(
						new ImagemapArea(680, 680, 340, 340), "9"),
				});
			result.Add(imagemapMessage);
		}

		/// <summary>
		/// 食字路口接龍遊戲開頭題目
		/// </summary>
		/// <param name="result"></param>
		public void foodNameGameBegin(List<ISendMessage> result)
		{
			string topic = "綠豆";
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
