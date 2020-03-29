using System.Linq;
using System.Text;

/// <summary>
/// simple jsonFormatter inspired by aloisdg at https://dotnetfiddle.net/z7Gfu5
/// </summary>
namespace JsonFormatter {
    static class JsonHelper {
        public static string FormatJson(string str) {
            int indent = 0;
            bool quoted = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++) {
                char ch = str[i];
                switch (ch) {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        padStringBuilder(sb, ++indent, quoted);
                        break;
                    case '}':
                    case ']':
                        padStringBuilder(sb, --indent, quoted);
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        padStringBuilder(sb, indent, quoted);
                        break;
                    case ':':
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        private static void padStringBuilder(StringBuilder sb, int indent, bool quoted) {
            const string INDENT_STRING = "    ";
            if (!quoted) {
                sb.AppendLine();
                foreach (int _ in Enumerable.Range(0, indent)) {
                    sb.Append(INDENT_STRING);
                }
            }
        }
    }
}
