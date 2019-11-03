using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace QiqqaTestHelpers
{
    //
    // Summary:
    //     Provides a set of methods and properties that help debug your code.


    /// <summary>
    /// Represents support for asserting unit tests.
    /// </summary>
    /// <remarks>This class exposes all of the methods provided by <see cref="Assert"/>. All other assertion
    /// extensions are expected to be typically provided by defining extension methods.</remarks>
    public static class DebugOnlyAssertions
    {
        /// <summary>Verifies that two specified objects are equal. The assertion fails if the objects are not equal.</summary>
        /// <param name="expected">The first object to compare. This is the object the unit test expects.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(object expected, object actual)
        {
            ASSERT.AreEqual(expected, actual);
        }

        /// <summary>Verifies that two specified generic type data are equal. The assertion fails if they are not equal.</summary>
        /// <param name="expected">The first generic type data to compare. This is the generic type data the unit test expects.</param>
        /// <param name="actual">The second generic type data to compare. This is the generic type data the unit test produced.</param>
        /// <typeparam name="T">The <see cref="Type">type</see> of expected value.</typeparam>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual<T>(T expected, T actual)
        {
            ASSERT.AreEqual<T>(expected, actual);
        }

        /// <summary>Verifies that two specified doubles are equal, or within the specified accuracy of each other. The assertion fails if they are not within the specified accuracy of each other.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(double expected, double actual, double delta)
        {
            ASSERT.AreEqual(expected, actual, delta);
        }

        /// <summary>Verifies that two specified singles are equal, or within the specified accuracy of each other. The assertion fails if they are not within the specified accuracy of each other.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(float expected, float actual, float delta)
        {
            ASSERT.AreEqual(expected, actual, delta);
        }

        /// <summary>Verifies that two specified objects are equal. The assertion fails if the objects are not equal. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first object to compare. This is the object the unit test expects.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(object expected, object actual, string message)
        {
            ASSERT.AreEqual(expected, actual, message);
        }

        /// <summary>Verifies that two specified strings are equal, ignoring case or not as specified. The assertion fails if they are not equal.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
#pragma warning disable CA1304 // Specify CultureInfo
        [Conditional("DEBUG")]
        public static void AreEqual(string expected, string actual, bool ignoreCase)
        {
            ASSERT.AreEqual(expected, actual, ignoreCase);
        }
#pragma warning restore CA1304 // Specify CultureInfo

        /// <summary>Verifies that two specified generic type data are equal. The assertion fails if they are not equal. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first generic type data to compare. This is the generic type data the unit test expects.</param>
        /// <param name="actual">The second generic type data to compare. This is the generic type data the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <typeparam name="T">The <see cref="Type">type</see> of expected value.</typeparam>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            ASSERT.AreEqual<T>(expected, actual, message);
        }

        /// <summary>Verifies that two specified doubles are equal, or within the specified accuracy of each other. The assertion fails if they are not within the specified accuracy of each other. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(double expected, double actual, double delta, string message)
        {
            ASSERT.AreEqual(expected, actual, delta, message);
        }

        /// <summary>Verifies that two specified singles are equal, or within the specified accuracy of each other. The assertion fails if they are not within the specified accuracy of each other. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(float expected, float actual, float delta, string message)
        {
            ASSERT.AreEqual(expected, actual, delta, message);
        }

        /// <summary>Verifies that two specified objects are equal. The assertion fails if the objects are not equal. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first object to compare. This is the object the unit test expects.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(object expected, object actual, string message, params object[] parameters)
        {
            ASSERT.AreEqual(expected, actual, message, parameters);
        }

        /// <summary>Verifies that two specified strings are equal, ignoring case or not as specified, and using the culture info specified. The assertion fails if they are not equal.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            ASSERT.AreEqual(expected, actual, ignoreCase, culture);
        }

        /// <summary>Verifies that two specified strings are equal, ignoring case or not as specified. The assertion fails if they are not equal. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message)
        {
            ASSERT.AreEqual(expected, actual, ignoreCase, message);
        }

        /// <summary>Verifies that two specified generic type data are equal. The assertion fails if they are not equal. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first generic type data to compare. This is the generic type data the unit test expects.</param>
        /// <param name="actual">The second generic type data to compare. This is the generic type data the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <typeparam name="T">The <see cref="Type">type</see> of expected value.</typeparam>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual<T>(T expected, T actual, string message, params object[] parameters)
        {
            ASSERT.AreEqual<T>(expected, actual, message, parameters);
        }

        /// <summary>Verifies that two specified doubles are equal, or within the specified accuracy of each other. The assertion fails if they are not within the specified accuracy of each other. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit tests expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(double expected, double actual, double delta, string message, params object[] parameters)
        {
            ASSERT.AreEqual(expected, actual, delta, message, parameters);
        }

        /// <summary>Verifies that two specified singles are equal, or within the specified accuracy of each other. The assertion fails if they are not within the specified accuracy of each other. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(float expected, float actual, float delta, string message, params object[] parameters)
        {
            ASSERT.AreEqual(expected, actual, delta, message, parameters);
        }

        /// <summary>Verifies that two specified strings are equal, ignoring case or not as specified, and using the culture info specified. The assertion fails if they are not equal. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message)
        {
            ASSERT.AreEqual(expected, actual, ignoreCase, culture, message);
        }

        /// <summary>Verifies that two specified strings are equal, ignoring case or not as specified. The assertion fails if they are not equal. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            ASSERT.AreEqual(expected, actual, ignoreCase, message, parameters);
        }

        /// <summary>Verifies that two specified strings are equal, ignoring case or not as specified, and using the culture info specified. The assertion fails if they are not equal. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message, params object[] parameters)
        {
            ASSERT.AreEqual(expected, actual, ignoreCase, culture, message, parameters);
        }

        /// <summary>Verifies that two specified objects are not equal. The assertion fails if the objects are equal.</summary>
        /// <param name="notExpected">The first object to compare. This is the object the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(object notExpected, object actual)
        {
            ASSERT.AreNotEqual(notExpected, actual);
        }

        /// <summary>Verifies that two specified generic type data are not equal. The assertion fails if they are equal.</summary>
        /// <param name="notExpected">The first generic type data to compare. This is the generic type data the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second generic type data to compare. This is the generic type data the unit test produced.</param>
        /// <typeparam name="T">The <see cref="Type">type</see> of expected value.</typeparam>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            ASSERT.AreNotEqual<T>(notExpected, actual);
        }

        /// <summary>Verifies that two specified doubles are not equal, and not within the specified accuracy of each other. The assertion fails if they are equal or within the specified accuracy of each other.</summary>
        /// <param name="notExpected">The first double to compare. This is the double the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required inaccuracy. The assertion fails only if <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(double notExpected, double actual, double delta)
        {
            ASSERT.AreNotEqual(notExpected, actual, delta);
        }

        /// <summary>Verifies that two specified singles are not equal, and not within the specified accuracy of each other. The assertion fails if they are equal or within the specified accuracy of each other.</summary>
        /// <param name="notExpected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required inaccuracy. The assertion will fail only if <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(float notExpected, float actual, float delta)
        {
            ASSERT.AreNotEqual(notExpected, actual, delta);
        }

        /// <summary>Verifies that two specified objects are not equal. The assertion fails if the objects are equal. Displays a message if the assertion fails.</summary>
        /// <param name="notExpected">The first object to compare. This is the object the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(object notExpected, object actual, string message)
        {
            ASSERT.AreNotEqual(notExpected, actual, message);
        }

        /// <summary>Verifies that two specified strings are not equal, ignoring case or not as specified. The assertion fails if they are equal.</summary>
        /// <param name="notExpected">The first string to compare. This is the string the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
