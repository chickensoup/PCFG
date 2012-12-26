using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace PCFG
{
    class Program
    {

        public class Trainer
        {

            public static RuleProb ruleprob;
            public static List<Rule> rulelist;
            public static List<Rule>[,] chart;
            public static Node head = new Node();
            public static List<NodeProb> nodeProbList = new List<NodeProb>();
            public static int m;

            static void Main(string[] args)
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("Invalid args! Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                ruleprob = new RuleProb();
                

                readModel(args[0]);

                StreamReader sr = new StreamReader(args[1]);

                String input = sr.ReadLine();
                sr.Close();

                if (input == null)
                {
                    Console.WriteLine("Invalid test string! Press any key to continue.");
                    return;
                }

                StreamWriter sw = new StreamWriter(new FileStream(args[2], FileMode.Create));

                //String input = "A boy with a telescope saw a girl";
                //String input = "The boy saw a girl with a telescope";
                input = "0 " + input;

                chart = readChart(input);
                m = input.Split(' ').Length - 1;

                double prob = calcInnerProb(1, m, head);

                String output = maxProbTree(head);

                for (int i = 0; i < nodeProbList.Count; i++ )
                {
                    NodeProb np = nodeProbList[i];
                    np.outerProb = calcOuterProb(np.lIndex, np.rIndex, np.info);
                }

                List<String> outputList = new List<String>();
                for (int i = 0; i < nodeProbList.Count; i++ )
                {
                    String st = nodeProbList[i].info + " # " + nodeProbList[i].lIndex.ToString() + " # " + nodeProbList[i].rIndex + " # " + nodeProbList[i].innerProb.ToString() + " # " + nodeProbList[i].outerProb;
                    outputList.Add(st);
                }

                outputList.Sort();

                sw.WriteLine(output);
                sw.WriteLine(prob.ToString());

                //Console.WriteLine(output);
                //Console.WriteLine(prob.ToString());

                for (int i = 0; i < outputList.Count; i++ )
                {
                   //Console.WriteLine(outputList[i]);
                    sw.WriteLine(outputList[i]);
                }

                sw.Flush();
                sw.Close();

            }

            public static String maxProbTree(Node node)
            {
                String str;

                String s = "";
                if (node.rChild != null)
                {
                    s = node.info + maxProbTree(node.lChild) + maxProbTree(node.rChild);
                }
                else
                {
                    s = node.info + " " + node.lChild.info;
                }

                str = "(" + s + ")";

                return str;
            }

            public static double calcOuterProb(int lChildIndex, int rChildIndex, String sNode)
            {
                double prob = 0;
                double p = 0;

                if (lChildIndex == 1 && rChildIndex == m)
                {
                    return 1;
                }

                //左侧
                for (int j = rChildIndex + 1; j <= m; j++)
                {
                    for (int k = 0; k < chart[lChildIndex, j].Count; k++ )
                    {
                        Rule rule = chart[lChildIndex, j][k];
                        p = 0;

                        if (rule.lChild.Equals(rule.rChild, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        if (rule.lChild.Equals(sNode, StringComparison.OrdinalIgnoreCase))
                        {
                            
                            for (int kt = 0; kt < nodeProbList.Count; kt++)
                            {
                                NodeProb np = nodeProbList[kt];
                                if (np.info.Equals(rule.rChild, StringComparison.OrdinalIgnoreCase) && (np.lIndex == (rChildIndex + 1)) && (np.rIndex == j))
                                {                                    
                                    p = np.innerProb;
                                    p *= (calcOuterProb(lChildIndex, j, rule.info) * rule.prob);
                                    prob += p;
                                }
                            }
                            
                        }
                    }
                }

                //右侧
                for (int i = 1; i <= lChildIndex - 1; i++ )
                {
                    for (int k = 0; k < chart[i, rChildIndex].Count; k++ )
                    {
                        Rule rule = chart[i, rChildIndex][k];
                        p = 0;

                        if (rule.rChild != null && rule.rChild.Equals(sNode, StringComparison.OrdinalIgnoreCase))
                        {
                            for (int kt = 0; kt < nodeProbList.Count; kt++ )
                            {
                                NodeProb np = nodeProbList[kt];
                                if (np.info.Equals(rule.lChild, StringComparison.OrdinalIgnoreCase) && (np.lIndex == i) && (np.rIndex == (lChildIndex - 1)))
                                {
                                    p = np.innerProb;
                                    p *= (calcOuterProb(i, rChildIndex, rule.info) * rule.prob);
                                    prob += p;
                                }
                            }
                        }
                    }
                }


                /*
                //左侧
                for (int i = 0; i < nodeProbList.Count; i++ )
                {
                    NodeProb np = nodeProbList[i];
                    if (np.lChild.Equals(sNode, StringComparison.OrdinalIgnoreCase) && (np.lIndex == lChildIndex))
                    {
                        if (nodeProbList[i].lChild.Equals(nodeProbList[i].rChild, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        for (int j = 0; j < nodeProbList.Count; j++)
                        {
                            NodeProb npt = nodeProbList[j];

                            if (npt.info.Equals(np.rChild, StringComparison.OrdinalIgnoreCase) && (npt.lIndex == lChildIndex + 1) && (npt.rIndex == np.rIndex))
                            {
                                double p = calcOuterProb(lChildIndex, npt.rIndex, np.info) * np.nodeProb * npt.innerProb;
                                prob += p;
                            }
                        }
                        
                    }
                }

                //右侧
                for (int i = 0; i < nodeProbList.Count; i++ )
                {
                    NodeProb np = nodeProbList[i];
                    if (np.rChild != null && np.rChild.Equals(sNode, StringComparison.OrdinalIgnoreCase) && (np.rIndex == rChildIndex))
                    {
                        for (int j = 0; j < nodeProbList.Count; j++)
                        {
                            NodeProb npt = nodeProbList[j];

                            if (npt.info.Equals(np.lChild, StringComparison.OrdinalIgnoreCase) && (npt.lIndex == np.lIndex) && ((npt.rIndex + 1)== rChildIndex))
                            {
                                double p = calcOuterProb(npt.lIndex, rChildIndex, np.info) * np.nodeProb * npt.innerProb;
                                prob += p;
                            }
                        }
                    }
                }
                 * 
                 * */

                return prob;
            }

         
            public static double calcInnerProb(int lChildIndex, int rChildIndex, Node father)
            {
                double maxProb = 0;
                List<Rule> rules = chart[lChildIndex, rChildIndex];
                NodeProb nodeProb;
                Node lNode = new Node(), rNode = new Node();
                double pprob = 0;

                if (lChildIndex == rChildIndex)
                {
                    father.info = rules[0].info;
                    father.lChild = new Node(rules[0].lChild);

                    nodeProb = new NodeProb(rules[0].info, rules[0].lChild, null, lChildIndex, rChildIndex, rules[0].prob, rules[0].prob);
                    nodeProbList.Add(nodeProb);

                    return rules[0].prob;
                }

                for (int i = 0; i < rules.Count; i++ )
                {
                    int jIndex = 0;
                    lNode = new Node();
                    rNode = new Node();
                    
                    lNode.leafFlag = false;
                    rNode.leafFlag = false;

                    for (int j = lChildIndex; j < rChildIndex; j++ )
                    {
                        bool ltree = false, rtree = false;
                        
                        for (int k = 0; k < chart[lChildIndex, j].Count; k++ )
                        {
                            if (chart[lChildIndex, j][k].info.Equals(rules[i].lChild))
                            {
                                ltree = true;
                                break;
                            }
                        }

                        if (ltree)
                        {
                            for (int k = 0; k < chart[j + 1, rChildIndex].Count; k++)
                            {
                                if (chart[j + 1, rChildIndex][k].info.Equals(rules[i].rChild))
                                {
                                    rtree = true;
                                    break;
                                }
                            }
                        }

                        if (ltree && rtree)
                        {
                            jIndex = j;
                            double prob = rules[i].prob * calcInnerProb(lChildIndex, jIndex, lNode) * calcInnerProb(jIndex + 1, rChildIndex, rNode);
                            if (prob > maxProb)
                            {
                                maxProb = prob;
                                father.info = rules[i].info;

                                lNode.info = chart[lChildIndex, rChildIndex][i].lChild;
                                rNode.info = chart[lChildIndex, rChildIndex][i].rChild;

                                father.lChild = lNode;
                                father.rChild = rNode;
                                father.leafFlag = false;
                                pprob = rules[i].prob;
                            }
                        }
                    }

                }

                nodeProb = new NodeProb(father.info, father.lChild.info, father.rChild.info, lChildIndex, rChildIndex, maxProb, pprob);
                nodeProbList.Add(nodeProb);

                return maxProb;
            }


            public static List<Rule>[,] readChart(String input)
            {
                String[] sInput = input.Split(' ');
                m = sInput.Length - 1;
                List<Rule>[,] chart = new List<Rule>[m + 1, m + 1];
                for (int j = 1; j <= m; j++)
                {
                    chart[j, j] = new List<Rule>();
                    for (int nt1 = 0; nt1 < ruleprob.dict.Count; nt1++)
                    {
                        List<Rule> ntrule = ruleprob.ruleDict[ruleprob.dict[nt1]];
                        for (int nt2 = 0; nt2 < ntrule.Count; nt2++)
                        {
                            if (ntrule[nt2].lChild.Equals(sInput[j], StringComparison.OrdinalIgnoreCase))
                            {
                                chart[j, j].Add(ntrule[nt2]);
                            }
                        }
                    }


                    for (int i = j - 1; i >= 1; i--)
                    {
                        if (chart[i, j] == null)
                        {
                            chart[i, j] = new List<Rule>();
                        }

                        for (int k = i; k <= j - 1; k++)
                        {
                            for (int nt1 = 0; nt1 < ruleprob.dict.Count; nt1++)
                            {
                                List<Rule> ntrule = ruleprob.ruleDict[ruleprob.dict[nt1]];
                                for (int nt2 = 0; nt2 < ntrule.Count; nt2++)
                                {
                                    if (ntrule[nt2].rChild == null)
                                    {
                                        continue;
                                    }

                                    Rule A = ntrule[nt2];
                                    String B = A.lChild;
                                    String C = A.rChild;
                                    bool bInChart = false, cInChart = false;

                                    List<Rule> rlist = chart[i, k];
                                    for (int nt3 = 0; nt3 < rlist.Count; nt3++)
                                    {
                                        if (rlist[nt3].info.Equals(B, StringComparison.OrdinalIgnoreCase))
                                        {
                                            bInChart = true;
                                            break;
                                        }
                                    }

                                    rlist = chart[k + 1, j];
                                    for (int nt3 = 0; nt3 < rlist.Count; nt3++)
                                    {
                                        if (rlist[nt3].info.Equals(C, StringComparison.OrdinalIgnoreCase))
                                        {
                                            cInChart = true;
                                            break;
                                        }
                                    }

                                    if (bInChart && cInChart)
                                    {
                                        chart[i, j].Add(A);
                                    }
                                }
                            }
                        }
                    }

                }

                return chart;
            }

            public static void readModel(String modelfile)
            {
                StreamReader sr = new StreamReader(modelfile);
                String line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    String[] srule = line.Split('#');
                    for (int i = 0; i < srule.Length; i++)
                    {
                        srule[i] = srule[i].Trim();
                    }
                    String[] sLeftRule = srule[1].Split(' ');
                    Rule rule;
                    if (sLeftRule.Length == 1)
                    {
                        rule = new Rule(srule[0], sLeftRule[0], null, double.Parse(srule[2]));
                    }
                    else
                    {
                        rule = new Rule(srule[0], sLeftRule[0], sLeftRule[1], double.Parse(srule[2]));
                    }

                    ruleprob.add(rule);
                }

                sr.Close();
            }

            public static void getRuleList(Node head)
            {
                Stack<Node> stack = new Stack<Node>();
                stack.Push(head);
                while (stack.Count > 0)
                {
                    Node peek = stack.Pop();

                    if (peek.leafFlag)
                    {
                        continue;
                    }

                    ruleprob.add(peek);
                    if (peek.rChild != null)
                    {
                        stack.Push(peek.rChild);
                    }
                    if (peek.lChild != null)
                    {
                        stack.Push(peek.lChild);
                    }
                }
            }
        }


    }


}
