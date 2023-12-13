using Moq;
using System.Text;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslox.Tests
{
    public class Tests
    {
        StringBuilder _ConsoleOutput;
        Mock<TextReader> _ConsoleInput;

        /// <summary>
        /// This method helps recieve and give
        /// information to the console.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _ConsoleOutput = new StringBuilder();
            var consoleOutputWriter = new StringWriter(_ConsoleOutput);
            _ConsoleInput = new Mock<TextReader>();
            Console.SetOut(consoleOutputWriter);
            Console.SetIn(_ConsoleInput.Object);
        }
        [Test]
        [TestCase("print \"one\";", "> one")]
        [TestCase("print true;",  "> true")]
        [TestCase("print 2 + 1;", "> 3")]
        [TestCase("if (true) print \"Ugh, already?\";", "> Ugh, already?")]
        [TestCase("if (false) print \"Ugh, already?\";", "> > ")]
        [TestCase("print \"hi\" or 2;", "> hi")]
        [TestCase("print nil or \"yes\";", "> yes")]
        public void Main_SingleLine(string response, string expected)
        {
            SetupUserResponses(response);
            var expectedPrompt = expected;

            var outputLines = RunMainAndGetConsoleOutput();

            Assert.That(outputLines[0], Is.EqualTo(expectedPrompt));
        }
        [Test]
        [TestCase("var beverage = \"espresso\";", null, "print beverage;", "> > espresso")]
        [TestCase("var nilvalue;", null, "print nilvalue;", "> > nil")]
        [TestCase("{ var scoped = \"first\";  print scoped;}", "> first", "{var scoped = \"second\";  print scoped;}", "> second")]
        [TestCase("var global = \"outside\";", null, "{var local = \"inside\"; print global + local;}", "> > outsideinside")]
        [TestCase("var greater = 1;", null, "if (0 < greater) { print \"greater is so much greater than 0\"; }", "> > greater is so much greater than 0")]
        [TestCase("var equal = 1;", null, "if (1 == equal) print \"they equal\";", "> > they equal")]
        [TestCase("var notGreater = -1;", null, "if (0 < notGreater) { print \"greater is so much greater than 0\"; } else { print \"nah\"; }", "> > nah")]
        [TestCase("var notEqual = 5;", null, "if (1 == notEqual) print \"they equal\"; else print \"they not equal\";", "> > they not equal")]
        [TestCase("var x42 = 10;", null, "print x42;", "> > 10")]
        [TestCase("var y99 = \"hello\";", null, "print y99;", "> > hello")]
        [TestCase("var a3 = 5;", null, "if (a3 > 0) { print \"positive\"; } else { print \"non-positive\"; }", "> > positive")]
        [TestCase("var a4 = 0;", null, "if (a4 > 0) { print \"positive\"; } else if (a4 == 0) { print \"zero\"; } else { print \"negative\"; }", "> > zero")]
        [TestCase("var j = 5;", null, "while (j > 5) { print j; j = j - 1; }", "> > > ")]
        public void Main_TwoLines(string? input1, string? output1, string? input2, string? output2)
        {
            SetupUserResponses(input1, input2);

            var outputLines = RunMainAndGetConsoleOutput();

            int position = 0;

            Assert.Multiple(() =>
            {
                if (output1 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output1));
                    position++;
                }

                if (output2 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output2));
                    position++;
                }
                
                if (output1 == null && output2 == null)
                    throw new Exception("All outputs are null");
            });
        }
        [Test]
        [TestCase("var changed = 7;", null, "print changed;", "> > 7", "print changed = 8;", "> 8")]
        [TestCase("var volume = 11;", null, "volume = 0;", null, "{ var volume = 3 * 4 * 5; print volume;}", "> > > 60")]
        [TestCase("var a1 = 5;", null, "var b2 = 2;", null, "print a1 + b2;", "> > > 7")]
        [TestCase("var testVar1 = 10;", null, "var testVar2 = 20;", null, "if (testVar1 > 5 or testVar2 > 15) { print \"condition met\"; }", "> > > condition met")]
        [TestCase("var conditionA = true;", null, "var conditionB = false;", null, "if (conditionA and !conditionB) { print \"both conditions met\"; }", "> > > both conditions met")]
        [TestCase("var checkTrue = true;", null, "var checkFalse = false;", null, "if (checkTrue or !checkFalse) { print \"at least one condition met\"; }", "> > > at least one condition met")]
        [TestCase("var checkTrue2 = true;", null, "var checkFalse2 = false;", null, "if (checkTrue2 and !checkFalse2) { print \"both conditions met\"; }", "> > > both conditions met")]
        [TestCase("var checkA = 5;", null, "var checkB = 10;", null, "if (checkA < 10 and checkB > 5) { print \"both conditions met\"; } else { print \"conditions not met\"; }", "> > > both conditions met")]
        [TestCase("var checkTrue3 = true;", null, "var checkFalse3 = false;", null, "if (checkTrue3 and checkFalse3) { print \"both conditions met\"; } else { print \"conditions not met\"; }", "> > > conditions not met")]
        [TestCase("for (var i6 = 0; i6 < 3; i6 = i6 + 1) { print i6; }", "> 0", null, "1", null, "2")]
        [TestCase("var result8 = 0;", null, "for (var i8 = 1; i8 <= 5; i8 = i8 + 1) { result8 = result8 + i8; }", null, "print result8;", "> > > 15")]
        public void Main_ThreeLines(string? input1, string? output1, string? input2, string? output2, string? input3, string? output3)
        {
            SetupUserResponses(input1, input2, input3);

            var outputLines = RunMainAndGetConsoleOutput();

            int position = 0;

            Assert.Multiple(() =>
            {
                if (output1 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output1));
                    position++;
                }

                if (output2 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output2));
                    position++;
                }

                if (output3 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output3));
                    position++;
                }

                if (output1 == null && output2 == null && output3 == null)
                    throw new Exception("All outputs are null");
            });
        }
        [Test]
        [TestCase(
            "var time = \"before\";", null, 
            "print time;", "> > before", 
            "time = \"after\";", null, 
            "print time;", "> > after"
            )]
        [TestCase(
            "var i5 = 0;", null, 
            "while (i5 < 3) { print i5; i5 = i5 + 1; }", "> > 0", 
            null, "1", 
            null, "2"
            )]
        [TestCase(
            "var x = 0;", null, 
            "while (x < 3) { print x; x = x + 1; }", "> > 0",
            null, "1",
            null, "2"
            )]
        [TestCase(
            "var a7 = 3;", null, 
            "for (var i7 = 0; i7 < a7; i7 = i7 + 1) { print i7; }", "> > 0",
            null, "1",
            null, "2"
            )]
        public void Main_FourLines(
            string? input1, string? output1, 
            string? input2, string? output2, 
            string? input3, string? output3, 
            string? input4, string? output4)
        {
            SetupUserResponses(input1, input2, input3, input4);

            var outputLines = RunMainAndGetConsoleOutput();

            int position = 0;

            Assert.Multiple(() =>
            {
                if (output1 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output1));
                    position++;
                }

                if (output2 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output2));
                    position++;
                }

                if (output3 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output3));
                    position++;
                }

                if (output4 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output4));
                    position++;
                }

                if (output1 == null && output2 == null && output3 == null && output4 == null)
                    throw new Exception("All outputs are null");
            });
        }
        [Test]
        [TestCase(
            "var number1 = 5;", null, 
            "var number2 = 32;", null, 
            "print number1 + number2;", "> > > 37", 
            "number1 = 40;", null, 
            "number2 = 50;", null, 
            "print number1 + number2;", "> > > 90")]
        [TestCase(
            "var i = 10;", null, 
            "while (i > 0) { print i; i = i - 2; }", "> > 10",
            null, "8", 
            null, "6",
            null, "4",
            null, "2"
            )]
        [TestCase(
            "var count = 0;", null, 
            "while (count < 5) { print count; count = count + 1; }", "> > 0",
            null, "1",
            null, "2",
            null, "3",
            null, "4")]
        public void Main_SixLines(
            string? input1, string? output1,
            string? input2, string? output2,
            string? input3, string? output3,
            string? input4, string? output4,
            string? input5, string? output5,
            string? input6, string? output6)
        {
            SetupUserResponses(input1, input2, input3, input4, input5, input6);

            var outputLines = RunMainAndGetConsoleOutput();

            int position = 0;

            Assert.Multiple(() =>
            {
                if (output1 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output1));
                    position++;
                }

                if (output2 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output2));
                    position++;
                }

                if (output3 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output3));
                    position++;
                }

                if (output4 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output4));
                    position++;
                }

                if (output5 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output5));
                    position++;
                }

                if (output6 != null)
                {
                    Assert.That(outputLines[position], Is.EqualTo(output6));
                    position++;
                }

                if (output1 == null && output2 == null && output3 == null && output4 == null && output5 == null && output6 == null)
                    throw new Exception("All outputs are null");
            });
        }
        [Test]
        public void Fibonacci_Test()
        {
            SetupUserResponses(
                "var a32 = 0;",
                "var temp;",
                "for (var b32 = 1; a32 <= 21; b32 = temp + b32) {print a32; temp = a32; a32 = b32;}"
                );

            var outputLines = RunMainAndGetConsoleOutput();

            Assert.Multiple(() =>
            {
                Assert.That(outputLines[0], Is.EqualTo("> > > 0"));
                Assert.That(outputLines[1], Is.EqualTo("1"));
                Assert.That(outputLines[2], Is.EqualTo("1"));
                Assert.That(outputLines[3], Is.EqualTo("2"));
                Assert.That(outputLines[4], Is.EqualTo("3"));
                Assert.That(outputLines[5], Is.EqualTo("5"));
                Assert.That(outputLines[6], Is.EqualTo("8"));
                Assert.That(outputLines[7], Is.EqualTo("13"));
                Assert.That(outputLines[8], Is.EqualTo("21"));
            });
        }
        [Test]
        public void Scope_Test()
        {
            SetupUserResponses(
                "var a = \"global a\";", 
                "var b = \"global b\";",
                "var c = \"global c\";",
                "{ var a = \"outer a\"; var b = \"outer b\"; { var a = \"inner a\"; print a; print b; print c; } print a; print b; print c; }",
                "print a;",
                "print b;",
                "print c;"
                );

            var outputLines = RunMainAndGetConsoleOutput();

            Assert.Multiple(() =>
            {
                Assert.That(outputLines[0], Is.EqualTo("> > > > inner a"));
                Assert.That(outputLines[1], Is.EqualTo("outer b"));
                Assert.That(outputLines[2], Is.EqualTo("global c"));
                Assert.That(outputLines[3], Is.EqualTo("outer a"));
                Assert.That(outputLines[4], Is.EqualTo("outer b"));
                Assert.That(outputLines[5], Is.EqualTo("global c"));
                Assert.That(outputLines[6], Is.EqualTo("> global a"));
                Assert.That(outputLines[7], Is.EqualTo("> global b"));
                Assert.That(outputLines[8], Is.EqualTo("> global c"));
            });
        }
        /// <summary>
        /// Runs the program and converts the
        /// console output to string.
        /// </summary>
        /// <returns>
        /// String array containing each line
        /// from the console.
        /// </returns>
        private string[] RunMainAndGetConsoleOutput()
        {
            string[] emptyStringArray = Array.Empty<string>();
            Lox.Main(emptyStringArray);
            return _ConsoleOutput.ToString().Split("\r\n");
        }
        /// <summary>
        /// This orders the user responses. 
        /// </summary>
        /// <param name="userResponses">
        /// A string array of the different
        /// responses ordered accordingly.
        /// </param>
        /// <returns>
        /// A <c>MockSequence</c> object
        /// containing the sequence console
        /// lines.
        /// </returns>
        private MockSequence SetupUserResponses(params string?[] userResponses)
        {
            var sequence = new MockSequence();
            foreach (var response in userResponses)
            {
                if (response != null)
                {
                    _ConsoleInput.InSequence(sequence).Setup(x => x.ReadLine()).Returns(response);
                }
            }
            return sequence;
        }
    }
}