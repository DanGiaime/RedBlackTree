This is my implementation of a Red-Black Tree in C# (created without any pseudo-code!).
It took a while, but I learned a lot, and I hope to teach you as well.
I'm going to cover all of the things you need to know to fully understand a Red-Black Tree,
from the intuition of self-balancing, to the various rotations and recolorings that handle the implementation.

## Self-Balancing Intuition

A Red-Black Tree is what is known as a self-balancing Binary Search Tree. So, what is a self-balancing Binary Search Tree?

Well, we already know what a Binary Search Tree is, right? A Binary Search Tree is a data structure that holds data in a node-based, tree-like structure, where any given node has up to two children, with the left child being "less than" the parent, and the right child being "greater than" the parent. However, Binary Search Trees have an inherent flaw, known as imbalance.

##### What is Imbalance?
Imbalance is what happens when data is lopsided, or the order in which data is put into a tree leads to a poor structure. For example, imagine you loaded the entire alphabet, A-Z, into a Binary Search Tree in order. You first load in A, which becomes the root. You then load in B, which becomes A's right child. You then load in C, which become's B's right child. You then load in D, which becomes C's right child. You continue this process, and when you've inserted Z, you realize that your tree isn't a tree at all, it's basically just a linked list!

This is the problem of Imbalance. Although Binary Search Tree's boast an O(logn) search time, such a thing is never guaranteed, and is frequently not present. This is a result of skewed data, or just non-optimized inserts. So, optimally, we want our tree to be exactly equal, or at least really close, with any given parent having an equal number of children on either side. In this way, we could more realistically begin to guarantee the O(logn) search time.

So, how do we rectify this imbalance? How can we start to move towards that optimal balanced tree? Well, what if we had first inserted L into our tree? Then, at least the tree would have the same number of nodes on the left as on the right. Perhaps we could even more optimally insert the nodes, to get even closer to the best-case tree. This is just one simple improvement we could give to our tree. However, so much more can and has been done in the field of data structures and algorithms to combat this problem.

Many smart people came together and came up with several strategies for resloving this issue of imbalance. This resulted in many different "Self-Balancing" Binary Search Trees, which have logic built-in to reorder the nodes in the tree to acheive a more optimal arrangement for the sake of search times. To name a few:
* 2-3 tree
* AA tree
* AVL tree
* Red-black tree
* Scapegoat tree
* Splay tree
* Treap

Here I will attempt to describe the Red-Black Tree, and its strategies for handling such problems, via coloring, rotations, and recursion.

## Red-Black Tree (Basic Overview)
A Red-Black Tree aims to combat the problem of imbalance using two main tools: colors and rotations.

The difference between a node in a Binary Search Tree and a node in a Red-Black Tree is that a node in a Red-Black Tree has an associated color. This color is, as you might guess, either red or black.

The rules for colors are as follows:
1. Every node is either red or black
2. Every leaf (NULL) is black
3. If a node is red, then both its children are black
4. Every simple path from a node to a descendent leaf contains the same number of black nodes.

So what do those mean in English? Well, the first rule is pretty straightforward. Each node will have a color value, and that value will only ever be red or black.

The second rule is a bit odd. In a Red-Black Tree, every node is understood to have two children at all times. If a node has two actual children, that's great! If a node has anything less than that, its missing children will be filled in by black "NULL" nodes. So, although these nodes may not actually "exist" in the tree, they are understood to be there. The actual existence/nonexistence is an implementation detail. In my implementation, I chose not to have any literal existence of these nodes; Simply null values for children that were treated as black when balancing. It may seem odd that we have such a rule, but when we view the balancing strategies later on, this will make more sense.

The third rule is probably the most important in terms of insertion. Every red node is guaranteed to have two black children at all times. Since every node enters the tree red, this causes a rebalance every time a node is inserted below a red node. How this rebalance takes place will be covered later.

The fourth rule speaks towards a concept known as "Black Height". "Black Height" is essentially the number of Black Nodes from any given node to the bottom of the tree (any NULL leaf node). In order for the tree to be valid, "Black Height" must be equal for every path from a given node.  Different nodes may have different black heights, but a given node can have exactly one black height shared by every path from that node to any (NULL) leaf node.

All of these rules will seem much more reasonable in the next sections, when we see how they're used to make rebalancing decisions.

## Insertion

Insertion in a Red-Black Tree can be compared to insertion in a BST (Binary Search Tree) combined with insertion into a Heap.

