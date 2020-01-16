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
        // technically these are parameterCounts + 1 -- but elsewhere compensates
        static Dictionary<int, int> parameterCounts = new Dictionary<int, int>()
        {
            {1, 4},
            {2, 4},
            {3, 2},
            {4, 2},
            {5, 3},
            {6, 3},
            {7, 4},
            {8, 4},
            {99, 0}
        };
        static List<int> phaseSettingOptions = new List<int>(){0, 1, 2, 3, 4};
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
            while (parameterTypeList.Count < parameterCount - 1)
            {
                parameterTypeList.Insert(0, 0);
            }
            var parameterTypes = new List<int>();
            parameterTypeList.Reverse();
            return parameterTypeList;
        }
        public static (List<int> result, List<int> outputs) RunComputer(string program, List<int> input)
        {
            var intList = IntList(program);
            var outputs = new List<int>();
            int currentCode = intList[0];
            int currentOpCode = GetOpCode(currentCode);
            int currentIndex = 0;
            var currentInput = input[0];
            var currentInputIndex = 0;
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
                int outputValue = 0;
                int? pointerLocation = null;
                if (op == "ADD")
                    outputValue = parameterValues[0] + parameterValues[1];
                if (op == "MULTIPLY")
                    outputValue = parameterValues[0] * parameterValues[1];
                if (op == "SAVE")
                {
                    outputValue = currentInput;
                    currentInputIndex = input.Count > currentInputIndex + 1 ? currentInputIndex + 1 : 0;
                    currentInput = input[currentInputIndex];
                }
                if (op == "OUTPUT")
                    outputs.Add(parameterValues[0]);
                if (op == "JUMP-IF-TRUE")
                    if (parameterValues[0] != 0) pointerLocation = parameterValues[1];
                if (op == "JUMP-IF-FALSE")
                    if (parameterValues[0] == 0) pointerLocation = parameterValues[1];
                if (op == "LESS THAN")
                    outputValue = parameterValues[0] < parameterValues[1] ? 1 : 0;
                if (op == "EQUALS")
                    outputValue = parameterValues[0] == parameterValues[1] ? 1 : 0;
               
                if (!pointerLocation.HasValue)
                {
                    var outputLocation = intList[currentIndex + parameterCount - 1];
                    if (op != "OUTPUT" && op != "JUMP-IF-FALSE" && op != "JUMP-IF-TRUE") intList[outputLocation] = outputValue;
                    
                    currentIndex += parameterCount; 
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
        public static List<int> RunForOutput(string program, int output, List<int> input)
        {
            var initialProgramList = IntList(program);
            var finalList = RunComputer(program, input).result;
            var noun = initialProgramList[1];
            var verb = initialProgramList[2];
            while (noun < 99)
            {
                bool reset = true;
                while (verb < 99)
                {
                    if (!reset)
                        verb = initialProgramList[2] + 1;
                        reset = false;
                    initialProgramList = IntList(program);
                    initialProgramList[1] = noun;
                    initialProgramList[2] = verb;
                    IEnumerable<string> listToRun = initialProgramList.Select(x => x.ToString());
                    finalList = RunComputer(string.Join(",", listToRun), input).result;
                    if (finalList[0] == output) break;
                }
                if (finalList[0] == output) break;
                noun = initialProgramList[1] + 1;
                reset = true;
                verb = 0;   
            }
            return finalList;
        }

        public static List<List<int>> SignalPermutations()
        {

            var count = phaseSettingOptions.Count;
            var permutationCount = 1;
            var allOptions = new List<List<int>>(){phaseSettingOptions};
            var currentIndex = 0;
            var currentNumber = phaseSettingOptions[0];

            while (count > 1)
            {
                permutationCount *= count;
                count--;
            }
            while (allOptions.Count < permutationCount)
            {
                var lastOption = allOptions[allOptions.Count - 1];
                var thisOption = new List<int>();
                lastOption.ForEach(x => thisOption.Add(x));
                var nextIndex = currentIndex < phaseSettingOptions.Count - 1 ? currentIndex + 1 : 0;
                var nextNumber = thisOption[nextIndex];
                currentNumber = thisOption[currentIndex];
                thisOption[nextIndex] = currentNumber;
                thisOption[currentIndex] = nextNumber;
                allOptions.Add(thisOption);
                currentIndex = nextIndex;
            }
            return allOptions;
        }

        public static (List<int> order, int signal) MaxThrusterSignal(string program)
        {
            var allSignals = new List<(List<int> order, int signal)>();
            var allSettingOrders = SignalPermutations();
            allSettingOrders.ForEach(settingOrder => {
                var lastOutput = 0;
                settingOrder.ForEach(currentSetting => {
                var input = new List<int>(){currentSetting, lastOutput};
                var output = RunComputer(program, input);
                var finalOutput = output.outputs;
                lastOutput = finalOutput[0];
                });
                var signalInfo = (settingOrder, lastOutput);
                allSignals.Add(signalInfo);
            });           
            var justSignals = new List<int>();
            allSignals.ForEach(x => justSignals.Add(x.signal));
            Console.WriteLine(justSignals);
            var maxSignal = justSignals.Max();
            var result = allSignals.First(x => x.signal == maxSignal);
            Console.WriteLine(result);
            return result;
        }
    }
}