using Xunit;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class Day2Test
{
    string puzzleInput = File.ReadAllText("../../../inputDay2.txt", Encoding.UTF8);
    string testInput1 = "1,9,10,3,2,3,11,0,99,30,40,50";
    string testOutput1 = "3500,9,10,70,2,3,11,0,99,30,40,50";
    int testResult1 = 3500;
    string testInput2 = "1,0,0,0,99";
    string testOutput2 = "2,0,0,0,99";
    int testResult2 = 2;
    string testInput3 = "2,3,0,3,99";
    string testOutput3 = "2,3,0,6,99";
    int testResult3 = 2;
    string testInput4 = "2,4,4,5,99,0";
    string testOutput4 = "2,4,4,5,99,9801";
    int testResult4 = 2;
    string testInput5 = "1,1,1,4,99,5,6,0,99";
    string testOutput5 = "30,1,1,4,2,5,6,0,99";
    int testResult5 = 30;

    [Fact]
    public void Part1_Test_1()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(testInput1, new List<int>(){0});
        var resultInStrings = result.result.Select(x => x.ToString());
        Assert.Equal(testOutput1, string.Join(',', resultInStrings));
        Assert.Equal(testResult1, result.result[0]);
    }
    [Fact]
    public void Part1_Test_2()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(testInput2, new List<int>(){0});
        var resultInStrings = result.result.Select(x => x.ToString());
        Assert.Equal(testOutput2, string.Join(',', resultInStrings));
        Assert.Equal(testResult2, result.result[0]);
    }
    [Fact]
    public void Part1_Test_3()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(testInput3, new List<int>(){0});
        var resultInStrings = result.result.Select(x => x.ToString());
        Assert.Equal(testOutput3, string.Join(',', resultInStrings));
        Assert.Equal(testResult3, result.result[0]);
    }
    [Fact]
    public void Part1_Test_4()
    {
       var result = intcode_computer.IntcodeComputer.RunComputer(testInput4, new List<int>(){0});
        var resultInStrings = result.result.Select(x => x.ToString());
        Assert.Equal(testOutput4, string.Join(',', resultInStrings));
        Assert.Equal(testResult4, result.result[0]);
    }
    [Fact]
    public void Part1_Test_5()
    {
        var result = intcode_computer.IntcodeComputer.RunComputer(testInput5, new List<int>(){0});
        var resultInStrings = result.result.Select(x => x.ToString());
        Assert.Equal(testOutput5, string.Join(',', resultInStrings));
        Assert.Equal(testResult5, result.result[0]);
    }
    [Fact]
    public void Part1_Puzzle()
    {
        var listInput = puzzleInput.Split(',').ToList();
        listInput[1] = 12.ToString();
        listInput[2] = 2.ToString();
        var input = string.Join(",", listInput);
        var result = intcode_computer.IntcodeComputer.RunComputer(input, new List<int>(){0});
        Assert.Equal(9706670, result.result[0]);
    }
    [Fact]
    public void Part2_Puzzle_Test()
    {
        var result = intcode_computer.IntcodeComputer.RunForOutput(puzzleInput, 19690720, new List<int>(){0});
        Assert.Equal(19690720, result[0]);
    }
    [Fact]
    public void Part2_Puzzle()
    {
        var result = intcode_computer.IntcodeComputer.RunForOutput(puzzleInput, 19690720, new List<int>(){0});

        Assert.Equal(2552, (100 * result[1]) + result[2]);
    }
}