#pragma warning disable CA1304 // Specify CultureInfo
        [Conditional("DEBUG")]
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase)
        {
            ASSERT.AreNotEqual(notExpected, actual, ignoreCase);
        }
#pragma warning restore CA1304 // Specify CultureInfo

        /// <summary>Verifies that two specified generic type data are not equal. The assertion fails if they are equal. Displays a message if the assertion fails.</summary>
        /// <param name="notExpected">The first generic type data to compare. This is the generic type data the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second generic type data to compare. This is the generic type data the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <typeparam name="T">The <see cref="Type">type</see> of expected value.</typeparam>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual<T>(T notExpected, T actual, string message)
        {
            ASSERT.AreNotEqual<T>(notExpected, actual, message);
        }

        /// <summary>Verifies that two specified doubles are not equal, and not within the specified accuracy of each other. The assertion fails if they are equal or within the specified accuracy of each other. Displays a message if the assertion fails.</summary>
        /// <param name="notExpected">The first double to compare. This is the double the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required inaccuracy. The assertion fails only if <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(double notExpected, double actual, double delta, string message)
        {
            ASSERT.AreNotEqual(notExpected, actual, delta, message);
        }

        /// <summary>Verifies that two specified singles are not equal, and not within the specified accuracy of each other. The assertion fails if they are equal or within the specified accuracy of each other. Displays a message if the assertion fails.</summary>
        /// <param name="notExpected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required inaccuracy. The assertion will fail only if <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(float notExpected, float actual, float delta, string message)
        {
            ASSERT.AreNotEqual(notExpected, actual, delta, message);
        }

        /// <summary>Verifies that two specified objects are not equal. The assertion fails if the objects are equal. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="notExpected">The first object to compare. This is the object the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(object notExpected, object actual, string message, params object[] parameters)
        {
            ASSERT.AreNotEqual(notExpected, actual, message, parameters);
        }

        /// <summary>Verifies that two specified strings are not equal, ignoring case or not as specified, and using the culture info specified. The assertion fails if they are equal.</summary>
        /// <param name="notExpected">The first string to compare. This is the string the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            ASSERT.AreNotEqual(notExpected, actual, ignoreCase, culture);
        }

        /// <summary>Verifies that two specified strings are not equal, ignoring case or not as specified. The assertion fails if they are equal. Displays a message if the assertion fails.</summary>
        /// <param name="notExpected">The first string to compare. This is the string the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message)
        {
            ASSERT.AreNotEqual(notExpected, actual, ignoreCase, message);
        }

        /// <summary>Verifies that two specified generic type data are not equal. The assertion fails if they are equal. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="notExpected">The first generic type data to compare. This is the generic type data the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second generic type data to compare. This is the generic type data the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <typeparam name="T">The <see cref="Type">type</see> of expected value.</typeparam>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual<T>(T notExpected, T actual, string message, params object[] parameters)
        {
            ASSERT.AreNotEqual<T>(notExpected, actual, message, parameters);
        }

        /// <summary>Verifies that two specified doubles are not equal, and not within the specified accuracy of each other. The assertion fails if they are equal or within the specified accuracy of each other. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="notExpected">The first double to compare. This is the double the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required inaccuracy. The assertion will fail only if <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(double notExpected, double actual, double delta, string message, params object[] parameters)
        {
            ASSERT.AreNotEqual(notExpected, actual, delta, message, parameters);
        }

        /// <summary>Verifies that two specified singles are not equal, and not within the specified accuracy of each other. The assertion fails if they are equal or within the specified accuracy of each other. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="notExpected">The first single to compare. This is the single the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required inaccuracy. The assertion will fail only if <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" /> or different from it by less than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(float notExpected, float actual, float delta, string message, params object[] parameters)
        {
            ASSERT.AreNotEqual(notExpected, actual, delta, message, parameters);
        }

        /// <summary>Verifies that two specified strings are not equal, ignoring case or not as specified, and using the culture info specified. The assertion fails if they are equal. Displays a message if the assertion fails.</summary>
        /// <param name="notExpected">The first string to compare. This is the string the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message)
        {
            ASSERT.AreNotEqual(notExpected, actual, ignoreCase, culture, message);
        }

        /// <summary>Verifies that two specified strings are not equal, ignoring case or not as specified. The assertion fails if they are equal. Displays a message if the assertion fails, and applies the specified formatting to it. </summary>
        /// <param name="notExpected">The first string to compare. This is the string the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            ASSERT.AreNotEqual(notExpected, actual, ignoreCase, message, parameters);
        }

        /// <summary>Verifies that two specified strings are not equal, ignoring case or not as specified, and using the culture info specified. The assertion fails if they are equal. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="notExpected">The first string to compare. This is the string the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> is equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message, params object[] parameters)
        {
            ASSERT.AreNotEqual(notExpected, actual, ignoreCase, culture, message, parameters);
        }

        /// <summary>Verifies that two specified object variables refer to different objects. The assertion fails if they refer to the same object.</summary>
        /// <param name="notExpected">The first object to compare. This is the object the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> refers to the same object as <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotSame(object notExpected, object actual)
        {
            ASSERT.AreNotSame(notExpected, actual);
        }

        /// <summary>Verifies that two specified object variables refer to different objects. The assertion fails if they refer to the same object. Displays a message if the assertion fails. </summary>
        /// <param name="notExpected">The first object to compare. This is the object the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> refers to the same object as <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotSame(object notExpected, object actual, string message)
        {
            ASSERT.AreNotSame(notExpected, actual, message);
        }

        /// <summary>Verifies that two specified object variables refer to different objects. The assertion fails if they refer to the same object. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="notExpected">The first object to compare. This is the object the unit test expects not to match <paramref name="actual" />.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="notExpected" /> refers to the same object as <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreNotSame(object notExpected, object actual, string message, params object[] parameters)
        {
            ASSERT.AreNotSame(notExpected, actual, message, parameters);
        }

        /// <summary>Verifies that two specified object variables refer to the same object. The assertion fails if they refer to different objects.</summary>
        /// <param name="expected">The first object to compare. This is the object the unit test expects.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> does not refer to the same object as <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreSame(object expected, object actual)
        {
            ASSERT.AreSame(expected, actual);
        }

        /// <summary>Verifies that two specified object variables refer to the same object. The assertion fails if they refer to different objects. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first object to compare. This is the object the unit test expects.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> does not refer to the same object as <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreSame(object expected, object actual, string message)
        {
            ASSERT.AreSame(expected, actual, message);
        }

        /// <summary>Verifies that two specified object variables refer to the same object. The assertion fails if they refer to different objects. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first object to compare. This is the object the unit test expects.</param>
        /// <param name="actual">The second object to compare. This is the object the unit test produced.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> does not refer to the same object as <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void AreSame(object expected, object actual, string message, params object[] parameters)
        {
            ASSERT.AreSame(expected, actual, message, parameters);
        }

        /// <summary>Fails the assertion without checking any conditions.</summary>
        /// <exception cref="AssertFailedException">Always thrown.</exception>
        [Conditional("DEBUG")]
        public static void Fail()
        {
            ASSERT.Fail();
        }

        /// <summary>Fails the assertion without checking any conditions. Displays a message.</summary>
        /// <param name="message">A message to display. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">Always thrown.</exception>
        [Conditional("DEBUG")]
        public static void Fail(string message)
        {
            ASSERT.Fail(message);
        }

        /// <summary>Fails the assertion without checking any conditions. Displays a message, and applies the specified formatting to it.</summary>
        /// <param name="message">A message to display. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">Always thrown.</exception>
        [Conditional("DEBUG")]
        public static void Fail(string message, params object[] parameters)
        {
            ASSERT.Fail(message, parameters);
        }

        /// <summary>Indicates that the assertion cannot be verified.</summary>
        /// <exception cref="AssertInconclusiveException">Always thrown.</exception>
        [Conditional("DEBUG")]
        public static void Inconclusive()
        {
            ASSERT.Inconclusive();
        }

        /// <summary>Indicates that the assertion can not be verified. Displays a message.</summary>
        /// <param name="message">A message to display. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertInconclusiveException">Always thrown.</exception>
        [Conditional("DEBUG")]
        public static void Inconclusive(string message)
        {
            ASSERT.Inconclusive(message);
        }

        /// <summary>Indicates that an assertion can not be verified. Displays a message, and applies the specified formatting to it.</summary>
        /// <param name="message">A message to display. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertInconclusiveException">Always thrown.</exception>
        [Conditional("DEBUG")]
        public static void Inconclusive(string message, params object[] parameters)
        {
            ASSERT.Inconclusive(message, parameters);
        }

        /// <summary>Verifies that the specified condition is false. The assertion fails if the condition is true.</summary>
        /// <param name="condition">The condition to verify is false.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="condition" /> evaluates to true.</exception>
        [Conditional("DEBUG")]
        public static void IsFalse(bool condition)
        {
            ASSERT.IsFalse(condition);
        }

        /// <summary>Verifies that the specified condition is false. The assertion fails if the condition is true. Displays a message if the assertion fails.</summary>
        /// <param name="condition">The condition to verify is false.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="condition" /> evaluates to true.</exception>
        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, string message)
        {
            ASSERT.IsFalse(condition, message);
        }

        /// <summary>Verifies that the specified condition is false. The assertion fails if the condition is true. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="condition">The condition to verify is false.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="condition" /> evaluates to true.</exception>
        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, string message, params object[] parameters)
        {
            ASSERT.IsFalse(condition, message, parameters);
        }

        /// <summary>Verifies that the specified object is an instance of the specified type. The assertion fails if the type is not found in the inheritance hierarchy of the object.</summary>
        /// <param name="value">The object to verify is of <paramref name="expectedType" />.</param>
        /// <param name="expectedType">The type expected to be found in the inheritance hierarchy of <paramref name="value" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is null or <paramref name="expectedType" /> is not found in the inheritance hierarchy of <paramref name="value" />.</exception>
        [Conditional("DEBUG")]
        public static void IsInstanceOfType(object value, Type expectedType)
        {
            ASSERT.IsInstanceOfType(value, expectedType);
        }

        /// <summary>Verifies that the specified object is an instance of the specified type. The assertion fails if the type is not found in the inheritance hierarchy of the object. Displays a message if the assertion fails.</summary>
        /// <param name="value">The object to verify is of <paramref name="expectedType" />.</param>
        /// <param name="expectedType">The type expected to be found in the inheritance hierarchy of <paramref name="value" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is null or <paramref name="expectedType" /> is not found in the inheritance hierarchy of <paramref name="value" />.</exception>
        [Conditional("DEBUG")]
        public static void IsInstanceOfType(object value, Type expectedType, string message)
        {
            ASSERT.IsInstanceOfType(value, expectedType, message);
        }

        /// <summary>Verifies that the specified object is an instance of the specified type. The assertion fails if the type is not found in the inheritance hierarchy of the object. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="value">The object to verify is of <paramref name="expectedType" />.</param>
        /// <param name="expectedType">The type expected to be found in the inheritance hierarchy of <paramref name="value" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is null or <paramref name="expectedType" /> is not found in the inheritance hierarchy of <paramref name="value" />.</exception>
        [Conditional("DEBUG")]
        public static void IsInstanceOfType(object value, Type expectedType, string message, params object[] parameters)
        {
            ASSERT.IsInstanceOfType(value, expectedType, message, parameters);
        }

        /// <summary>Verifies that the specified object is not an instance of the specified type. The assertion fails if the type is found in the inheritance hierarchy of the object.</summary>
        /// <param name="value">The object to verify is not of <paramref name="wrongType" />.</param>
        /// <param name="wrongType">The type that should not be found in the inheritance hierarchy of <paramref name="value" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is not null and <paramref name="wrongType" /> is found in the inheritance hierarchy of <paramref name="value" />.</exception>
        [Conditional("DEBUG")]
        public static void IsNotInstanceOfType(object value, Type wrongType)
        {
            ASSERT.IsNotInstanceOfType(value, wrongType);
        }

        /// <summary>Verifies that the specified object is not an instance of the specified type. The assertion fails if the type is found in the inheritance hierarchy of the object. Displays a message if the assertion fails.</summary>
        /// <param name="value">The object to verify is not of <paramref name="wrongType" />.</param>
        /// <param name="wrongType">The type that should not be found in the inheritance hierarchy of <paramref name="value" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results. </param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is not null and <paramref name="wrongType" /> is found in the inheritance hierarchy of <paramref name="value" />.</exception>
        [Conditional("DEBUG")]
        public static void IsNotInstanceOfType(object value, Type wrongType, string message)
        {
            ASSERT.IsNotInstanceOfType(value, wrongType, message);
        }

        /// <summary>Verifies that the specified object is not an instance of the specified type. The assertion fails if the type is found in the inheritance hierarchy of the object. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="value">The object to verify is not of <paramref name="wrongType" />.</param>
        /// <param name="wrongType">The type that should not be found in the inheritance hierarchy of <paramref name="value" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results. </param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is not null and <paramref name="wrongType" /> is found in the inheritance hierarchy of <paramref name="value" />.</exception>
        [Conditional("DEBUG")]
        public static void IsNotInstanceOfType(object value, Type wrongType, string message, params object[] parameters)
        {
            ASSERT.IsNotInstanceOfType(value, wrongType, message, parameters);
        }

        /// <summary>Verifies that the specified object is not null. The assertion fails if it is null.</summary>
        /// <param name="value">The object to verify is not null.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is null.</exception>
        [Conditional("DEBUG")]
        public static void IsNotNull(object value)
        {
            ASSERT.IsNotNull(value);
        }

        /// <summary>Verifies that the specified object is not null. The assertion fails if it is null. Displays a message if the assertion fails.</summary>
        /// <param name="value">The object to verify is not null.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is null.</exception>
        [Conditional("DEBUG")]
        public static void IsNotNull(object value, string message)
        {
            ASSERT.IsNotNull(value, message);
        }

        /// <summary>Verifies that the specified object is not null. The assertion fails if it is null. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="value">The object to verify is not null.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is null.</exception>
        [Conditional("DEBUG")]
        public static void IsNotNull(object value, string message, params object[] parameters)
        {
            ASSERT.IsNotNull(value, message, parameters);
        }

        /// <summary>Verifies that the specified object is null. The assertion fails if it is not null.</summary>
        /// <param name="value">The object to verify is null.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is not null.</exception>
        [Conditional("DEBUG")]
        public static void IsNull(object value)
        {
            ASSERT.IsNull(value);
        }

        /// <summary>Verifies that the specified object is null. The assertion fails if it is not null. Displays a message if the assertion fails.</summary>
        /// <param name="value">The object to verify is null.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is not null.</exception>
        [Conditional("DEBUG")]
        public static void IsNull(object value, string message)
        {
            ASSERT.IsNull(value, message);
        }

        /// <summary>Verifies that the specified object is null. The assertion fails if it is not null. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="value">The object to verify is null.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="value" /> is not null.</exception>
        [Conditional("DEBUG")]
        public static void IsNull(object value, string message, params object[] parameters)
        {
            ASSERT.IsNull(value, message, parameters);
        }

        /// <summary>Verifies that the specified condition is true. The assertion fails if the condition is false.</summary>
        /// <param name="condition">The condition to verify is true.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="condition" /> evaluates to false.</exception>
        [Conditional("DEBUG")]
        public static void IsTrue(bool condition)
        {
            ASSERT.IsTrue(condition);
        }

        /// <summary>Verifies that the specified condition is true. The assertion fails if the condition is false. Displays a message if the assertion fails.</summary>
        /// <param name="condition">The condition to verify is true.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="condition" /> evaluates to false.</exception>
        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string message)
        {
            ASSERT.IsTrue(condition, message);
        }

        /// <summary>Verifies that the specified condition is true. The assertion fails if the condition is false. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="condition">The condition to verify is true.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="condition" /> evaluates to false.</exception>
        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string message, params object[] parameters)
        {
            ASSERT.IsTrue(condition, message, parameters);
        }

        internal const string NullParameterToAssert = "Parameter {0} must not be null.";

        /// <summary>
        /// Asserts the specified assertion test parameter is not null.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> to evaluate.</typeparam>
        /// <param name="parameter">The parameter to evaluate.</param>
        /// <param name="parameterName">The parameter name.</param>
        [Conditional("DEBUG")]
        public static void AssertParameterIsNotNull<T>(T parameter, string parameterName)
        {
            ASSERT.AssertParameterIsNotNull<T>(parameter, parameterName);
        }

        /// <summary>
        /// Asserts the specified assertion test parameter is not null.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> to evaluate.</typeparam>
        /// <param name="parameter">The array parameter to evaluate.</param>
        /// <param name="parameterName">The parameter name.</param>
        [Conditional("DEBUG")]
        public static void AssertParameterIsNotNull<T>(T[] parameter, string parameterName)
        {
            ASSERT.AssertParameterIsNotNull<T>(parameter, parameterName);
        }

        /// <summary>
        /// Asserts the specified assertion test parameter is not null.
        /// </summary>
        /// <typeparam name="T">The nullable <see cref="Type">type</see> to evaluate.</typeparam>
        /// <param name="parameter">The <see cref="IEnumerable{T}">parameter</see> to evaluate.</param>
        /// <param name="parameterName">The parameter name.</param>
        [Conditional("DEBUG")]
        public static void AssertParameterIsNotNull<T>(IEnumerable<T> parameter, string parameterName)
        {
            ASSERT.AssertParameterIsNotNull<T>(parameter, parameterName);
        }


        // ==================================================================================================

        #region GreaterOrEqual

        /// <summary>Verifies that the expected value is greater than or equal to the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(double expected, double actual)
        {
            ASSERT.IsGreaterOrEqual(expected, actual);
        }


        /// <summary>Verifies that the expected value is greater than or equal to the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(float expected, float actual)
        {
            ASSERT.IsGreaterOrEqual(expected, actual);
        }


        /// <summary>Verifies that string value expected evaluates as greater than or equal to string value actual, ignoring case or not as specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
