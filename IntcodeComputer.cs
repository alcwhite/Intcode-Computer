using System;
using System.Collections.Generic;
using System.Linq;

namespace intcode_computer
{
    public static class IntcodeComputer
    {
         static Dictionary<int, string> opCodes = new Dictionary<int, string>()
        {
            {1, "ADD"},
            {2, "MULTIPLY"},
            {3, "SAVE"},
            {4, "OUTPUT"},
            {5, "JUMP-IF-TRUE"},
            {6, "JUMP-IF-FALSE"},
            {7, "LESS THAN"},
            {8, "EQUALS"},
            {99, "END"}
        };
        static Dictionary<int, int> parameterCounts = new Dictionary<int, int>()
        {
            {1, 3},
            {2, 3},
            {3, 1},
            {4, 1},
            {5, 2},
            {6, 2},
            {7, 3},
            {8, 3},
            {99, 0}
        };
        private static List<int> IntList(string input)
        {
            var stringList = input.Split(',').ToList();
            return stringList.Select(x => int.Parse(x.Trim())).ToList();
        }
        private static int GetOpCode(int fullCode)
        {
           return fullCode.ToString().Length < 3 ? fullCode : 
                    fullCode.ToString().Length == 3 ? int.Parse(fullCode.ToString().Substring(1)) : 
                    fullCode.ToString().Length == 4 ? int.Parse(fullCode.ToString().Substring(2)) : 
                    fullCode.ToString().Length == 5 ? int.Parse(fullCode.ToString().Substring(3)) :
                    0;
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

            public Calculate(List<int> parameterValues, string op, int id, List<int> outputs)
            {
                this.outputs = outputs;
                if (op == "ADD")
                    outputValue = parameterValues[0] + parameterValues[1];
                if (op == "MULTIPLY")
                    outputValue = parameterValues[0] * parameterValues[1];
                if (op == "SAVE")
                    outputValue = id;
                if (op == "OUTPUT")
                    this.outputs.Add(parameterValues[0]);
                if (op == "JUMP-IF-TRUE")
                    if (parameterValues[0] != 0) pointerLocation = parameterValues[1];
                if (op == "JUMP-IF-FALSE")
                    if (parameterValues[0] == 0) pointerLocation = parameterValues[1];
                if (op == "LESS THAN")
                    outputValue = parameterValues[0] < parameterValues[1] ? 1 : 0;
                if (op == "EQUALS")
                    outputValue = parameterValues[0] == parameterValues[1] ? 1 : 0;
            }
               
        }
        public static (List<int> result, List<int> outputs) RunComputer(string input, int id)
        {
            var intList = IntList(input);
            var outputs = new List<int>();
            int currentCode = intList[0];
            int currentOpCode = GetOpCode(currentCode);
            int currentIndex = 0;
            while (opCodes.Keys.Contains(currentOpCode) && currentOpCode != 99)
            {
                if (currentOpCode == 99) break;
                opCodes.TryGetValue(currentOpCode, out string op);
                parameterCounts.TryGetValue(currentOpCode, out int parameterCount);
                var parameterTypes = GetParameterTypes(currentCode, parameterCount);
                var parameterValues = parameterTypes.Select((x, i) => 
                {
                    return x == 0 ? intList[intList[currentIndex + i + 1]] : intList[currentIndex + i + 1];
                }).ToList();
                var values = new Calculate(parameterValues, op, id, outputs); 
                int outputValue = values.outputValue;
                int? pointerLocation = values.pointerLocation;
                outputs = values.outputs;
                
                if (!pointerLocation.HasValue)
                {
                    var outputLocation = intList[currentIndex + parameterCount];
                    if (op != "OUTPUT" && op != "JUMP-IF-FALSE" && op != "JUMP-IF-TRUE") intList[outputLocation] = outputValue;       
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