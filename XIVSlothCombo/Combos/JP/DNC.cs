using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JP
{
    internal class DNC_ST : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is not DNC.Cascade) 
                return actionID;

            // State
            var gauge = GetJobGauge<DNCGauge>();
            var hasSymmetry = HasEffect(DNC.Buffs.SilkenSymmetry) || HasEffect(DNC.Buffs.FlourishingSymmetry);
            var hasFlow = HasEffect(DNC.Buffs.SilkenFlow) || HasEffect(DNC.Buffs.FlourishingFlow);
            var hasStandardFinish = HasEffect(DNC.Buffs.StandardFinish);
            var hasTechnicalFinish = HasEffect(DNC.Buffs.TechnicalFinish);
            
            // Dancing
            if (HasEffect(DNC.Buffs.TechnicalStep))
                return gauge.CompletedSteps < 4 ? gauge.NextStep : DNC.TechnicalFinish4;
            if (HasEffect(DNC.Buffs.StandardStep))
                return gauge.CompletedSteps < 2 ? gauge.NextStep : DNC.StandardFinish2;

            // Weaving
            // Hack to prevent a fail state, since Finishing move has a recast of 30s
            var lastWeaponSkill = ActionWatching.LastWeaponskill;
            if (lastWeaponSkill == DNC.FinishingMove)
                lastWeaponSkill = DNC.Cascade;
            if (CanWeave(lastWeaponSkill))
            {
                if (ActionReady(DNC.Devilment))
                {
                    if (WasLastWeaponskill(DNC.TechnicalFinish4) || !LevelChecked(DNC.TechnicalStep))
                        return DNC.Devilment;
                }
                if (ActionReady(DNC.Flourish))
                {
                    // 60  = Flourish cooldown 
                    // 2.5 = Next GCD after Technical Finish since it'll never go in the first weave window  
                    // 1.5 = Standard Step Recast to accomodate for FanDance3 overcap 
                    // 0.7 = Default Weave Time 
                    var flourishCooldownOffset = 60 - 2.5 - 1.5 - 0.7;
                    if (IsOnCooldown(DNC.Devilment) && GetCooldownRemainingTime(DNC.Devilment) >= flourishCooldownOffset)
                        return DNC.Flourish;
                }
                if (hasTechnicalFinish || hasStandardFinish)
                {
                    if (HasEffect(DNC.Buffs.FourFoldFanDance))
                        return DNC.FanDance4;
                    if (HasEffect(DNC.Buffs.ThreeFoldFanDance))
                        return DNC.FanDance3;
                    if (gauge.Feathers > 0)
                    {
                        if (hasTechnicalFinish || gauge.Feathers > 3)
                            return DNC.FanDance1;
                    }
                }
            }

            // GCDs
            if (ActionReady(DNC.TechnicalStep) && hasStandardFinish)
                return DNC.TechnicalStep;
            if (HasEffect(DNC.Buffs.FlourishingFinish) && gauge.Esprit < 50)
                return DNC.Tillana;
            if (HasEffect(DNC.Buffs.DanceOfTheDawnReady) && gauge.Esprit >= 50)
                return DNC.DanceOfTheDawn;
            if (HasEffect(DNC.Buffs.LastDanceReady))
            {
                if (hasTechnicalFinish)
                    return DNC.LastDance;
                // Hold it for burst check
                // 7 = Technical Step + Dances + Technical Finish (1.5 + (4 * 1.0) + 1.5)
                // 2.5 + 2.5 = Tillana + Dance of the Dawn (Priority coverage at worst)
                var lastDanceCooldownOffset = 7 + 2.5 + 2.5;
                var burstCooldown = GetCooldownRemainingTime(DNC.TechnicalStep) + lastDanceCooldownOffset;
                if (burstCooldown > GetBuffRemainingTime(DNC.Buffs.LastDanceReady))
                    return DNC.LastDance;
            }
            if (HasEffect(DNC.Buffs.FinishingMoveReady) && ActionReady(DNC.FinishingMove))
                return DNC.FinishingMove;
            if (HasEffect(DNC.Buffs.FlourishingStarfall) && gauge.Esprit < 80)
                return DNC.StarfallDance;
            if (ActionReady(DNC.SaberDance))
            {
                if (hasTechnicalFinish && gauge.Esprit >= 50)
                    return DNC.SaberDance;
                if (gauge.Esprit >= 80)
                    return DNC.SaberDance;
            }
            if (ActionReady(DNC.StandardStep))
            {
                if (!hasStandardFinish)
                    return DNC.StandardStep;
                // Hold it for burst check
                // 5 = Standard Step + Dances + Standard Finish (1.5 + (2 * 1.0) + 1.5)
                var standardStepCooldownOffset = 5;
                var burstCooldown = GetCooldownRemainingTime(DNC.TechnicalStep);
                if (burstCooldown > standardStepCooldownOffset)
                    return DNC.StandardStep;
            }
            if (ActionReady(DNC.Fountainfall) && hasFlow)
                return DNC.Fountainfall;
            if (ActionReady(DNC.ReverseCascade) && hasSymmetry)
                return DNC.ReverseCascade;
            if (ActionReady(DNC.Fountain) && lastComboMove is DNC.Cascade && comboTime > 0)
                return DNC.Fountain;

            return actionID;
        }
    }

    internal class DNC_AoE : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is not DNC.Windmill)
                return actionID;

            // State
            var gauge = GetJobGauge<DNCGauge>();
            var hasSymmetry = HasEffect(DNC.Buffs.SilkenSymmetry) || HasEffect(DNC.Buffs.FlourishingSymmetry);
            var hasFlow = HasEffect(DNC.Buffs.SilkenFlow) || HasEffect(DNC.Buffs.FlourishingFlow);
            var hasStandardFinish = HasEffect(DNC.Buffs.StandardFinish);
            var hasTechnicalFinish = HasEffect(DNC.Buffs.TechnicalFinish);

            // Dancing
            if (HasEffect(DNC.Buffs.TechnicalStep))
                return gauge.CompletedSteps < 4 ? gauge.NextStep : DNC.TechnicalFinish4;
            if (HasEffect(DNC.Buffs.StandardStep))
                return gauge.CompletedSteps < 2 ? gauge.NextStep : DNC.StandardFinish2;

            // Weaving
            // Hack to prevent a fail state, since Finishing move has a recast of 30s
            var lastWeaponSkill = ActionWatching.LastWeaponskill;
            if (lastWeaponSkill == DNC.FinishingMove)
                lastWeaponSkill = DNC.Cascade;
            if (CanWeave(lastWeaponSkill))
            {
                if (ActionReady(DNC.Devilment))
                {
                    if (WasLastWeaponskill(DNC.TechnicalFinish4) || !LevelChecked(DNC.TechnicalStep))
                        return DNC.Devilment;
                }
                if (ActionReady(DNC.Flourish))
                {
                    // 60  = Flourish cooldown 
                    // 2.5 = Next GCD after Technical Finish since it'll never go in the first weave window  
                    // 1.5 = Standard Step Recast to accomodate for FanDance3 overcap 
                    // 0.7 = Default Weave Time 
                    var flourishCooldownOffset = 60 - 2.5 - 1.5 - 0.7;
                    if (IsOnCooldown(DNC.Devilment) && GetCooldownRemainingTime(DNC.Devilment) >= flourishCooldownOffset)
                        return DNC.Flourish;
                }
                if (hasTechnicalFinish || hasStandardFinish)
                {
                    if (HasEffect(DNC.Buffs.FourFoldFanDance))
                        return DNC.FanDance4;
                    if (HasEffect(DNC.Buffs.ThreeFoldFanDance))
                        return DNC.FanDance3;
                    if (gauge.Feathers > 0)
                    {
                        if (hasTechnicalFinish || gauge.Feathers > 3)
                            return DNC.FanDance2;
                    }
                }
            }

            // GCDs
            if (ActionReady(DNC.TechnicalStep) && hasStandardFinish)
                return DNC.TechnicalStep;
            if (HasEffect(DNC.Buffs.FlourishingFinish) && gauge.Esprit < 50)
                return DNC.Tillana;
            if (HasEffect(DNC.Buffs.DanceOfTheDawnReady) && gauge.Esprit >= 50)
                return DNC.DanceOfTheDawn;
            if (HasEffect(DNC.Buffs.LastDanceReady))
            {
                if (hasTechnicalFinish)
                    return DNC.LastDance;
                // Hold it for burst check
                // 7 = Technical Step + Dances + Technical Finish (1.5 + (4 * 1.0) + 1.5)
                // 2.5 + 2.5 = Tillana + Dance of the Dawn (Priority coverage at worst)
                var lastDanceCooldownOffset = 7 + 2.5 + 2.5;
                var burstCooldown = GetCooldownRemainingTime(DNC.TechnicalStep) + lastDanceCooldownOffset;
                if (burstCooldown > GetBuffRemainingTime(DNC.Buffs.LastDanceReady))
                    return DNC.LastDance;
            }
            if (HasEffect(DNC.Buffs.FinishingMoveReady) && ActionReady(DNC.FinishingMove))
                return DNC.FinishingMove;
            if (HasEffect(DNC.Buffs.FlourishingStarfall) && gauge.Esprit < 80)
                return DNC.StarfallDance;
            if (ActionReady(DNC.SaberDance))
            {
                if (hasTechnicalFinish && gauge.Esprit >= 50)
                    return DNC.SaberDance;
                if (gauge.Esprit >= 80)
                    return DNC.SaberDance;
            }
            if (ActionReady(DNC.StandardStep))
            {
                if (!hasStandardFinish)
                    return DNC.StandardStep;
                // Hold it for burst check
                // 5 = Standard Step + Dances + Standard Finish (1.5 + (2 * 1.0) + 1.5)
                var standardStepCooldownOffset = 5;
                var burstCooldown = GetCooldownRemainingTime(DNC.TechnicalStep);
                if (burstCooldown > standardStepCooldownOffset)
                    return DNC.StandardStep;
            }
            if (ActionReady(DNC.Bloodshower) && hasFlow)
                return DNC.Bloodshower;
            if (ActionReady(DNC.RisingWindmill) && hasSymmetry)
                return DNC.RisingWindmill;
            if (ActionReady(DNC.Bladeshower) && lastComboMove is DNC.Windmill && comboTime > 0)
                return DNC.Bladeshower;

            return actionID;
        }
    }
}