#pragma warning disable CA1304 // Specify CultureInfo
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(string expected, string actual, bool ignoreCase)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, ignoreCase);
        }
#pragma warning restore CA1304 // Specify CultureInfo

        /// <summary>Verifies that the expected value is greater than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(double expected, double actual, string message)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, message);
        }

        /// <summary>Verifies that the expected value is greater than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(float expected, float actual, string message)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, message);
        }

        /// <summary>Verifies that string value expected evaluates as greater than or equal to string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, ignoreCase, culture);
        }


        /// <summary>Verifies that string value expected evaluates as greater than or equal to string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(string expected, string actual, bool ignoreCase, string message)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, ignoreCase, message);
        }

        /// <summary>Verifies that the expected value is greater than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit tests expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(double expected, double actual, string message, params object[] parameters)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, message, parameters);
        }

        /// <summary>Verifies that the expected value is greater than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(float expected, float actual, string message, params object[] parameters)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, message, parameters);
        }


        /// <summary>Verifies that string value expected evaluates as greater than or equal to string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, ignoreCase, culture, message);
        }

        /// <summary>Verifies that string value expected evaluates as greater than or equal to string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, ignoreCase, message, parameters);
        }

        /// <summary>Verifies that string value expected evaluates as greater than or equal to string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterOrEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message, params object[] parameters)
        {
            ASSERT.IsGreaterOrEqual(expected, actual, ignoreCase, culture, message, parameters);
        }

        #endregion



        // ==================================================================================================

        #region GreaterThan

        /// <summary>Verifies that the expected value is greater than the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(double expected, double actual)
        {
            ASSERT.IsGreaterThan(expected, actual);
        }


        /// <summary>Verifies that the expected value is greater than the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(float expected, float actual)
        {
            ASSERT.IsGreaterThan(expected, actual);
        }


        /// <summary>Verifies that string value expected evaluates as greater than string value actual, ignoring case or not as specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
