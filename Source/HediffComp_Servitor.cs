using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace ServitorMod
{
    public class HediffCompProperties_Servitor : HediffCompProperties
    {
        public HediffCompProperties_Servitor()
        {
            compClass = typeof(HediffComp_Servitor);
        }
    }

    public class HediffComp_Servitor : HediffComp
    {
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            ApplyOneTimeEffects();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            ApplyConstantEffects();
        }

        private void ApplyOneTimeEffects()
        {
            Pawn pawn = Pawn;
            if (pawn == null || pawn.Dead) return;

            //Change name
            if (pawn.Name is NameTriple name)
            {
                pawn.Name = new NameTriple(
                    name.First,
                    $"Servitor {pawn.thingIDNumber}",
                    name.Last
                );
            }

            pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("Seg_Servitors_Forgotten");
            pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("Seg_Servitors_Converted");

            //Change appearance
            pawn.story.skinColorOverride = new Color(0.75f, 0.75f, 0.72f);
            pawn.story.hairDef = HairDefOf.Bald;
            pawn.style.beardDef = BeardDefOf.NoBeard;

            //Remove traits
            pawn.story.traits.allTraits.Clear();

            //Reassign work
            pawn.workSettings?.DisableAll();

            //Remove passions
            foreach (var skill in pawn.skills.skills)
            {
                skill.passion = Passion.None;
            }

            //Apply skills
            ApplyServitorSkills(pawn);

            //Add traits
            TraitDef servitor = TraitDef.Named("Seg_Servitors_Servitor");
            pawn.story.traits.GainTrait(new Trait(servitor));
            pawn.story.traits.GainTrait(new Trait(TraitDefOf.Asexual));

            //Remove anesthetic
            var anesthetic = pawn.health.hediffSet
            .GetFirstHediffOfDef(HediffDefOf.Anesthetic);

            if (anesthetic != null)
            {
                pawn.health.RemoveHediff(anesthetic);
            }

        }

        private void ApplyServitorSkills(Pawn pawn)
        {
            string type = parent.def.defName;

            if (type == "Seg_Servitors_LexomatServitorHediff")
            {
                SetSkillFloor(pawn, SkillDefOf.Intellectual, 10);
            }

            if (type == "Seg_Servitors_MedicaeServitorHediff")
            {
                SetSkillFloor(pawn, SkillDefOf.Medicine, 5);
                SetSkillFloor(pawn, SkillDefOf.Crafting, 5);
                SetSkillFloor(pawn, SkillDefOf.Intellectual, 5);
            }
        }

        private void SetSkillFloor(Pawn pawn, SkillDef skill, int level)
        {
            SkillRecord record = pawn.skills.GetSkill(skill);

            if (record.Level < level)
            {
                record.Level = level;
            }
        }

        private void ApplyConstantEffects()
        {
            Pawn pawn = Pawn;
            if (pawn == null || pawn.Dead) return;

            // No need for sleep
            if (pawn.needs?.rest != null)
            {
                pawn.needs.rest.CurLevelPercentage = Mathf.Max(
                    pawn.needs.rest.CurLevelPercentage,
                    0.3f
                );
            }

            // No need for joy
            if (pawn.needs?.joy != null)
            {
                pawn.needs.joy.CurLevelPercentage = Mathf.Max(
                    pawn.needs.joy.CurLevelPercentage,
                    0.3f
                );
            }

            // No positive mental states
            if (pawn.MentalState != null)
            {
                pawn.mindState.mentalStateHandler.Reset();
            }
        }
    }
}