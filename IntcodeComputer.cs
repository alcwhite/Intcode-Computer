using System;
using System.Collections.Generic;
using System.Linq;

namespace intcode_computer
{
    public static class IntcodeComputer
    {
        public static (List<int> result, List<int> outputs) RunComputer(string input, int valueForOutputOp)
        {
            var programAsList = ConvertProgramStringToList(input);
            var outputs = new List<int>();
            int currentCode = programAsList[0];
            var currentOpCode = GetOpCode(currentCode, currentCode.ToString());
            int currentIndex = 0;
            while (currentOpCode != 99)
            {
                var calculator = CreateCalculator(currentOpCode, currentIndex, currentCode, programAsList); 
                
                calculator.SetParameterValues(currentCode, programAsList, currentIndex);
                
                int? outputValue = calculator.OutputValue(valueForOutputOp);
                int? pointerLocation = calculator.PointerLocation();
                outputs = calculator.Outputs(outputs);
                
                if (!pointerLocation.HasValue)
                {
                    var outputLocation = programAsList[currentIndex + calculator.parameterCount];
                    if (outputValue.HasValue) programAsList[outputLocation] = (int)outputValue;       
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
        private static List<int> ConvertProgramStringToList(string input)
        {
            var stringList = input.Split(',').ToList();
            return stringList.Select(x => int.Parse(x.Trim())).ToList();
        }
        private static int GetOpCode(int currentCode, string codeString)
        {
            return (codeString.Length < 3 ? currentCode : 
                    int.Parse(codeString.Substring(codeString.Length - 2)));
        }
        
        private class Calculator
        {
            public virtual int parameterCount => 0;
            public List<int> parameterValues { get; set; }
            private int currentCode;
            private int currentIndex;
            private List<int> programAsList;
            public Calculator(int currentCode, int currentIndex, List<int> programAsList)
            {
                this.currentCode = currentCode;
                this.currentIndex = currentIndex;
                this.programAsList = programAsList;
            }
            private List<int> SetParameterTypes(int currentCode)
            {
                var codeString = currentCode.ToString();
                var parameterTypeList = codeString.Length > 2 ? codeString.Substring(0, codeString.Length - 2).ToList().Select(x => int.Parse(x.ToString())).ToList() : new List<int>();
                while (parameterTypeList.Count < parameterCount)
                {
                    parameterTypeList.Insert(0, 0);
                }
                var parameterTypes = new List<int>();
                parameterTypeList.Reverse();
                return parameterTypeList;
            }
            public void SetParameterValues(int currentCode, List<int> programAsList, int currentIndex)
            {
                var parameterTypes = SetParameterTypes(currentCode);
                parameterValues = parameterTypes.Select((x, i) => 
                    {
                        return x == 0 ? programAsList[programAsList[currentIndex + i + 1]] : programAsList[currentIndex + i + 1];
                    }).ToList();
            }
            
            public virtual int? OutputValue(int valueForOutputOp)
            {
                return null;
            }
            public virtual int? PointerLocation()
            {
                return null;
            }
            public virtual List<int> Outputs(List<int> outputs)
            {
                return outputs;
            }      
        }
        private class Add : Calculator
        {
            public Add(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 3;
            public override int? OutputValue(int valueForOutputOp)
            {
                return parameterValues[0] + parameterValues[1];
            }
        }
        private class Multiply : Calculator
        {
            public Multiply(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 3;
            public override int? OutputValue(int valueForOutputOp)
            {
                return parameterValues[0] * parameterValues[1];
            }
        }
        private class Save : Calculator
        {
            public Save(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 1;
            public override int? OutputValue(int valueForOutputOp)
            {
                return valueForOutputOp;
            }
        }
        private class Output : Calculator
        {
            public Output(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 1;
            public override List<int> Outputs(List<int> outputs)
            {
                var finalOutputs = outputs;
                finalOutputs.Add(parameterValues[0]);
                return finalOutputs;
            }
        }
        private class JumpIfTrue : Calculator
        {
            public JumpIfTrue(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 2;
            public override int? PointerLocation()
            {
                if (parameterValues[0] != 0) return parameterValues[1];
                return null;
            }
        }
        private class JumpIfFalse : Calculator
        {
            public JumpIfFalse(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 2;
            public override int? PointerLocation()
            {
                if (parameterValues[0] == 0) return parameterValues[1];
                return null;
            }
        }
        private class LessThan : Calculator
        {
            public LessThan(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 3;
            public override int? OutputValue(int valueForOutputOp)
            {
                return parameterValues[0] < parameterValues[1] ? 1 : 0;
            }
        }
        private class Equal : Calculator
        {
            public Equal(int currentCode, int currentIndex, List<int> programAsList) : base(currentCode, currentIndex, programAsList) {}
            public override int parameterCount => 3;
            public override int? OutputValue(int valueForOutputOp)
            {
                return parameterValues[0] == parameterValues[1] ? 1 : 0;
            }
        }
        private static Calculator CreateCalculator(int op, int currentIndex, int currentCode, List<int> programAsList)
        {
            switch(op)
            {
                case 1:
                    return new Add(currentCode, currentIndex, programAsList);
                case 2:
                    return new Multiply(currentCode, currentIndex, programAsList);
                case 3:
                    return new Save(currentCode, currentIndex, programAsList);
                case 4:
                    return new Output(currentCode, currentIndex, programAsList);
                case 5:
                    return new JumpIfTrue(currentCode, currentIndex, programAsList);
                case 6:
                    return new JumpIfFalse(currentCode, currentIndex, programAsList);
                case 7:
                    return new LessThan(currentCode, currentIndex, programAsList);
                case 8:
                    return new Equal(currentCode, currentIndex, programAsList);
                default:
                    throw new Exception();
            }
        }
    }
}