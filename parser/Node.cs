using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PCFG
{
    public class Node
    {
        public String info;
        public bool leafFlag;
        public Node lChild;
        public Node rChild;


        public Node()
        {
            this.info = null;
            this.leafFlag = true;
            this.lChild = null;
            this.rChild = null;
        }

        public Node(Rule rule)
        {
            this.info = rule.info;
            this.leafFlag = false;
        }

        public Node(String info)
        {
            this.info = info;
            this.leafFlag = true;
            this.lChild = null;
            this.rChild = null;

        }

        public Node(String info, Node lchild)
        {
            this.info = info;
            this.leafFlag = false;
            this.lChild = lchild;
            this.rChild = null;
        }

        public Node(String info, Node lchild, Node rchild)
        {
            this.info = info;
            this.leafFlag = false;
            this.lChild = lchild;
            this.rChild = rchild;
        }

        public Node getLChild()
        {
            return this.lChild;
        }

        public Node getRChild()
        {
            return this.rChild;
        }

        public bool isLeaf()
        {
            return this.leafFlag;
        }

        public String getInfo()
        {
            return this.info;
        }
    }
}
