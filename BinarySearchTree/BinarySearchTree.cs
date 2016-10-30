using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySearchTree
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
                 * There are exactly five cases we must handle: root, no children, only a right child, only a left child, or two children.
                 * We will cover how to handle each of those as we reach them.
                 */

                //If the root is the node we're removing, there is no parent, so this case is special.
                if (currNode == root)
                {
                    //If the root has no children, simply delete the root.
                    if (currNode.LeftChild == null && currNode.RightChild == null)
                    {
                        root = null;
                        return true;
                    }
                    //If the root has only a left child, set that left child to the root.
                    //Then, clean up the previous root by removing all of its references
                    else if (currNode.LeftChild != null && currNode.RightChild == null)
                    {
                        root = currNode.LeftChild;
                        currNode.LeftChild = null;
                        return true;
                    }
                    //If the root has only a right child, set that right child to the root
                    //then, clean up the previous root by removing all of its referenecs
                    else if (currNode.LeftChild == null && currNode.RightChild != null)
                    {
                        root = currNode.RightChild;
                        currNode.RightChild = null;
                        return true;
                    }
                    /*Finally, we hit the case of two children. In this case, there are a few things we must do.
                     *           A
                     *         /   \
                     *        B     C
                     *       / \   / \
                     *      D   E F   G
                     *      
                     * Alright, so  A is our root. If we want to remove A, we definitely need to set C as the new root.
                     * However, doing that isn't so easy. C has children that need to be dealt with.
                     * We know that all of C's children are greater than A, so what does that tell us?
                     * Well, we know for sure that C, F, and G are greater than B, D, and E.
                     * Since this is true, B can be placed as a left child of F, since B, as well as B's children, are less than F.
                     * 
                     * So, what is our process?
                     * 
                     * Set C to the new root.
                     * Set B as the lowest child in C's left tree. (vvv SEE IMPORTANT NOTE vvv)
                     * Clean up A by removing all of its references.
                     * Set C's parent to null, since C is now the root.
                     * This results in the following tree:
                     * 
                     *          C
                     *        /   \
                     *       F     G
                     *      /
                     *     B
                     *    / \
                     *   D   E 
                     *   
                     *   IMPORTANT NOTE:
                     *   If F has any left children, we must go all the way down until there are no more left children.
                     *   In other words, B is being inserted at the bottom left of C's left tree, because B is less than all of those values.
                     *   This may seem extremely complicated, but in reality, all we're doing is readding B to the tree, starting from C.
                     *   B can maintain all of its children (as these will not change). 
                     *   So, we just add B to the tree as though it were new, and all of its descendents will come with it.
                    */
                    else {
                        root = currNode.RightChild;
                        Add(currNode.LeftChild);
                        currNode.LeftChild = null;
                        currNode.RightChild = null;
                        root.Parent = null;
                        return true;
                    }
                }
                else
                {
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
                        Console.WriteLine("No kids!");
                        if (comp == 1)
                        {
                            currNode.Parent.LeftChild = null;
                            currNode.Parent = null;
                        }
                        else
                        {

                            currNode.Parent.RightChild = null;
                            currNode.Parent = null;
                        }
                        return true;
                    }

                    /*Case 2: One child
                     * How do we remove a Node with only one child?
                     * 
                     * If that node itself is a leftChild:
                     * Here's a Visualization: 
                     *     A        A       A       A
                     *    /        /       /       /
                     *   B  --->  C   OR  B  ---> C
                     *  /                  \
                     * C                    C
                     * Where B is the NodeToRemove.
                     * 1st, We set A's leftChild to C
                     * 2nd, We set C's Parent to A
                     * Finally, we clean up B by removing all its references.
                     * 
                     * If that node itself is a rightChild:
                     * Here's a Visualization: 
                     * A        A           A       A
                     *  \        \           \       \
                     *   B  --->  C    OR     B --->  C  
                     *  /                      \
                     * C                        C
                     * Where B is the NodeToRemove.
                     * 1st, We set A's rightChild to C
                     * 2nd, We set C's Parent to A
                     * Finally, we clean up B by removing all its references.
                     * 
                     * Notice that all steps other than the 1st are the same. 
                     * As a result our conditional only changes which child of the Parent we change.
                     * 
                     * The only difference between having a left or right child is which reference you change.
                     * Given a left child, we want to set the parent's child to the left child.
                     * Given a right child, we want to set the parent's child to the right child.
                     */

                    //Checks that the Node has only a left child
                    else if (currNode.LeftChild != null && currNode.RightChild == null)
                    {
                        Console.WriteLine("Only a left child!");
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

                        //return true, since we successfully added a node.
                        return true;
                    }

                    //Checks that the Node has only a right child
                    else if (currNode.LeftChild == null && currNode.RightChild != null)
                    {
                        Console.WriteLine("Only a right child!");
                        if (comp == 1)
                        {
                            currNode.Parent.LeftChild = currNode.RightChild;
                        }
                        else
                        {
                            currNode.Parent.RightChild = currNode.RightChild;
                        }

                        //This happens regardless of what type of child our nodeToRemove is.
                        currNode.RightChild.Parent = currNode.Parent;
                        currNode.Parent = null;
                        currNode.LeftChild = null;

                        //return true, since we successfully added a node.
                        return true;

                    }

                    //Final case: Two children. This scenario has been covered in the root scenario
                    //The only difference is that we must account for the parent of the nodeToRemove
                    //Instead of setting the node replacing our nodeToRemove equal to root,
                    //we make it the correct child of its parent
                    else {
                        Console.WriteLine("Two kids!");
                        if (comp == 1)
                        {
                            //Was I removing a right child? Then my replacement will become a right child
                            currNode.Parent.LeftChild = currNode.RightChild;
                        }
                        else {
                            //Was I replacing a left child? Then my replacement will become a left child
                            currNode.Parent.RightChild = currNode.RightChild;
                        }
                        Add(currNode.LeftChild);                                //Add the left child, as we did in the root example
                        currNode.RightChild.Parent = currNode.Parent;           //Set our replacement's parent to the currNode's parent
                        currNode.LeftChild = null;                              //Clean up currNode
                        currNode.RightChild = null;
                        currNode.Parent = null;
                        return true;
                    }

                }

                Console.WriteLine("I'm equal! But I have not been removed.");   //This should never happen
                return false;
            }

        }

        public void Print()
        {
            Console.WriteLine(root);
        }



    }
}