#pragma warning disable CA1304 // Specify CultureInfo
        [Conditional("DEBUG")]
        public static void IsGreaterThan(string expected, string actual, bool ignoreCase)
        {
            ASSERT.IsGreaterThan(expected, actual, ignoreCase);
        }
#pragma warning restore CA1304 // Specify CultureInfo

        /// <summary>Verifies that the expected value is greater than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(double expected, double actual, string message)
        {
            ASSERT.IsGreaterThan(expected, actual, message);
        }

        /// <summary>Verifies that the expected value is greater than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(float expected, float actual, string message)
        {
            ASSERT.IsGreaterThan(expected, actual, message);
        }

        /// <summary>Verifies that string value expected evaluates as greater than string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            ASSERT.IsGreaterThan(expected, actual, ignoreCase, culture);
        }


        /// <summary>Verifies that string value expected evaluates as greater than string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(string expected, string actual, bool ignoreCase, string message)
        {
            ASSERT.IsGreaterThan(expected, actual, ignoreCase, message);
        }

        /// <summary>Verifies that the expected value is greater than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit tests expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(double expected, double actual, string message, params object[] parameters)
        {
            ASSERT.IsGreaterThan(expected, actual, message, parameters);
        }

        /// <summary>Verifies that the expected value is greater than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(float expected, float actual, string message, params object[] parameters)
        {
            ASSERT.IsGreaterThan(expected, actual, message, parameters);
        }


        /// <summary>Verifies that string value expected evaluates as greater than string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message)
        {
            ASSERT.IsGreaterThan(expected, actual, ignoreCase, culture, message);
        }

        /// <summary>Verifies that string value expected evaluates as greater than string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            ASSERT.IsGreaterThan(expected, actual, ignoreCase, message, parameters);
        }

        /// <summary>Verifies that string value expected evaluates as greater than string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsGreaterThan(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message, params object[] parameters)
        {
            ASSERT.IsGreaterThan(expected, actual, ignoreCase, culture, message, parameters);
        }

        #endregion


        // ==================================================================================================

        #region LessOrEqual

        /// <summary>Verifies that the expected value is less than or equal to the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(double expected, double actual)
        {
            ASSERT.IsLessOrEqual(expected, actual);
        }


        /// <summary>Verifies that the expected value is less than or equal to the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(float expected, float actual)
        {
            ASSERT.IsLessOrEqual(expected, actual);
        }


        /// <summary>Verifies that string value expected evaluates as less than or equal to string value actual, ignoring case or not as specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
