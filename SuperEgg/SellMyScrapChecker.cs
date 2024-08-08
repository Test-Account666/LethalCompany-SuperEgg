using System;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;

namespace SuperEgg;

public static class SellMyScrapChecker {
    public static bool IsSellMyScrapInstalled() =>
        Chainloader.PluginInfos.Values.Any<PluginInfo>((Func<PluginInfo, bool>) (metadata => metadata.Metadata.GUID.Contains("SellMyScrap")));
}