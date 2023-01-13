using System;
using System.Text;

public class Tree
{
    public class RefNode { public Node p; }

    private RefNode root = new RefNode();

    public void AddNode(int val)
    {
        if (root.p == null) { root.p = new Leaf() { value = val }; return; }

        if (root.p is Leaf)
        {
            var lastVal = (root.p as Leaf).value;

            int xor = lastVal ^ val;
            int pos = 0;
            while ((xor >>= 1)!=0) // unroll for more speed...
            {
                pos++;
            }

            Branch newBranch = new Branch();
            newBranch.level = pos;
            newBranch.footprint = val;
            if ((val | (1 << pos)) != 0)
            {
                newBranch.LeftNode = root.p;
                newBranch.RightNode = new Leaf() { value = val };
            } else {
                newBranch.LeftNode = new Leaf() { value = val };
                newBranch.RightNode = root.p;
            }
            root.p = newBranch;
            return;
        }

        if (root.p is Branch)
        {
            RefNode curBranch = root;

            while (curBranch.p is Branch)
            {
                int xor = (curBranch.p as Branch).footprint ^ val;
                int pos = 0;
                while ((xor >>= 1)!=0) // unroll for more speed...
                {
                    pos++;
                }
                if (pos > (curBranch.p as Branch).level) {/* add branch*/
                    Branch newBranch = new Branch();
                    newBranch.level = pos;
                    newBranch.footprint = val;
                    if ((val | (1 << pos)) != 0)
                    {
                        newBranch.LeftNode = curBranch.p;
                        newBranch.RightNode = new Leaf() { value = val };
                    } else {
                        newBranch.LeftNode = new Leaf() { value = val };
                        newBranch.RightNode = curBranch.p;
                    }
                    curBranch.p = newBranch;
                    return;
                }
                else { // navigate to child 
                    Console.WriteLine("branch {0}", Utils.toBin((curBranch.p as Branch).footprint));
                    if(((val | (1 << (curBranch.p as Branch).level)) != 0)) curBranch.p = (curBranch.p as Branch).RightNode as Branch;
                    else curBranch.p = (curBranch.p as Branch).LeftNode as Branch;
                }

            }
        }
    }

    public void Print() 
    {
        PrintNode(root.p, "");
    }
    public void PrintNode(Node node, string	linePrepend) 
    {
        if (node is Leaf)
            Console.WriteLine("{0}[{1}]", linePrepend, node.ToString());
        else if (node is Branch) {
            Console.WriteLine("{0}[branch]", linePrepend);
            PrintNode((node as Branch).LeftNode, linePrepend + "|-");
            PrintNode((node as Branch).RightNode, linePrepend + "|-");
        }
    }
}

public abstract class Node { }
public class Branch : Node {
    public Node LeftNode;
    public Node RightNode;
    public int level;
    public int footprint;
}
public class Leaf : Node {
    public int value;
    public override string ToString() {
        return Utils.toBin(value);
    }
}

public class Program
{
    static void Main(string[] args)
    {
        Tree tree = new Tree();

        Console.WriteLine("-> adding "+Utils.toBin(0b00000100));
        tree.AddNode(0b00000100);
        tree.Print();

        Console.WriteLine("-> adding "+Utils.toBin(0b00010100));
        tree.AddNode(0b00010100);
        tree.Print();

        Console.WriteLine("-> adding "+Utils.toBin(0b01010000));
        tree.AddNode(0b01010000);
        tree.Print();

        Console.WriteLine("-> adding "+Utils.toBin(0b01110100));
        tree.AddNode(0b11010100);
        tree.Print();

        Console.WriteLine("-> adding "+Utils.toBin(0b01010100));
        tree.AddNode(0b01010100);
        tree.Print();
    }
}



public static class Utils
{
    public static string toBin(int value)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 1 << 7; i != 0; i >>= 1) sb.Append((value & i) > 0 ? "1" : "0");
        return sb.ToString();
    }
    public static int fromBin(string value)
    {
        int ret = 0;
        char[] aValue = value.ToCharArray();
        for (int i = 0; i < value.Length; i++) if (aValue[i] == '1') ret += 1 << i;
        return ret;
    }
}

