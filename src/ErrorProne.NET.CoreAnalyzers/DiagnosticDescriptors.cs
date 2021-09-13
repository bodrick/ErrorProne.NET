﻿using System.Collections.Immutable;
using System.Linq;
using ErrorProne.NET.CoreAnalyzers;
using Microsoft.CodeAnalysis;

namespace ErrorProne.NET
{
    internal static class DiagnosticDescriptors
    {
        private const string CodeSmellCategory = "CodeSmell";
        private const string ConcurrencyCategory = "Concurrency";
        private static readonly string[] UnnecessaryTag = new [] { WellKnownDiagnosticTags.Unnecessary };

        /// <nodoc />
        internal static readonly DiagnosticDescriptor EPC11 = new DiagnosticDescriptor(
            nameof(EPC11), 
            title: "Suspicious equality implementation", 
            messageFormat: "Suspicious equality implementation: {0}.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "Equals method that does not use any instance members or other instance is suspicious.");

        /// <nodoc /> 
        public static readonly DiagnosticDescriptor EPC12 = new DiagnosticDescriptor(
            nameof(EPC12), title: "Suspicious exception handling: only the 'Message' property is observed in the catch block.",
            messageFormat: "Suspicious exception handling: only '{0}.Message' is observed in the catch block.",
            category: CodeSmellCategory, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: "In many cases the 'Message' property contains irrelevant information and actual data is kept in inner exceptions.",
            // The diagnostics fades parts of the code.
            customTags: UnnecessaryTag);

        /// <nodoc />
        internal static readonly DiagnosticDescriptor EPC13 = new DiagnosticDescriptor(
            nameof(EPC13), 
            title: "Suspiciously unobserved result.", 
            messageFormat: "The result of type '{0}' should be observed.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "Return values of some methods should always be observed.");

        /// <nodoc />
        internal static readonly DiagnosticDescriptor EPC14 = new DiagnosticDescriptor(
            nameof(EPC14), 
            title: "ConfigureAwait(false) call is redundant.", 
            messageFormat: "ConfigureAwait(false) call is redundant.", 
            CodeSmellCategory, 
            // Using Info to fade away the call, not warn on it.
            DiagnosticSeverity.Info, isEnabledByDefault: true, 
            description: "The assembly is configured not to use .ConfigureAwait(false)",
            // The diagnostics fades parts of the code.
            customTags: UnnecessaryTag);

        /// <nodoc />
        internal static readonly DiagnosticDescriptor EPC15 = new DiagnosticDescriptor(
            nameof(EPC15), 
            title: "ConfigureAwait(false) must be used.", 
            messageFormat: "ConfigureAwait(false) must be used.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "The assembly is configured to use .ConfigureAwait(false)");

        /// <nodoc />
        internal static readonly DiagnosticDescriptor EPC16 = new DiagnosticDescriptor(
            nameof(EPC16), 
            title: "Awaiting a result of a null-conditional expression will cause NullReferenceException.", 
            messageFormat: "Awaiting a result of a null-conditional expression will cause NullReferenceException.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "Null-conditional operator returns null when 'lhs' is null, causing NRE when a task is awaited.");

        /// <nodoc />
        internal static readonly DiagnosticDescriptor EPC17 = new DiagnosticDescriptor(
            nameof(EPC17), 
            title: "Avoid async-void delegates", 
            messageFormat: "Avoid async-void delegates", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "Async-void delegates can cause application to crash.");

        /// <nodoc />
        public static readonly DiagnosticDescriptor EPC18 = new DiagnosticDescriptor(
            nameof(EPC18), 
            title: "A task instance is implicitly converted to a string.", 
            messageFormat: "A task instance is implicitly converted to a string.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "An implicit conversion of a task instance to a string potentially indicates an lack of 'await'.");

        /// <nodoc />
        public static readonly DiagnosticDescriptor EPC19 = new DiagnosticDescriptor(
            nameof(EPC19), 
            title: "Observe and Dispose a 'CancellationTokenRegistration' to avoid memory leaks.", 
            messageFormat: "Observe and Dispose a 'CancellationTokenRegistration' to avoid memory leaks.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "Failure to dispose 'CancellationTokenRegistration' may cause a memory leak if obtained from a non-local token.");

        /// <nodoc />
        internal static readonly DiagnosticDescriptor ERP021 = new DiagnosticDescriptor(
            nameof(ERP021), 
            title: "Incorrect exception propagation.", 
            messageFormat: "Incorrect exception propagation. Use 'throw;' instead.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: "Incorrect exception propagation alters the stack trace of a thrown exception.");

        /// <nodoc />
        internal static readonly DiagnosticDescriptor ERP022 = new DiagnosticDescriptor(
            nameof(ERP022), 
            title: "Unobserved exception in a generic exception handler.", 
            messageFormat: "An exit point '{0}' swallows an unobserved exception.", 
            CodeSmellCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "A generic catch block swallows an exception that was not observed.");

        /// <nodoc />
        public static readonly DiagnosticDescriptor ERP031 = new DiagnosticDescriptor(
            nameof(ERP031), 
            title: "The API is not thread-safe.", 
            messageFormat: "The API is not thread-safe.{0}", 
            ConcurrencyCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, 
            description: "The API is not thread safe and can cause runtime failures.");
    }
}