using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class ImmediateWindow
{
    [Test]
    public void OutputStuff()
    {
        Console.WriteLine("Hello World");

        var q = new Queue<int>();
        q.Enqueue(1);
        var qv = q.Dequeue();
        
        var st = new Stack<int>();
        st.Push(1);
        var stv = st.Pop();
        
        
        
        Assert.IsTrue(true);
    }
}