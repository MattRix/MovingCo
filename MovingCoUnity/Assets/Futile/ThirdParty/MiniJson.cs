/*
 * Copyright (c) 2012 Calvin Rien
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Example usage:
//
//  using UnityEngine;
//  using System.Collections;
//  using System.Collections.Generic;
//  using MiniJson;
//
//  public class MiniJsonTest : MonoBehaviour {
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
/// JSON uses Arrays and Objects. These correspond here to the datatypes List<object> and Dictionary<string,object>.
/// All numbers are parsed to doubles.
/// </summary>
public static class Json {
	/// <summary>
	/// Parses the string json into a value
	/// </summary>
	/// <param name="json">A JSON string.</param>
	/// <returns>An List<object>, a Dictionary<string,object>, a double, an integer,a string, null, true, or false</returns>
	public static object Deserialize(string json) 
	{
	    // save the string for debug information
	    if (json == null) 
		{
	        return null;
	    }

	    return Parser.Parse(json);
	}

	sealed class Parser : IDisposable 
	{
	    const string WHITE_SPACE = " \t\n\r";
	    const string WORD_BREAK = " \t\n\r{}[],:\"";

	    enum TOKEN 
		{
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

	    Parser(string jsonString) 
		{
	        json = new StringReader(jsonString);
	    }

	    public static object Parse(string jsonString) 
		{
	        using (var instance = new Parser(jsonString)) 
			{
	            return instance.ParseValue();
	        }
	    }

	    public void Dispose() 
		{
	        json.Dispose();
	        json = null;
	    }

	    Dictionary<string,object> ParseObject() 
		{
	        Dictionary<string,object> table = new Dictionary<string,object>();

	        // ditch opening brace
	        json.Read();

	        // {
	        while (true) 
			{
	            switch (NextToken) 
				{
	            case TOKEN.NONE:
	                return null;
	            case TOKEN.COMMA:
	                continue;
	            case TOKEN.CURLY_CLOSE:
	                return table;
	            default:
	                // name
	                string name = ParseString();
	                if (name == null) 
						{
	                    return null;
	                }

	                // :
	                if (NextToken != TOKEN.COLON)
					{
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

	    List<object> ParseArray()
		{
	        List<object> array = new List<object>();

	        // ditch opening bracket
	        json.Read();

	        // [
	        var parsing = true;
	        while (parsing) 
			{
	            TOKEN nextToken = NextToken;

	            switch (nextToken) 
				{
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

	    object ParseValue() 
		{
	        TOKEN nextToken = NextToken;
	        return ParseByToken(nextToken);
	    }

	    object ParseByToken(TOKEN token) 
		{
	        switch (token)
			{
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

	    string ParseString() 
		{
	        StringBuilder s = new StringBuilder();
	        char c;

	        // ditch opening quote
	        json.Read();

	        bool parsing = true;
	        while (parsing) 
			{

	            if (json.Peek() == -1) 
				{
	                parsing = false;
	                break;
	            }

	            c = NextChar;
	            switch (c) 
				{
	            case '"':
	                parsing = false;
	                break;
	            case '\\':
	                if (json.Peek() == -1)
					{
	                    parsing = false;
	                    break;
	                }

	                c = NextChar;
	                switch (c) 
					{
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
	                    var hex = new StringBuilder();

	                    for (int i=0; i< 4; i++) 
						{
	                        hex.Append(NextChar);
	                    }

	                    s.Append((char) Convert.ToInt32(hex.ToString(), 16));
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

	    object ParseNumber() 
		{
	        string number = NextWord;

	        if (number.IndexOf('.') == -1)
			{
	            long parsedInt;
	            Int64.TryParse(number, out parsedInt);
	            return parsedInt;
	        }

	        double parsedDouble;
	        Double.TryParse(number, out parsedDouble);
	        return parsedDouble;
	    }

	    void EatWhitespace() 
		{
			if (json.Peek() == -1) return;

	        while (WHITE_SPACE.IndexOf(PeekChar) != -1) 
			{
	            json.Read();

	            if (json.Peek() == -1) 
				{
	                break;
	            }
	        }
	    }

	    char PeekChar 
		{
	        get 
			{
	            return Convert.ToChar(json.Peek());
	        }
	    }

	    char NextChar 
		{
	        get
			{
	            return Convert.ToChar(json.Read());
	        }
	    }

	    string NextWord 
		{
	        get 
			{
	            StringBuilder word = new StringBuilder();

				if (json.Peek() == -1) return "";

	            while (WORD_BREAK.IndexOf(PeekChar) == -1) 
				{
	                word.Append(NextChar);

	                if (json.Peek() == -1) 
					{
	                    break;
	                }
	            }

	            return word.ToString();
	        }
	    }

	    TOKEN NextToken 
		{
	        get 
			{
	            EatWhitespace();

	            if (json.Peek() == -1) 
				{
	                return TOKEN.NONE;
	            }

	            char c = PeekChar;
	            switch (c)
				{
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

	            string word = NextWord;

	            switch (word) 
				{
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
	/// Converts a Dictionary<string,object> / List<object> object or a simple type (string, int, etc.) into a JSON string
	/// </summary>
	/// <param name="json">A Dictionary<string,object>&lt;string, object&gt; / List<object>&lt;object&gt;</param>
	/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
	public static string Serialize(object obj) 
	{
	    return Serializer.Serialize(obj);
	}

	sealed class Serializer 
	{
	    StringBuilder builder;

	    Serializer() 
		{
	        builder = new StringBuilder(1000);
	    }

	    public static string Serialize(object obj) 
		{
	        var instance = new Serializer();

	        instance.SerializeValue(obj);

	        return instance.builder.ToString();
	    }

	    void SerializeValue(object value) 
		{
	        List<object> asList;
	        Dictionary<string,object> asDict;
	        string asStr;


			if (value == null) 
			{
	            builder.Append("null");
	        }
	        else if ((asStr = value as string) != null) 
			{
	            SerializeString(asStr);
	        }
	        else if (value is bool) 
			{
	            builder.Append(value.ToString().ToLower());
	        }
	        else if ((asList = value as List<object>) != null) 
			{
	            SerializeArray(asList);
	        }
	        else if ((asDict = value as Dictionary<string,object>) != null) 
			{
	            SerializeObject(asDict);
	        }
	        else if (value is char) 
			{
	            SerializeString(value.ToString());
	        }
			else if(value is List<Dictionary<string,object>>)
			{
				List<Dictionary<string,object>> dictList = value as List<Dictionary<string,object>>;
				asList = new List<object>();
				
				for(int s = 0;s<dictList.Count;s++)
				{
					asList.Add(dictList[s]);
				}
				
				SerializeArray(asList);
			}
			else if(value is List<string>)
			{
				List<string> stringList = value as List<string>;
				asList = new List<object>();
				
				for(int s = 0;s<stringList.Count;s++)
				{
					asList.Add(stringList[s]);
				}
				
				SerializeArray(asList);
			}
			else if(value is List<int>)
			{
				List<int> valueList = value as List<int>;
				asList = new List<object>();
				
				for(int s = 0;s<valueList.Count;s++)
				{
					asList.Add(valueList[s]);
				}
				
				SerializeArray(asList);
			}
			else if(value is List<bool>)
			{
				List<bool> valueList = value as List<bool>;
				asList = new List<object>();
				
				for(int s = 0;s<valueList.Count;s++)
				{
					asList.Add(valueList[s]);
				}
				
				SerializeArray(asList);
			}
			else if(value is List<float>)
			{
				List<float> valueList = value as List<float>;
				asList = new List<object>();
				
				for(int s = 0;s<valueList.Count;s++)
				{
					asList.Add(valueList[s]);
				}
				
				SerializeArray(asList);
			}
	        else 
			{
	            SerializeOther(value);
	        }
	    }

	    void SerializeObject(Dictionary<string,object> obj) 
		{
	        bool first = true;

	        builder.Append('{');

	        foreach (string key in obj.Keys) 
			{
	            if (!first) 
				{
	                builder.Append(',');
	            }

				builder.Append("\""+key.ToString()+"\"");

	            builder.Append(':');

				SerializeValue(obj[key]);

	            first = false;
	        }

	        builder.Append('}');
	    }

	    void SerializeArray(List<object> anArray) 
		{
	        builder.Append('[');

			for(int a = 0; a<anArray.Count; a++)
			{
	            if (a != 0) 
				{
	                builder.Append(',');
	            }

				SerializeValue(anArray[a]);
	        }

	        builder.Append(']');
	    }

	    void SerializeString(string str) 
		{
	        builder.Append('\"');

	        char[] charArray = str.ToCharArray();
			for(int c = 0; c<charArray.Length; c++)
			{
				char ch = charArray[c]; 

				switch (ch) {
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
	                int codepoint = Convert.ToInt32(ch);
	                if ((codepoint >= 32) && (codepoint <= 126)) 
					{
	                    builder.Append(ch);
	                }
	                else 
					{
	                    builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
	                }
	                break;
	            }
			}

			builder.Append('\"');
		}

		void SerializeOther(object value) 
		{
			if (value is int
			    || value is uint
			    || value is long
			    || value is sbyte
			    || value is byte
			    || value is short
			    || value is ushort
			    || value is ulong) 
			{
				builder.Append(value.ToString());
			}
			else if(value is float
			        || value is double
			        || value is decimal)
			{
				builder.Append(string.Format("{0:0.000}", value));
			}
			else 
			{
				SerializeString(value.ToString());
			}
		}
	}
}

#region Extension methods

public static class MiniJsonExtensions
{
	public static string toJson( this Dictionary<string,object> obj )
	{
		return Json.Serialize( obj );
	}

	public static List<object> listFromJson( this string json )
	{
		return (List<object>) Json.Deserialize( json );
	}

	public static Dictionary<string,object> dictionaryFromJson( this string json )
	{
		return (Dictionary<string,object>) Json.Deserialize( json );
	}
}

#endregion