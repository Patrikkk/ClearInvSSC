using System;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.Localization;

namespace ClearInvSSC
{
	[ApiVersion(2, 1)]

	public class Plugin : TerrariaPlugin
	{
		#region PluginInfo
		public override string Name { get { return "ClearInvSSC"; } }
		public override string Author { get { return "IcyPhoenix - Patrikk Update"; } }
		public override string Description { get { return "Clear Inventory/buffs if SSC Activated"; } }
		public override Version Version { get { return new Version("2.0"); } }
		#endregion

		public Plugin(Main game)
			: base(game)
		{
		}

		#region Initialize
		public override void Initialize()
		{
			if (!Main.ServerSideCharacter)
			{
				TShock.Log.ConsoleError("ClearInvSSC did not initialize, because the server is not set to SSC!");
				return;
			}
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			TShockAPI.Hooks.PlayerHooks.PlayerLogout += PlayerHooks_PlayerLogout;
		}

		private void PlayerHooks_PlayerLogout(TShockAPI.Hooks.PlayerLogoutEventArgs e)
		{
			/// This should never be true. But I haven't tested.
			if (!e.Player.IsLoggedIn)
				CleanInventory(e.Player);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
			base.Dispose(disposing);
		}
		#endregion
		#region Hooks
		private void OnGetData(GetDataEventArgs args)
		{
			TSPlayer player = TShock.Players[args.Msg.whoAmI];
			if (player == null)
				return;

			if (args.MsgID == PacketTypes.TileGetSection)
			{
				if (Netplay.Clients[args.Msg.whoAmI].State == 2 && !player.IsLoggedIn)
				{
					CleanInventory(player);
				}
			}
		}
		#endregion

		private void CleanInventory(TSPlayer player)
		{
			player.TPlayer.SpawnX = -1;
			player.TPlayer.SpawnY = -1;
			player.sX = -1;
			player.sY = -1;

			float slot = 0f;
			for (int k = 0; k < NetItem.InventorySlots; k++)
			{
				player.TPlayer.inventory[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].inventory[k].Name), player.Index, slot, (float)Main.player[player.Index].inventory[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.ArmorSlots; k++)
			{
				player.TPlayer.armor[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].armor[k].Name), player.Index, slot, (float)Main.player[player.Index].armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.DyeSlots; k++)
			{
				player.TPlayer.dye[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].dye[k].Name), player.Index, slot, (float)Main.player[player.Index].dye[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.MiscEquipSlots; k++)
			{
				player.TPlayer.miscEquips[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscEquips[k].Name), player.Index, slot, (float)Main.player[player.Index].miscEquips[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.MiscDyeSlots; k++)
			{
				player.TPlayer.miscDyes[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscDyes[k].Name), player.Index, slot, (float)Main.player[player.Index].miscDyes[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.PiggySlots; k++)
			{
				player.TPlayer.bank.item[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.SafeSlots; k++)
			{
				player.TPlayer.bank2.item[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank2.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank2.item[k].prefix);
				slot++;
			}

			player.TPlayer.trashItem.netDefaults(0);
			NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].trashItem.Name), player.Index, slot++, (float)Main.player[player.Index].trashItem.prefix);

			for (int k = 0; k < NetItem.ForgeSlots; k++)
			{
				player.TPlayer.bank3.item[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank3.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank3.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.VoidSlots; k++)
			{
				player.TPlayer.bank4.item[k].netDefaults(0);
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank4.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank4.item[k].prefix);
				slot++;
			}



			for (int k = 0; k < Player.maxBuffs; k++)
			{
				player.TPlayer.buffType[k] = 0;
			}

			NetMessage.SendData(4, -1, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(42, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(16, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(50, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);

			for (int k = 0; k < NetItem.MaxInventory; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, (float)k, 0f, 0f, 0);
			}

			for (int k = 0; k < Player.maxBuffs; k++)
			{
				player.TPlayer.buffType[k] = 0;
			}

			NetMessage.SendData(4, player.Index, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(42, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(16, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(50, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
		}
	}
}