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
                        nodeToAdd.Parent = currNode;
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
                        nodeToAdd.Parent = currNode;
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
        public bool Remove(BSTNode<TData> nodeToRemove, BSTNode<TData> currNode = null)
        {

            if (currNode == null)
            {
                currNode = root;
            }
            if (nodeToRemove.Data.CompareTo(currNode.Data) == -1)
            {
                if (currNode.LeftChild == null)
                {
                    //If this occurs, the node does not exist.
                    return false;
                }
                else
                {
                    //If this occurs, we must continue searching.
                    return Remove(nodeToRemove, currNode.LeftChild);
                }
            }
            else if (nodeToRemove.Data.CompareTo(currNode.Data) == 1)
            {
                if (currNode.RightChild == null)
                {
                    //If this occurs, the node does not exist.
                    return false;
                }
                else
                {
                    //If this occurs, we must continue searching.
                    return Remove(nodeToRemove, currNode.RightChild);
                }
            }
            else
            {
                /*This is where it gets tricky. We've found the node, so now we must properly remove it.
                 * There are exactly four cases we must handle: no children, only a left child, only a right child, or both a left and right child.
                 * We will cover how to handle each of those as we reach them.
                 */               

                //First, we compare the node to its parent. This way, we know if the nodeToRemove is a left or right child.
                // 1 = left child, -1 = right child
                int comp = currNode.Parent.Data.CompareTo(currNode.Data);

                /*Case 1: No children
                * For the sake of code simplicity, this section is going to be slightly inefficient.
                * What do we do when we want to remove a node with no children?
                * We simply remove the parent's connection to it.
                * This will send the node to garbage collection.
                * From the point-of-view of the tree, however, the node will no longer exist, having been replaced by null.
               */
                if (currNode.LeftChild == null && currNode.RightChild == null)
                {
                    if (comp == 1)
                    {
                        currNode.Parent.LeftChild = null;
                        currNode.Parent = null;
                    }
                    else {

                        currNode.Parent.RightChild = null;
                        currNode.Parent = null;
                    }
                    return true;
                }

                /*Case 2: Only a left child
                 * How do we remove a Node with only a left child?
                 * 
                 * If that node itself is a leftChild:
                 * Here's a Visualization: 
                 *     A        A
                 *    /        /
                 *   B  --->  C
                 *  /
                 * C
                 * Where B is the NodeToRemove.
                 * 1st, We set A's leftChild to C
                 * 2nd, We set C's Parent to A
                 * Finally, we clean up B by removing all its references.
                 * 
                 * If that node itself is a rightChild
                 *                  * If that node itself is a leftChild:
                 * Here's a Visualization: 
                 * A          A
                 *  \          \
                 *   B  --->    C
                 *  /
                 * C
                 * Where B is the NodeToRemove.
                 * 1st, We set A's rightChild to C
                 * 2nd, We set C's Parent to A
                 * Finally, we clean up B by removing all its references.
                 * 
                 * Notice that all steps other than the 1st are the same. 
                 * As a result our conditional only changes which child of the Parent we change.
                 */
                if (currNode.LeftChild != null && currNode.RightChild == null)
                {
                    if (comp == 1)
                    {
                        currNode.Parent.LeftChild = currNode.LeftChild;
                    }
                    else
                    {
                        currNode.Parent.RightChild = currNode.LeftChild;
                    }

                    //This happens regardless of what type of child our nodeToRemove is.
                    currNode.LeftChild.Parent = currNode.Parent;
                    currNode.Parent = null;
                    currNode.LeftChild = null;

                    return true;
                }

                Console.WriteLine("I'm equal! But I have not been removed.");
                return false;
            }

        }

        public void Print()
        {
            Console.WriteLine(root);
        }



    }
}
