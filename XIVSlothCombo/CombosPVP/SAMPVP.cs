﻿namespace XIVSlothComboPlugin.Combos
{
    internal static class SAMPvP
    {
        public const byte JobID = 34;

        public const uint
            KashakCombo = 58,
            Yukikaze = 29523,
            Gekko = 29524,
            Kasha = 29525,
            Hyosetsu = 29526,
            Mangetsu = 29527,
            Oka = 29528,
            OgiNamikiri = 29530,
            Soten = 29532,
            Chiten = 29533,
            Mineuchi = 29535,
            MeikyoShisui = 29536,
            Midare = 29529,
            Kaeshi = 29531;

        public static class Buffs
        {
            public const ushort
                Kaiten = 3201;
        }

        public static class Config
        {
            public const string
                SamSotenCharges = "SamSotenCharges";
        }

        internal class SAMBurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAMBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var sotenCharges = Service.Configuration.GetCustomIntValue(SAMPvP.Config.SamSotenCharges);
                
                if ((IsNotEnabled(CustomComboPreset.SamPVPMainComboFeature) && actionID == SAMPvP.MeikyoShisui) ||
                    (IsEnabled(CustomComboPreset.SamPVPMainComboFeature) && actionID is SAMPvP.Yukikaze or SAMPvP.Gekko or SAMPvP.Kasha or SAMPvP.Hyosetsu or SAMPvP.Oka or SAMPvP.Mangetsu))
                { 
                        //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                        if (IsOffCooldown(SAMPvP.MeikyoShisui))
                            return OriginalHook(SAMPvP.MeikyoShisui);
                        if (IsEnabled(CustomComboPreset.SAMBurstChitenFeature) && IsOffCooldown(SAMPvP.Chiten) && InCombat() && PlayerHealthPercentageHp() <= 95)
                            return OriginalHook(SAMPvP.Chiten);
                        if (GetCooldownRemainingTime(SAMPvP.Soten) < 1 && CanWeave(SAMPvP.Yukikaze))
                            return OriginalHook(SAMPvP.Soten);
                        if (OriginalHook(SAMPvP.MeikyoShisui) == SAMPvP.Midare)
                            return OriginalHook(SAMPvP.MeikyoShisui);
                        if (IsEnabled(CustomComboPreset.SAMBurstStunFeature) && IsOffCooldown(SAMPvP.Mineuchi))
                            return OriginalHook(SAMPvP.Mineuchi);
                        if (IsOffCooldown(SAMPvP.OgiNamikiri))
                            return OriginalHook(SAMPvP.OgiNamikiri);
                        if (GetRemainingCharges(SAMPvP.Soten) > sotenCharges)
                            return OriginalHook(SAMPvP.Soten);
                        if (OriginalHook(SAMPvP.OgiNamikiri) == SAMPvP.Kaeshi)
                            return OriginalHook(SAMPvP.OgiNamikiri);
                }

                return actionID;
            }
        }

        internal class SamPvPKashaFeatures : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamPvPKashaFeatures;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SAMPvP.Yukikaze or SAMPvP.Gekko or SAMPvP.Kasha or SAMPvP.Hyosetsu or SAMPvP.Mangetsu or SAMPvP.Oka)
                {
                    if (!InMeleeRange())
                    {
                        if (IsEnabled(CustomComboPreset.SamGapCloserFeature) && GetRemainingCharges(SAMPvP.Soten) > 0)
                            return OriginalHook(SAMPvP.Soten);
                        if (IsEnabled(CustomComboPreset.SamAOEMeleeFeature) && HasEffect(SAMPvP.Buffs.Kaiten))
                            return SAM.Yukikaze;
                    }
                }

                return actionID;
            }
        }
    }
}