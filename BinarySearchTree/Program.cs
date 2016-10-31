using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBlackTree
{
    class Program
    {
        static void Main(string[] args)
        {
            RedBlackTree<int> myRBT = new RedBlackTree<int>();
            String input = "";
            RBTNode<int> currNode = null;
            while (input != "done") {
                Console.WriteLine();
                Console.WriteLine("Enter a command");
                input = Console.ReadLine();
                
                switch (input)
                {
                    case "add":
                        Console.WriteLine("Enter data to be added to tree");
                        myRBT.Add(new RBTNode<int>(int.Parse(Console.ReadLine())));
                        break;
                    case "print":
                        myRBT.Print();
                        break;
                    case "remove":
                        Console.WriteLine("Enter data to be removed from tree");
                        myRBT.Remove(new RBTNode<int>(int.Parse(Console.ReadLine())));
                        break;
                    case "root":
                        currNode = (RBTNode<int>)myRBT.Root;
                        Console.WriteLine(currNode.Data);
                        break;
                    case "right":
                        try
                        {
                            currNode = (RBTNode<int>)currNode.RightChild;
                        }
                        catch (NullReferenceException e) { Console.WriteLine(currNode.Data + " does not have a right child!"); }
                        Console.WriteLine(currNode.Data + " " + (currNode as RBTNode<int>).Color);
                        break;
                    case "left":
                        try {
                            currNode = (RBTNode<int>)currNode.LeftChild;
                            Console.WriteLine(currNode.Data + " " + (currNode as RBTNode<int>).Color);
                        }
                        catch (NullReferenceException e) { Console.WriteLine(currNode.Data + " does not have a left child!"); }
                        break;
                    case "parent":
                        try
                        {
                            currNode = (RBTNode<int>)currNode.Parent;
                            Console.WriteLine(currNode.Data + " " + (currNode as RBTNode<int>).Color);
                        }
                        catch (NullReferenceException e) { Console.WriteLine(currNode.Data + " does not have a parent!"); }
                        break;
                    case "curr":
                        Console.WriteLine(currNode.Data + " " + (currNode as RBTNode<int>).Color);
                        break;

                }
            }
        }
    }
}
