﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace CK_QOL.Features.CraftingRange.Patches
{
	/// <summary>
	///     Harmony patch to modify the behavior of <see cref="CraftingHandler.GetAnyNearbyChests()" /> method.
	///     This patch overrides the default behavior to include chests within the crafting range defined by
	///     the <see cref="CraftingRange" /> feature.
	/// </summary>
	[HarmonyPatch(typeof(CraftingHandler))]
	internal static class CraftingHandlerPatches
	{
		/// <summary>
		///     A prefix patch that modifies the result of the <see cref="CraftingHandler.GetAnyNearbyChests()" /> method
		///     to include only the chests detected by the <see cref="CraftingRange" /> feature.
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(nameof(CraftingHandler.GetAnyNearbyChests), new Type[] { })]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private static bool GetAnyNearbyChests(ref List<Chest> __result)
		{
			if (!CraftingRange.Instance.IsEnabled)
			{
				return true;
			}

			__result = CraftingRange.Instance.Chests;

			return false;
		}
	}
}