RBT (Red-Black Tree) insertion begins with a [Standard BST Insertion](http://quiz.geeksforgeeks.org/binary-search-tree-set-1-search-and-insertion/). In essence, we traverse the tree to find where our Node fits in (based solely on the value of the data in the node), then place the Node there.

RBT insertion continues with a method similar to that of ["Heapify" or "Sift"](http://www.cs.yale.edu/homes/aspnes/pinewiki/Heaps.html) (check the "Bottom-up heapification" section). After a standard BST insertion, a RBT will run a "Rebalance-Insert" method on the tree. This method attempt to, as it says, rebalance the tree after an insert. Sometimes, nothing needs to be done, but other times rules are broken, and steps must be taken to reclaim the correct RBT structure.

These steps consist of 4 main rotations, or a recoloring. Left, Right, LeftRight, and RightLeft. Each of these methods describes the rotation(s) it will preform based on the arrangement and colors of the node we have inserted, that node's parent, sibling, and possible nephews. Similarly, in specific cases, we will recolor based on this same arrangement and coloring in order to maintain a valid structure.

First, we will cover a Right Rotation, as is described in the comments of my implementation.

__A right rotation occurs when we have two red nodes that are right children of their respective parents.__

    In this example, capital letters are RED nodes, lowercase are black nodes

             a                    c
              \                  / \
               C      -->       A   D
              / \                \
             b   D                b

     Where D is the most recently added node

     The key to understanding rotations is understanding that
     a rotation consists of exactly three things:
     1) Two nodes shift position
     2) Those same two nodes swap colors
     3) The child of one node becomes the child of the other

     So, in this scenario, a and C change in the right rotation,
     the grandparent of the added node
     becomes the left child of the parent of the added.
     So, a becomes C's left child.
     Since there two nodes have shifted positions, they will swap their colors.
     Since we previously had a and C, we now have A and c.

     ***IMPORTANT NOTE ABOUT COLOR SWAP:***
     When two nodes switch colors, they will NOT ALWAYS BE DIFFERENT.
     We will see this later when we get to the
     "rightleft" and "leftright" scenarios.
     As such, if two nodes are red and change positions,
     the two nodes will "swap colors" from red to red.

     Since c now has A as it's left child,
     c's previous left child, b, must be appended to A.
     Since b must be greater than A, we append b as A's right child

     ***IMPORTANT NOTE ABOUT CHILD SHIFT:***
     Only one child of each moved node should change.
     In this scenario, A would maintain any previous left children,
     and c would maintain any previous right children

__The Left rotation scenario is simply the inverse of the Right rotation. The Left rotation scenario occurs when we have two red nodes that are left children of their respective parents.__

  As such, not much explanation beyond the previous is necessary, only that a few rights and lefts will swap. However, for the sake of absolute clarity, let's go through it anyway.

    In this example,
    capital letters are RED nodes,
    lowercase are BLACK nodes.

            d                b
           /                / \
          B      -->       A   D
         / \                  /
        A   c                c

__The rightleft rotation scenario occurs when we have two red nodes that are right and left children of their parents, respectively__

Ok, so what is rightleft()?
It's not actually a method in our case, but is a concept.
In order to resolve the following scenario, two consecutive rotations are needed.

         a                 a                    b
          \                 \                  / \
           C      -->        B        -->     A   C
          /                   \
         B                     C

    This is obviously a very basic form,
    as I have not included any
    theoretical children, but those are not
    necessarry for understanding this scenario.
    In order to handle this situation,
    we must preform a Left() rotation from B,
    then a Right() rotation from B.

    Effectively, the goal of the first rotation is
    to move the nodes so that they
    match the right rotation scenario.
    By placing B above C, we create a
    Right() rotation scenario,
    which we know how to handle.
    So, by performing a Left() rotation from B,
    we will create that scenario.

__The leftright rotation is simply the inverse of the rightleft rotation. The leftright rotation scenario occurs when we have two red nodes that are left and right children of their parents, respectively__

  As such, not much explanation beyond the previous is necessary,
  only that a few rights and lefts will swap.
  However, for the sake of absolute clarity,
  let's go through it anyway.


           a               a                b
          /               /                / \
         C      -->      B        -->     A   C
          \             /
           B           C

That covers the basic rotations. We can see where these occur upon insertion relatively easily, as in almost any scenario where we insert a Red node below another Red node, we're going to rotate. However, there exists one scenario that fits this description in which we will not rotate. This scenario is as follows, and is known as "Promotion".


## Deletion

 Removal is a bit confusing.
 Unlike insertion, which depends on
 the uncle of the inserted node,
 removal depends on the sibling of the node to be removed.
 The following notation will be used the removal process:

              p
             / \
            v   s
           /     \
          u       r

     Where v is the node to remove,
     u is the child of the (left or right) of v,
     p is the parent of u,
     s is the child of p this is not v,
     r is the red child of s
     (right or left, or right in the case of both).

 Removal has many, many cases based on the color of these nodes.
 The important thing to keep in mind is
 that the node to be removed will always have either
 one child or none, based on standard BST removal.
