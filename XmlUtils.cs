using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SilverConfig
{
    //The licenses in comments apply to only the functions below them
    public static class XmlUtils
    {
        // https://stackoverflow.com/a/2548782
        // https://creativecommons.org/licenses/by-sa/2.5/
        /// <summary>
        /// Serializes a specified object and returns a string containing XML
        /// </summary>
        /// <param name="input">The object to serialize</param>
        /// <returns>A string containing that object as XML</returns>
        public static async Task<string> SerializeToXmlAsync(object input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var ser = new XmlSerializer(input.GetType());
            var memStm = new MemoryStream();
            await using (memStm.ConfigureAwait(false))
            {
                ser.Serialize(memStm, input);
                memStm.Position = 0;
                using var streamreader = new StreamReader(memStm);
                return await streamreader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        //  https://stackoverflow.com/a/2548782
        //  https://creativecommons.org/licenses/by-sa/2.5/
        /// <summary>
        /// Serializes a specified object and returns a XmlDocument containing the object
        /// </summary>
        /// <param name="input">The object to serialize</param>
        /// <returns>A XmlDocument containing the object as XML</returns>
        public static XmlDocument SerializeToXmlDocument(object input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var ser = new XmlSerializer(input.GetType());
            using var memStm = new MemoryStream();
            ser.Serialize(memStm, input);
            memStm.Position = 0;
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true
            };
            using var xtr = XmlReader.Create(memStm, settings);
            var xd = new XmlDocument();
            xd.Load(xtr);
            return xd;
        }

        //    https://stackoverflow.com/a/6328534
        //    https://creativecommons.org/licenses/by-sa/3.0/
        /// <summary>
        /// Adds a comment in an object
        /// </summary>
        /// <param name="inputdoc">The input document</param>
        /// <param name="xpath">Xpath of the Object</param>
        /// <param name="comment">The comment</param>
        /// <returns>A <see cref="XmlDocument"/> that has the comment before the xpath thingy</returns>
        /// <exception cref="ElementNotFoundException">If xpath is not found</exception>
        public static XmlDocument CommentInObject(XmlDocument inputdoc, string xpath, string comment)
        {
            if (inputdoc is null)
            {
                throw new ArgumentNullException(nameof(inputdoc));
            }
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentException("xpath may not be null or empty", nameof(xpath));
            }

            var elementToComment = inputdoc.SelectSingleNode(xpath);
            if (elementToComment == null)
                throw new ElementNotFoundException("Could not find the element to comment, looked for " + xpath);
            elementToComment.InnerText = $"<!--{comment}-->\n{elementToComment.InnerText}";
            return inputdoc;
        }

        //     https://stackoverflow.com/a/6328534
        //     https://creativecommons.org/licenses/by-sa/3.0/
        /// <summary>
        /// Adds a comment to an object
        /// </summary>
        /// <param name="inputdoc">The input document</param>
        /// <param name="xpath">Xpath of the Object</param>
        /// <param name="comment">The comment</param>
        /// <returns>A <see cref="XmlDocument"/> that has the comment before the xpath thingy</returns>
        public static XmlDocument CommentBeforeObject(XmlDocument inputdoc, string xpath, string comment)
        {
            var elementToComment = inputdoc.SelectSingleNode(xpath);
            var commentNode = inputdoc.CreateComment(comment);
            var parentNode = elementToComment?.ParentNode;
            parentNode?.InsertBefore(commentNode, elementToComment);
            return inputdoc;
        }
    }
}