#pragma warning disable CA1304 // Specify CultureInfo
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(string expected, string actual, bool ignoreCase)
        {
            ASSERT.IsLessOrEqual(expected, actual, ignoreCase);
        }
#pragma warning restore CA1304 // Specify CultureInfo

        /// <summary>Verifies that the expected value is less than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(double expected, double actual, string message)
        {
            ASSERT.IsLessOrEqual(expected, actual, message);
        }

        /// <summary>Verifies that the expected value is less than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(float expected, float actual, string message)
        {
            ASSERT.IsLessOrEqual(expected, actual, message);
        }

        /// <summary>Verifies that string value expected evaluates as less than or equal to string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            ASSERT.IsLessOrEqual(expected, actual, ignoreCase, culture);
        }


        /// <summary>Verifies that string value expected evaluates as less than or equal to string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(string expected, string actual, bool ignoreCase, string message)
        {
            ASSERT.IsLessOrEqual(expected, actual, ignoreCase, message);
        }

        /// <summary>Verifies that the expected value is less than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit tests expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(double expected, double actual, string message, params object[] parameters)
        {
            ASSERT.IsLessOrEqual(expected, actual, message, parameters);
        }

        /// <summary>Verifies that the expected value is less than or equal to the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(float expected, float actual, string message, params object[] parameters)
        {
            ASSERT.IsLessOrEqual(expected, actual, message, parameters);
        }


        /// <summary>Verifies that string value expected evaluates as less than or equal to string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message)
        {
            ASSERT.IsLessOrEqual(expected, actual, ignoreCase, culture, message);
        }

        /// <summary>Verifies that string value expected evaluates as less than or equal to string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            ASSERT.IsLessOrEqual(expected, actual, ignoreCase, message, parameters);
        }

        /// <summary>Verifies that string value expected evaluates as less than or equal to string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessOrEqual(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message, params object[] parameters)
        {
            ASSERT.IsLessOrEqual(expected, actual, ignoreCase, culture, message, parameters);
        }

        #endregion



        // ==================================================================================================

        #region LessThan

        /// <summary>Verifies that the expected value is less than the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(double expected, double actual)
        {
            ASSERT.IsLessThan(expected, actual);
        }


        /// <summary>Verifies that the expected value is less than the actual value. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(float expected, float actual)
        {
            ASSERT.IsLessThan(expected, actual);
        }


        /// <summary>Verifies that string value expected evaluates as less than string value actual, ignoring case or not as specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
