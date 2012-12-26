using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCFG
{
    class NodeProb
    {
        public String info, lChild, rChild;
        public int lIndex, rIndex;
        public double innerProb, outerProb;
        public double nodeProb;

        public NodeProb(String sinfo, String lChild, String rChild, int sl, int sr, double prob)
        {
            this.info = sinfo;
            this.lIndex = sl;
            this.rIndex = sr;
            if (rChild == null)
            {
                this.rChild = rChild;
            }
            else
            {
                this.rChild = rChild;
            }
            this.lChild = lChild;

            this.innerProb = 0;
            this.outerProb = 0;
            this.nodeProb = prob;
        }

        public NodeProb(String sinfo, String lChild, String rChild, int sl, int sr, double inner, double prob)
        {
            this.info = sinfo;
            this.lIndex = sl;
            this.rIndex = sr;
            if (rChild == null)
            {
                this.rChild = rChild;
            }
            else
            {
                this.rChild = rChild;
            }
            this.lChild = lChild;
            this.innerProb = inner;
            this.outerProb = 0;
            this.nodeProb = prob;
        }
    }
}
