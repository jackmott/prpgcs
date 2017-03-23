using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPG
{
    public interface NPCState
    {
    }

    public struct CraftingState : NPCState
    {
        public readonly CraftingRecipe recipe;
        public DateTime startTime;

        public CraftingState(CraftingRecipe recipe)
        {
            this.recipe = recipe;
            startTime = DateTime.Now;
        }
    }

    public struct RestingState : NPCState
    {

    }
}
