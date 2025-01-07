﻿using System.Collections.Generic;
using System.Linq;
using CK_QOL.Core;
using CK_QOL.Core.Features;
using CK_QOL.Features.ChestAutoUnlock;
using CK_QOL.Features.CraftingRange;
using CK_QOL.Features.ItemPickUpNotifier;
using CK_QOL.Features.NoDeathPenalty;
using CK_QOL.Features.NoEquipmentDurabilityLoss;
using CK_QOL.Features.QuickEat;
using CK_QOL.Features.QuickHeal;
using CK_QOL.Features.QuickStash;
using CK_QOL.Features.QuickSummon;
using CK_QOL.Features.ShiftClick;
using CK_QOL.Features.Wormhole;
using CoreLib;
using CoreLib.Localization;
using CoreLib.RewiredExtension;
using CoreLib.Util.Extensions;
using PugMod;
using Rewired;
using UnityEngine;

namespace CK_QOL
{
	public class Entry : IMod
	{
		private readonly List<IFeature> _features = new();

		internal static LoadedMod ModInfo { get; private set; }
		internal static AssetBundle AssetBundle => ModInfo.AssetBundles.First();
		internal static Player RewiredPlayer { get; private set; }

		public void EarlyInit()
		{
			InitializeModInfo();
			LoadModules();
			InitializeFeatures();
		}

		public void Init()
		{
			ModLogger.Info("Mod successfully initialized.");
		}

		public void Shutdown()
		{
			ModLogger.Warn("Mod shutdown initiated.");
		}

		public void ModObjectLoaded(Object obj)
		{
		}

		public void Update()
		{
			foreach (var feature in _features.Where(feature => feature.IsEnabled))
			{
				feature.Update();
			}
		}

		private void InitializeModInfo()
		{
			ModLogger.Info($"{ModSettings.Name} v{ModSettings.Version} by {ModSettings.Author} with contributors {ModSettings.Contributors}");

			ModInfo = this.GetModInfo();
			if (ModInfo is null)
			{
				ModLogger.Error("Failed to load mod information.");
				Shutdown();
			}
		}

		private static void LoadModules()
		{
			CoreLibMod.LoadModules(typeof(LocalizationModule));
			CoreLibMod.LoadModule(typeof(RewiredExtensionModule));

			RewiredExtensionModule.rewiredStart += () => RewiredPlayer = ReInput.players.GetPlayer(0);
		}

		private void InitializeFeatures()
		{
			_features.AddRange(new IFeature[]
			{
				CraftingRange.Instance,
				QuickStash.Instance,
				ItemPickUpNotifier.Instance,
				NoDeathPenalty.Instance,
				NoEquipmentDurabilityLoss.Instance,
				QuickHeal.Instance,
				QuickEat.Instance,
				QuickSummon.Instance,
				ShiftClick.Instance,
				ChestAutoUnlock.Instance,
				Wormhole.Instance
			});

			foreach (var feature in _features.OrderBy(feature => feature.IsEnabled))
			{
				LogFeatureState(feature);
			}

			ModLogger.Info("All features loaded.");
		}

		private static void LogFeatureState(IFeature feature)
		{
			ModLogger.Info($"{feature.DisplayName} ({feature.FeatureType})");

			if (feature.IsEnabled)
			{
				switch (feature)
				{
					case CraftingRange { IsEnabled: true } craftingRange:
						ModLogger.Info($"{nameof(craftingRange.MaxRange)}: {craftingRange.MaxRange} ");
						ModLogger.Info($"{nameof(craftingRange.MaxChests)}: {craftingRange.MaxChests}");

						break;
					case QuickStash { IsEnabled: true } quickStash:
						ModLogger.Info($"{nameof(quickStash.MaxRange)}: {quickStash.MaxRange} ");
						ModLogger.Info($"{nameof(quickStash.MaxChests)}: {quickStash.MaxChests}");

						break;
					case ItemPickUpNotifier { IsEnabled: true } itemPickUpNotifier:
						ModLogger.Info($"{nameof(itemPickUpNotifier.AggregateDelay)}: {itemPickUpNotifier.AggregateDelay}");

						break;
					case QuickHeal { IsEnabled: true } quickHeal:
						ModLogger.Info($"{nameof(quickHeal.EquipmentSlotIndex)}: {quickHeal.EquipmentSlotIndex}");

						break;
					case QuickEat { IsEnabled: true } quickEat:
						ModLogger.Info($"{nameof(quickEat.EquipmentSlotIndex)}: {quickEat.EquipmentSlotIndex}");

						break;
					case QuickSummon { IsEnabled: true } quickSummon:
						ModLogger.Info($"{nameof(quickSummon.EquipmentSlotIndex)}: {quickSummon.EquipmentSlotIndex}");

						break;
					case Wormhole { IsEnabled: true } wormhole:
						ModLogger.Info($"{nameof(wormhole.RequiredAncientGemstones)}: {wormhole.RequiredAncientGemstones}");

						break;
					case NoDeathPenalty { IsEnabled: true }:
					case NoEquipmentDurabilityLoss { IsEnabled: true }:
					case ShiftClick { IsEnabled: true }:
					case ChestAutoUnlock { IsEnabled: true }:

						break;
				}
			}
			else
			{
				ModLogger.Info("Disabled");
			}
		}
	}
}