#pragma warning disable CA1304 // Specify CultureInfo
        [Conditional("DEBUG")]
        public static void IsLessThan(string expected, string actual, bool ignoreCase)
        {
            ASSERT.IsLessThan(expected, actual, ignoreCase);
        }
#pragma warning restore CA1304 // Specify CultureInfo

        /// <summary>Verifies that the expected value is less than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit test expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(double expected, double actual, string message)
        {
            ASSERT.IsLessThan(expected, actual, message);
        }

        /// <summary>Verifies that the expected value is less than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(float expected, float actual, string message)
        {
            ASSERT.IsLessThan(expected, actual, message);
        }

        /// <summary>Verifies that string value expected evaluates as less than string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            ASSERT.IsLessThan(expected, actual, ignoreCase, culture);
        }


        /// <summary>Verifies that string value expected evaluates as less than string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(string expected, string actual, bool ignoreCase, string message)
        {
            ASSERT.IsLessThan(expected, actual, ignoreCase, message);
        }

        /// <summary>Verifies that the expected value is less than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first double to compare. This is the double the unit tests expects.</param>
        /// <param name="actual">The second double to compare. This is the double the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(double expected, double actual, string message, params object[] parameters)
        {
            ASSERT.IsLessThan(expected, actual, message, parameters);
        }

        /// <summary>Verifies that the expected value is less than the actual value. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first single to compare. This is the single the unit test expects.</param>
        /// <param name="actual">The second single to compare. This is the single the unit test produced.</param>
        /// <param name="delta">The required accuracy. The assertion will fail only if <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is different from <paramref name="actual" /> by more than <paramref name="delta" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(float expected, float actual, string message, params object[] parameters)
        {
            ASSERT.IsLessThan(expected, actual, message, parameters);
        }


        /// <summary>Verifies that string value expected evaluates as less than string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message)
        {
            ASSERT.IsLessThan(expected, actual, ignoreCase, culture, message);
        }

        /// <summary>Verifies that string value expected evaluates as less than string value actual, ignoring case or not as specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            ASSERT.IsLessThan(expected, actual, ignoreCase, message, parameters);
        }

        /// <summary>Verifies that string value expected evaluates as less than string value actual, ignoring case or not as specified, and using the culture info specified. The assertion fails if this condition is not met. Displays a message if the assertion fails, and applies the specified formatting to it.</summary>
        /// <param name="expected">The first string to compare. This is the string the unit test expects.</param>
        /// <param name="actual">The second string to compare. This is the string the unit test produced.</param>
        /// <param name="ignoreCase">A Boolean value that indicates a case-sensitive or insensitive comparison. true indicates a case-insensitive comparison.</param>
        /// <param name="culture">A <see cref="CultureInfo" /> object that supplies culture-specific comparison information.</param>
        /// <param name="message">A message to display if the assertion fails. This message can be seen in the unit test results.</param>
        /// <param name="parameters">An array of parameters to use when formatting <paramref name="message" />.</param>
        /// <exception cref="AssertFailedException">
        /// <paramref name="expected" /> is not equal to <paramref name="actual" />.</exception>
        [Conditional("DEBUG")]
        public static void IsLessThan(string expected, string actual, bool ignoreCase, System.Globalization.CultureInfo culture, string message, params object[] parameters)
        {
            ASSERT.IsLessThan(expected, actual, ignoreCase, culture, message, parameters);
        }

        #endregion

    }
}