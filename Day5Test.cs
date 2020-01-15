using Xunit;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Day5Test
{
    string puzzleInput = File.ReadAllText("../../../inputDay5.txt", Encoding.UTF8);
    string jumpTest1Input = "3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9";
    string jumpTest2Input = "3,3,1105,-1,9,1101,0,0,12,4,12,99,1";

    [Fact]
    public void Part1_Puzzle()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(puzzleInput, 1);
        Assert.Equal(4511442, result.outputs[result.outputs.Count - 1]);
    }
    [Fact]
    public void JumpTest1()
    {
        Assert.Equal(1, intcode_computer.IntcodeComputer.RunComputer(jumpTest1Input, 5).outputs[0]);
    }
    [Fact]
    public void JumpTest1_ZeroInput()
    {
        Assert.Equal(0, intcode_computer.IntcodeComputer.RunComputer(jumpTest1Input, 0).outputs[0]);
    }
    [Fact]
    public void JumpTest2()
    {
        Assert.Equal(1, intcode_computer.IntcodeComputer.RunComputer(jumpTest2Input, 5).outputs[0]);
    }
    [Fact]
    public void JumpTest2_ZeroInput()
    {
        Assert.Equal(0, intcode_computer.IntcodeComputer.RunComputer(jumpTest2Input, 0).outputs[0]);
    }
    [Fact]
    public void Large_Test()
    {
        string input = "3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99";
        int input1 = 3;
        int input2 = 8;
        int input3 = 10;
        Assert.Equal(999, intcode_computer.IntcodeComputer.RunComputer(input, input1).outputs[0]);
        Assert.Equal(1000, intcode_computer.IntcodeComputer.RunComputer(input, input2).outputs[0]);
        Assert.Equal(1001, intcode_computer.IntcodeComputer.RunComputer(input, input3).outputs[0]);


    }
    [Fact]
    public void Part2_Puzzle()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(puzzleInput, 5);
        Assert.Equal(12648139, result.outputs[0]);
    }
}