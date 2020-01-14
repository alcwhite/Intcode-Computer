using Xunit;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Day5Test
{
    string puzzleInput = File.ReadAllText("../../../inputDay5.txt", Encoding.UTF8);
    [Fact]
    public void Part1_Puzzle()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(puzzleInput, 1);
        Assert.Equal(4511442, result.outputs[result.outputs.Count - 1]);
    }
    [Fact]
    public void Part2_Puzzle()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(puzzleInput, 5);
        Assert.Equal(4511442, result.outputs[0]);
    }
}