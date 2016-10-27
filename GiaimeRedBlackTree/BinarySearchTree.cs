using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiaimeRedBlackTree
{
    class BinarySearchTree<TData> where TData : IComparable
    {
        private BSTNode<TData> root;

        /// <summary>
        /// Creates a BinarySearchTree. Takes a root node with the desired data type.
        /// BSTs can only have 1 data type because all nodes must be Comparable to each other
        /// </summary>
        /// <param name="root">The root node of the BST.</param>
        public BinarySearchTree(BSTNode<TData> root = null)
        {
            this.root = root;
        }

        public BSTNode<TData> Root
        {
            get { return root; }
            set { root = value; }
        }

        /// <summary>
        /// Adds a node to the tree.
        /// </summary>
        /// <param name="nodeToAdd">Node to be added to the tree.</param>
        /// <param name="currNode">Node currently being examined. This defaults to the root.</param>
        /// <returns>True if Node was added, false otherwise.</returns>
        public bool Add(BSTNode<TData> nodeToAdd, BSTNode<TData> currNode = null)
        {
            if (root == null)
            {
                root = nodeToAdd;
                return true;
            }
            else
            {
                if (currNode == null)
                {
                    currNode = root;
                }
                if (nodeToAdd.Data.CompareTo(currNode.Data) == -1)
                {
                    if (currNode.LeftChild == null)
                    {
                        currNode.LeftChild = nodeToAdd;
                        return true;
                    }
                    else
                    {
                        return Add(nodeToAdd, currNode.LeftChild);
                    }
                }
                else if (nodeToAdd.Data.CompareTo(currNode.Data) == 1)
                {
                    if (currNode.RightChild == null)
                    {
                        currNode.RightChild = nodeToAdd;
                        return true;
                    }
                    else
                    {
                        return Add(nodeToAdd, currNode.RightChild);
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        //TODO Remove

        public void Print() {
            Console.WriteLine(root);
        }



    }
}
