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

        //Разово применить
        private void ApplyOneTimeEffects()
        {
            Pawn pawn = Pawn;
            if (pawn == null || pawn.Dead) return;

            //Меняем имя
            if (pawn.Name is NameTriple name)
            {
                pawn.Name = new NameTriple(
                    name.First,
                    $"Servitor {pawn.thingIDNumber}",
                    name.Last
                );
            }

            //Меняем внешность
            pawn.story.skinColorOverride = new Color(0.75f, 0.75f, 0.72f);
            pawn.story.hairDef = HairDefOf.Bald;

            //Убераем черты характера
            pawn.story.traits.allTraits.Clear();

            //Перенавзначаем работы
            pawn.workSettings?.DisableAll();
            pawn.workSettings?.SetPriority(WorkTypeDefOf.Mining, 3);
            pawn.workSettings?.SetPriority(WorkTypeDefOf.Cleaning, 3);
            pawn.workSettings?.SetPriority(WorkTypeDefOf.Hauling, 3);

            // Убираем пашшены
            foreach (var skill in pawn.skills.skills)
            {
                skill.passion = Passion.None;
            }

            // Делаем частью колонии
            pawn.SetFaction(Faction.OfPlayer);
            pawn.guest?.SetGuestStatus(null);
            pawn.guest?.ClearLastRecruiter();


            // Снимаем наркоз сразу после операции
            var anesthetic = pawn.health.hediffSet
                .GetFirstHediffOfDef(HediffDefOf.Anesthetic);

            if (anesthetic != null)
            {
                pawn.health.RemoveHediff(anesthetic);
            }

        }

        private void ApplyConstantEffects()
        {
            Pawn pawn = Pawn;
            if (pawn == null || pawn.Dead) return;

            // Почти не нуждается в еде
            if (pawn.needs?.food != null)
            {
                pawn.needs.food.CurLevelPercentage = Mathf.Max(
                pawn.needs.food.CurLevelPercentage,
               0.3f
                );
            }

            // Почти не нуждается во сне
            if (pawn.needs?.rest != null)
            {
                pawn.needs.rest.CurLevelPercentage = Mathf.Max(
                    pawn.needs.rest.CurLevelPercentage,
                    0.3f
                );
            }

            // Почти не нуждается в радости
            if (pawn.needs?.joy != null)
            {
                pawn.needs.joy.CurLevelPercentage = Mathf.Max(
                    pawn.needs.joy.CurLevelPercentage,
                    0.3f
                );
            }

            // Нет вдохновлений
            if (pawn.MentalState != null)
            {
                pawn.mindState.mentalStateHandler.Reset();
            }
        }
    }
}