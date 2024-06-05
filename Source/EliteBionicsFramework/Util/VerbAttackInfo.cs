using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Util
{
    /// <summary>
    /// A struct for storing the verb attack info of a (melee) attack, for convenient identification and calculation.
    /// </summary>
    /// <param name="tool"></param>
    /// <param name="source"></param>
    public readonly struct VerbAttackInfo(Tool tool, HediffComp_VerbGiver source) : IEquatable<VerbAttackInfo>
    {
        public readonly Tool tool = tool;
        public readonly HediffComp_VerbGiver source = source;

        public bool IsValid
        {
            get
            {
                // the tool must exist, but the verbgiver source can be optional (when given by e.g. genes, special hediffs, etc. etc.)
                return tool != null;
            }
        }

        public bool Equals(VerbAttackInfo other)
        {
            return tool == other.tool && source == other.source;
        }
    }
}
