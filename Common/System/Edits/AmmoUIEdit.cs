using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace ManaAmmo.Common.System.Edits
{
    public class AmmoUIEdit : ModSystem
    {
        public override void Load()
        {
            IL_Item.FitsAmmoSlot += IL_Item_FitsAmmoSlot;
        }

        public override void Unload()
        {
            IL_Item.FitsAmmoSlot -= IL_Item_FitsAmmoSlot;
        }

        private void IL_Item_FitsAmmoSlot(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var label = c.DefineLabel();
            // look for load int 849 to eval stack
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(849));
            //index cursor forward 1 instruction
            c.Index++;
            //load the Item to the stack
            c.Emit(OpCodes.Ldarg_0);
            //using item from the stack, return bool whether in ItemGroup ManaPotion
            c.EmitDelegate<Func<Item, bool>>(item =>
            {
                if ((int)ContentSamples.CreativeHelper.GetItemGroup(item, out _) == 51)
                    return false;
                return true;
            });
            //if false, follow instruction to cursor label
            c.Emit(OpCodes.Brfalse, label);
            c.GotoNext(MoveType.After, i => i.Match(OpCodes.Ceq));
            c.GotoNext(MoveType.Before, i => i.Match(OpCodes.Ldc_I4_1));
            //set cursor label to IL 'return true'
            c.MarkLabel(label);
        }
    }
}
