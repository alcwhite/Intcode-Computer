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
        private static List<int> IntList(string input)
        {
            var stringList = input.Split(',').ToList();
            return stringList.Select(x => int.Parse(x.Trim())).ToList();
        }
        private static OpCode GetOpCode(int fullCode)
        {
           return fullCode.ToString().Length < 3 ? (OpCode)fullCode : 
                    (OpCode)int.Parse(fullCode.ToString().Substring(fullCode.ToString().Length - 2));
        }
        private static List<int> GetParameterTypes(int fullCode, int parameterCount)
        {
            var codeString = fullCode.ToString();
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

            public Calculate(List<int> parameterValues, OpCode op, int id, List<int> outputs)
            {
                this.outputs = outputs;
                if (op == OpCode.Add)
                    outputValue = parameterValues[0] + parameterValues[1];
                if (op == OpCode.Multiply)
                    outputValue = parameterValues[0] * parameterValues[1];
                if (op == OpCode.Save)
                    outputValue = id;
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
        public static (List<int> result, List<int> outputs) RunComputer(string input, int id)
        {
            var intList = IntList(input);
            var outputs = new List<int>();
            int currentCode = intList[0];
            var currentOpCode = (OpCode)GetOpCode(currentCode);
            int currentIndex = 0;
            while (currentOpCode != OpCode.End)
            {
                parameterCounts.TryGetValue(currentOpCode, out int parameterCount);
                var parameterTypes = GetParameterTypes(currentCode, parameterCount);
                var parameterValues = parameterTypes.Select((x, i) => 
                {
                    return x == 0 ? intList[intList[currentIndex + i + 1]] : intList[currentIndex + i + 1];
                }).ToList();
                var values = new Calculate(parameterValues, currentOpCode, id, outputs); 
                int outputValue = values.outputValue;
                int? pointerLocation = values.pointerLocation;
                outputs = values.outputs;
                
                if (!pointerLocation.HasValue)
                {
                    var outputLocation = intList[currentIndex + parameterCount];
                    if (currentOpCode != OpCode.Output && currentOpCode != OpCode.JumpIfFalse && currentOpCode != OpCode.JumpIfTrue) intList[outputLocation] = outputValue;       
                    currentIndex += parameterCount + 1; 
                }
                else
                {
                    currentIndex = (int)pointerLocation;
                }
                
                currentCode = intList[currentIndex];
                currentOpCode = GetOpCode(currentCode);  
            }
            return (intList, outputs);
        }
        public static List<int> RunForOutput(string input, int output, int id)
        {
            var initialInputList = IntList(input);
            var finalList = RunComputer(input, id).result;
            var noun = initialInputList[1];
            var verb = initialInputList[2];
            while (noun < 99)
            {
                bool reset = true;
                while (verb < 99)
                {
                    if (!reset)
                        verb = initialInputList[2] + 1;
                        reset = false;
                    initialInputList = IntList(input);
                    initialInputList[1] = noun;
                    initialInputList[2] = verb;
                    IEnumerable<string> listToRun = initialInputList.Select(x => x.ToString());
                    finalList = RunComputer(string.Join(",", listToRun), id).result;
                    if (finalList[0] == output) break;
                }
                if (finalList[0] == output) break;
                noun = initialInputList[1] + 1;
                reset = true;
                verb = 0;   
            }
            return finalList;
        }
    }
}