using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Json.JsonParser
{
    public enum JsonToken
    {
        LeftCurlyBracket,
        RightCurlyBracket,
        LeftBracket,
        RightBracket,
        Colon,
        Comma,
        StringMarker,
        Number,
        BoolTrue,
        BoolFalse,
        Null,
        Unknown
    }

    public class JsonParser
    {
        public static T Deserialize<T>(string json)
        {
            int index = 0;
            object item = ParseValue(json, ref index);

            T instance = (T)GetInstance(item, typeof(T));

            return instance;
        }

        // TODO: add support for creating instances of collection types
        internal static object GetInstance(object @object, Type type)
        {
            object instance;

            if (IsJsonObject(@object.GetType()))
            {
                instance = MapJsonObjectToClass(@object as JsonObject, type);
            }
            else
            {
                instance = @object;
            }

            return instance;
        }

        // TODO: optimize method with expression trees or dynamic code generation
        internal static object MapJsonObjectToClass(JsonObject jsonObject, Type type)
        {
            object instance = Activator.CreateInstance(type);

            PropertyInfo[] propInfoArray = type.GetProperties();

            foreach (var propInfo in propInfoArray)
            {
                string propName = propInfo.Name;

                if (jsonObject.Keys.Contains(propName))
                {
                    object value = jsonObject[propName];

                    // handles json objects within json objects
                    if (IsJsonObject(value.GetType()))
                    {
                        Type nestedType = propInfo.PropertyType;
                        object nestedInstance = MapJsonObjectToClass(value as JsonObject, nestedType);
                        value = nestedInstance;
                    }

                    propInfo.SetValue(instance, value);
                }
            }

            return instance;
        }

        internal static JsonToken PeekNextToken(string json, int index)
        {
            int tempIndex = index;
            JsonToken token = GetNextToken(json, ref tempIndex);

            return token;
        }

        internal static JsonToken GetNextToken(string json, ref int index)
        {
            index = SkipWhitespace(json, index);

            char currentChar = json[index];
            index++;
            switch (currentChar)
            {
                case '{':
                    return JsonToken.LeftCurlyBracket;
                case '}':
                    return JsonToken.RightCurlyBracket;
                case '[':
                    return JsonToken.LeftBracket;
                case ']':
                    return JsonToken.RightBracket;
                case ':':
                    return JsonToken.Colon;
                case ',':
                    return JsonToken.Comma;
                case '\'':
                case '"':
                    return JsonToken.StringMarker;
                case '+':
                case '-':
                case '.':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '0':
                    return JsonToken.Number;
            }

            index--;
            int remainingLength = json.Length - index;

            if (remainingLength >= 4)
            {
                if (json.Substring(index, 4) == "true")
                {
                    index += 4;
                    return JsonToken.BoolTrue;
                }
                else if (json.Substring(index, 4) == "null")
                {
                    index += 4;
                    return JsonToken.Null;
                }
            }
            if (remainingLength >= 5)
            {
                if (json.Substring(index, 5) == "false")
                {
                    index += 5;
                    return JsonToken.BoolFalse;
                }
            }

            return JsonToken.Unknown;
        }

        internal static IDictionary<string, object> ParseObject(string json, ref int index)
        {
            IDictionary<string, object> @object = new JsonObject();
            JsonToken token;

            GetNextToken(json, ref index);

            while (true)
            {
                token = PeekNextToken(json, index);
                index = SkipWhitespace(json, index);

                if (token == JsonToken.RightCurlyBracket)
                {
                    GetNextToken(json, ref index);
                    return @object;
                }
                else if (token == JsonToken.Comma)
                {
                    GetNextToken(json, ref index);
                }
                else
                {
                    string name = ParseString(json, ref index);

                    token = GetNextToken(json, ref index);
                    if (token != JsonToken.Colon)
                    {
                        throw new NotImplementedException();
                    }

                    object value;
                    if (token == JsonToken.LeftCurlyBracket)
                    {
                        value = ParseObject(json, ref index);
                    }
                    else
                    {
                        value = ParseValue(json, ref index);
                    }

                    @object.Add(name, value);
                }
            }
        }

        internal static IEnumerable<object> ParseArray(string json, ref int index)
        {
            List<object> array = new List<object>();
            JsonToken token;

            index++;

            while (true)
            {
                token = PeekNextToken(json, index);
                index = SkipWhitespace(json, index);

                switch (token)
                {
                    case JsonToken.RightBracket:
                        GetNextToken(json, ref index);
                        return array;
                    case JsonToken.Comma:
                        GetNextToken(json, ref index);
                        break;
                    case JsonToken.LeftCurlyBracket:
                        object @object = ParseObject(json, ref index);
                        array.Add(@object);
                        break;
                    case JsonToken.LeftBracket:
                    case JsonToken.StringMarker:
                    case JsonToken.Number:
                    case JsonToken.BoolTrue:
                    case JsonToken.BoolFalse:
                    case JsonToken.Null:
                        object item = ParseValue(json, ref index);
                        array.Add(item);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        internal static object ParseValue(string json, ref int index)
        {
            JsonToken currentToken = PeekNextToken(json, index);
            index = SkipWhitespace(json, index);

            switch (currentToken)
            {
                case JsonToken.LeftCurlyBracket:
                    return ParseObject(json, ref index);
                case JsonToken.LeftBracket:
                    return ParseArray(json, ref index);
                case JsonToken.StringMarker:
                    return ParseString(json, ref index);
                case JsonToken.Number:
                    return ParseNumber(json, ref index);
                case JsonToken.BoolTrue:
                    GetNextToken(json, ref index);
                    return true;
                case JsonToken.BoolFalse:
                    GetNextToken(json, ref index); ;
                    return false;
                case JsonToken.Null:
                    GetNextToken(json, ref index);
                    return null;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static string ParseString(string json, ref int index)
        {
            StringBuilder builder = new StringBuilder();

            index++;

            while (json[index] != '"' && json[index] != '\'')
            {
                if (json[index] == '\\')
                {
                    if (index == json.Length)
                    {
                        break;
                    }

                    char nextChar = json[++index];

                    switch (nextChar)
                    {
                        case '"':
                            builder.Append('\"');
                            break;
                        case '\\':
                            builder.Append('\\');
                            break;
                        case '/':
                            builder.Append('/');
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 'u':
                            if (json.Length - index < 4)
                            {
                                break;
                            }
                            int number;
                            if (int.TryParse(json.Substring(index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out number))
                            {
                                builder.Append((char)number);
                                index += 4;
                            }
                            break;
                    }
                }
                else
                {
                    builder.Append(json[index++]);
                }
            }

            index++;

            return builder.ToString();
        }

        // TODO: support other numeric data types
        internal static object ParseNumber(string json, ref int index)
        {
            char[] validChars = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '-', '+', 'E', 'e' };

            StringBuilder builder = new StringBuilder();

            while (validChars.Contains(json[index]))
            {
                builder.Append(json[index++]);
            }

            string number = builder.ToString();
            if (number.Contains('.') || number.Contains('E') || number.Contains('e'))
            {
                double result;
                if (double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }
            else
            {
                long result;
                if (long.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }

            throw new NotImplementedException();
        }

        internal static bool IsJsonObject(Type type)
        {
            bool isJsonObject = type == typeof(JsonObject);
            return isJsonObject;
        }

        internal static int SkipWhitespace(string json, int index)
        {
            while (char.IsWhiteSpace(json[index]))
            {
                index++;
            }

            return index;
        }
    }
}
