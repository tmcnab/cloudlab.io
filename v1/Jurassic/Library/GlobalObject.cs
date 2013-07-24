using System;
using System.Collections.Generic;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents functions and properties within the global scope.
    /// </summary>
    [Serializable]
    public class GlobalObject : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Global object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal GlobalObject(ObjectInstance prototype)
            : base(prototype)
        {
            // Add the global constants.
            // Infinity, NaN and undefined are read-only in ECMAScript 5.
            this.FastSetProperty("Infinity", double.PositiveInfinity, PropertyAttributes.Sealed);
            this.FastSetProperty("NaN", double.NaN, PropertyAttributes.Sealed);
            this.FastSetProperty("undefined", Undefined.Value, PropertyAttributes.Sealed);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Global"; }
        }





        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________


        /// <summary>
        /// Evaluates the given javascript source code and returns the result.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="code"> The source code to evaluate. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        [JSFunction(Name = "eval", Flags = JSFunctionFlags.HasEngineParameter)]
        public static object Eval(ScriptEngine engine, object code)
        {
            if (TypeUtilities.IsString(code) == false)
                return code;
            return engine.Eval(TypeConverter.ToString(code), engine.CreateGlobalScope(), engine.Global, false);
        }

        /// <summary>
        /// Evaluates the given javascript source code and returns the result.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="code"> The source code to evaluate. </param>
        /// <param name="scope"> The containing scope. </param>
        /// <param name="thisObject"> The value of the "this" keyword in the containing scope. </param>
        /// <param name="strictMode"> Indicates whether the eval statement is being called from
        /// strict mode code. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        public static object Eval(ScriptEngine engine, object code, Compiler.Scope scope, object thisObject, bool strictMode)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (TypeUtilities.IsString(code) == false)
                return code;
            return engine.Eval(TypeConverter.ToString(code), scope, thisObject, strictMode);
        }

        /// <summary>
        /// Determines whether the given number is finite.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>false</c> if the number is NaN or positive or negative infinity,
        /// <c>true</c> otherwise. </returns>
        [JSFunction(Name = "isFinite")]
        public static bool IsFinite(double value)
        {
            return double.IsNaN(value) == false && double.IsInfinity(value) == false;
        }

        /// <summary>
        /// Determines whether the given number is NaN.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>true</c> if the number is NaN, <c>false</c> otherwise. </returns>
        [JSFunction(Name = "isNaN")]
        public static bool IsNaN(double value)
        {
            return double.IsNaN(value);
        }

        /// <summary>
        /// Parses the given string and returns the equivalent numeric value. 
        /// </summary>
        /// <param name="input"> The string to parse. </param>
        /// <returns> The equivalent numeric value of the given string. </returns>
        /// <remarks> Leading whitespace is ignored.  Parsing continues until the first invalid
        /// character, at which point parsing stops.  No error is returned in this case. </remarks>
        [JSFunction(Name = "parseFloat")]
        public static double ParseFloat(string input)
        {
            return NumberParser.ParseFloat(input);
        }

        /// <summary>
        /// Parses the given string and returns the equivalent integer value. 
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="input"> The string to parse. </param>
        /// <param name="radix"> The numeric base to use for parsing.  Pass zero to use base 10
        /// except when the input string starts with '0' in which case base 16 or base 8 are used
        /// instead (base 8 is only supported in compatibility mode). </param>
        /// <returns> The equivalent integer value of the given string. </returns>
        /// <remarks> Leading whitespace is ignored.  Parsing continues until the first invalid
        /// character, at which point parsing stops.  No error is returned in this case. </remarks>
        [JSFunction(Name = "parseInt", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double ParseInt(ScriptEngine engine, string input, [DefaultParameterValue(0.0)] double radix = 0)
        {
            // Check for a valid radix.
            // Note: this is the only function that uses TypeConverter.ToInt32() for parameter
            // conversion (as opposed to the normal method which is TypeConverter.ToInteger() so
            // the radix parameter must be converted to an integer in code.
            int radix2 = TypeConverter.ToInt32(radix);
            if (radix2 < 0 || radix2 == 1 || radix2 > 36)
                return double.NaN;

            return NumberParser.ParseInt(input, radix2, engine.CompatibilityMode == CompatibilityMode.ECMAScript3);
        }

        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________


        /// <summary>
        /// Reads an integer value using the given reader.
        /// </summary>
        /// <param name="reader"> The reader to read characters from. </param>
        /// <param name="digitsRead"> Upon returning, contains the number of digits that were read. </param>
        /// <returns> The numeric value, or <c>double.NaN</c> if no number was present. </returns>
        private static double ReadInteger(System.IO.StringReader reader, out int digitsRead)
        {
            // If the input is empty, then return NaN.
            double result = double.NaN;
            digitsRead = 0;

            while (true)
            {
                int c = reader.Peek();
                if (c < '0' || c > '9')
                    break;
                reader.Read();
                digitsRead++;
                if (double.IsNaN(result))
                    result = c - '0';
                else
                    result = result * 10 + (c - '0');
            }

            return result;
        }

        /// <summary>
        /// Determines if the given character is whitespace or a line terminator.
        /// </summary>
        /// <param name="c"> The unicode code point for the character. </param>
        /// <returns> <c>true</c> if the character is whitespace or a line terminator; <c>false</c>
        /// otherwise. </returns>
        private static bool IsWhiteSpaceOrLineTerminator(int c)
        {
            return c == 9 || c == 0x0b || c == 0x0c || c == ' ' || c == 0xa0 || c == 0xfeff ||
                c == 0x1680 || c == 0x180e || (c >= 0x2000 && c <= 0x200a) || c == 0x202f || c == 0x205f || c == 0x3000 ||
                c == 0x0a || c == 0x0d || c == 0x2028 || c == 0x2029;
        }

        /// <summary>
        /// Parses a hexidecimal number from within a string.
        /// </summary>
        /// <param name="input"> The string containing the hexidecimal number. </param>
        /// <param name="start"> The start index of the hexidecimal number. </param>
        /// <param name="length"> The number of characters in the hexidecimal number. </param>
        /// <returns> The numeric value of the hexidecimal number, or <c>-1</c> if the number
        /// is not valid. </returns>
        private static int ParseHexNumber(string input, int start, int length)
        {
            if (start + length > input.Length)
                return -1;
            int result = 0;
            for (int i = start; i < start + length; i++)
            {
                result *= 0x10;
                char c = input[i];
                if (c >= '0' && c <= '9')
                    result += c - '0';
                else if (c >= 'A' && c <= 'F')
                    result += c - 'A' + 10;
                else if (c >= 'a' && c <= 'f')
                    result += c - 'a' + 10;
                else
                    return -1;
            }
            return result;
        }

        /// <summary>
        /// Creates a 128 entry lookup table for the characters in the given string.
        /// </summary>
        /// <param name="characters"> The characters to include in the set. </param>
        /// <returns> An array containing <c>true</c> for each character in the set. </returns>
        private static bool[] CreateCharacterSetLookupTable(string characters)
        {
            var result = new bool[128];
            for (int i = 0; i < characters.Length; i ++)
            {
                char c = characters[i];
                if (c >= 128)
                    throw new ArgumentException("Characters must be ASCII.", "characters");
                result[c] = true;
            }
            return result;
        }
    }
}
