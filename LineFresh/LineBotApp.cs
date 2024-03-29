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
						//文字回應
						switch (textMessage.Text)
						{
							#region 三個遊戲規則
							case "老虎燈箱賓果":
								var bingoContainer = textTemp("老虎燈箱賓果遊戲規則",
									"開始遊戲後會得到一個老虎燈箱的九宮格，在商圈內找到九宮格內對應的老虎燈箱圖後，可點擊對應的宮格進行答題賓果，若完成兩條連線則可獲得點數。");
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
									@"由官方帳號出題，需利用指定食物的最後一個字（可為同音字）來進行美食接龍，並輸入接龍之美食與回傳在商圈內的定位，成功接龍三次後即可獲得點數。

*若食物之最後一個字無法接出美食，則可利用倒數第二個字進行接龍（以此類推）

答題範例：豆漿->薑汁豆花 or 包子->包心粉圓");
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
									@"深入走訪虎尾魅力商圈，用心觀察每一個巷弄、每一間商家以及每一個片刻，拍下最符合投稿主題的畫面。

加入社群後，即可將您的照片投稿至平台上，與社群內的好友們互相交流任何關於魅力商圈的一切。");
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
							#endregion

							case "立即推薦店家":
								var message = new TextMessage("選擇想找的店家類型");
								message.QuickReply = new QuickReply
								{
									Items = new List<QuickReplyButtonObject>
									{
										new QuickReplyButtonObject(new PostbackTemplateAction("美食", $"shopType=美食", "美食", true)),
										new QuickReplyButtonObject(new PostbackTemplateAction("飲料", $"shopType=飲料", "飲料", true)),
										new QuickReplyButtonObject(new PostbackTemplateAction("服飾", $"shopType=服飾", "服飾", true))
									}
								};
								result.Add(message);
								break;

							case "設定接收推播訊息":
								var activeMsg = new TextMessage($"是否願意接收商圈活動相關訊息推播");
								activeMsg.QuickReply = new QuickReply
								{
									Items = new List<QuickReplyButtonObject>
									{
										new QuickReplyButtonObject(new PostbackTemplateAction("是", $"activeMsg=true", "我願意接收商圈活動相關訊息推播", true)),
										new QuickReplyButtonObject(new PostbackTemplateAction("否", $"activeMsg=false", "我不想接收商圈活動相關訊息推播", true)),
									}
								};
								result.Add(activeMsg);
								break;

							#region 還沒寫
							case "查看我的集點卡":
								result.Add(new TextMessage($"您擁有的集點卡如下"));
								break;
							case "查看我的優惠券":
								result.Add(new TextMessage($"您擁有的優惠券如下"));
								break;
							case "商圈活動查詢":
								result.Add(new TextMessage($"近期商圈的活動資訊如下"));
								break;
							#endregion

							#region 找到燈箱
							case "找到第1張燈箱":
							case "找到第2張燈箱":
							case "找到第3張燈箱":
							case "找到第4張燈箱":
							case "找到第5張燈箱":
							case "找到第6張燈箱":
							case "找到第7張燈箱":
							case "找到第8張燈箱":
							case "找到第9張燈箱":
								int position = int.Parse(textMessage.Text.Substring(3, 1));
								result.Add(new TextMessage($"請問第{position}張燈箱的店家為？"));
								break;
							#endregion

							case "樂包子":
								result.Add(new TextMessage($"答對了！完成第8格燈箱"));
								result.Add(bingoTemp("https://i.imgur.com/pYIwLeE.jpg#"));
								break;

							case "豆漿":
								var lastWord = textMessage.Text.Substring(textMessage.Text.Length - 1);
								var foodNameLocationMessage = new TextMessage($"接龍成功！請傳送您的定位");
								foodNameLocationMessage.QuickReply = new QuickReply
								{
									Items = new List<QuickReplyButtonObject>
									{
										new QuickReplyButtonObject(new LocationTemplateAction("傳送定位")),
									}
								};
								result.Add(foodNameLocationMessage);
								break;

							default:
								result.Add(new TextMessage($"答錯了！再找看看吧"));
								result.Add(bingoTemp("https://i.imgur.com/eHxanaE.jpg#"));
								break;
						}
					}
					break;
				case LocationEventMessage locationMessage:
					{
						result.Add(new TextMessage($"定位成功！接下來請回答以 漿 開頭的食物"));
					}
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
				string topic = query["foodName"].Substring(query["foodName"].Length - 1);
				var message = new TextMessage($"請回答以 {topic} 開頭的食物");
				result.Add(message);
				#endregion
			}

			if (query["shopType"] != null)
			{
				#region 推薦店家類型
				string type = query["shopType"];
				//var message = new TextMessage($"想找類型：{type}");
				var message = new TextMessage($"選擇推薦的條件依據");
				message.QuickReply = new QuickReply
				{
					Items = new List<QuickReplyButtonObject>
					{
						new QuickReplyButtonObject(new PostbackTemplateAction("隨機", $"findShop=隨機", "隨機", true)),
						new QuickReplyButtonObject(new PostbackTemplateAction("價位", $"findShop=價位", "價位", true)),
						new QuickReplyButtonObject(new PostbackTemplateAction("距離", $"findShop=距離", "距離", true)),
					}
				};
				result.Add(message);
				#endregion
			}

			if (query["findShop"] != null)
			{
				#region 推薦店家條件
				string type = query["findShop"];
				var message = new TextMessage($"條件：{type}");
				var lineSpotMessage = new FlexMessage("北港阿榮古早味-虎尾店") { Contents = lineSpotTemp() };
				//result.Add(message);
				result.Add(lineSpotMessage);
				#endregion
			}

			if (query["activeMsg"] != null)
			{
				#region 接收活動訊息推播
				var discountMsg = new TextMessage($"是否願意接收商圈店家優惠相關訊息推播");
				discountMsg.QuickReply = new QuickReply
				{
					Items = new List<QuickReplyButtonObject>
					{
						new QuickReplyButtonObject(new PostbackTemplateAction("是", $"discountMsg=true", "我願意接收商圈店家優惠相關訊息推播", true)),
						new QuickReplyButtonObject(new PostbackTemplateAction("否", $"discountMsg=false", "我不想接收商圈店家優惠相關訊息推播", true)),
					}
				}; result.Add(discountMsg);
				#endregion
			}

			if (query["discountMsg"] != null)
			{
				#region 完成設定活動訊息推播
				var message = new TextMessage($"完成設定接收推播訊息");
				message.QuickReply = new QuickReply
				{
					Items = new List<QuickReplyButtonObject>
					{
						new QuickReplyButtonObject(new MessageTemplateAction("重新設定", "設定接收推播訊息")),
						new QuickReplyButtonObject(new MessageTemplateAction("完成" ,"完成")),
					}
				}; result.Add(message);

				#endregion
			}

			if (result != null) await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
		}

		#region 加入好友歡迎訊息
		/// <summary>
		/// 加入好友事件
		/// </summary>
		/// <param name="ev"></param>
		/// <returns></returns>
		protected override async Task OnFollowAsync(FollowEvent ev)
		{
			//回傳訊息
			var result = new List<ISendMessage>();
			var message = new TextMessage($"你好！歡迎來到虎尾魅力商圈，請點選下一步開始設定接收推播訊息。");
			message.QuickReply = new QuickReply
			{
				Items = new List<QuickReplyButtonObject>
				{
					new QuickReplyButtonObject(new MessageTemplateAction("下一步", "設定接收推播訊息")),
				}
			}; result.Add(message);
			if (result != null) await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
		}
		#endregion

		#region 老虎燈箱
		/// <summary>
		/// 老虎燈箱賓果遊戲
		/// </summary>
		/// <param name="result"></param>
		public void bingoGame(List<ISendMessage> result)
		{
			result.Add(bingoTemp("https://i.imgur.com/eHxanaE.jpg#"));
		}
		#endregion

		#region 食字路口
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
		#endregion

		#region 小鎮散步
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
				Action = new UriTemplateAction("加入社群", "https://line.me/ti/g2/recwmpsp10zamlO3ptRLCVmC3fNpjdCFoyj5gA?utm_source=invitation&utm_medium=link_copy&utm_campaign=default")
			};
			var container = buttonTemp("小鎮散步觀察家", "本期投稿主題：愜意", button);
			result.Add(new FlexMessage("小鎮散步觀察家本期投稿主題")
			{
				Contents = container
			});
		}
		#endregion

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

		#region LINE熱點 模板 lineSpotTemp
		public BubbleContainer lineSpotTemp()
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
							Text = $"北港阿榮古早味-虎尾店",
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
						new BoxComponent
						{
							Layout = BoxLayout.Baseline,
							Contents = new IFlexComponent[]
							{
								new IconComponent
								{
									Url = "https://cdn-icons-png.flaticon.com/512/535/535137.png"
								},
								new TextComponent
								{
									Text = $" 雲林縣虎尾鎮中山路61號",
									Wrap = true,
								}
							}
						},
						new BoxComponent
						{
							Layout = BoxLayout.Baseline,
							Contents = new IFlexComponent[]
							{
								new IconComponent
								{
									Url = "https://cdn-icons.flaticon.com/png/512/2954/premium/2954123.png?token=exp=1636711883~hmac=e295f2e164e0057fc13958302d440944"
								},
								new TextComponent
								{
									Text = $" 0905 398 060",
									Wrap = true,
								}
							}
						}
					},
				},
				Footer = new BoxComponent
				{
					Layout = BoxLayout.Vertical,
					Spacing = Spacing.Sm,
					Contents = new IFlexComponent[]
					{
						new ButtonComponent
						{
							Style = ButtonStyle.Secondary,
							Height = ButtonHeight.Sm,
							Action = new UriTemplateAction("查看更多", "https://spot.line.me/detail/608436397870683437?utm_campaign=608436397870683437&utm_medium=CopyURL&utm_source=Share")
						},
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

		#region 賓果 bingoTemp
		public ImagemapMessage bingoTemp(string baseUrl)
		{
			var imagemapMessage = new ImagemapMessage(
			baseUrl,
			"老虎燈箱賓果",
			new ImagemapSize(1040, 1040),
			new List<IImagemapAction>
			{
				new MessageImagemapAction( new ImagemapArea(0, 0, 345, 345), "找到第1張燈箱"),
				new MessageImagemapAction( new ImagemapArea(345, 0, 345, 345), "找到第2張燈箱"),
				new MessageImagemapAction( new ImagemapArea(690, 0, 345, 345), "找到第3張燈箱"),
				new MessageImagemapAction( new ImagemapArea(0, 345, 345, 345), "找到第4張燈箱"),
				new MessageImagemapAction( new ImagemapArea(345, 345, 345, 345), "找到第5張燈箱"),
				new MessageImagemapAction( new ImagemapArea(690, 345, 345, 345), "找到第6張燈箱"),
				new MessageImagemapAction( new ImagemapArea(0, 690, 345, 345), "找到第7張燈箱"),
				new MessageImagemapAction( new ImagemapArea(345, 690, 345, 345), "找到第8張燈箱"),
				new MessageImagemapAction( new ImagemapArea(690, 690, 345, 345), "找到第9張燈箱"),
			});
			return imagemapMessage;
		}
		#endregion
	}
}
