using System;
using System.Collections.Generic;
using System.Linq;
using ChilliCream.Testing;
using HotChocolate.Language;
using Xunit;

namespace HotChocolate.Stitching
{
    public class MergeQueryRewriterTests
    {
        [Fact]
        public void SimpleShortHandQuery()
        {
            // arrange
            string query_a = "{ a { b } }";
            string query_b = "{ c { d } }";
            string query_c = "{ a { c } }";

            // act
            var rewriter = new MergeQueryRewriter(Array.Empty<string>());
            rewriter.AddQuery(Parser.Default.Parse(query_a), "_a", false);
            rewriter.AddQuery(Parser.Default.Parse(query_b), "_b", false);
            rewriter.AddQuery(Parser.Default.Parse(query_c), "_c", false);
            DocumentNode document = rewriter.Merge();

            // assert
            QuerySyntaxSerializer.Serialize(document).Snapshot();
        }

        [Fact]
        public void QueryWithPrivateVariables()
        {
            // arrange
            DocumentNode query_a = Parser.Default.Parse(
                FileResource.Open("StitchingQueryWithUnion.graphql"));
            DocumentNode query_b = Parser.Default.Parse(
                FileResource.Open("StitchingQueryWithVariables.graphql"));

            // act
            var rewriter = new MergeQueryRewriter(Array.Empty<string>());
            rewriter.AddQuery(query_a, "_a", false);
            rewriter.AddQuery(query_b, "_b", false);
            DocumentNode document = rewriter.Merge();

            // assert
            QuerySyntaxSerializer.Serialize(document).Snapshot();
        }

        [Fact]
        public void QueryWithGlobalVariables()
        {
            // arrange
            DocumentNode query_a = Parser.Default.Parse(
                FileResource.Open("MergeQueryWithVariable.graphql"));
            DocumentNode query_b = Parser.Default.Parse(
                FileResource.Open("MergeQueryWithVariable.graphql"));

            // act
            var rewriter = new MergeQueryRewriter(
                new HashSet<string>(new[] { "global" }));
            rewriter.AddQuery(query_a, "_a", true);
            rewriter.AddQuery(query_b, "_b", true);
            DocumentNode document = rewriter.Merge();

            // assert
            QuerySyntaxSerializer.Serialize(document).Snapshot();
        }

        [Fact]
        public void AliasesMapIsCorrect()
        {
            // arrange
            DocumentNode query_a = Parser.Default.Parse(
                FileResource.Open("MergeQueryWithVariable.graphql"));
            DocumentNode query_b = Parser.Default.Parse(
                FileResource.Open("MergeQueryWithVariable.graphql"));

            // act
            var rewriter = new MergeQueryRewriter(Array.Empty<string>());
            IDictionary<string, string> a =
                rewriter.AddQuery(query_a, "_a", true);
            IDictionary<string, string> b =
                rewriter.AddQuery(query_b, "_b", true);

            // assert
            a.Snapshot("AliasesMapIsCorrect_A");
            b.Snapshot("AliasesMapIsCorrect_B");
        }


        [Fact]
        public void DocumentHasNoOperation()
        {
            // arrange
            DocumentNode query = Parser.Default.Parse(
                "type Foo { s: String }");

            // act
            var rewriter = new MergeQueryRewriter(Array.Empty<string>());
            Action action = () => rewriter.AddQuery(query, "_a", false);

            // assert
            Assert.Equal("document",
                Assert.Throws<ArgumentException>(action).ParamName);
        }

        [Fact]
        public void DocumentIsNull()
        {
            // arrange
            DocumentNode query = Parser.Default.Parse(
                "type Foo { s: String }");

            // act
            var rewriter = new MergeQueryRewriter(Array.Empty<string>());
            Action action = () => rewriter.AddQuery(null, "_a", false);

            // assert
            Assert.Equal("document",
                Assert.Throws<ArgumentNullException>(action).ParamName);
        }

        [Fact]
        public void QueriesAreNotOfTheSameOperationType()
        {
            // arrange
            DocumentNode query_a = Parser.Default.Parse("query a { b }");
            DocumentNode query_b = Parser.Default.Parse("mutation a { b }");

            // act
            var rewriter = new MergeQueryRewriter(Array.Empty<string>());
            rewriter.AddQuery(query_a, "abc", false);
            Action action = () => rewriter.AddQuery(query_b, "abc", false);

            // assert
            Assert.Equal("document",
                Assert.Throws<ArgumentException>(action).ParamName);
        }

        [Fact]
        public void RequestPrefixIsEmpty()
        {
            // arrange
            DocumentNode query = Parser.Default.Parse(
                "type Foo { s: String }");

            // act
            var rewriter = new MergeQueryRewriter(Array.Empty<string>());
            Action action = () => rewriter.AddQuery(
                query, default(NameString), false);

            // assert
            Assert.Equal("requestPrefix",
                Assert.Throws<ArgumentException>(action).ParamName);
        }

        [Fact]
        public void CreateNewInstance_GlobalVariablesIsNull()
        {
            // arrange
            DocumentNode query = Parser.Default.Parse(
                "type Foo { s: String }");

            // act
            Action action = () => new MergeQueryRewriter(null);

            // assert
            Assert.Equal("globalVariableNames",
                Assert.Throws<ArgumentNullException>(action).ParamName);
        }
    }
}
