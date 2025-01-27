﻿using System.Collections.Generic;
using System.Collections.Immutable;
using ErrorProne.NET.Core;
using ErrorProne.NET.CoreAnalyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using ExceptionReference = ErrorProne.NET.CoreAnalyzers.ExceptionReference;

namespace ErrorProne.NET.ExceptionsAnalyzers
{
    /// <summary>
    /// Detects `catch` blocks that swallow an exception.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SwallowAllExceptionsAnalyzer : DiagnosticAnalyzer
    {
        // Catch is not empty, `catch` or `catch(Exception)` and some return statement exists. 
        internal static DiagnosticDescriptor Rule => DiagnosticDescriptors.ERP022;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.CatchClause);
        }

        // Called when Roslyn encounters a catch clause.
        private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            var catchBlock = (CatchClauseSyntax)context.Node;

            if (catchBlock.Declaration == null || catchBlock.Declaration.CatchIsTooGeneric(context.SemanticModel))
            {
                var usages = context.SemanticModel.GetExceptionIdentifierUsages(catchBlock);

                if (IsObserved(usages))
                {
                    // Exception was observed!
                    return;
                }

                StatementSyntax syntax = catchBlock.Block;
                var controlFlow = context.SemanticModel.AnalyzeControlFlow(syntax);
                if (controlFlow == null)
                {
                    return;
                }

                // Warn for every exit points
                foreach (SyntaxNode @return in controlFlow.ExitPoints)
                {
                    // Due to some very weird behavior, return statement would be an exit point of the method
                    // even if the return statement is unreachable (for instance, because throw statement is preceding it);
                    // So analyzing control flow once more and emitting a warning only when the endpoint is reachable!
                    var localFlow = context.SemanticModel.AnalyzeControlFlow(@return);

                    if (localFlow.Succeeded && localFlow.StartPointIsReachable)
                    {
                        if (context.SemanticModel.GetOperation(@return) is IReturnOperation returnOperation &&
                            returnOperation.ReturnedValue is ILocalReferenceOperation lr)
                        {
                            // This is 'return e;' case. Just ignore it.
                            if (lr.Local.ExceptionFromCatchBlock())
                            {
                                continue;
                            }
                        }

                        // Block is empty, create and report diagnostic warning.
                        var diagnostic = Diagnostic.Create(Rule, @return.GetLocation(), @return.WithoutTrivia().GetText());
                        context.ReportDiagnostic(diagnostic);
                    }
                }

                // EndPoint (end of the block) is not a exit point. Should be covered separately!
                if (controlFlow.EndPointIsReachable)
                {
                    var diagnostic = Diagnostic.Create(Rule, catchBlock.Block.CloseBraceToken.GetLocation(), catchBlock.Block.CloseBraceToken.ValueText);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool IsObserved(IEnumerable<ExceptionReference> usages)
        {
            foreach (var usage in usages)
            {
                var u = usage.Identifier;
                if (
                    u.Parent is ArgumentSyntax || // Exception object was used directly
                    u.Parent is AssignmentExpressionSyntax || // Was saved to field or local
                    // For instance in Console.WriteLine($"e = {e}");
                    u.Parent is InterpolationSyntax ||
                    // or Inner exception, Message or other properties were used
                    u.Parent is MemberAccessExpressionSyntax)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
