using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBlackTree
{
    class RedBlackTree<TData> where TData : IComparable
    {

        private RBTNode<TData> root;

        public RedBlackTree(RBTNode<TData> root = null)
        {
            this.root = root;
        }

        public RBTNode<TData> Root
        {
            get { return root; }
            set { root = value; }
        }


        /// <summary>
        /// Adds a node to the tree. Basic BST insertion, then calls RebalanceInsert.
        /// </summary>
        /// <param name="nodeToAdd">Node to be added to the tree.</param>
        /// <param name="currNode">Node currently being examined. This defaults to the root.</param>
        /// <returns>True if Node was added, false otherwise.</returns>
        public bool Add(RBTNode<TData> nodeToAdd, RBTNode<TData> currNode = null)
        {
            if (root == null)
            {
                nodeToAdd.Color = NodeColor.BLACK;
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
                        Console.WriteLine(nodeToAdd.Data + " is now the left child of " + currNode.Data);
                        RebalanceInsert(nodeToAdd.Parent);
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
                        Console.WriteLine(nodeToAdd.Data + " is now the right child of " + currNode.Data);
                        RebalanceInsert(nodeToAdd.Parent);
                        return true;
                    }
                    else
                    {
                        return Add(nodeToAdd, currNode.RightChild);
                    }
                }
                else
                {
                    RebalanceInsert(nodeToAdd.Parent);
                    return false;
                }
            }
        }

        /// <summary>
        /// Should be run post addition.
        /// Rebalances the tree from the parent of the added node.
        /// </summary>
        /// <param name="currNode">The parent of the node that was added.</param>
        private void RebalanceInsert(RBTNode<TData> currNode)
        {
            Console.WriteLine("Rebalancing!");

            //Keeps the root black
            //This should only happen if we get to the top or in early cases
            //Since the nodes above the root are "null", we can simply have this null case to make the root black at the end
            if (root.Color == NodeColor.RED && currNode != root)
            {
                root.Color = NodeColor.BLACK;
            }
            else if (currNode == root)
            {
                if (currNode.Color == NodeColor.RED)
                {
                    if (currNode.RightChild.Color == NodeColor.RED)
                    {
                        Right(currNode);
                        /* In this scenario, after this Rebalance, there is nothing left to rebalance
                         * So, we call rebalance on null, which will handle the root being red
                         * (if it is red), then end the rebalancing cycle.
                         */
                        RebalanceInsert(null);
                    }
                    else if (currNode.LeftChild.Color == NodeColor.RED)
                    {
                        Left(currNode);
                        /* In this scenario, after this Rebalance, there is nothing left to rebalance
                         * So, we call rebalance on null, which will handle the root being red
                         * (if it is red), then end the rebalancing cycle.
                         */
                        RebalanceInsert(null);
                    }
                }
            }
            else if (currNode == null)
            {
                //This is how we escape! This means we have rebalanced everything and the root is black.
                return;
            }
            else
            {

                //-1 if right child, 1 if left child
                int comp = currNode.Parent.Data.CompareTo(currNode.Data);

                //If the uncle of the added node is red, a promotion is possible.
                //This boolean tells us whether or not the uncle of the added node is red, telling us if a promotion is possible.
                bool promotion;
                if (currNode.Parent.RightChild != null && currNode.Parent.LeftChild != null)
                {
                    promotion = comp == 1 ? currNode.Parent.RightChild.Color == NodeColor.RED : currNode.Parent.LeftChild.Color == NodeColor.RED;
                }
                else
                {
                    promotion = false;
                }

                //If the parent of the added node isn't red, we don't need to do anything
                if (currNode.Color == NodeColor.RED)
                {
                    Console.WriteLine("I, " + currNode.Data + " am red!");
                    //If my leftChild is red, I need to fix that!
                    if (currNode.LeftChild != null && currNode.LeftChild.Color == NodeColor.RED)
                    {
                        Console.WriteLine("I, " + currNode.Data + ", see that my left child is red!");
                        if (promotion)
                        {
                            //My brother and child are red! A promotion is necessary!
                            Promotion(currNode);
                            try
                            {
                                /*If there exists a grandpa, we rebalance from there.
                                 *If not, we have reached the top of the tree, so we call RebalanceInsert on null.
                                 */
                                RebalanceInsert(currNode.Parent.Parent);
                            }
                            catch (NullReferenceException e) { Console.WriteLine("No grandpa! null it is!"); RebalanceInsert(null); }

                        }
                        else if (comp == -1)
                        {
                            //I'm a right child! My left child is red! rightleft() is needed!

                            /*
                             * Ok, so what is rightleft()?
                             * It's not actually a method in our case, but is a concept.
                             * In order to resolve the following scenario, two consecutive rotations are needed.
                             * 
                             *         a                 a                    b
                             *          \                 \                  / \
                             *           C      -->        B        -->     A   C
                             *          /                   \
                             *         B                     C
                             *         
                             * This is obviously a very basic form, as I have not included any
                             * theoretical children, but those are not necessarry for understanding this scenario.
                             * In order to handle this situation, we must preform a Left() rotation from B,
                             * then a Right() rotation from B.
                             * 
                             * Effectively, the goal of the first rotation is to move the nodes 
                             * so that they match the right rotation scenario.
                             * By placing B above C, we create a Right() rotation scenario, which we know how to handle.
                             * So, by performing a Left() rotation from B, we will create that scenario.
                             * 
                             * Since the Left() and Right() rotations are already set up to handle
                             * and children not shown in the diagram, we do not need to consider them.
                             *
                             * Note that we must save B in a temporary variable to ensure we can
                             * perform two rotations on it without losing track of it. 
                             * Note that we first call Left(B), then Right(B).        
                             */
                            currNode = currNode.LeftChild;
                            Left(currNode);
                            Print();
                            Right(currNode);
                            Print();
                            try
                            {
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e) { Console.WriteLine("No grandpa! null it is!"); RebalanceInsert(null); }
                        }
                        else
                        {
                            //I'm a left child! My left child is red! left() is needed!
                            Left(currNode);
                            Print();
                            try
                            {
                                /* If there exists a great grandpa, we rebalance from there.
                                 * If not, we have reached the top of the tree, so we call RebalanceInsert on null.
                                 * Why a great grandpa and not just a grandpa?
                                 * In this scenario, the parent of currNode will end up black, meaning the only way
                                 * we would need to Rebalance would be if the grandpa and great grandpa were both red.
                                 */
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e) { Console.WriteLine("No grandpa! null it is!"); RebalanceInsert(null); }
                        }
                    }

                    //If my rightChild is red, I need to fix that!
                    else if (currNode.RightChild != null && currNode.RightChild.Color == NodeColor.RED)
                    {
                        Console.WriteLine("I, " + currNode.Data + ", see that my right child is red!");
                        if (promotion)
                        {
                            //My brother and child are red! A promotion is necessary!
                            Promotion(currNode);
                            try
                            {
                                /*If there exists a grandpa, we rebalance from there.
                                 *If not, we have reached the top of the tree, so we call RebalanceInsert on null.
                                 */
                                RebalanceInsert(currNode.Parent.Parent);
                            }
                            catch (NullReferenceException e) { Console.WriteLine("No grandpa! null it is!"); RebalanceInsert(null); }
                        }
                        else if (comp == -1)
                        {
                            //I'm a right child! My right child is red! right() is needed!
                            Console.WriteLine("Right rotation!");
                            Right(currNode);
                            Print();
                            try
                            {
                                /* If there exists a great grandpa, we rebalance from there.
                                 * If not, we have reached the top of the tree, so we call RebalanceInsert on null.
                                 * Why a great grandpa and not just a grandpa?
                                 * In this scenario, the parent of currNode will end up black, meaning the only way
                                 * we would need to Rebalance would be if the grandpa and great grandpa were both red.
                                 */
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e) { Console.WriteLine("No grandpa! null it is!"); RebalanceInsert(null); }
                        }
                        else
                        {
                            /* I'm a left child! My right child is red! leftright() is needed!
                             * This is simply a mirror of the above rightleft() scenario,
                             * check that to understand why we rotate in this way.
                            */
                            currNode = currNode.RightChild;
                            Right(currNode);
                            Print();
                            Left(currNode);
                            Print();
                            try
                            {
                                /* If there exists a great grandpa, we rebalance from there.
                                 * If not, we have reached the top of the tree, so we call RebalanceInsert on null.
                                 * Why a great grandpa and not just a grandpa?
                                 * In this scenario, the parent of currNode will end up black, meaning the only way
                                 * we would need to Rebalance would be if the grandpa and great grandpa were both red.
                                 */
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e) { Console.WriteLine("No grandpa! null it is!"); RebalanceInsert(null); }
                        }
                    }

                    //If I'm not red, nothing needs to change! We're still balanced!
                }
            }
        }

        /*
         * A right rotation occurs when we have two red nodes that are right children.
         * In this example, capital letters are RED nodes, lowercase are black nodes
         *         a                    c
         *          \                  / \
         *           C      -->       A   D
         *          / \                \
         *         b   D                b
         * Where D is the most recently added node
         * 
         * The key to understanding rotations is understanding that a rotation consists of exactly three things:
         * 1) Two nodes shift position
         * 2) Those same two nodes swap colors
         * 3) The child of one node becomes the child of the other
         * 
         * So, in this scenario, a and C change positions
         * In the right rotation, the grandparent of the added node becomes the left child of the parent of the added node
         * So, a becomes C's left child.
         * Since there two nodes have shifted positions, they will swap their colors.
         * Since we previously had a and C, we now have A and c
         * 
         * ***IMPORTANT NOTE ABOUT COLOR SWAP:***
         * When two nodes switch colors, they will NOT ALWAYS BE DIFFERENT
         * We will see this later when we get to the "rightleft" and "leftright" scenarios
         * As such, if two nodes are red and change positions, the two nodes will "swap colors" from red to red.
         * 
         * Since c now has A as it's left child, c's previous left child, b, must be appended to A
         * Since b must be greater than A, we append b as A's right child
         * 
         * ***IMPORTANT NOTE ABOUT CHILD SHIFT:***
         * Only one child of each moved node should change.
         * In this scenario, A would maintain any previous left children, and c would maintain any previous right children
         * 
         * So, now into the details, how do we, step by step, preform this operation?
         * 
         * Our method takes in the node "parentofAddition", which is the parent of the node that has been added
         * In our diagram, this is node C.
         * Both nodes C and a must change positions, so we must simultaneously have C take a's place, 
         * while also not losing track of C's left child
         * 
         * So, for this movement, we must:
         * Set a's right child to b
         * Set b's parent to a
         * Set C's left child to a
         * Set C's parent to a's parent
         * Set a's parent to C
         * Swap the colors of a and C
         * 
         * This order may seem a bit odd, but it ensures we don't accidentally lose any references mid swap
         * 
         */
        private void Right(RBTNode<TData> parentOfAddition)
        {

            //Remember, parentOfAddition is node C in this scenario
            if (parentOfAddition.Parent == root)
            {
                Console.WriteLine("Beginning Right shift of " + parentOfAddition.Data);
                parentOfAddition.Parent.RightChild = parentOfAddition.LeftChild;        //set a's right child to b
                if (parentOfAddition.LeftChild != null)
                {
                    parentOfAddition.LeftChild.Parent = parentOfAddition.Parent;            //set b's parent to a
                }
                parentOfAddition.LeftChild = parentOfAddition.Parent;                   //set C's left child to a
                parentOfAddition.Parent = null;
                root = parentOfAddition;
                parentOfAddition.LeftChild.Parent = parentOfAddition;                   //set a's parent to C **Note: at this point, a is the leftChild of our node**
            }
            else
            {
                Console.WriteLine("Beginning Right shift of " + parentOfAddition.Data);
                parentOfAddition.Parent.RightChild = parentOfAddition.LeftChild;        //set a's right child to b
                if (parentOfAddition.LeftChild != null)
                {
                    parentOfAddition.LeftChild.Parent = parentOfAddition.Parent;            //set b's parent to a
                }
                parentOfAddition.LeftChild = parentOfAddition.Parent;                   //set C's left child to a
                parentOfAddition.Parent = parentOfAddition.Parent.Parent;               //set C's parent to a's parent
                parentOfAddition.LeftChild.Parent = parentOfAddition;                   //set a's parent to C **Note: at this point, a is the leftChild of our node**


                /*  This just checks to see if a was a left or right child,
                 *  so that a's parent has the appropriate child set to c
                 *  -1 if right child, 1 if left child
                */

                if (parentOfAddition.Parent.Data.CompareTo(parentOfAddition.Data) == -1)
                {
                    parentOfAddition.Parent.RightChild = parentOfAddition;
                }
                else
                {
                    parentOfAddition.Parent.LeftChild = parentOfAddition;
                }
            }

            //Checks that the nodes are of different colors. If they are, changes the color of each.
            //This works beacuse we only have two colors, so if they do not match, we can just convert each to the opposite color and be correct.
            //Otherwise, does nothing
            if (parentOfAddition.Color != parentOfAddition.LeftChild.Color)
            {
                parentOfAddition.Color = (NodeColor)(((int)parentOfAddition.Color + 1) % 2);                    //Reverses the color of C
                parentOfAddition.LeftChild.Color = (NodeColor)(((int)parentOfAddition.Color + 1) % 2);          //Reverses the color of a
            }

        }

        /*
         * The Left rotation scenario is simply the inverse of the Right rotation
         * As such, not much explanation beyond the previous is necessary, only that a few rights and lefts will swap.
         * However, for the sake of absolute clarity, let's go through it anyway.
         * 
         * In this example, capital letters are RED nodes, lowercase are black nodes
         *       d                b
         *      /                / \
         *     B      -->       A   D
         *    / \                  /
         *   A   c                c
         * 
         * Since we've already discussed the importance of color swaps, and what a rotation consists of,
         * We're just going to cover the exact manner in which we will perform this shift
         * 
         * Set d's left child to c
         * 
         * 
         */
        private void Left(RBTNode<TData> parentOfAddition)
        {

            //Remember, parentOfAddition is node C in this scenario
            if (parentOfAddition.Parent == root)
            {
                Console.WriteLine("Beginning Left shift of " + parentOfAddition.Data);
                parentOfAddition.Parent.LeftChild = parentOfAddition.RightChild;        //set d's left child to c
                if (parentOfAddition.RightChild != null)
                {
                    parentOfAddition.RightChild.Parent = parentOfAddition.Parent;       //set c's parent to d
                }
                parentOfAddition.RightChild = parentOfAddition.Parent;                  //set B's right child to d
                parentOfAddition.Parent = null;                                         //set the new root's parent to null
                root = parentOfAddition;                                                //set the actual root of the 
                parentOfAddition.RightChild.Parent = parentOfAddition;                  //set d's parent to b **Note: at this point, a is the leftChild of our node**
            }
            else
            {
                Console.WriteLine("Beginning Left shift of " + parentOfAddition.Data);
                parentOfAddition.Parent.LeftChild = parentOfAddition.RightChild;        //set d's left child to c
                if (parentOfAddition.RightChild != null)
                {
                    parentOfAddition.RightChild.Parent = parentOfAddition.Parent;       //set c's parent to d
                }
                parentOfAddition.RightChild = parentOfAddition.Parent;                   //set B's right child to d
                parentOfAddition.Parent = parentOfAddition.Parent.Parent;                //set B's parent to d's previous parent
                parentOfAddition.RightChild.Parent = parentOfAddition;                   //set d's parent to b **Note: at this point, a is the leftChild of our node**

                /*  This just checks to see if d was a left or right child,
                 *  so that d's parent has the appropriate child set to B
                 *  -1 if right child, 1 if left child
                 */
                if (parentOfAddition.Parent.Data.CompareTo(parentOfAddition.Data) == -1)
                {
                    parentOfAddition.Parent.RightChild = parentOfAddition;
                }
                else
                {
                    parentOfAddition.Parent.LeftChild = parentOfAddition;
                }
            }
            //Checks that the nodes are of different colors. If they are, changes the color of each.
            //This works beacuse we only have two colors, so if they do not match, we can just convert each to the opposite color and be correct.
            //Otherwise, does nothing
            if (parentOfAddition.Color != parentOfAddition.RightChild.Color)
            {
                parentOfAddition.Color = (NodeColor)(((int)parentOfAddition.Color + 1) % 2);                    //Reverses the color of C
                parentOfAddition.RightChild.Color = (NodeColor)(((int)parentOfAddition.Color + 1) % 2);          //Reverses the color of a
            }

        }

        public void Promotion(RBTNode<TData> parentOfAddition)
        {
            int comp = parentOfAddition.Parent.Data.CompareTo(parentOfAddition.Data);

            Console.WriteLine("Promotion from " + parentOfAddition.Data);

            parentOfAddition.Parent.Color = NodeColor.RED;
            parentOfAddition.Color = NodeColor.BLACK;
            if (comp == -1)
            {
                parentOfAddition.Parent.LeftChild.Color = NodeColor.BLACK;
            }
            else
            {
                parentOfAddition.Parent.RightChild.Color = NodeColor.BLACK;
            }

        }

        public bool Remove(RBTNode<TData> nodeToRemove, RBTNode<TData> currNode = null)
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
                    else
                    {
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
                    else
                    {
                        Console.WriteLine("Two kids!");
                        if (comp == 1)
                        {
                            //Was I removing a right child? Then my replacement will become a right child
                            currNode.Parent.LeftChild = currNode.RightChild;
                        }
                        else
                        {
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
