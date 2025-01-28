using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Class that proviedes common assertions.
    /// </summary>
    internal static class Assert
    {
        /// <summary>
        /// Checks if argument is not null. If it is null, throws the ArgumentNullException.
        /// </summary>
        /// <param name="argument">Argument to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="message">A message that describes the error.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="argument"/> is null.</exception>
        public static void ArgumentNotNull(object argument, string argumentName, string message)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }

        /// <summary>
        /// Checks if the condition is true. If it is not, throws the ArgumentException.
        /// </summary>
        /// <param name="condition">Condition to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="message">A message that describes the error.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="condition"/> is false.</exception>
        public static void ArgumentValid(bool condition, string argumentName, string message)
        {
            if (!condition)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Checks if the condition is true. If it is not, throw the InvalidOperationException.
        /// </summary>
        /// <param name="condition">Condition to check.</param>
        /// <param name="message">A message that describes the error.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="condition"/> is false.</exception>
        public static void OperationValid(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
