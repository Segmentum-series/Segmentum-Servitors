using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ServitorMod
{
    public class RecipeWorker_Servitor : Recipe_InstallImplant
    {
        public override void ApplyOnPawn(
            Pawn pawn,
            BodyPartRecord part,
            Pawn billDoer,
            List<Thing> ingredients,
            Bill bill)
        {
            base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);

            if (pawn.Dead) return;

            if (pawn.Faction != Faction.OfPlayer)
            {
                pawn.SetFaction(Faction.OfPlayer);
                pawn.guest?.SetGuestStatus(null);
            }
        }
    }
}