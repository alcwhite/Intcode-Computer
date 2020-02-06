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
        private static int GetOpCode(int fullOpCode, string codeString)
        {
            return (codeString.Length < 3 ? fullOpCode : 
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
        private class Calculator
        {
            public virtual int parameterCount => 0;
            public virtual string opName => "Default";
            
            public virtual int? OutputValue(List<int> parameterValues, int valueForOutputOp)
            {
                return null;
            }
            public virtual int? PointerLocation(List<int> parameterValues)
            {
                return null;
            }
            public virtual List<int> Outputs(List<int> parameterValues, List<int> outputs)
            {
                return outputs;
            }
               
        }
        private static Calculator CreateCalculator(int op)
        {
            switch(op)
            {
                case 1:
                    return new Add();
                case 2:
                    return new Multiply();
                case 3:
                    return new Save();
                case 4:
                    return new Output();
                case 5:
                    return new JumpIfTrue();
                case 6:
                    return new JumpIfFalse();
                case 7:
                    return new LessThan();
                case 8:
                    return new Equal();
                default:
                    throw new Exception();
            }
        }
        private class Add : Calculator
        {
            public override int parameterCount => 3;
            public override string opName => "Add";
            public override int? OutputValue(List<int> parameterValues, int valueForOutputOp)
            {
                return parameterValues[0] + parameterValues[1];
            }
        }
        private class Multiply : Calculator
        {
            public override int parameterCount => 3;
            public override string opName => "Multiply";
            public override int? OutputValue(List<int> parameterValues, int valueForOutputOp)
            {
                return parameterValues[0] * parameterValues[1];
            }
        }
        private class Save : Calculator
        {
            public override int parameterCount => 1;
            public override string opName => "Save";
            public override int? OutputValue(List<int> parameterValues, int valueForOutputOp)
            {
                return valueForOutputOp;
            }
        }
        private class Output : Calculator
        {
            public override int parameterCount => 1;
            public override string opName => "Output";
            public override List<int> Outputs(List<int> parameterValues, List<int> outputs)
            {
                var finalOutputs = outputs;
                finalOutputs.Add(parameterValues[0]);
                return finalOutputs;
            }
        }
        private class JumpIfTrue : Calculator
        {
            public override int parameterCount => 2;
            public override string opName => "JumpIfTrue";
            public override int? PointerLocation(List<int> parameterValues)
            {
                if (parameterValues[0] != 0) return parameterValues[1];
                return null;
            }
        }
        private class JumpIfFalse : Calculator
        {
            public override int parameterCount => 2;
            public override string opName => "JumpIfFalse";
            public override int? PointerLocation(List<int> parameterValues)
            {
                if (parameterValues[0] == 0) return parameterValues[1];
                return null;
            }
        }
        private class LessThan : Calculator
        {
            public override int parameterCount => 3;
            public override string opName => "LessThan";
            public override int? OutputValue(List<int> parameterValues, int valueForOutputOp)
            {
                return parameterValues[0] < parameterValues[1] ? 1 : 0;
            }
        }
        private class Equal : Calculator
        {
            public override int parameterCount => 3;
            public override string opName => "Equal";
            public override int? OutputValue(List<int> parameterValues, int valueForOutputOp)
            {
                return parameterValues[0] == parameterValues[1] ? 1 : 0;
            }
        }
        public static (List<int> result, List<int> outputs) RunComputer(string input, int valueForOutputOp)
        {
            var programAsList = ConvertProgramStringToList(input);
            var outputs = new List<int>();
            int currentCode = programAsList[0];
            var currentOpCode = GetOpCode(currentCode, currentCode.ToString());
            int currentIndex = 0;
            while (currentOpCode != 99)
            {
                var calculator = CreateCalculator(currentOpCode); 
                if (calculator.opName == "Default") throw new Exception();
                var parameterTypes = GetParameterTypes(currentCode, calculator.parameterCount, currentCode.ToString());
                var parameterValues = parameterTypes.Select((x, i) => 
                {
                    return x == 0 ? programAsList[programAsList[currentIndex + i + 1]] : programAsList[currentIndex + i + 1];
                }).ToList();
                
                int? outputValue = calculator.OutputValue(parameterValues, valueForOutputOp);
                int? pointerLocation = calculator.PointerLocation(parameterValues);
                outputs = calculator.Outputs(parameterValues, outputs);
                
                if (!pointerLocation.HasValue)
                {
                    var outputLocation = programAsList[currentIndex + calculator.parameterCount];
                    if (calculator.opName != "Output" && calculator.opName != "JumpIfFalse" && calculator.opName != "JumpIfTrue") programAsList[outputLocation] = (int)outputValue;       
                    currentIndex += calculator.parameterCount + 1; 
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