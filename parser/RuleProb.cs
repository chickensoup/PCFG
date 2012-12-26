using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PCFG
{
    public class Rule
    {
        public String info, lChild, rChild;
        public int count;
        public double prob;

        public Rule()
        {
            count = 0;
            prob = 0;
        }

        public Rule(String sinfo, String slchild, String srchild)
        {
            this.info = sinfo;
            this.lChild = slchild;
            this.rChild = srchild;
            this.count = 1;
        }

        public Rule(String sinfo, String slchild, String srchild, double sprob)
        {
            this.info = sinfo;
            this.lChild = slchild;
            this.rChild = srchild;
            this.count = 1;
            this.prob = sprob;
        }

        public Rule(Rule rule)
        {
            if (rule.rChild == null)
            {
                this.info = rule.info;
                this.lChild = rule.lChild;
                this.rChild = null;
                this.prob = rule.prob;
            }
            else
            {
                this.info = rule.info;
                this.lChild = rule.lChild;
                this.rChild = rule.rChild;
                this.prob = rule.prob;
            }
        }

        public bool Equals(Node node)
        {
            if (node.rChild == null)
            {
                if (this.info.Equals(node.info) && this.lChild.Equals(node.lChild.info))
                {
                    return true;
                }
            }
            else
            {
                if (this.info.Equals(node.info) && this.lChild.Equals(node.lChild.info) && this.rChild.Equals(node.rChild.info))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Equals(Rule rule)
        {
            if (rule.rChild == null)
            {
                if (this.info.Equals(rule.info) && this.lChild.Equals(rule.lChild))
                {
                    return true;
                }
            }
            else
            {
                if (this.info.Equals(rule.info) && this.lChild.Equals(rule.lChild) && this.rChild.Equals(rule.rChild))
                {
                    return true;
                }
            }

            return false;
        }


    }

    public class RuleProb
    {
        public Dictionary<String, List<Rule>> ruleDict;
        public List<String> dict;

        public RuleProb()
        {
            ruleDict = new Dictionary<String, List<Rule>>();
            dict = new List<String>();
        }

        public void add(Rule rule)
        {
            if (dict.Contains(rule.info))
            {
                ruleDict[rule.info].Add(rule);
            }
            else
            {
                dict.Add(rule.info);
                List<Rule> list = new List<Rule>();
                list.Add(rule);

                ruleDict[rule.info] = list;
            }


        }
        
        public void add(Node node)
        {
            if (dict.Contains(node.info))
            {
                bool nodeExist = false;
                List<Rule> list = ruleDict[node.info];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Equals(node))
                    {
                        nodeExist = true;
                        list[i].count++;
                        break;
                    }
                }

                if (!nodeExist)
                {
                    if (node.rChild == null)
                    {
                        ruleDict[node.info].Add(new Rule(node.info, node.lChild.info, null));
                    }
                    else
                    {
                        ruleDict[node.info].Add(new Rule(node.info, node.lChild.info, node.rChild.info));
                    }
                }

            }
            else
            {
                dict.Add(node.info);
                List<Rule> list = new List<Rule>();
                if (node.rChild == null)
                {
                    list.Add(new Rule(node.info, node.lChild.info, null));
                }
                else
                {
                    list.Add(new Rule(node.info, node.lChild.info, node.rChild.info));
                }
                ruleDict[node.info] = list;
            }
        }

        public Node readInput(String sInput)
        {
            Stack<String> stack = new Stack<String>();
            Stack<Node> nodeStack = new Stack<Node>();
            Queue<Node> queue = new Queue<Node>();

            sInput = Regex.Replace(sInput, @"[(]", " ( ");
            sInput = Regex.Replace(sInput, @"[)]", " ) ");
            sInput = sInput.Trim();
            String[] st = Regex.Split(sInput, @"\s+");

            for (int i = 0; i < st.Length; i++)
            {
                if (st[i].Equals("("))
                {
                    stack.Push(st[i]);
                }
                else if (st[i].Equals(")"))
                {
                    List<String> list = new List<String>();
                    String sTop = stack.Pop();
                    while (!sTop.Equals("("))
                    {
                        list.Add(sTop);
                        sTop = stack.Pop();
                    }

                    if (list.Count == 2)
                    {
                        //lexical rule
                        Node node1 = new Node(list[0]);
                        Node node2 = new Node(list[1], node1);
                        nodeStack.Push(node2);
                    }
                    else
                    {
                        //phrasal rule
                        Node nt1 = nodeStack.Pop();
                        Node nt2 = nodeStack.Pop();
                        Node node1 = new Node(list[0], nt2, nt1);
                        nodeStack.Push(node1);
                    }
                }
                else
                {
                    stack.Push(st[i]);
                }
            }

            return nodeStack.Pop();
        }


    }
}
