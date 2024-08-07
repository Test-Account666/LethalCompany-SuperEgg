using BepInEx.Configuration;

namespace SuperEgg;

public static class EggConfig {
    public static ConfigEntry<int> superEggChance = null!;
    public static ConfigEntry<bool> disableThrowAndDamageOnDeskCounter = null!;

    public static ConfigEntry<int> superExplosionChance = null!;
    public static ConfigEntry<float> superExplosionMinStrength = null!;
    public static ConfigEntry<float> superExplosionAdditionalStrength = null!;
    public static ConfigEntry<float> superExplosionKillRange = null!;
    public static ConfigEntry<float> superExplosionDamageRange = null!;
    public static ConfigEntry<int> superExplosionDamage = null!;


    public static ConfigEntry<float> normalExplosionMinStrength = null!;
    public static ConfigEntry<float> normalExplosionAdditionalStrength = null!;
    public static ConfigEntry<float> normalExplosionKillRange = null!;
    public static ConfigEntry<float> normalExplosionDamageRange = null!;
    public static ConfigEntry<int> normalExplosionDamage = null!;


    public static ConfigEntry<string> eggName = null!;

    public static ConfigEntry<bool> enableDebugLogs = null!;

    public static void Initialize(ConfigFile configFile) {
        superEggChance = configFile.Bind("General", "Super Egg Chance", 100,
                                         new ConfigDescription("Chance for an egg to be a super egg.",
                                                               new AcceptableValueRange<int>(0, 100)));

        disableThrowAndDamageOnDeskCounter = configFile.Bind("General", "Disable Throw And Damage On Desk Counter", false,
                                                             "If set to true, will disable any throw/damage caused by eggs that you put on the sell counter. "
                                                           + "This will NOT disable the visual explosion.");

        #region Super Explosion

        superExplosionChance = configFile.Bind("Super Explosion", "Super Explosion Chance", 45,
                                               new ConfigDescription("Chance for an super egg explosion to be a super explosion.",
                                                                     new AcceptableValueRange<int>(0, 100)));

        superExplosionMinStrength = configFile.Bind("Super Explosion", "Super Explosion Min Strength", 60F,
                                                    new ConfigDescription("The minimum strength of a super explosion",
                                                                          new AcceptableValueRange<float>(50F, 100F)));

        superExplosionAdditionalStrength = configFile.Bind("Super Explosion", "Super Explosion Additional Strength", 400F,
                                                           new ConfigDescription("The additional strength of a super explosion. "
                                                                               + "This is used to define the max strength value.",
                                                                                 new AcceptableValueRange<float>(0F, 400F)));

        superExplosionKillRange = configFile.Bind("Super Explosion", "Super Explosion Kill Range", 0F,
                                                  new ConfigDescription("The kill range of a super explosion. Vanilla value is 0.3 btw.",
                                                                        new AcceptableValueRange<float>(0F, 10F)));

        superExplosionDamageRange = configFile.Bind("Super Explosion", "Super Explosion Damage Range", 3F,
                                                    new ConfigDescription("The damage range of a super explosion. Vanilla value is 3 btw.",
                                                                          new AcceptableValueRange<float>(0F, 15)));

        superExplosionDamage = configFile.Bind("Super Explosion", "Super Explosion Damage", 40,
                                               new ConfigDescription("The damage of a super explosion. Vanilla value is 40 btw.",
                                                                     new AcceptableValueRange<int>(0, 100)));

        #endregion Super Explosion


        #region Normal Explosion

        normalExplosionMinStrength = configFile.Bind("Normal Explosion", "Normal Explosion Min Strength", 45F,
                                                     new ConfigDescription("The minimum strength of a normal explosion",
                                                                           new AcceptableValueRange<float>(15F, 45F)));

        normalExplosionAdditionalStrength = configFile.Bind("Normal Explosion", "Normal Explosion Additional Strength", 0F,
                                                            new ConfigDescription("The additional strength of a normal explosion. "
                                                                                + "This is used to define the max strength value.",
                                                                                  new AcceptableValueRange<float>(0F, 25F)));

        normalExplosionKillRange = configFile.Bind("Normal Explosion", "Normal Explosion Kill Range", 0.3F,
                                                   new ConfigDescription("The kill range of a normal explosion. Vanilla value is 0.3 btw.",
                                                                         new AcceptableValueRange<float>(0F, 10F)));

        normalExplosionDamageRange = configFile.Bind("Normal Explosion", "Normal Explosion Damage Range", 3F,
                                                     new ConfigDescription("The damage range of a normal explosion. Vanilla value is 3 btw.",
                                                                           new AcceptableValueRange<float>(0F, 15)));

        normalExplosionDamage = configFile.Bind("Normal Explosion", "Normal Explosion Damage", 40,
                                                new ConfigDescription("The damage of a normal explosion. Vanilla value is 40 btw.",
                                                                      new AcceptableValueRange<int>(0, 100)));

        #endregion Normal Explosion

        eggName = configFile.Bind("Edge Cases", "Egg Name", "Easter egg",
                                  "This option exists, because translation mods exist. Enter the translated Easter egg value here to make the mod work.");

        enableDebugLogs = configFile.Bind("Debugging", "Enable Debug Log", false, "Enable this, if you really hate your log. "
                                                                                + "Lunx, you probably want to ignore this one!");
    }
}