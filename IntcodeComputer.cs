using System;
using System.Collections.Generic;
using System.Linq;

namespace intcode_computer
{
    public static class IntcodeComputer
    {
         enum OpCode
        {
            Add = 1,
            Multiply = 2,

            Save = 3,
            Output = 4,
            JumpIfTrue = 5,
            JumpIfFalse = 6,
            LessThan = 7,
            Equals = 8,
            End = 99
        };
        static Dictionary<OpCode, int> parameterCounts = new Dictionary<OpCode, int>()
        {
            {OpCode.Add, 3},
            {OpCode.Multiply, 3},
            {OpCode.Save, 1},
            {OpCode.Output, 1},
            {OpCode.JumpIfTrue, 2},
            {OpCode.JumpIfFalse, 2},
            {OpCode.LessThan, 3},
            {OpCode.Equals, 3},
            {OpCode.End, 0}
        };
        private static List<int> ConvertProgramStringToList(string input)
        {
            var stringList = input.Split(',').ToList();
            return stringList.Select(x => int.Parse(x.Trim())).ToList();
        }
        private static OpCode GetOpCode(int fullOpCode, string codeString)
        {
            return (OpCode)(codeString.Length < 3 ? fullOpCode : 
                    int.Parse(codeString.Substring(codeString.Length - 2)));
        }
        private static List<int> GetParameterTypes(int fullOpCode, int parameterCount, string codeString)
        {
            var parameterTypeList = codeString.Length > 2 ? codeString.Substring(0, codeString.Length - 2).ToList().Select(x => int.Parse(x.ToString())).ToList() : new List<int>();
            while (parameterTypeList.Count < parameterCount)
            {
                parameterTypeList.Insert(0, 0);
            }
            var parameterTypes = new List<int>();
            parameterTypeList.Reverse();
            return parameterTypeList;
        }
        private class Calculate
        {
            public int? pointerLocation;
            public int outputValue;
            public List<int> outputs;

            public Calculate(List<int> parameterValues, OpCode op, int valueForOutputOp, List<int> outputs)
            {
                this.outputs = outputs;
                if (op == OpCode.Add)
                    outputValue = parameterValues[0] + parameterValues[1];
                if (op == OpCode.Multiply)
                    outputValue = parameterValues[0] * parameterValues[1];
                if (op == OpCode.Save)
                    outputValue = valueForOutputOp;
                if (op == OpCode.Output)
                    this.outputs.Add(parameterValues[0]);
                if (op == OpCode.JumpIfTrue)
                    if (parameterValues[0] != 0) pointerLocation = parameterValues[1];
                if (op == OpCode.JumpIfFalse)
                    if (parameterValues[0] == 0) pointerLocation = parameterValues[1];
                if (op == OpCode.LessThan)
                    outputValue = parameterValues[0] < parameterValues[1] ? 1 : 0;
                if (op == OpCode.Equals)
                    outputValue = parameterValues[0] == parameterValues[1] ? 1 : 0;
            }
               
        }
        public static (List<int> result, List<int> outputs) RunComputer(string input, int valueForOutputOp)
        {
            var programAsList = ConvertProgramStringToList(input);
            var outputs = new List<int>();
            int currentCode = programAsList[0];
            var currentOpCode = (OpCode)GetOpCode(currentCode, currentCode.ToString());
            int currentIndex = 0;
            while (currentOpCode != OpCode.End)
            {
                parameterCounts.TryGetValue(currentOpCode, out int parameterCount);
                var parameterTypes = GetParameterTypes(currentCode, parameterCount, currentCode.ToString());
                var parameterValues = parameterTypes.Select((x, i) => 
                {
                    return x == 0 ? programAsList[programAsList[currentIndex + i + 1]] : programAsList[currentIndex + i + 1];
                }).ToList();
                var values = new Calculate(parameterValues, currentOpCode, valueForOutputOp, outputs); 
                int outputValue = values.outputValue;
                int? pointerLocation = values.pointerLocation;
                outputs = values.outputs;
                
                if (!pointerLocation.HasValue)
                {
                    var outputLocation = programAsList[currentIndex + parameterCount];
                    if (currentOpCode != OpCode.Output && currentOpCode != OpCode.JumpIfFalse && currentOpCode != OpCode.JumpIfTrue) programAsList[outputLocation] = outputValue;       
                    currentIndex += parameterCount + 1; 
                }
                else
                {
                    currentIndex = (int)pointerLocation;
                }
                
                currentCode = programAsList[currentIndex];
                currentOpCode = GetOpCode(currentCode, currentCode.ToString());  
            }
            return (programAsList, outputs);
        }
        public static List<int> RunForOutput(string input, int expectedOutput, int valueForOutputOp)
        {
            var programAsIntList = ConvertProgramStringToList(input);
            var finalList = RunComputer(input, valueForOutputOp).result;
            var noun = programAsIntList[1];
            var verb = programAsIntList[2];
            while (noun < 99)
            {
                bool reset = true;
                while (verb < 99)
                {
                    if (!reset) verb = programAsIntList[2] + 1;
                    reset = false;
                    programAsIntList = ConvertProgramStringToList(input);
                    programAsIntList[1] = noun;
                    programAsIntList[2] = verb;
                    IEnumerable<string> listToRun = programAsIntList.Select(x => x.ToString());
                    finalList = RunComputer(string.Join(",", listToRun), valueForOutputOp).result;
                    if (finalList[0] == expectedOutput) break;
                }
                if (finalList[0] == expectedOutput) break;
                noun = programAsIntList[1] + 1;
                reset = true;
                verb = 0;   
            }
            return finalList;
        }
    }
}