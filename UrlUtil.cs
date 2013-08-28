using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Xuld.RazorEngine {

    /// <summary>
    /// 提供 URL 解析的函数。
    /// </summary>
    public static class UrlUtil {

        public static string BuildUrl(string path, params object[] pathParts) {
            path = HttpUtility.UrlPathEncode(path);
            if (pathParts.Length > 0) {
                StringBuilder builder = new StringBuilder();
                foreach (object obj2 in pathParts) {
                    if (obj2.GetType().GetInterfaces().Length > 0) {
                        string str = Convert.ToString(obj2, CultureInfo.InvariantCulture);
                        path = path + "/" + HttpUtility.UrlPathEncode(str);
                    } else {
                        RouteValueDictionary dictionary = new RouteValueDictionary(obj2);
                        foreach (KeyValuePair<string, object> pair in dictionary) {
                            if (builder.Length == 0) {
                                builder.Append('?');
                            } else {
                                builder.Append('&');
                            }
                            string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
                            builder.Append(HttpUtility.UrlEncode(pair.Key)).Append('=').Append(HttpUtility.UrlEncode(str2));
                        }
                    }
                }
                path += builder;
            }
            return path;
        }

        public static string AppendSlash(string path) {
            if (String.IsNullOrEmpty(path)) {
                return "./";
            }

            if (path[path.Length - 1] == '/') {
                return path;
            }

            return path.Substring(0, path.Length - 1) + '/';
        }

        /// <summary>
        /// 获取后者相对前者的相对路径。
        /// </summary>
        /// <param name="fromPath">主目录</param>
        /// <param name="toPath">文件的绝对路径</param>
        /// <returns>toPath相对于fromPath的路径。</returns>
        /// <example>
        /// GetRelativePath(@"D:\Windows\Web\Wallpaper\aa.css", @"D:\Windows\regedit.exe" ) = @"..\..\regedit.exe" = ;
        /// </example>
        public static string Relative(string fromPath, string toPath, char seperatorChar = '/') {
            fromPath = NormalizePath(fromPath);
            toPath = NormalizePath(toPath);
            string[] fromParts = fromPath.Split('/');
            string[] toParts = toPath.Split('/');

            int length = Math.Min(fromParts.Length, toParts.Length);
            int samePartsLength = length;
            for (int i = 0; i < length; i++) {
                if (fromParts[i] != toParts[i]) {
                    samePartsLength = i;
                    break;
                }
            }

            StringBuilder sb = new StringBuilder();

            for (int i = samePartsLength + 1; i < fromParts.Length; i++) {
                sb.Append("../");
            }

            for (int i = samePartsLength; i < toParts.Length; i++) {
                sb.Append(toParts[i]);
                sb.Append('/');
            }

            if (sb.Length == 0) {
                return ".";
            }

            sb.Length--;

            return sb.ToString();
        }

        public static string Combine(string left, string right) {
            return NormalizePath(left + '/' + right);
        }

        public static string Resolve(string left, string right) {
            return Path.IsPathRooted(right) ? right : NormalizePath(BasePath(left) + right);
        }

        // resolves . and .. elements in a path array with directory names there
        // must be no slashes, empty elements, or device names (c:\) in the array
        // (so also no leading and trailing slashes - it does not distinguish
        // relative and absolute paths)
        static int NormalizeArray(string[] parts, int end = 0) {
            // if the path tries to go above the root, `up` ends up > 0
            int topLevelAboveRoot = 0;
            for (int i = parts.Length - 1; i >= end; i--) {
                string last = parts[i];
                if (last == ".") {
                    parts[i] = String.Empty;
                } else if (last == "..") {
                    parts[i] = String.Empty;
                    topLevelAboveRoot++;
                } else if (topLevelAboveRoot > 0) {
                    parts[i] = String.Empty;
                    topLevelAboveRoot--;
                }
            }

            return topLevelAboveRoot;
        }

        // Split a filename into [root, dir, basename, ext], unix version
        // 'root' is just a slash, or nothing.
        static string[] SplitPath(string path) {
            return path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        // Split a filename into [root, dir, basename, ext], unix version
        // 'root' is just a slash, or nothing.
        static string JoinPath(string[] parts) {
            StringBuilder sb = new StringBuilder(parts.Length * 20);
            for (int i = 0; i < parts.Length; i++) {
                if (parts[i] != null) {
                    sb.Append(parts[i]);
                }
            }

            return sb.ToString();
        }

        //static string Resolve(params string[] path) {
        //    var resolvedPath = '',
        //        resolvedAbsolute = false;

        //    for (var i = arguments.length - 1; i >= -1 && !resolvedAbsolute; i--) {
        //        var path = (i >= 0) ? arguments[i] : Path.basePath;

        //        // Skip empty and invalid entries
        //        if (typeof path !== 'string') {
        //            throw new TypeError('Arguments to path.resolve must be strings');
        //        } else if (!path) {
        //            continue;
        //        }

        //        resolvedPath = path + '/' + resolvedPath;
        //        resolvedAbsolute = path.charAt(0) === '/';
        //    }

        //    // At this point the path should be resolved to a full absolute path, but
        //    // handle relative paths to be safe (might happen when process.cwd() fails)

        //    // Normalize the path
        //    resolvedPath = Path._normalizeArray(resolvedPath.split('/').filter(function (p) {
        //        return !!p;
        //    }), !resolvedAbsolute).join('/');

        //    return ((resolvedAbsolute ? '/' : '') + resolvedPath) || '.';
        //},

        public static string NormalizePath(string path) {

            if (String.IsNullOrEmpty(path)) {
                return String.Empty;
            }

            char c;
            string[] parts = SplitPath(path);

            c = path[0];

            int end = 0;
            if (parts[0].Length == 0 || parts[0] == "~") {
                end = 1;
            } else if (parts[0][parts[0].Length - 1] == ':') {
                end = parts[0].Length > 2 && parts.Length > 1 && parts[1].Length == 0 ? 2 : 1;
            }

            c = path[path.Length - 1];
            bool trailingSlash = c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
            int count = NormalizeArray(parts, end);

            StringBuilder sb = new StringBuilder(parts.Length * 20);

            if (end == 0) {
                while (count-- > 0) {
                    sb.Append("../");
                }
            } else if (end == 1) {
                sb.Append(parts[0]);
                sb.Append('/');
            } else if (end == 2) {
                sb.Append(parts[0]);
                sb.Append('/');
                sb.Append('/');
            }


            for (int i = end; i < parts.Length; i++) {
                if (parts[i].Length > 0) {
                    sb.Append(parts[i]);
                    sb.Append('/');
                }
            }

            if (sb.Length == 0) {
                if (end == 0) {
                    sb.Append('.');
                }
            } else if (!trailingSlash) {
                sb.Length--;
            }

            return sb.ToString();
        }

        public static string BasePath(string path) {
            string newPath = System.Text.RegularExpressions.Regex.Replace(path, @"[\\/][^\\/]*$", "/");
            if (newPath.Length == path.Length) {
                if(path.IndexOf('\\') == -1 && path.IndexOf('/') == -1) {
                    newPath = String.Empty;
                }
            }

            return newPath;
        }

    }
}
