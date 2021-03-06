﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EOLib;
using EOLib.Net;

namespace EndlessClient.Handlers
{
	public struct ShopItem
	{
		private readonly int m_id, m_buy, m_sell, m_maxBuy;

		public int ID { get { return m_id; } }
		public int Buy { get { return m_buy; } }
		public int Sell { get { return m_sell; } }
		public int MaxBuy { get { return m_maxBuy; } }

		public ShopItem(int ID, int BuyPrice, int SellPrice, int MaxBuy)
		{
			m_id = ID;
			m_buy = BuyPrice;
			m_sell = SellPrice;
			m_maxBuy = MaxBuy;
		}
	}

	public struct CraftItem
	{
		private readonly int m_id;
		public int ID { get { return m_id; } }

		private readonly List<Tuple<int, int>> m_ingreds;
		public ReadOnlyCollection<Tuple<int, int>> Ingredients { get { return m_ingreds.AsReadOnly(); } }

		public CraftItem(int ID, IEnumerable<Tuple<int, int>> Ingredients)
		{
			m_ingreds = new List<Tuple<int, int>>();
			m_ingreds.AddRange(Ingredients.Where(x => x.Item1 != 0 && x.Item2 != 0));
			m_id = ID;
		}
	}

	public static class Shop
	{
		/// <summary>
		/// Sends SHOP_OPEN to server, opening a "Shop" dialog
		/// </summary>
		/// <returns>false on connection problems, true otherwise</returns>
		public static bool RequestShop(short NpcID)
		{
			EOClient client = (EOClient) World.Instance.Client;
			if (!client.ConnectedAndInitialized)
				return false;

			Packet pkt = new Packet(PacketFamily.Shop, PacketAction.Open);
			pkt.AddShort(NpcID);

			return client.SendPacket(pkt);
		}

		/// <summary>
		/// Buy an item from a shopkeeper
		/// </summary>
		public static bool BuyItem(short ItemID, int amount)
		{
			EOClient client = (EOClient)World.Instance.Client;
			if (!client.ConnectedAndInitialized)
				return false;

			Packet pkt = new Packet(PacketFamily.Shop, PacketAction.Buy);
			pkt.AddShort(ItemID);
			pkt.AddInt(amount);

			return client.SendPacket(pkt);
		}

		/// <summary>
		/// Sell an item to a shopkeeper
		/// </summary>
		public static bool SellItem(short ItemID, int amount)
		{
			EOClient client = (EOClient)World.Instance.Client;
			if (!client.ConnectedAndInitialized)
				return false;

			Packet pkt = new Packet(PacketFamily.Shop, PacketAction.Sell);
			pkt.AddShort(ItemID);
			pkt.AddInt(amount);

			return client.SendPacket(pkt);
		}

		/// <summary>
		/// Craft an item with a shopkeeper
		/// </summary>
		public static bool CraftItem(short ItemID)
		{
			EOClient client = (EOClient)World.Instance.Client;
			if (!client.ConnectedAndInitialized)
				return false;

			Packet pkt = new Packet(PacketFamily.Shop, PacketAction.Create);
			pkt.AddShort(ItemID);

			return client.SendPacket(pkt);
		}

		/// <summary>
		/// Handles SHOP_OPEN from server, contains shop data for a shop dialog
		/// </summary>
		public static void ShopOpen(Packet pkt)
		{
			if (EOShopDialog.Instance == null) return;

			int shopKeeperID = pkt.GetShort();
			if (shopKeeperID != EOShopDialog.Instance.ID) return;

			string shopName = pkt.GetBreakString();

			List<ShopItem> tradeItems = new List<ShopItem>();
			while (pkt.PeekByte() != 255)
			{
				ShopItem nextItem = new ShopItem(pkt.GetShort(), pkt.GetThree(), pkt.GetThree(), pkt.GetChar());
				tradeItems.Add(nextItem);
			}
			pkt.GetByte();

			List<CraftItem> craftItems = new List<CraftItem>();
			while (pkt.PeekByte() != 255)
			{
				int ID = pkt.GetShort();
				List<Tuple<int, int>> ingreds = new List<Tuple<int, int>>();

				for (int i = 0; i < 4; ++i)
				{
					ingreds.Add(new Tuple<int, int>(pkt.GetShort(), pkt.GetChar()));
				}
				craftItems.Add(new CraftItem(ID, ingreds));
			}
			pkt.GetByte();

			EOShopDialog.Instance.SetShopData(shopKeeperID, shopName, tradeItems, craftItems);
		}

		/// <summary>
		/// Handles SHOP_BUY from server, response to buying an item
		/// </summary>
		public static void ShopBuy(Packet pkt)
		{
			int charGoldLeft = pkt.GetInt();
			short itemID = pkt.GetShort();
			int amount = pkt.GetInt();
			byte charWeight = pkt.GetChar();
			byte charMaxWeight = pkt.GetChar();

			EndlessClient.Character c = World.Instance.MainPlayer.ActiveCharacter;
			c.UpdateInventoryItem(1, charGoldLeft);
			c.UpdateInventoryItem(itemID, amount, charWeight, charMaxWeight, true);
		}

		/// <summary>
		/// Handles SHOP_SELL from server, response to selling an item
		/// </summary>
		public static void ShopSell(Packet pkt)
		{
			int charNumLeft = pkt.GetInt();
			short itemID = pkt.GetShort();
			int charGold = pkt.GetInt();
			byte weight = pkt.GetChar();
			byte maxWeight = pkt.GetChar();

			EndlessClient.Character c = World.Instance.MainPlayer.ActiveCharacter;
			c.UpdateInventoryItem(1, charGold);
			c.UpdateInventoryItem(itemID, charNumLeft, weight, maxWeight);
		}

		/// <summary>
		/// Handles SHOP_CREATE from server, response to crafting an item
		/// </summary>
		public static void ShopCreate(Packet pkt)
		{
			short itemID = pkt.GetShort();
			byte weight = pkt.GetChar();
			byte maxWeight = pkt.GetChar();

			EndlessClient.Character c = World.Instance.MainPlayer.ActiveCharacter;
			c.UpdateInventoryItem(itemID, 1, weight, maxWeight, true);
			while (pkt.ReadPos != pkt.Length)
			{
				if (pkt.PeekShort() <= 0) break;

				c.UpdateInventoryItem(pkt.GetShort(), pkt.GetInt());
			}
		}
	}
}
