using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySearchTree
{
    class Program
    {
        static void Main(string[] args)
        {
            BinarySearchTree<int> myBST = new BinarySearchTree<int>();
            String input = "";
            while (input != "done") {
                Console.WriteLine();
                Console.WriteLine("Enter a command");
                input = Console.ReadLine();
                switch (input)
                {
                    case "add":
                        Console.WriteLine("Enter data to be added to tree");
                        myBST.Add(new BSTNode<int>(int.Parse(Console.ReadLine())));
                        break;
                    case "print":
                        myBST.Print();
                        break;
                    case "remove":
                        Console.WriteLine("Enter data to be removed from tree");
                        myBST.Remove(new BSTNode<int>(int.Parse(Console.ReadLine())));
                        break;

                }
            }
        }
    }
}
