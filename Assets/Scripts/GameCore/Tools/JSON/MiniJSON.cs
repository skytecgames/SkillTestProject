/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace MiniJSON {
    // Example usage:
    //
    //  using UnityEngine;
    //  using System.Collections;
    //  using System.Collections.Generic;
    //  using MiniJSON;
    //
    //  public class MiniJSONTest : MonoBehaviour {
    //      void Start () {
    //          var jsonString = "{ \"array\": [1.44,2,3], " +
    //                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    //                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    //                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    //                          "\"int\": 65536, " +
    //                          "\"float\": 3.1415926, " +
    //                          "\"bool\": true, " +
    //                          "\"null\": null }";
    //
    //          var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
    //
    //          Debug.Log("deserialized: " + dict.GetType());
    //          Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    //          Debug.Log("dict['string']: " + (string) dict["string"]);
    //          Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    //          Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    //          Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    //
    //          var str = Json.Serialize(dict);
    //
    //          Debug.Log("serialized: " + str);
    //      }
    //  }

    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
    /// All numbers are parsed to doubles.
    /// </summary>
    public static class Json {

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
        public static object Deserialize(string json) {
            // save the string for debug information
            if (json == null) {
                return null;
            }

            return Parser.Parse(json);
        }

        sealed class Parser : IDisposable {
            const string WORD_BREAK = "{}[],:\"";

            public static bool IsWordBreak(char c) {
                return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
            }

            enum TOKEN {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            };

            StringReader json;

            Parser(string jsonString) {
                json = new StringReader(jsonString);
            }

            public static object Parse(string jsonString) {
                using (var instance = new Parser(jsonString)) {
                    return instance.ParseValue();
                }
            }

            public void Dispose() {
                json.Dispose();
                json = null;
            }

            Dictionary<string, object> ParseObject() {
                Dictionary<string, object> table = new Dictionary<string, object>();

                // ditch opening brace
                json.Read();

                // {
                while (true) {
                    switch (NextToken) {
                    case TOKEN.NONE:
                        return null;
                    case TOKEN.COMMA:
                        continue;
                    case TOKEN.CURLY_CLOSE:
                        return table;
                    default:
                        // name
                        string name = ParseString();
                        if (name == null) {
                            return null;
                        }

                        // :
                        if (NextToken != TOKEN.COLON) {
                            return null;
                        }
                        // ditch the colon
                        json.Read();

                        // value
                        table[name] = ParseValue();
                        break;
                    }
                }
            }

            List<object> ParseArray() {
                List<object> array = new List<object>();

                // ditch opening bracket
                json.Read();

                // [
                var parsing = true;
                while (parsing) {
                    TOKEN nextToken = NextToken;

                    switch (nextToken) {
                    case TOKEN.NONE:
                        return null;
                    case TOKEN.COMMA:
                        continue;
                    case TOKEN.SQUARED_CLOSE:
                        parsing = false;
                        break;
                    default:
                        object value = ParseByToken(nextToken);

                        array.Add(value);
                        break;
                    }
                }

                return array;
            }

            object ParseValue() {
                TOKEN nextToken = NextToken;
                return ParseByToken(nextToken);
            }

            object ParseByToken(TOKEN token) {
                switch (token) {
                case TOKEN.STRING:
                    return ParseString();
                case TOKEN.NUMBER:
                    return ParseNumber();
                case TOKEN.CURLY_OPEN:
                    return ParseObject();
                case TOKEN.SQUARED_OPEN:
                    return ParseArray();
                case TOKEN.TRUE:
                    return true;
                case TOKEN.FALSE:
                    return false;
                case TOKEN.NULL:
                    return null;
                default:
                    return null;
                }
            }

            string ParseString() {
                StringBuilder s = new StringBuilder();
                char c;

                // ditch opening quote
                json.Read();

                bool parsing = true;
                while (parsing) {

                    if (json.Peek() == -1) {
                        parsing = false;
                        break;
                    }

                    c = NextChar;
                    switch (c) {
                    case '"':
                        parsing = false;
                        break;
                    case '\\':
                        if (json.Peek() == -1) {
                            parsing = false;
                            break;
                        }

                        c = NextChar;
                        switch (c) {
                        case '"':
                        case '\\':
                        case '/':
                            s.Append(c);
                            break;
                        case 'b':
                            s.Append('\b');
                            break;
                        case 'f':
                            s.Append('\f');
                            break;
                        case 'n':
                            s.Append('\n');
                            break;
                        case 'r':
                            s.Append('\r');
                            break;
                        case 't':
                            s.Append('\t');
                            break;
                        case 'u':
                            var hex = new char[4];

                            for (int i=0; i< 4; i++) {
                                hex[i] = NextChar;
                            }

                            s.Append((char) Convert.ToInt32(new string(hex), 16));
                            break;
                        }
                        break;
                    default:
                        s.Append(c);
                        break;
                    }
                }

                return s.ToString();
            }

            object ParseNumber() {
                string number = NextWord;

                if (number.IndexOf('.') == -1) {
                    long parsedInt;
                    Int64.TryParse(number, out parsedInt);
                    return parsedInt;
                }

                double parsedDouble;
                //Double.TryParse(number, out parsedDouble);
                Double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedDouble);
                return parsedDouble;
            }

            void EatWhitespace() {
                while (Char.IsWhiteSpace(PeekChar)) {
                    json.Read();

                    if (json.Peek() == -1) {
                        break;
                    }
                }
            }

            char PeekChar {
                get {
                    return Convert.ToChar(json.Peek());
                }
            }

            char NextChar {
                get {
                    return Convert.ToChar(json.Read());
                }
            }

            string NextWord {
                get {
                    StringBuilder word = new StringBuilder();

                    while (!IsWordBreak(PeekChar)) {
                        word.Append(NextChar);

                        if (json.Peek() == -1) {
                            break;
                        }
                    }

                    return word.ToString();
                }
            }

            TOKEN NextToken {
                get {
                    EatWhitespace();

                    if (json.Peek() == -1) {
                        return TOKEN.NONE;
                    }

                    switch (PeekChar) {
                    case '{':
                        return TOKEN.CURLY_OPEN;
                    case '}':
                        json.Read();
                        return TOKEN.CURLY_CLOSE;
                    case '[':
                        return TOKEN.SQUARED_OPEN;
                    case ']':
                        json.Read();
                        return TOKEN.SQUARED_CLOSE;
                    case ',':
                        json.Read();
                        return TOKEN.COMMA;
                    case '"':
                        return TOKEN.STRING;
                    case ':':
                        return TOKEN.COLON;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                        return TOKEN.NUMBER;
                    }

                    switch (NextWord) {
                    case "false":
                        return TOKEN.FALSE;
                    case "true":
                        return TOKEN.TRUE;
                    case "null":
                        return TOKEN.NULL;
                    }

                    return TOKEN.NONE;
                }
            }
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj) {
            return Serializer.Serialize(obj);
        }

        sealed class Serializer {
            StringBuilder builder;

            Serializer() {
                builder = new StringBuilder();
            }

            public static string Serialize(object obj) {
                var instance = new Serializer();

                instance.SerializeValue(obj);

                return instance.builder.ToString();
            }

            void SerializeValue(object value) {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null) {
                    builder.Append("null");
                } else if ((asStr = value as string) != null) {
                    SerializeString(asStr);
                } else if (value is bool) {
                    builder.Append((bool) value ? "true" : "false");
                } else if ((asList = value as IList) != null) {
                    SerializeArray(asList);
                } else if ((asDict = value as IDictionary) != null) {
                    SerializeObject(asDict);
                } else if (value is char) {
                    SerializeString(new string((char) value, 1));
                } else {
                    SerializeOther(value);
                }
            }

            void SerializeObject(IDictionary obj) {
                bool first = true;

                builder.Append('{');

                foreach (object e in obj.Keys) {
                    if (!first) {
                        builder.Append(',');
                    }

                    SerializeString(e.ToString());
                    builder.Append(':');

                    SerializeValue(obj[e]);

                    first = false;
                }

                builder.Append('}');
            }

            void SerializeArray(IList anArray) {
                builder.Append('[');

                bool first = true;

                foreach (object obj in anArray) {
                    if (!first) {
                        builder.Append(',');
                    }

                    SerializeValue(obj);

                    first = false;
                }

                builder.Append(']');
            }

            void SerializeString(string str) {
                builder.Append('\"');

                char[] charArray = str.ToCharArray();
                foreach (var c in charArray) {
                    switch (c) {
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        int codepoint = Convert.ToInt32(c);
                        if ((codepoint >= 32) && (codepoint <= 126)) {
                            builder.Append(c);
                        } else {
                            builder.Append("\\u");
                            builder.Append(codepoint.ToString("x4"));
                        }
                        break;
                    }
                }

                builder.Append('\"');
            }

            void SerializeOther(object value) {
                // NOTE: decimals lose precision during serialization.
                // They always have, I'm just letting you know.
                // Previously floats and doubles lost precision too.
                if (value is float) {
                    builder.Append(((float) value).ToString("R", CultureInfo.InvariantCulture));
                } else if (value is int
                    || value is uint
                    || value is long
                    || value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is ulong) {
                    builder.Append(value);
                } else if (value is double
                    || value is decimal) {
                    builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
                } else {
                    SerializeString(value.ToString());
                }
            }
        }
    }

	//========================================================

	public class MiniJSON
	{
		public const int TOKEN_NONE = 0; 
		public const int TOKEN_CURLY_OPEN = 1;
		public const int TOKEN_CURLY_CLOSE = 2;
		public const int TOKEN_SQUARED_OPEN = 3;
		public const int TOKEN_SQUARED_CLOSE = 4;
		public const int TOKEN_COLON = 5;
		public const int TOKEN_COMMA = 6;
		public const int TOKEN_STRING = 7;
		public const int TOKEN_NUMBER = 8;
		public const int TOKEN_TRUE = 9;
		public const int TOKEN_FALSE = 10;
		public const int TOKEN_NULL = 11;
		
		private const int BUILDER_CAPACITY = 5000;
		
		protected static MiniJSON instance = new MiniJSON();
		
		/// <summary>
		/// On decoding, this value holds the position at which the parse failed (-1 = no error).
		/// </summary>
		protected int lastErrorIndex = -1;
		protected string lastDecode = "";
		
		
		public static T JsonDecode<T> (T obj, string json, bool isCamelCase = true) where T: class
		{
			// save the string for debug information
			MiniJSON.instance.lastDecode = json;
			
			if (json != null) {
				char[] charArray = json.ToCharArray ();
				int index = 0;
				//bool success = true;
				switch (MiniJSON.instance.LookAhead (charArray, index)) {
				case MiniJSON.TOKEN_CURLY_OPEN:
					return MiniJSON.instance.ParseObject (obj, charArray, ref index, isCamelCase);			
				default:
					return null;
				}
			} else {
				return null;
			}
		}
		
		
		/// <summary>
		/// Parses the s
		
		/// <summary>
		/// Parses the string json into a value
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		public static object JsonDecode(string json)
		{
			// save the string for debug information
			MiniJSON.instance.lastDecode = json;
			
			if(json != null)
			{
				char[] charArray = json.ToCharArray();
				int index = 0;
				bool success = true;
				object value = MiniJSON.instance.ParseValue(charArray, ref index, ref success);
				
				if (success)
					MiniJSON.instance.lastErrorIndex = -1;
				else
					MiniJSON.instance.lastErrorIndex = index;
				
				return value;
			}
			else
			{
				return null;
			}
		}
		
		
		/// <summary>
		/// Converts a Hashtable / ArrayList object into a JSON string
		/// </summary>
		/// <param name="json">A Hashtable / ArrayList</param>
		/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
		public static string JsonEncode (object json)
		{
			StringBuilder builder = new StringBuilder (BUILDER_CAPACITY);
			bool success = MiniJSON.instance.SerializeValue (json, builder);
			return (success ? builder.ToString () : null);
		}
		
		public static byte[] JsonEncodeUTF8(object json) {
			return System.Text.Encoding.UTF8.GetBytes (JsonEncode(json));
		}
		
		
		/// <summary>
		/// On decoding, this function returns the position at which the parse failed (-1 = no error).
		/// </summary>
		/// <returns></returns>
		public static bool LastDecodeSuccessful()
		{
			return (MiniJSON.instance.lastErrorIndex == -1);
		}
		
		
		/// <summary>
		/// On decoding, this function returns the position at which the parse failed (-1 = no error).
		/// </summary>
		/// <returns></returns>
		public static int GetLastErrorIndex()
		{
			return MiniJSON.instance.lastErrorIndex;
		}
		
		
		/// <summary>
		/// If a decoding error occurred, this function returns a piece of the JSON string 
		/// at which the error took place. To ease debugging.
		/// </summary>
		/// <returns></returns>
		public static string GetLastErrorSnippet()
		{
			if (MiniJSON.instance.lastErrorIndex == -1) {
				return "";
			} else {
				int startIndex = MiniJSON.instance.lastErrorIndex - 5;
				int endIndex = MiniJSON.instance.lastErrorIndex + 15;
				if (startIndex < 0) {
					startIndex = 0;
				}
				if (endIndex >= MiniJSON.instance.lastDecode.Length) {
					endIndex = MiniJSON.instance.lastDecode.Length - 1;
				}
				
				return MiniJSON.instance.lastDecode.Substring(startIndex, endIndex - startIndex + 1);
			}
		}
		
		
		protected Hashtable ParseObject(char[] json, ref int index)
		{
			Hashtable table = new Hashtable();
			int token;
			
			// {
			NextToken(json, ref index);
			
			bool done = false;
			while (!done) {
				token = LookAhead(json, index);
				if (token == MiniJSON.TOKEN_NONE) {
					return null;
				} else if (token == MiniJSON.TOKEN_COMMA) {
					NextToken(json, ref index);
				} else if (token == MiniJSON.TOKEN_CURLY_CLOSE) {
					NextToken(json, ref index);
					return table;
				} else {
					
					// name
					string name = ParseString(json, ref index);
					if (name == null) {
						return null;
					}
					
					// :
					token = NextToken(json, ref index);
					if (token != MiniJSON.TOKEN_COLON) {
						return null;
					}
					
					// value
					bool success = true;
					object value = ParseValue(json, ref index, ref success);
					if (!success) {
						return null;
					}
					
					table[name] = value;
				}
			}
			
			return table;
		}
		
		protected T ParseObject<T> (T obj, char[] json, ref int index, bool isCamelCase = true) where T:class
		{
			int token;
			
			// {
			NextToken (json, ref index);
			
			bool done = false;
			while (!done) {
				token = LookAhead (json, index);
				if (token == MiniJSON.TOKEN_NONE) {
					return null;
				} else if (token == MiniJSON.TOKEN_COMMA) {
					NextToken (json, ref index);
				} else if (token == MiniJSON.TOKEN_CURLY_CLOSE) {
					NextToken (json, ref index);
					return obj;
				} else {
					
					// name
					string name = ParseString (json, ref index);
					if (name == null) {
						return null;
					}
					
					// :
					token = NextToken (json, ref index);
					if (token != MiniJSON.TOKEN_COLON) {
						return null;
					}
					
					// value
					bool success = true;
					if(isCamelCase)
					{
						name = name.Replace("_", "");
					}
					System.Reflection.FieldInfo field = System.Array.Find(obj.GetType ().GetFields(), x => x.Name.ToLower() == name.ToLower());
					if (field == null) {
						throw new Exception("Failed to set field: " + name + " of " + obj);
						//return null;
					}
					var itemType = GetGenericListType (field.FieldType);
					
					if (itemType != null) {
						var list = System.Activator.CreateInstance (field.FieldType);
						var value = ParseArray ((IList)list, itemType, json, ref index);					
						if (value == null) {
							return null;
						}
						field.SetValue (obj, value);
					} else if (field.FieldType.IsClass && 
					           !field.FieldType.IsAssignableFrom (typeof(string)) &&
					           !field.FieldType.IsAssignableFrom (typeof(Hashtable)) && 
					           !field.FieldType.IsAssignableFrom (typeof(ArrayList))
					           ) {
						var inner = System.Activator.CreateInstance (field.FieldType);
						var value = ParseObject (inner, json, ref index);
						if (value == null) {
							return null;
						}
						field.SetValue (obj, value);
					} else {
						object value = ParseValue (json, ref index, ref success);
						if (!success) {
							return null;
						}
						
						if (field.FieldType.IsEnum) {
							field.SetValue (obj, (int)(double)value);
						} else if (field.FieldType.IsAssignableFrom (typeof(double))) {
							field.SetValue (obj, (double)value);
						} else if (field.FieldType.IsAssignableFrom (typeof(float))) {
							field.SetValue (obj, (float)(double)value);
						} else if (field.FieldType.IsAssignableFrom (typeof(int))) {
							field.SetValue (obj, (int)(double)value);
						} else if (field.FieldType.IsAssignableFrom (typeof(long))) {
							field.SetValue (obj, (long)(double)value);
						} else {
							field.SetValue (obj, value);
						}
					}
					
				}
			}
			
			return obj;
		}		
		
		Type GetGenericListType(Type type) {
			foreach (Type i in type.GetInterfaces()) {
				if (i.IsGenericType) {
					if (i.GetGenericTypeDefinition() == typeof(System.Collections.Generic.ICollection<>)) {
						return i.GetGenericArguments()[0];
					}
				}
			}
			return null;
		}
		
		
		protected ArrayList ParseArray (char[] json, ref int index)
		{
			ArrayList array = new ArrayList ();
			
			// [
			NextToken (json, ref index);
			
			bool done = false;
			while (!done) {
				int token = LookAhead (json, index);
				if (token == MiniJSON.TOKEN_NONE) {
					return null;
				} else if (token == MiniJSON.TOKEN_COMMA) {
					NextToken (json, ref index);
				} else if (token == MiniJSON.TOKEN_SQUARED_CLOSE) {
					NextToken (json, ref index);
					break;
				} else {
					bool success = true;
					object value = ParseValue (json, ref index, ref success);
					if (!success) {
						return null;
					}
					
					array.Add (value);
				}
			}
			
			return array;
		}
		
		
		protected IList ParseArray (IList array, Type itemType, char[] json, ref int index)
		{
			// [
			NextToken (json, ref index);
			
			bool done = false;
			while (!done) {
				int token = LookAhead (json, index);
				if (token == MiniJSON.TOKEN_NONE) {
					return null;
				} else if (token == MiniJSON.TOKEN_COMMA) {
					NextToken (json, ref index);
				} else if (token == MiniJSON.TOKEN_SQUARED_CLOSE) {
					NextToken (json, ref index);
					break;
				} else {
					object value = null;
					bool success = true;
					if (itemType.IsAssignableFrom (typeof(int)) ||
					    itemType.IsAssignableFrom (typeof(string)) ||
					    itemType.IsAssignableFrom (typeof(Hashtable)) || 
					    itemType.IsAssignableFrom (typeof(ArrayList))) {
						value = ParseValue (json, ref index, ref success);
					} else {
						value = ParseObject (System.Activator.CreateInstance (itemType), json, ref index);
					}
					if (value == null || !success) {
						return null;
					}
					
					if (itemType.IsAssignableFrom (typeof(int)))
						array.Add ((int)(double)value);
					else
						array.Add (value);
				}
			}
			
			return array;
		}
		
		
		protected object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (LookAhead(json, index)) {
			case MiniJSON.TOKEN_STRING:
				return ParseString(json, ref index);
			case MiniJSON.TOKEN_NUMBER:
				return ParseNumber(json, ref index);
			case MiniJSON.TOKEN_CURLY_OPEN:
				return ParseObject(json, ref index);
			case MiniJSON.TOKEN_SQUARED_OPEN:
				return ParseArray(json, ref index);
			case MiniJSON.TOKEN_TRUE:
				NextToken(json, ref index);
				return Boolean.Parse("TRUE");
			case MiniJSON.TOKEN_FALSE:
				NextToken(json, ref index);
				return Boolean.Parse("FALSE");
			case MiniJSON.TOKEN_NULL:
				NextToken(json, ref index);
				return null;
			case MiniJSON.TOKEN_NONE:
				break;
			}
			
			success = false;
			return null;
		}
		
		
		protected string ParseString(char[] json, ref int index)
		{
			string s = "";
			char c;
			
			EatWhitespace(json, ref index);
			
			// "
			c = json[index++];
			
			bool complete = false;
			while (!complete) {
				
				if (index == json.Length) {
					break;
				}
				
				c = json[index++];
				if (c == '"') {
					complete = true;
					break;
				} else if (c == '\\') {
					
					if (index == json.Length) {
						break;
					}
					c = json[index++];
					if (c == '"') {
						s += '"';
					} else if (c == '\\') {
						s += '\\';
					} else if (c == '/') {
						s += '/';
					} else if (c == 'b') {
						s += '\b';
					} else if (c == 'f') {
						s += '\f';
					} else if (c == 'n') {
						s += '\n';
					} else if (c == 'r') {
						s += '\r';
					} else if (c == 't') {
						s += '\t';
					} else if (c == 'u') {
						int remainingLength = json.Length - index;
						if (remainingLength >= 4) {
							char[] unicodeCharArray = new char[4];
							Array.Copy(json, index, unicodeCharArray, 0, 4);
							
							// Drop in the HTML markup for the unicode character
							//s += "&#x" + new string(unicodeCharArray) + ";";
							
							
							uint codePoint = UInt32.Parse(new string(unicodeCharArray), System.Globalization.NumberStyles.HexNumber);
							// convert the integer codepoint to a unicode char and add to string
							s += Char.ConvertFromUtf32((int)codePoint);
							
							
							// skip 4 chars
							index += 4;
						} else {
							break;
						}					
					}
				} else {
					s += c;
				}
				
			}
			
			if (!complete) {
				return null;
			}
			
			return s;
		}
		
		
		protected double ParseNumber(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);
			
			int lastIndex = GetLastIndexOfNumber(json, index);
			int charLength = (lastIndex - index) + 1;
			char[] numberCharArray = new char[charLength];
			
			Array.Copy(json, index, numberCharArray, 0, charLength);
			index = lastIndex + 1;
			return Double.Parse(new string(numberCharArray)); // , CultureInfo.InvariantCulture);
		}
		
		
		protected int GetLastIndexOfNumber(char[] json, int index)
		{
			int lastIndex;
			for (lastIndex = index; lastIndex < json.Length; lastIndex++) {
				if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}
		
		
		protected void EatWhitespace(char[] json, ref int index)
		{
			for (; index < json.Length; index++) {
				if (" \t\n\r".IndexOf(json[index]) == -1) {
					break;
				}
			}
		}
		
		
		protected int LookAhead(char[] json, int index)
		{
			int saveIndex = index;
			return NextToken(json, ref saveIndex);
		}
		
		
		protected int NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);
			
			if (index == json.Length) {
				return MiniJSON.TOKEN_NONE;
			}
			
			char c = json[index];
			index++;
			switch (c) {
			case '{':
				return MiniJSON.TOKEN_CURLY_OPEN;
			case '}':
				return MiniJSON.TOKEN_CURLY_CLOSE;
			case '[':
				return MiniJSON.TOKEN_SQUARED_OPEN;
			case ']':
				return MiniJSON.TOKEN_SQUARED_CLOSE;
			case ',':
				return MiniJSON.TOKEN_COMMA;
			case '"':
				return MiniJSON.TOKEN_STRING;
			case '0': case '1': case '2': case '3': case '4': 
			case '5': case '6': case '7': case '8': case '9':
			case '-': 
				return MiniJSON.TOKEN_NUMBER;
			case ':':
				return MiniJSON.TOKEN_COLON;
			}
			index--;
			
			int remainingLength = json.Length - index;
			
			// false
			if (remainingLength >= 5) {
				if (json[index] == 'f' &&
				    json[index + 1] == 'a' &&
				    json[index + 2] == 'l' &&
				    json[index + 3] == 's' &&
				    json[index + 4] == 'e') {
					index += 5;
					return MiniJSON.TOKEN_FALSE;
				}
			}
			
			// true
			if (remainingLength >= 4) {
				if (json[index] == 't' &&
				    json[index + 1] == 'r' &&
				    json[index + 2] == 'u' &&
				    json[index + 3] == 'e') {
					index += 4;
					return MiniJSON.TOKEN_TRUE;
				}
			}
			
			// null
			if (remainingLength >= 4) {
				if (json[index] == 'n' &&
				    json[index + 1] == 'u' &&
				    json[index + 2] == 'l' &&
				    json[index + 3] == 'l') {
					index += 4;
					return MiniJSON.TOKEN_NULL;
				}
			}
			
			return MiniJSON.TOKEN_NONE;
		}
		
		
		protected bool SerializeObjectOrArray(object objectOrArray, StringBuilder builder)
		{
			if (objectOrArray is Hashtable)
				return SerializeObject((Hashtable)objectOrArray, builder);
			else if (objectOrArray is ArrayList)
				return SerializeArray((ArrayList)objectOrArray, builder);
			else
				return false;
		}
		
		
		protected bool SerializeObject (Hashtable anObject, StringBuilder builder)
		{
			builder.Append ("{");
			
			IDictionaryEnumerator e = anObject.GetEnumerator ();
			bool first = true;
			while (e.MoveNext()) {
				string key = e.Key.ToString ();
				object value = e.Value;
				
				if (!first)
					builder.Append (", ");
				
				SerializeString (key, builder);
				builder.Append (":");
				if (!SerializeValue (value, builder))
					return false;
				
				first = false;
			}
			
			builder.Append ("}");
			return true;
		}
		
		
		protected bool SerializeProperties (object anObject, StringBuilder builder)
		{
			builder.Append ("{");
			
			
			var properties = anObject.GetType ().GetFields (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			bool first = true;
			foreach (System.Reflection.FieldInfo pi in properties) {			
				string key = pi.Name;
				object value = pi.GetValue (anObject);
				
				if (!first)
					builder.Append (", ");
				
				SerializeString (key, builder);
				builder.Append (":");
				if (!SerializeValue (value, builder))
					return false;
				
				first = false;
			}
			
			builder.Append ("}");
			return true;
		}
		
		protected bool SerializeArray(IList anArray, StringBuilder builder)
		{
			builder.Append("[");
			
			bool first = true;
			for (int i = 0; i < anArray.Count; i++) {
				object value = anArray[i];
				
				if (!first) {
					builder.Append(", ");
				}
				
				if (!SerializeValue(value, builder)) {
					return false;
				}
				
				first = false;
			}
			
			builder.Append("]");
			return true;
		}
		
		
		protected bool SerializeValue (object value, StringBuilder builder)
		{
			// Type t = value.GetType();
			// Debug.Log("type: " + t.ToString() + " isArray: " + t.IsArray);
			
			if (value == null)
				builder.Append ("null");
			else if (value.GetType ().IsArray)
				SerializeArray (new ArrayList ((ICollection)value), builder);
			else if (value is string)
				SerializeString ((string)value, builder);
			else if (value is Char) 		
				SerializeString (Convert.ToString ((char)value), builder);
			else if (value is Hashtable)
				SerializeObject ((Hashtable)value, builder);
			else if (value is IList)
				SerializeArray ((IList)value, builder);
			else if ((value is Boolean) && ((Boolean)value == true))
				builder.Append ("true");
			else if ((value is Boolean) && ((Boolean)value == false))
				builder.Append ("false");
			else if (value.GetType ().IsPrimitive)
				SerializeNumber (Convert.ToDouble (value), builder);
			else if (value.GetType ().IsEnum)
				SerializeNumber (Convert.ToDouble (value), builder);
			else
				return SerializeProperties (value, builder);
			
			return true;
		}
		
		
		protected void SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");
			
			char[] charArray = aString.ToCharArray();
			for (int i = 0; i < charArray.Length; i++) {
				char c = charArray[i];
				if (c == '"') {
					builder.Append("\\\"");
				} else if (c == '\\') {
					builder.Append("\\\\");
				} else if (c == '\b') {
					builder.Append("\\b");
				} else if (c == '\f') {
					builder.Append("\\f");
				} else if (c == '\n') {
					builder.Append("\\n");
				} else if (c == '\r') {
					builder.Append("\\r");
				} else if (c == '\t') {
					builder.Append("\\t");
				} else {
					int codepoint = Convert.ToInt32(c);
					if ((codepoint >= 32) && (codepoint <= 126)) {
						builder.Append(c);
					} else {
						builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
					}
				}
			}
			
			builder.Append("\"");
		}
		
		
		protected void SerializeNumber(double number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number)); // , CultureInfo.InvariantCulture));
		}
		
		/*
	/// <summary>
	/// Determines if a given object is numeric in any way
	/// (can be integer, double, etc). C# has no pretty way to do this.
	/// </summary>
	protected bool IsNumeric(object o)
	{
		try {
			Double.Parse(o.ToString());
		} catch (Exception) {
			return false;
		}
		return true;
	}
	*/
	}
}
