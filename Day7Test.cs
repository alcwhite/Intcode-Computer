using Xunit;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Day7Test
{
    string puzzleInput = File.ReadAllText("../../../inputDay7.txt", Encoding.UTF8);

    [Fact]
    public void Part1_Puzzle()
    {
        var result = intcode_computer.IntcodeComputer.MaxThrusterSignal(puzzleInput);
        Assert.Equal(255590, result.signal);
    }
    [Fact]
    public void Part1_Test()
    {
        var input = "3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0";
        var result = intcode_computer.IntcodeComputer.MaxThrusterSignal(input);
        Assert.Equal(new List<int>(){0,1,2,3,4}, result.order);
        Assert.Equal(54321, result.signal);
    }
    [Fact]
    public void Permutations_Test()
    {
        var result = intcode_computer.IntcodeComputer.SignalPermutations();
        var resultCount = new List<int>();
        foreach (var thisResult in result)
        {
            resultCount.Add(thisResult.Distinct().Count());
        }
        Assert.Equal(120, result.Count);
        Assert.Equal(120, result.Distinct().Count());
        Assert.Equal(1, resultCount.Distinct().Count());
    }
}