using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Util
{
    public class ToolFinderUtils
    {
        public static bool ToolsAreComparable(Tool a, Tool b)
        {
            if (a == null || b == null)
            {
                // it seems sometimes some of them are null; in that case, do not compare.
                return false;
            }
            if (a == b)
            {
                // Trivial.
                return true;
            }
            List<ToolCapacityDef> capList_A = a.capacities;
            List<ToolCapacityDef> capList_B = b.capacities;
            if (capList_A.Count != capList_B.Count)
            {
                return false;
            }
            // Check that both tools have the same capacity defs
            foreach (ToolCapacityDef def_B in capList_B)
            {
                if (!capList_A.Contains(def_B))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ToolIsOriginalToolOfPawn(Tool tool, Pawn pawn)
        {
            if (pawn == null || tool == null)
            {
                // This normally shouldn't happen, but we better be prepared.
                return false;
            }
            // stronger null-safe for other mods e.g. Save Our Ships 2
            return pawn.def.tools?.Contains(tool) ?? false;
        }

        public static List<Tool> FindAllOriginalToolsUnderBodyPart(Pawn pawn, BodyPartRecord part)
        {
            List<BodyPartGroupDef> listAllUniqueGroups = new List<BodyPartGroupDef>();
            FindUniqueBodyPartGroups(ref listAllUniqueGroups, part);
            List<Tool> listFoundTools = new List<Tool>();
            foreach (Tool originalTool in pawn.Tools)
            {
                if (listAllUniqueGroups.Contains(originalTool.linkedBodyPartsGroup))
                {
                    listFoundTools.Add(originalTool);
                }
            }
            return listFoundTools;
        }

        public static Tool FindCorrespondingOriginalToolInBaseBody(Tool attackingTool, Pawn attacker, BodyPartRecord attackingPart)
        {
            List<Tool> allOriginalTools = FindAllOriginalToolsUnderBodyPart(attacker, attackingPart);
            foreach (Tool originalTool in allOriginalTools)
            {
                if (ToolsAreComparable(attackingTool, originalTool))
                {
                    return originalTool;
                }
            }
            return null;
        }

        public static void FindUniqueBodyPartGroups(ref List<BodyPartGroupDef> resultList, BodyPartRecord part)
        {
            foreach (BodyPartGroupDef group in part.groups)
            {
                if (resultList.Contains(group))
                {
                    continue;
                }
                resultList.Add(group);
            }
            foreach (BodyPartRecord child in part.GetDirectChildParts())
            {
                FindUniqueBodyPartGroups(ref resultList, child);
            }
        }
    }
}
