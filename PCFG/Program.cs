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
            
            static void Main(string[] args)
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid args! Press any key to continue.");
                    Console.ReadKey();
                    return;
                }
                
                ruleprob = new RuleProb();

                StreamReader sr = new StreamReader(args[0]);
                StreamWriter sw = new StreamWriter(new FileStream(args[1], FileMode.Create));

                
                //String input1 = "(S(NP(DT The)(NN boy))(VP(VP(VBD saw)(NP(DT a)(NN girl)))(PP(IN with)(NP(DT a)(NN telescope)))))";
                //String input2 = "(S(NP(DT The)(NN girl))(VP(VBD saw)(NP(NP(DT a)(NN boy))(PP(IN with)(NP(DT a)(NN telescope))))))";

                Node head;
                
                while(!sr.EndOfStream)
                {
                    String input = sr.ReadLine();
                    head = ruleprob.readInput(input);
                    getRuleList(head);
                }

                sr.Close();

                rulelist = ruleprob.calcRuleProb();

                List<String> modelprob = new List<String>();
                for (int i = 0; i < rulelist.Count; i++ )
                {
                    Rule rule = rulelist[i];
                    String t;
                    if (rule.rChild == null)
                    {
                        t = rule.info + " # " + rule.lChild + " # " + rule.prob.ToString();
                    }
                    else
                    {
                        t = rule.info + " # " + rule.lChild + " "+ rule.rChild + " # " + rule.prob.ToString();
                    }
                    modelprob.Add(t);
                }

                modelprob.Sort();
                foreach (String t in modelprob)
                {
                    sw.WriteLine(t);
                }
                sw.Flush();
                sw.Close();

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
